// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using GidIndustrial.Gideon.WebApi.Models;
// using System.Net;
// using System.Net.Http;
// using Microsoft.Extensions.Configuration;
// using System.Text.RegularExpressions;
// using GidIndustrial.Gideon.WebApi.Libraries;

// namespace WebApi.Features.Controllers {
//     [Produces("application/json")]
//     [Route("ScannerEvents")]
//     public class ScannerEventsController : Controller {
//         private readonly AppDBContext _context;
//         private static readonly HttpClient HttpClient;
//         private IConfiguration _configuration;

//         static ScannerEventsController() {
//             HttpClient = new HttpClient();
//         }

//         public ScannerEventsController(AppDBContext context, IConfiguration appConfig) {
//             _context = context;
//             this._configuration = appConfig;
//         }

//         // GET: ScannerEvents
//         [HttpGet]
//         public ListResult GetScannerEvents(
//             [FromQuery] int skip = 0,
//             [FromQuery] int perPage = 10,
//             [FromQuery] string partNumber = null,
//             [FromQuery] string searchString = null,
//             [FromQuery] string sortBy = null,
//             [FromQuery] bool sortAscending = true
//         ) {
//             var query = from scannerEvent in _context.ScannerEvents select scannerEvent;
//             query = query.OrderByDescending(c => c.CreatedAt);

//             switch (sortBy) {
//                 case "Id":
//                     query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
//                     break;
//                 case "CreatedAt":
//                     query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
//                     break;
//                 default:
//                     query = query.OrderByDescending(item => item.CreatedAt);
//                     break;
//             }
//             return new ListResult {
//                 Items = query.Skip(skip).Take(perPage),
//                 Count = query.Count()
//             };
//         }

//         // GET: ScannerEvents/Search?query=...
//         [HttpGet("Search")]
//         public IEnumerable<dynamic> Search([FromQuery] string query) {
//             int id = 0;
//             Int32.TryParse(query, out id);
//             IQueryable<ScannerEvent> search = _context.ScannerEvents.Where(scannerEvent =>
//                 scannerEvent.Id == id
//             ).OrderBy(item => item.Id);
//             return search;
//         }

//         // GET: ScannerEvents/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetScannerEvent([FromRoute] int id) {
//             if (!ModelState.IsValid) {
//                 return BadRequest(ModelState);
//             }

//             var scannerEvent = await _context.ScannerEvents
//                 .Include(item => item.DataLabelTypes)
//                 .Include(item => item.ScannerActionUpdateLocation)
//                 .Include(item => item.ScannerActionRelatePieceParts)
//                 .Include(item => item.ScannerActionUpdateWorkLog)
//                 .Include(item => item.ScannerActionUpdateSystemData)
//                 .SingleOrDefaultAsync(item => item.Id == id);

//             if (scannerEvent == null) {
//                 return NotFound();
//             }

//             return Ok(scannerEvent);
//         }

//         // PUT: ScannerEvents/5
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutScannerEvent([FromRoute] int id, [FromBody] ScannerEvent scannerEvent) {
//             // if (!ModelState.IsValid) {
//             //     return BadRequest(ModelState);
//             // }

//             // if (id != scannerEvent.Id) {
//             //     return BadRequest();
//             // }

//             _context.Entry(scannerEvent).State = EntityState.Modified;
//             await _context.SaveChangesAsync();
//             _context.Entry(scannerEvent).State = EntityState.Detached;
//             scannerEvent.DataLabelTypes.ForEach(item => _context.Entry(item).State = EntityState.Detached);

//             //now get object's current relations to figure out what have been added/removed/changed
//             var oldScannerEvent = await _context.ScannerEvents
//                 .Include(item => item.DataLabelTypes)
//                 // .Include(item => item.ScannerActionUpdateLocation)
//                 .Include(item => item.ScannerActionRelatePieceParts)
//                 .Include(item => item.ScannerActionUpdateWorkLog)
//                 .Include(item => item.ScannerActionUpdateSystemData)
//                 .FirstOrDefaultAsync(item => item.Id == id);
//             if (oldScannerEvent == null) {
//                 return NotFound("A scanner label type with that Id was not found");
//             }

//             //add/remove/update list items
//             oldScannerEvent.DataLabelTypes.RemoveAll(item => !scannerEvent.DataLabelTypes.Any(item2 => item2.ScannerLabelTypeId == item.ScannerLabelTypeId));
//             var newItems = scannerEvent.DataLabelTypes.Where(item => !oldScannerEvent.DataLabelTypes.Any(item2 => item2.ScannerLabelTypeId == item.ScannerLabelTypeId)).ToList();
//             oldScannerEvent.DataLabelTypes.AddRange(newItems);
//             await _context.SaveChangesAsync();

//             //add/update actions
//             // if (scannerEvent.ScannerActionUpdateLocation != null) {
//             //     if (oldScannerEvent.ScannerActionUpdateLocation == null) {
//             //         scannerEvent.ScannerActionUpdateLocation.CreatedAt = DateTime.UtcNow;
//             //         scannerEvent.ScannerActionUpdateLocation.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//             //         _context.ScannerActionsUpdateLocation.Add(scannerEvent.ScannerActionUpdateLocation);
//             //     } else {
//             //         _context.Entry(oldScannerEvent.ScannerActionUpdateLocation).State = EntityState.Detached;
//             //         _context.Entry(scannerEvent.ScannerActionUpdateLocation).State = EntityState.Modified;
//             //     }
//             // }
//             if (scannerEvent.ScannerActionRelatePieceParts != null) {
//                 if (oldScannerEvent.ScannerActionRelatePieceParts == null) {
//                     scannerEvent.ScannerActionRelatePieceParts.CreatedAt = DateTime.UtcNow;
//                     scannerEvent.ScannerActionRelatePieceParts.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//                     _context.ScannerActionsRelatePieceParts.Add(scannerEvent.ScannerActionRelatePieceParts);
//                 } else {
//                     _context.Entry(oldScannerEvent.ScannerActionRelatePieceParts).State = EntityState.Detached;
//                     _context.Entry(scannerEvent.ScannerActionRelatePieceParts).State = EntityState.Modified;
//                 }
//             }
//             if (scannerEvent.ScannerActionUpdateWorkLog != null) {
//                 if (oldScannerEvent.ScannerActionUpdateWorkLog == null) {
//                     scannerEvent.ScannerActionUpdateWorkLog.CreatedAt = DateTime.UtcNow;
//                     scannerEvent.ScannerActionUpdateWorkLog.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//                     _context.ScannerActionsUpdateWorkLog.Add(scannerEvent.ScannerActionUpdateWorkLog);
//                 } else {
//                     _context.Entry(oldScannerEvent.ScannerActionUpdateWorkLog).State = EntityState.Detached;
//                     _context.Entry(scannerEvent.ScannerActionUpdateWorkLog).State = EntityState.Modified;
//                 }
//             }
//             if (scannerEvent.ScannerActionUpdateSystemData != null) {
//                 if (oldScannerEvent.ScannerActionUpdateSystemData == null) {
//                     scannerEvent.ScannerActionUpdateSystemData.CreatedAt = DateTime.UtcNow;
//                     scannerEvent.ScannerActionUpdateSystemData.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//                     scannerEvent.ScannerActionUpdateSystemData.ObjectField = Utilities.CleanSqlColumnName(scannerEvent.ScannerActionUpdateSystemData.ObjectField);
//                     _context.ScannerActionsUpdateSystemData.Add(scannerEvent.ScannerActionUpdateSystemData);
//                 } else {
//                     _context.Entry(oldScannerEvent.ScannerActionUpdateSystemData).State = EntityState.Detached;
//                     _context.Entry(scannerEvent.ScannerActionUpdateSystemData).State = EntityState.Modified;
//                 }
//             }
//             await _context.SaveChangesAsync();


//             return NoContent();
//         }

//         // POST: ScannerEvents
//         [HttpPost]
//         public async Task<IActionResult> PostScannerEvent([FromBody] ScannerEvent scannerEvent) {
//             scannerEvent.CreatedAt = DateTime.UtcNow;
//             scannerEvent.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

//             if(scannerEvent.ScannerActionUpdateLocation != null){
//                 scannerEvent.ScannerActionUpdateLocation.CreatedAt = DateTime.UtcNow;
//                 scannerEvent.ScannerActionUpdateLocation.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//             }
//             if(scannerEvent.ScannerActionRelatePieceParts != null){
//                 scannerEvent.ScannerActionRelatePieceParts.CreatedAt = DateTime.UtcNow;
//                 scannerEvent.ScannerActionRelatePieceParts.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//             }
//             if(scannerEvent.ScannerActionUpdateSystemData != null){
//                 scannerEvent.ScannerActionUpdateSystemData.CreatedAt = DateTime.UtcNow;
//                 scannerEvent.ScannerActionUpdateSystemData.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//             }
//             if(scannerEvent.ScannerActionUpdateWorkLog != null){
//                 scannerEvent.ScannerActionUpdateWorkLog.CreatedAt = DateTime.UtcNow;
//                 scannerEvent.ScannerActionUpdateWorkLog.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
//             }

//             ModelState.Clear();
//             TryValidateModel(scannerEvent);
//             this.TryValidateModel(scannerEvent);
//             if (scannerEvent.ScannerActionUpdateSystemData != null && scannerEvent.ScannerActionUpdateSystemData.ObjectField != null) {
//                 scannerEvent.ScannerActionUpdateSystemData.ObjectField = Utilities.CleanSqlColumnName(scannerEvent.ScannerActionUpdateSystemData.ObjectField);
//             }

//             if (!ModelState.IsValid) {
//                 return BadRequest(ModelState);
//             }
//             _context.ScannerEvents.Add(scannerEvent);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetScannerEvent", new { id = scannerEvent.Id }, scannerEvent);
//         }

//         // DELETE: ScannerEvents/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteScannerEvent([FromRoute] int id) {
//             if (!ModelState.IsValid) {
//                 return BadRequest(ModelState);
//             }

//             var scannerEvent = await _context.ScannerEvents
//                 .SingleOrDefaultAsync(item => item.Id == id);
//             if (scannerEvent == null) {
//                 return NotFound();
//             }

//             _context.ScannerEvents.Remove(scannerEvent);
//             try {
//                 await _context.SaveChangesAsync();
//             }
//             catch (Exception ex) {
//                 return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
//             }

//             return Ok(scannerEvent);
//         }

//         private bool ScannerEventExists(int id) {
//             return _context.ScannerEvents.Any(e => e.Id == id);
//         }
//     }
// }