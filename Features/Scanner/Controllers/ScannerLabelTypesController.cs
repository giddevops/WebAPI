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
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ScannerLabelTypes")]
    public class ScannerLabelTypesController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        private IConfiguration _configuration;

        static ScannerLabelTypesController() {
            HttpClient = new HttpClient();
        }

        public ScannerLabelTypesController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: ScannerLabelTypes
        [HttpGet]
        public ListResult GetScannerLabelTypes(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string partNumber = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from scannerLabelType in _context.ScannerLabelTypes select scannerLabelType;
            query = query.OrderByDescending(c => c.CreatedAt);

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
            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        [HttpGet("SelectOptions")]
        public async Task<IActionResult> GetSelectOptions([FromQuery] ScannerLabelTypeClass? scannerLabelTypeClass) {
            var query = from scannerLabelType in _context.ScannerLabelTypes select scannerLabelType;

            if (scannerLabelTypeClass != null) {
                query = query.Where(item => item.ScannerLabelTypeClass == scannerLabelTypeClass);
            }

            query = query.OrderBy(item => item.Name);

            var results = await query.Select(item => new {
                Id = item.Id,
                Value = item.Name
            }).ToListAsync();

            return Ok(results);
        }

        [HttpGet("{id}/VariableSelectOptions")]
        public async Task<IActionResult> VariableSelectOptions([FromRoute] int? id) {
            return Ok(
                _context.ScannerLabelTypeVariables.Where(item => item.ScannerLabelTypeId == id)
                .Select(item => new {
                    Id = item.Id,
                    Value = item.Name
                })
            );
        }

        // GET: ScannerLabelTypes/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            int id = 0;
            Int32.TryParse(query, out id);
            IQueryable<ScannerLabelType> search = _context.ScannerLabelTypes.Where(scannerLabelType =>
                scannerLabelType.Id == id
            ).OrderBy(item => item.Id);
            return search;
        }

        // GET: ScannerLabelTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScannerLabelType([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerLabelType = await _context.ScannerLabelTypes
                .Include(item => item.ScannerActionRelatePieceParts)
                .Include(item => item.ScannerActionUpdateLocation)
                .Include(item => item.ScannerActionUpdateSystemData)
                    .ThenInclude(item => item.Commands)
                .Include(item => item.ScannerActionUpdateWorkLog)
                .Include(item => item.Variables)
                .SingleOrDefaultAsync(item => item.Id == id);

            if (scannerLabelType == null) {
                return NotFound();
            }

            return Ok(scannerLabelType);
        }

        // PUT: ScannerLabelTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScannerLabelType([FromRoute] int id, [FromBody] ScannerLabelType scannerLabelType) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != scannerLabelType.Id) {
                return BadRequest();
            }

            var original = await _context.ScannerLabelTypes.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (original == null) {
                return NotFound("A scanner label type was not found with that Id");
            }
            //do not allow changing the scanner label type once it is created. I just have a simple rule that says if it's an event label type, automatically create an "end" event for each label. this would be negatively affected if it was changed to a data type
            scannerLabelType.ScannerLabelTypeClass = original.ScannerLabelTypeClass;

            _context.Entry(scannerLabelType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _context.Entry(scannerLabelType).State = EntityState.Detached;

            var oldScannerLabelType = await _context.ScannerLabelTypes
                .Include(item => item.ScannerActionUpdateLocation)
                .Include(item => item.ScannerActionRelatePieceParts)
                .Include(item => item.ScannerActionUpdateWorkLog)
                .Include(item => item.ScannerActionUpdateSystemData)
                    .ThenInclude(item => item.Commands)
                .Include(item => item.Variables)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (oldScannerLabelType == null) {
                return NotFound("A scanner label type with that Id was not found");
            }

            //add/remove/update list items
            oldScannerLabelType.Variables.RemoveAll(item => !scannerLabelType.Variables.Any(f => f.Id == item.Id));
            scannerLabelType.Variables.Where(item => oldScannerLabelType.Variables.Any(f => f.Id == item.Id)).ToList().ForEach(item => {
                item.ScannerLabelTypeId = (int)oldScannerLabelType.Id;
                _context.Entry(oldScannerLabelType.Variables.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                _context.Entry(item).State = EntityState.Modified;
            });
            oldScannerLabelType.Variables.AddRange(
                scannerLabelType.Variables.Where(item => item.Id == null)
            );
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            //add/update actions
            if (scannerLabelType.ScannerActionUpdateLocation != null) {
                if (oldScannerLabelType.ScannerActionUpdateLocation == null) {
                    scannerLabelType.ScannerActionUpdateLocation.CreatedAt = DateTime.UtcNow;
                    scannerLabelType.ScannerActionUpdateLocation.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                    _context.ScannerActionsUpdateLocation.Add(scannerLabelType.ScannerActionUpdateLocation);
                } else {
                    _context.Entry(oldScannerLabelType.ScannerActionUpdateLocation).State = EntityState.Detached;
                    _context.Entry(scannerLabelType.ScannerActionUpdateLocation).State = EntityState.Modified;
                }
            }
            if (scannerLabelType.ScannerActionRelatePieceParts != null) {
                if (oldScannerLabelType.ScannerActionRelatePieceParts == null) {
                    scannerLabelType.ScannerActionRelatePieceParts.CreatedAt = DateTime.UtcNow;
                    scannerLabelType.ScannerActionRelatePieceParts.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                    _context.ScannerActionsRelatePieceParts.Add(scannerLabelType.ScannerActionRelatePieceParts);
                } else {
                    _context.Entry(oldScannerLabelType.ScannerActionRelatePieceParts).State = EntityState.Detached;
                    _context.Entry(scannerLabelType.ScannerActionRelatePieceParts).State = EntityState.Modified;
                }
            }
            if (scannerLabelType.ScannerActionUpdateWorkLog != null) {
                if (oldScannerLabelType.ScannerActionUpdateWorkLog == null) {
                    scannerLabelType.ScannerActionUpdateWorkLog.CreatedAt = DateTime.UtcNow;
                    scannerLabelType.ScannerActionUpdateWorkLog.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                    _context.ScannerActionsUpdateWorkLog.Add(scannerLabelType.ScannerActionUpdateWorkLog);
                } else {
                    _context.Entry(oldScannerLabelType.ScannerActionUpdateWorkLog).State = EntityState.Detached;
                    _context.Entry(scannerLabelType.ScannerActionUpdateWorkLog).State = EntityState.Modified;
                }
            }
            if (scannerLabelType.ScannerActionUpdateSystemData != null) {
                if (oldScannerLabelType.ScannerActionUpdateSystemData == null) {
                    scannerLabelType.ScannerActionUpdateSystemData.CreatedAt = DateTime.UtcNow;
                    scannerLabelType.ScannerActionUpdateSystemData.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                    foreach (var command in scannerLabelType.ScannerActionUpdateSystemData.Commands) {
                        command.ObjectField = Utilities.CleanSqlColumnName(command.ObjectField);
                    }
                    _context.ScannerActionsUpdateSystemData.Add(scannerLabelType.ScannerActionUpdateSystemData);
                } else {
                    //remove deleted
                    var removed = oldScannerLabelType.ScannerActionUpdateSystemData.Commands.Where(item => !scannerLabelType.ScannerActionUpdateSystemData.Commands.Any(f => f.Id == item.Id));
                    _context.ScannerActionUpdateSystemDataCommands.RemoveRange(removed);
                    //update modified
                    scannerLabelType.ScannerActionUpdateSystemData.Commands
                        .Where(item => oldScannerLabelType.ScannerActionUpdateSystemData.Commands.Any(f => f.Id == item.Id))
                        .ToList()
                        .ForEach(item => {
                            item.ScannerActionUpdateSystemDataId = (int)scannerLabelType.ScannerActionUpdateSystemData.Id;
                            item.ObjectField = Utilities.CleanSqlColumnName(item.ObjectField);
                            _context.Entry(oldScannerLabelType.ScannerActionUpdateSystemData.Commands.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                            _context.Entry(item).State = EntityState.Modified;
                        });
                    //add new
                    oldScannerLabelType.ScannerActionUpdateSystemData.Commands.AddRange(
                        scannerLabelType.ScannerActionUpdateSystemData.Commands.Where(item => item.Id == null)
                    );
                    await _context.SaveChangesAsync();

                    _context.Entry(oldScannerLabelType.ScannerActionUpdateSystemData).State = EntityState.Detached;
                    _context.Entry(scannerLabelType.ScannerActionUpdateSystemData).State = EntityState.Modified;

                }
            }
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: ScannerLabelTypes
        [HttpPost]
        public async Task<IActionResult> PostScannerLabelType([FromBody] ScannerLabelType scannerLabelType) {
            var now = DateTime.UtcNow;
            scannerLabelType.CreatedAt = now;
            scannerLabelType.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            scannerLabelType.Variables.ForEach(item => {
                item.CreatedAt = now;
                item.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            });

            ModelState.Clear();
            TryValidateModel(scannerLabelType);
            this.TryValidateModel(scannerLabelType);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.ScannerLabelTypes.Add(scannerLabelType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScannerLabelType", new { id = scannerLabelType.Id }, scannerLabelType);
        }

        // DELETE: ScannerLabelTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScannerLabelType([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerLabelType = await _context.ScannerLabelTypes
                .SingleOrDefaultAsync(item => item.Id == id);
            if (scannerLabelType == null) {
                return NotFound();
            }

            if (scannerLabelType.Locked) {
                return BadRequest("You can't delete this because it is a special type used by the system");
            }

            _context.ScannerLabelTypes.Remove(scannerLabelType);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(scannerLabelType);
        }

        private bool ScannerLabelTypeExists(int id) {
            return _context.ScannerLabelTypes.Any(e => e.Id == id);
        }
    }
}