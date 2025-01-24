using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Scans")]
    public class ScansController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient = new HttpClient();
        private IConfiguration _configuration;

        public ScansController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: Scanners
        [HttpGet]
        public ListResult GetScanners(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] int? scannerStationId = null
        ) {
            var query = from scan in _context.Scans select scan;
            query = query
                .Include(item => item.ScannerLabel)
                    .ThenInclude(item => item.ScannerLabelType)
                .OrderByDescending(c => c.CreatedAt);

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (scannerStationId != null) {
                query = query.Where(item => item.ScannerStationId == scannerStationId);
            }

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        public async Task<List<ScanGroup>> GetOpenScanGroups(AppDBContext context, int? scannerId, ScannerStation scannerStation) {
            var openScanGroups = await context.ScanGroups
                .Include(item => item.Scans)
                    .ThenInclude(item => item.ScannerLabel)
                        .ThenInclude(item => item.ScannerLabelType)
                            .ThenInclude(item => item.Variables)
                .Include(item => item.Scans)
                    .ThenInclude(item => item.ScannerLabel)
                        .ThenInclude(item => item.VariableValues)
                .Where(item => item.Open)
                //.Where(item => item.LastScanOccurredAt > DateTime.UtcNow.AddSeconds(-60))
                .Where(item => item.ScannerId == scannerId)
                .OrderByDescending(item => item.LastScanOccurredAt)
                .ToListAsync();

            foreach (var scanGroup in openScanGroups) {
                scanGroup.ScannerLabelType = await scanGroup.GetEventLabelType(_context, scannerStation);
            }
            return openScanGroups;
        }

        [HttpPost]
        public async Task<ObjectResult> PostScan([FromBody] IncomingScanData incomingScanData, bool implied = false) {
            var scannerStation = await _context.ScannerStations
                .Include(item => item.Scanner)
                .Include(item => item.DefaultScannerLabel)
                    .ThenInclude(item => item.ScannerLabelType)
                .Where(item => item.Scanner.SerialNumber == incomingScanData.SerialNumber)
                .FirstOrDefaultAsync();
            var scan = new Scan();
            scan.CreatedAt = DateTime.UtcNow;
            scan.ScannerSerialNumber = incomingScanData.SerialNumber;
            scan.Implied = implied;

            if (scannerStation == null) {
                //first make sure that the scanner is in the system
                var scanner = await _context.Scanners.FirstOrDefaultAsync(item => item.SerialNumber == incomingScanData.SerialNumber && item.ModelNumber == incomingScanData.ModelNumber);
                if (scanner == null) {
                    scanner = new Scanner {
                        ModelNumber = incomingScanData.ModelNumber,
                        SerialNumber = incomingScanData.SerialNumber
                    };
                    _context.Scanners.Add(scanner);
                    await _context.SaveChangesAsync();

                    scan.ResultMessage = "Scanner was added successfully.";
                    scan.ResultCode = ScanCode.SCANNER_ADDED;
                    _context.Scans.Add(scan);
                    await _context.SaveChangesAsync();
                    return Ok(new ScanResponse {
                        Message = scan.ResultMessage,
                        Code = scan.ResultCode
                    });
                }

                scan.ResultMessage = "Scanner has not been assigned to a station yet.";
                scan.ResultCode = ScanCode.SCANNER_HAS_NO_STATION;
                _context.Scans.Add(scan);
                await _context.SaveChangesAsync();
                return BadRequest(new ScanResponse {
                    Message = scan.ResultMessage,
                    Code = scan.ResultCode
                });
            }
            scan.ScannerStationId = scannerStation.Id;
            scan.ScannerId = scannerStation.ScannerId;

            var barcodeGuid = new Guid(incomingScanData.Content);
            var scannerLabel = await _context.ScannerLabels
                .Include(item => item.ScannerLabelType)
                    .ThenInclude(item => item.Variables)
                .Include(item => item.VariableValues)
                .FirstOrDefaultAsync(item => item.BarcodeGuid == barcodeGuid);

            EndScannerLabel endScannerLabel = null;

            if (scannerLabel == null) {
                endScannerLabel = await _context.EndScannerLabels.FirstOrDefaultAsync(item => item.BarcodeGuid == barcodeGuid);
            }

            if (scannerLabel == null && endScannerLabel == null) {
                scan.ResultMessage = "That label was not found in the database";
                scan.ResultCode = ScanCode.LABEL_NOT_FOUND;
                _context.Scans.Add(scan);
                await _context.SaveChangesAsync();
                return BadRequest(new ScanResponse {
                    Message = scan.ResultMessage,
                    Code = scan.ResultCode
                });
            }

            if (scannerLabel != null) {
                scan.ScannerLabelId = scannerLabel.Id;
            } else if (endScannerLabel != null) {
                scan.EndScannerLabelId = endScannerLabel.Id;
            }

            //first check if there are ongoing scan groups
            var openScanGroups = await this.GetOpenScanGroups(_context, scan.ScannerId, scannerStation);


            var scannerLabelAppliesToExistingGroup = false;

            // VALIDATION: now run some checks to make sure that this is an allowed scan
            // If it's an end scan check for a matching start scan
            if (endScannerLabel != null) {
                var scanGroup = openScanGroups.FirstOrDefault(item => item.ScannerLabelType.Id == endScannerLabel.StartScannerLabel.ScannerLabelTypeId);
                if (scanGroup == null) {
                    scan.ResultMessage = "You scanned an 'end' label, but there was no corresponding scan group in progress";
                    scan.ResultCode = ScanCode.END_SCANNED_WITHOUT_ACTIVE_MATCHING_START;
                    _context.Scans.Add(scan);
                    await _context.SaveChangesAsync();
                    return BadRequest(new ScanResponse {
                        Message = scan.ResultMessage,
                        Code = scan.ResultCode
                    });
                }
                //Now check if the scan group has enough data in order to call the scan complete
                var result = await scanGroup.CheckIfAllRequiredPartsArePresentAndValid(_context);
                if (result != null) {
                    scan.ResultMessage = $"You scanned an 'end' label, but not all required data was present. {result.Code} {result.Message}";
                    scan.ResultCode = ScanCode.END_SCANNED_BEFORE_ALL_REQUIRED_DATA_PRESENT;
                    _context.Scans.Add(scan);
                    await _context.SaveChangesAsync();
                    return BadRequest(new ScanResponse {
                        Message = scan.ResultMessage,
                        Code = scan.ResultCode
                    });
                }
            }
            // If it's an event label, make sure that it can be concurrent with any other events in progress.  For example, you can't have 2 relate piece parts groups going on at once because it would be ambiguous what type of labels were needed
            else if (scannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.EVENT) {
                var possibleScannerLabelTypesForNewEvent = await scannerLabel.ScannerLabelType.GetDataScannerLabelTypesThatCannotBeShared(_context);
                var possibleScannerLabelTypeIds = possibleScannerLabelTypesForNewEvent.Select(item => item.Id).ToList();
                foreach (var scanGroup in openScanGroups) {
                    var types = await scanGroup.ScannerLabelType.GetDataScannerLabelTypesThatCannotBeShared(_context);
                    var typeIds = types.Select(item => item.Id);
                    var intersection = possibleScannerLabelTypeIds.Intersect(typeIds).ToList();
                    if (intersection.Count > 0) {
                        scan.ResultMessage = "You can't open this scan group because there is already a scan group that is open that requires the same labels to be scanned.  Then the computer couldn't figure out what data label goes to what scan group.";
                        scan.ResultCode = ScanCode.NEW_EVENT_CONFLICTS_WITH_OPEN_EVENT_DATA_TYPE;
                        _context.Scans.Add(scan);
                        await _context.SaveChangesAsync();
                        return BadRequest(new ScanResponse {
                            Message = scan.ResultMessage,
                            Code = scan.ResultCode
                        });
                    }
                }
            }
            // If it's a data label, make sure that there is an open group (or that the station has a default event type so a new group can be created)
            // Also need to make sure that the label type is one that one of the open groups need
            else if (scannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA) {
                //Check if it's valid for group type
                var matchFound = false;
                foreach (var scanGroup in openScanGroups) {
                    if (await scanGroup.ScannerLabelType.CheckIfDataLabelTypeIsValid(_context, scannerLabel.ScannerLabelType)) {
                        matchFound = true;
                        break;
                    }
                }
                scannerLabelAppliesToExistingGroup = matchFound;
                //if it's an invalid data label type, it could be that the station has a default event label type for which this works.
                if (matchFound == false && scannerStation.DefaultScannerLabel != null) {
                    if (await scannerStation.DefaultScannerLabel.ScannerLabelType.CheckIfDataLabelTypeIsValid(_context, scannerLabel.ScannerLabelType)) {
                        matchFound = true;
                    }
                }
                if (matchFound == false) {
                    scan.ResultMessage = "You scanned a data label that did not apply to any active scan group, nor did it apply to the default scan group if there was one.";
                    scan.ResultCode = ScanCode.DATA_LABEL_NOT_APPLICABLE;
                    _context.Scans.Add(scan);
                    await _context.SaveChangesAsync();
                    return BadRequest(new ScanResponse {
                        Message = scan.ResultMessage,
                        Code = scan.ResultCode
                    });
                }

                //Now check if any group actually needs it
                foreach (var scanGroup in openScanGroups) {
                    var labelAlreadyUsedAndNowUneeded = await scanGroup.ScannerLabelType.CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(_context, scannerLabel, scanGroup);
                    if (labelAlreadyUsedAndNowUneeded != null) {
                        scan.ResultCode = labelAlreadyUsedAndNowUneeded.Code;
                        scan.ResultMessage = labelAlreadyUsedAndNowUneeded.Message;
                        _context.Scans.Add(scan);
                        await _context.SaveChangesAsync();
                        return BadRequest(labelAlreadyUsedAndNowUneeded);
                    }
                }
            }


            using (var transaction = _context.Database.BeginTransaction()) {
                //vaildation is done. Now add the scan to the scan group. 
                if (scannerLabel != null) {
                    if (scannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.EVENT) {
                        //Open a new scan group
                        var newScanGroup = new ScanGroup {
                            CreatedAt = DateTime.UtcNow,
                            LastScanOccurredAt = DateTime.UtcNow,
                            ScannerId = scan.ScannerId,
                            Open = true,
                            Scans = new List<Scan> { scan }
                        };
                        scan.ResultCode = ScanCode.SCAN_GROUP_CREATED;
                        scan.ResultMessage = "A new scan group was created";
                        _context.ScanGroups.Add(newScanGroup);
                        await _context.SaveChangesAsync();
                        openScanGroups = await this.GetOpenScanGroups(_context, scan.ScannerId, scannerStation);
                    } else if (scannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA) {
                        //First, need to check if the station's default event needs to be specified.
                        if (scannerLabelAppliesToExistingGroup == false) {
                            var result = await this.PostScan(new IncomingScanData {
                                Content = scannerStation.DefaultScannerLabel.BarcodeGuid.ToString(),
                                ModelNumber = incomingScanData.ModelNumber,
                                SerialNumber = incomingScanData.SerialNumber
                            }, true);
                            if (result.StatusCode != 200) {
                                scan.ResultCode = ScanCode.ERROR_DOING_IMPLIED_EVENT_SCAN;
                                scan.ResultMessage = $"Error doing the default scan for this station. Details: {JsonConvert.SerializeObject(result.Value)}";
                                _context.Scans.Add(scan);
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                                return BadRequest(new ScanResponse {
                                    Message = scan.ResultMessage,
                                    Code = scan.ResultCode
                                });
                            }
                            openScanGroups = await this.GetOpenScanGroups(_context, scannerLabel.ScannerId, scannerStation);
                        }
                        //Now apply the data label to the appropriate group
                        foreach (var group in openScanGroups) {
                            if (await group.ScannerLabelType.CheckIfDataTypeIsAllowed(_context, scannerLabel.ScannerLabelType)) {
                                group.LastScanOccurredAt = DateTime.UtcNow;
                                group.Scans.Add(scan);
                                scan.ResultCode = ScanCode.SCAN_ADDED_TO_GROUP;
                                scan.ResultMessage = "Scan was added to group #" + group.Id + " successfully";
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                } else if (endScannerLabel != null) {
                    foreach (var scanGroup in openScanGroups) {
                        if (scanGroup.ScannerLabelType.Id == endScannerLabel.StartScannerLabel.ScannerLabelTypeId) {
                            scanGroup.Scans.Add(scan);
                            scanGroup.LastScanOccurredAt = DateTime.UtcNow;
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                var committed = false;
                //now that whatever it is has been added, figure out if the scan groups are completed. if so take whatever action they specify and close them.
                foreach (var scanGroup in openScanGroups) {
                    if (scanGroup.IsComplete()) {
                        await scanGroup.Commit(_context, scannerStation);
                        scan.ResultMessage = "Scan group committed to database";
                        scan.ResultCode = ScanCode.SCAN_GROUP_COMMITTED;
                        await _context.SaveChangesAsync();
                        committed = true;
                    }
                }
                transaction.Commit();
                if (committed && this._configuration != null) {
                    var tuscanyApiKey = this._configuration.GetValue<string>("TuscanyApiKey");
                    var modelNumber = scannerStation.Scanner.ModelNumber;
                    var serialNumber = scannerStation.Scanner.SerialNumber;
                    var content = new StringContent("");
                    content.Headers.Add("API-Key", tuscanyApiKey);
                    HttpClient.PostAsync($"https://tuscany-api.gidindustrial.com/v1/scanners/{modelNumber}/{serialNumber}/beep/LowHighBeep", content);
                }
            }

            return Ok(new ScanResponse {
                Message = scan.ResultMessage,
                Code = scan.ResultCode
            });
        }
    }

    public class IncomingScanData {
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Content { get; set; }
    }
    public class ScanCode {
        public const string DATA_LABEL_NOT_APPLICABLE = "DATA_LABEL_NOT_APPLICABLE";
        public const string END_SCANNED_BEFORE_ALL_REQUIRED_DATA_PRESENT = "END_SCANNED_BEFORE_ALL_REQUIRED_DATA_PRESENT";
        public const string END_SCANNED_WITHOUT_ACTIVE_MATCHING_START = "END_SCANNED_WITHOUT_ACTIVE_MATCHING_START";
        public const string ERROR_DOING_IMPLIED_EVENT_SCAN = "ERROR_DOING_IMPLIED_EVENT_SCAN";
        public const string EVENT_NOT_FOUND = "EVENT_NOT_FOUND";
        public const string EVENT_NOT_SPECIFIED = "EVENT_NOT_SPECIFIED";
        public const string INVENTORY_ITEM_NOT_FOUND = "INVENTORY_ITEM_NOT_FOUND";
        public const string IS_PIECE_PART_NOT_SPECIFIED_ON_PRODUCT = "IS_PIECE_PART_NOT_SPECIFIED_ON_PRODUCT";
        public const string LABEL_NOT_FOUND = "LABEL_NOT_FOUND";
        public const string NEW_EVENT_CONFLICTS_WITH_OPEN_EVENT_DATA_TYPE = "NEW_EVENT_CONFLICTS_WITH_OPEN_EVENT_DATA_TYPE";
        public const string OK = "OK";
        public const string ONLY_ONE_USER_ALLOWED = "ONLY_ONE_USER_ALLOWED";
        public const string PRODUCT_DOES_NOT_HAVE_PRODUCT_TYPE = "PRODUCT_DOES_NOT_HAVE_PRODUCT_TYPE";
        public const string REQUIRED_LABEL_TYPE_MISSING = "REQUIRED_LABEL_TYPE_MISSING";
        public const string SCAN_ADDED_TO_GROUP = "SCAN_ADDED_TO_GROUP";
        public const string SCAN_GROUP_COMMITTED = "SCAN_GROUP_COMMITTED";
        public const string SCANNER_HAS_NO_STATION = "SCANNER_HAS_NO_STATION";
        public const string SCANNER_NOT_FOUND = "SCANNER_NOT_FOUND";
        public const string SECOND_NON_PIECE_PART_SCANNED = "SECOND_NON_PIECE_PART_SCANNED";
        public const string WRONG_LABEL_TYPE = "WRONG_LABEL_TYPE";
        public const string SCANNER_ADDED = "SCANNER_ADDED";
        public const string SCAN_GROUP_CREATED = "SCAN_GROUP_CREATED";
    }
}