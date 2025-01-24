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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ScannerLabels")]
    public class ScannerLabelsController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        private IConfiguration _configuration;

        static ScannerLabelsController() {
            HttpClient = new HttpClient();
        }

        public ScannerLabelsController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        [HttpGet("LabelForUnserializedProduct")]
        public async Task<IActionResult> LabelForUnserializedProduct([FromQuery] int? ProductId){
            if(ProductId == null)
                return BadRequest("You need a productId");
            var product = await _context.Products.FirstOrDefaultAsync(item => item.Id == ProductId);
            if(product == null)
                return BadRequest("A product was not found with that Id");

            var scannerLabel = await ScannerLabel.GetScannerLabel(product, _context);

            return Ok(scannerLabel);
        }

        [HttpPost("VerifyVariableValues")]
        public async Task<IActionResult> VerifyVariableValues([FromBody] ScannerLabel scannerLabel) {
            var scannerLabelId = Convert.ToInt32(scannerLabel.Id);
            var errors = scannerLabel.VariableValues.Select(item => "").ToList();
            var cleanFieldNameRegex = new Regex(@"[^a-zA-Z_0-9]");

            for (var i = 0; i < scannerLabel.VariableValues.Count; ++i) {
                var scannerLabelVariableValue = scannerLabel.VariableValues[i];
                var scannerLabelTypeVariable = await _context.ScannerLabelTypeVariables.FirstAsync(item => item.Id == scannerLabelVariableValue.ScannerLabelTypeVariableId);
                //Only verify object types
                if (scannerLabelTypeVariable.ScannerLabelTypeVariableDataType != ScannerLabelTypeVariableDataType.OBJECT_REFERENCE)
                    continue;

                Type objectType = Type.GetType(scannerLabelTypeVariable.ObjectName);
                var objectTableName = _context.Model.FindEntityType(objectType).SqlServer().TableName;

                var fieldNameCleaned = cleanFieldNameRegex.Replace(scannerLabelTypeVariable.ObjectField, "");

                using (var conn = _context.Database.GetDbConnection()) {
                    await conn.OpenAsync();
                    //main query
                    using (var command = conn.CreateCommand()) {
                        command.CommandText = $"SELECT * FROM \"{objectTableName}\" WHERE \"{fieldNameCleaned}\" = @value";
                        var param = command.CreateParameter();
                        param.ParameterName = "@value";
                        param.Value = scannerLabelVariableValue.Value;
                        command.Parameters.Add(param);

                        var reader = command.ExecuteReader();
                        if (reader.HasRows) {
                            //there was a row row is ok
                            break;
                        } else {
                            errors[i] = "No " + objectTableName + " was found where " + fieldNameCleaned + " = " + scannerLabelVariableValue.Value;
                        }
                    }
                }
            }
            return Ok(errors);
        }

        [HttpGet("{id}/CreateEndScannerLabel")]
        public async Task<IActionResult> CreateEndScannerLabel([FromRoute] int? id) {
            var scannerLabel = await _context.ScannerLabels
                .Include(item => item.EndScannerLabel)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (scannerLabel == null) {
                return NotFound("Scanner label was not found");
            }

            if (scannerLabel.EndScannerLabel != null) {
                return BadRequest("This label already has an end scanner label");
            }

            var endScannerLabel = new EndScannerLabel {
                StartScannerLabelId = scannerLabel.Id,
                BarcodeGuid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            _context.EndScannerLabels.Add(endScannerLabel);

            await _context.SaveChangesAsync();

            return Ok(endScannerLabel);
        }

        // GET: ScannerLabels
        [HttpGet]
        public ListResult GetScannerLabels(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string partNumber = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] int? scannerLabelTypeId = null
        ) {
            var query = from scannerLabel in _context.ScannerLabels select scannerLabel;
            query = query.OrderByDescending(c => c.CreatedAt);

            if (scannerLabelTypeId != null) {
                query = query.Where(item => item.ScannerLabelTypeId == scannerLabelTypeId);
            }
            query = query.Include(item => item.VariableValues);

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

        // GET: ScannerLabels/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            int id = 0;
            Int32.TryParse(query, out id);
            IQueryable<ScannerLabel> search = _context.ScannerLabels.Where(scannerLabel =>
                scannerLabel.Id == id
            ).OrderBy(item => item.Id);
            return search;
        }

        [HttpPost("LabelsByGuid")]
        public async Task<IActionResult> LabelsByGuid([FromBody] string[] GuidStrings){
            var Guids = GuidStrings.Select(item => new Guid(item)).ToList();
            var labels = await _context.ScannerLabels
                .Include(item => item.ScannerLabelType)
                    .ThenInclude(item => item.Variables)
                .Where(item => item.BarcodeGuid.HasValue == true && Guids.Contains(item.BarcodeGuid.Value))
                .ToListAsync();
            return Ok(labels);
        }

        // GET: ScannerLabels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScannerLabel([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerLabel = await _context.ScannerLabels
                .Include(item => item.VariableValues)
                .Include(item => item.EndScannerLabel)
                .Include(item => item.ScannerLabelType)
                    .ThenInclude(item => item.Variables)
                .SingleOrDefaultAsync(item => item.Id == id);

            if (scannerLabel == null) {
                return NotFound();
            }

            return Ok(scannerLabel);
        }

        // PUT: ScannerLabels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScannerLabel([FromRoute] int id, [FromBody] ScannerLabel scannerLabel) {
            if (id != scannerLabel.Id) {
                return BadRequest("Route id doesnt match item Id");
            }

            //make sure the GUID isn't changed
            var oldScannerLabel = await _context.ScannerLabels.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldScannerLabel == null) {
                return NotFound("A scanner label type with that Id was not found");
            }
            scannerLabel.BarcodeGuid = oldScannerLabel.BarcodeGuid;

            //save changes on main object - this won't update any reltions though
            _context.Entry(scannerLabel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _context.Entry(scannerLabel).State = EntityState.Detached;

            //now get object's current relations to figure out what have been added/removed/changed
            oldScannerLabel = await _context.ScannerLabels
                .Include(item => item.VariableValues)
                .FirstOrDefaultAsync(item => item.Id == id);
            //add/remove/update list items
            oldScannerLabel.VariableValues.RemoveAll(item => !scannerLabel.VariableValues.Any(f => f.Id == item.Id));
            scannerLabel.VariableValues.Where(item => oldScannerLabel.VariableValues.Any(f => f.Id == item.Id)).ToList().ForEach(item => {
                item.ScannerLabelId = (int)oldScannerLabel.Id;
                _context.Entry(oldScannerLabel.VariableValues.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                _context.Entry(item).State = EntityState.Modified;
            });
            var newItems = scannerLabel.VariableValues.Where(item => item.Id == null).ToList();
            newItems.ForEach(item => {
                item.CreatedAt = DateTime.UtcNow;
                item.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            });
            oldScannerLabel.VariableValues.AddRange(newItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: ScannerLabels
        [HttpPost]
        public async Task<IActionResult> PostScannerLabel([FromBody] ScannerLabel scannerLabel) {
            var now = DateTime.UtcNow;
            scannerLabel.CreatedAt = now;
            scannerLabel.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            scannerLabel.VariableValues.ForEach(value => {
                value.CreatedAt = now;
                value.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            });
            scannerLabel.BarcodeGuid = Guid.NewGuid();

            ModelState.Clear();
            TryValidateModel(scannerLabel);
            this.TryValidateModel(scannerLabel);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //need to create an "end" label if it's a EVENT label type
            var scannerLabelType = await _context.ScannerLabelTypes.FirstOrDefaultAsync(item => item.Id == scannerLabel.ScannerLabelTypeId);
            if (scannerLabelType == null) {
                return BadRequest("Scanner Label Type with that scannerLabelTypeId was not found");
            }
            if (scannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.EVENT) {
                scannerLabel.EndScannerLabel = new EndScannerLabel {
                    CreatedAt = now,
                    BarcodeGuid = Guid.NewGuid()
                };
            }

            _context.ScannerLabels.Add(scannerLabel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScannerLabel", new {
                id = scannerLabel.Id
            }, scannerLabel);
        }

        // DELETE: ScannerLabels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScannerLabel([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerLabel = await _context.ScannerLabels
                .SingleOrDefaultAsync(item => item.Id == id);
            if (scannerLabel == null) {
                return NotFound();
            }

            _context.ScannerLabels.Remove(scannerLabel);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(scannerLabel);
        }

        private bool ScannerLabelExists(int id) {
            return _context.ScannerLabels.Any(e => e.Id == id);
        }
    }
}