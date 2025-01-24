using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using ImageProcessor;
using System.Drawing;
using ImageProcessor.Imaging.Formats;
using iTextSharp;
using iTextSharp.text.pdf;
using System.Text;
using iTextSharp.text.pdf.parser;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using WebApi.Services;
using DinkToPdf.Contracts;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Attachments")]
    public class AttachmentsController : Controller {
        private readonly AppDBContext _context;
        private readonly IConfiguration configuration;
        public IHostingEnvironment Environment;

        public AttachmentsController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env) {
            _context = context;
            configuration = config;
            Environment = env;  
        }

        // GET: Attachments
        [HttpGet]
        public IEnumerable<Attachment> GetAttachments([FromQuery] int? leadId, [FromQuery] int? quoteId, [FromQuery] int? contactId, [FromQuery] int? billId, [FromQuery] int? companyId, [FromQuery] int? productId, [FromQuery] int? sourceId, [FromQuery] int? salesOrderId, [FromQuery] int? purchaseOrderId, [FromQuery] int? rmaId, [FromQuery] int? invoiceId, [FromQuery] int? inventoryItemId, [FromQuery] int? incomingShipmentId) {
            IEnumerable<Attachment> Attachments = null;

            // var query = HttpContext.Request.Query;
            // foreach (var queryItem in query) {
            //     if (queryItem.Key.Substring(queryItem.Key.Length - 2) == "Id") {
            //         var typeName = "GidIndustrial.Gideon.WebApi.Models." + queryItem.Key.Substring(0, 1).ToUpper() + queryItem.Key.Substring(1, queryItem.Key.Length - 3);
            //         Type objectType = Type.GetType(typeName);
            //         _context.
            //     }
            // }

            if (leadId != null) {
                Attachments = _context.LeadAttachments
                    .Where(item => item.LeadId == leadId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (quoteId != null) {
                Attachments = _context.QuoteAttachments
                    .Where(item => item.QuoteId == quoteId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (contactId != null) {
                Attachments = _context.ContactAttachments
                    .Where(item => item.ContactId == contactId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (companyId != null) {
                Attachments = _context.CompanyAttachments
                    .Where(item => item.CompanyId == companyId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (productId != null) {
                Attachments = _context.ProductAttachments
                    .Where(item => item.ProductId == productId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (sourceId != null) {
                Attachments = _context.SourceAttachments
                    .Where(item => item.SourceId == sourceId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (salesOrderId != null) {
                Attachments = _context.SalesOrderAttachments
                    .Where(item => item.SalesOrderId == salesOrderId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (purchaseOrderId != null) {
                Attachments = _context.PurchaseOrderAttachments
                    .Where(item => item.PurchaseOrderId == purchaseOrderId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (rmaId != null) {
                Attachments = _context.RmaAttachments
                    .Where(item => item.RmaId == rmaId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (billId != null) {
                Attachments = _context.BillAttachments
                    .Where(item => item.BillId == billId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (invoiceId != null) {
                Attachments = _context.InvoiceAttachments
                    .Where(item => item.InvoiceId == invoiceId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (inventoryItemId != null) {
                Attachments = _context.InventoryItemAttachments
                    .Where(item => item.InventoryItemId == inventoryItemId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else if (incomingShipmentId != null) {
                Attachments = _context.IncomingShipmentAttachments
                    .Where(item => item.IncomingShipmentId == incomingShipmentId)
                    .Include(l => l.Attachment)
                    .Select(l => l.Attachment);
            } else {

                // Attachments = _context.Attachments;
            }
            return Attachments;
        }

        // GET: Attachments/ViewingLink/{id}
        /// <summary>
        /// This method generates a link that allows the client to view an upload.null  It expires after a short time, but the client can easily generate a new one by clicking on "View Attachment"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ViewingLink/{id}")]
        public async Task<IActionResult> GetAttachmentViewingLink([FromRoute] int id) {

            Attachment attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureBlobStorage"));
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");

            CloudBlockBlob blob = container.GetBlockBlobReference(attachment.OfficialFilename);

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
            // sasConstraints.

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);
            return Content(attachment.Uri + sasBlobToken);
        }

        // GET: Attachments/UploadAuthorization


        [HttpGet("UploadAuthorization")]
        public async Task<IActionResult> GetUploadAuthorization(
            [FromQuery] int fileSize, [FromQuery] string fileName,
            [FromQuery] int? leadId, [FromQuery] int? quoteId, 
            [FromQuery] int? contactId, [FromQuery] int? companyId, [FromQuery] int? productId, 
            [FromQuery] int? sourceId, [FromQuery] int? salesOrderId, 
            [FromQuery] int? purchaseOrderId, [FromQuery] int? inventoryItemId, [FromQuery] int? rmaId, 
            [FromQuery] int? billId, [FromQuery] int? incomingShipmentId,
            [FromQuery] int? invoiceId, [FromQuery] int? repairId,
            [FromQuery] int? chatMessageId,
            [FromQuery] int? attachmentTypeId,
            [FromQuery] string contentType = ""
        ) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //Parse the connection string and return a reference to the storage account.
            var newAttachment = await Attachment.CreateNewAuthorizedAttachment(configuration.GetConnectionString("AzureBlobStorage"), fileSize, fileName, contentType, attachmentTypeId);
            newAttachment.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);

            if (leadId != null) {
                var existing = _context.LeadAttachments.Where(a => a.LeadId == leadId && a.AttachmentId == newAttachment.Id).FirstOrDefault<LeadAttachment>();

                if (existing == null)
                {
                    _context.LeadAttachments.Add(new LeadAttachment
                    {
                        LeadId = (int)leadId,
                        Attachment = newAttachment
                    });
                }
                
            } else if (quoteId != null) {
                _context.QuoteAttachments.Add(new QuoteAttachment {
                    QuoteId = (int)quoteId,
                    Attachment = newAttachment
                });
            } else if (contactId != null) {
                _context.ContactAttachments.Add(new ContactAttachment {
                    ContactId = (int)contactId,
                    Attachment = newAttachment
                });
            } else if (companyId != null) {
                _context.CompanyAttachments.Add(new CompanyAttachment {
                    CompanyId = (int)companyId,
                    Attachment = newAttachment
                });
            } else if (productId != null) {
                _context.ProductAttachments.Add(new ProductAttachment {
                    ProductId = (int)productId,
                    Attachment = newAttachment
                });
            } else if (sourceId != null) {
                _context.SourceAttachments.Add(new SourceAttachment {
                    SourceId = (int)sourceId,
                    Attachment = newAttachment
                });
            } else if (salesOrderId != null) {
                _context.SalesOrderAttachments.Add(new SalesOrderAttachment {
                    SalesOrderId = (int)salesOrderId,
                    Attachment = newAttachment
                });
            } else if (purchaseOrderId != null) {
                _context.PurchaseOrderAttachments.Add(new PurchaseOrderAttachment {
                    PurchaseOrderId = (int)purchaseOrderId,
                    Attachment = newAttachment
                });
            } else if (inventoryItemId != null) {
                _context.InventoryItemAttachments.Add(new InventoryItemAttachment {
                    InventoryItemId = (int)inventoryItemId,
                    Attachment = newAttachment
                });
            } else if (rmaId != null) {
                _context.RmaAttachments.Add(new RmaAttachment {
                    RmaId = (int)rmaId,
                    Attachment = newAttachment
                });
            } else if (billId != null) {
                _context.BillAttachments.Add(new BillAttachment {
                    BillId = (int)billId,
                    Attachment = newAttachment
                });
            } else if (chatMessageId != null) {
                _context.ChatMessageAttachments.Add(new ChatMessageAttachment {
                    ChatMessageId = (int)chatMessageId,
                    Attachment = newAttachment
                });
            } else if (invoiceId != null) {
                _context.InvoiceAttachments.Add(new InvoiceAttachment {
                    InvoiceId = (int)invoiceId,
                    Attachment = newAttachment
                });
            } else if (incomingShipmentId != null) {
                _context.IncomingShipmentAttachments.Add(new IncomingShipmentAttachment {
                    IncomingShipmentId = (int)incomingShipmentId,
                    Attachment = newAttachment
                });
            } else {
                throw new Exception("Must have an Id for some type of object: leadId, quoteId, contactId, companyId, or productId");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex) {
                string t = ex.Message;
            }

            //Return the URI string for the container, including the SAS token.
            return CreatedAtAction("GetUploadAuthorization", new { id = newAttachment.Id }, newAttachment);
        }

        // GET: Attachments/ConfirmUploadSuccess/{id}
        /// <summary>
        /// This method is called by the client once it has successfully uploaded the attachment to azure
        /// </summary>
        /// <param name="id">The id of the attachment</param>
        /// <returns></returns>
        [HttpGet("ConfirmUploadSuccess/{id}")]
        public async Task<IActionResult> ConfirmUploadSuccess([FromRoute] int id) {
            var attachment = await _context.Attachments.SingleOrDefaultAsync(m => m.Id == id);
            attachment.Confirmed = true;
            _context.Entry(attachment).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!AttachmentExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("GetLineCardPdf")]
        public async Task GetLineCardPdf()
        {
            try
            {
                string filePath = Environment.ContentRootPath + @"\Features\Attachment\Views\GIDIndustrialLineCard.pdf"; // Replace with the actual file path
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                Response.ContentType = "application/pdf";
                await Response.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }

            return;
        }

        [HttpGet("GetEuropeLineCardPdf")]
        public async Task GetEuropeLineCardPdf()
        {
            try
            {
                string filePath = Environment.ContentRootPath + @"\Features\Attachment\Views\GIDEuropeLineCard.pdf"; // Replace with the actual file path
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                Response.ContentType = "application/pdf";
                await Response.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }

            return;
        }

        // GET: Attachments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttachment([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var attachment = await _context.Attachments.SingleOrDefaultAsync(m => m.Id == id);

            if (attachment == null) {
                return NotFound();
            }

            return Ok(attachment);
        }

        // PUT: Attachments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttachment([FromRoute] int id, [FromBody] Attachment attachment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != attachment.Id) {
                return BadRequest();
            }

            Attachment dbAttachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
            if (dbAttachment == null) {
                return NotFound();
            }

            dbAttachment.AttachmentTypeId = attachment.AttachmentTypeId;
            _context.Entry(dbAttachment).State = EntityState.Modified;

            // _context.Entry(attachment).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!AttachmentExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        public class ImageProcess
        {
            public byte[] Data { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public string WMText { get; set; }

        }

        [HttpPost("ProcessPolyRFQ")]
        public async Task<bool> ProcessPolyRFQ([FromBody] int fileId, [FromQuery] int leadId)
        {
            // Get attachment from db
            var attachment = await _context.Attachments.SingleOrDefaultAsync(m => m.Id == fileId);

            // Pull media file
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureBlobStorage"));
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
            // Get blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(attachment.OfficialFilename);

            // Get pdf as byte array
            blob.FetchAttributesAsync().Wait(); 
            long fileByteLength = blob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            await blob.DownloadToByteArrayAsync(fileContent, 0);

            // Create Lead Line Items
            List<LeadLineItem> lineItems = new List<LeadLineItem>();
            LeadLineItem lineItem = new LeadLineItem();
            int currentLineItem = 0;
            bool activeLine = false;

            // Set pdf reader
            using (PdfReader reader = new PdfReader(fileContent))
            {
                StringBuilder text = new StringBuilder();
                //ITextExtractionStrategy Strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string page = "";

                    page = PdfTextExtractor.GetTextFromPage(reader, i);
                    string[] lines = page.Split('\n');

                    for (int j = 0; j < lines.Length; j++)
                    {
                        // Set line
                        string line = lines[j];

                        if (line.Contains("____________") && !line.Contains("P-A Number"))
                        {
                            // Set current line item
                            currentLineItem++;

                            // Set active line
                            activeLine = true;

                            // Reset line item
                            lineItem = new LeadLineItem();

                            // Get quantity
                            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[0-9]*\\.0000");
                            var qtyMatch = regex.Match(line);

                            if (qtyMatch != null)
                            {
                                lineItem.Quantity = System.Int32.Parse(qtyMatch.Value.Replace(".0000", ""));
                            }

                            // Set description
                            lineItem.Description = lines[j + 1];

                            int increment = 2;

                            // Now loop over future lines to see how many manufacturers we have and add line items to each
                            while (activeLine)
                            {
                                // Get next line
                                string manLine = lines[j + increment];

                                if (manLine.StartsWith("Mfg"))
                                {
                                    // Set indexes
                                    int manufacturerIndex = manLine.IndexOf("Mfg:");
                                    int pnIndex = manLine.IndexOf("Mfg P/N");

                                    // Set manufacturer
                                    string manufacturer = manLine.Substring(manufacturerIndex + 5, pnIndex - 6);
                                    string partNumber = manLine.Substring(pnIndex + 8);

                                    // Clean part number
                                    partNumber = partNumber.Replace("*NO SUB*", "").Replace("*NO SUBS*", "").Replace("(NO SUB)", "").Replace("(NO SUBS)", "").Replace("NO SUBS!!!", "").Replace("NO SUBS!!", "").
                                        Replace("NO SUBS", "").Replace("NO SUB", "");

                                    // Create new line item
                                    lineItems.Add(new LeadLineItem() { Quantity = lineItem.Quantity, Description = lineItem.Description, ManufacturerName = manufacturer, ProductName = partNumber });
                                }
                                else
                                {
                                    activeLine = false;
                                }

                                increment++;
                            }
                        }

                    }
                }
            }

            // If there are line items to add, continue
            if (lineItems.Count > 0)
            {
                // Get lead
                var lead = await _context.Leads
                .Include(m => m.Attachments)
                    .ThenInclude(l => l.Attachment)
                .Include(m => m.LineItems)
                .Include(m => m.Quotes)
                .Include(m => m.EventLogEntries)
                    .ThenInclude(m => m.EventLogEntry)
                .SingleOrDefaultAsync(m => m.Id == leadId);

                // If lead exists
                if (lead != null)
                {
                    try
                    {
                        // Now loop over all line items to make sure we don't add a duplicate
                        for (int j = 1; j <= lineItems.Count; j++)
                        {
                            LeadLineItem lineItem1 = lineItems[j - 1];

                            var existingLineItem = lead.LineItems.Where(i => i.ProductName == lineItem1.ProductName && i.ManufacturerName == lineItem1.ManufacturerName).FirstOrDefault();

                            // If existing line item is not found, add it
                            if (existingLineItem == null)
                            {
                                lineItem1.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
                                lineItem1.LeadId = leadId;
                                lineItem1.Id = 0;
                                lineItem1.LineItemServiceTypeId = 1;
                                lineItem1.Order = j;
                                lead.LineItems.Add(lineItem1);
                            }
                        }
                    }
                    catch(Exception ex) {
                        string t = "";
                    }

                    try
                    {
                        // Save the lead line items
                        foreach (LeadLineItem lineItem1 in lead.LineItems)
                        {
                            _context.LeadLineItems.Add(lineItem1);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        string t = "";
                    }

                    try
                    {
                        // Set lead line item products
                        SqlParameter leadParam = new SqlParameter("@leadId", System.Data.SqlDbType.Int);
                        leadParam.Value = leadId;
                        _context.Database.ExecuteSqlCommand("exec LeadLineItemProductsSet @leadId", leadParam);
                    }
                    catch (Exception ex)
                    {
                        string t = "";
                    }
                }
            }

            return true;
        }


        [HttpPost("ProcessImage")]
        public async Task<byte[]> ProcessImage([FromBody] ImageProcess process)
        {
            // Create return array
            byte[] byteArray;

            // Create size
            Size size = new Size(process.Width, process.Height);

            // Create format
            ISupportedImageFormat format = new JpegFormat { Quality = 90 };

            // Create out stream
            using (MemoryStream outStream = new MemoryStream())
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    // Load, resize, set the format and quality and save an image.
                    imageFactory.Load(process.Data)
                                .Resize(size)
                                .Format(format)
                                .Watermark(new ImageProcessor.Imaging.TextLayer()
                                {
                                    Text = process.WMText,
                                    FontColor = Color.LightGray,
                                    Opacity = 50,
                                    RightToLeft = true,
                                    FontSize = process.Width > 260 ? 32 : 28,//julietAttachment.Domain.Length > 20 ? 24 : 32, // If domain is greater than 20 characters make it 14, otherwise 18
                                    Style = System.Drawing.FontStyle.Bold,
                                    DropShadow = true,
                                    FontFamily = FontFamily.GenericSansSerif
                                }) // If domain is greater than 20 characters make it 14, otherwise 18
                                .Save(outStream);
                }

                // Reset position
                outStream.Position = 0;

                // Do something with the stream.
                byte[] buffer = new byte[outStream.Length];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = outStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    byteArray = ms.ToArray();
                }
            }

            //System.IO.File.WriteAllBytes("C:\\Temp\\Test0606web.jpg", byteArray);

            return byteArray;
        }


        // DELETE: Attachments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(
            [FromRoute] int id,
            [FromQuery] int? leadId,
            [FromQuery] int? quoteId,
            [FromQuery] int? contactId,
            [FromQuery] int? billId,
            [FromQuery] int? companyId,
            [FromQuery] int? productId,
            [FromQuery] int? sourceId,
            [FromQuery] int? salesOrderId,
            [FromQuery] int? purchaseOrderId,
            [FromQuery] int? rmaId,
            [FromQuery] int? invoiceId,
            [FromQuery] int? inventoryItemId,
            [FromQuery] int? incomingShipmentId
        ) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var attachment = await _context.Attachments.SingleOrDefaultAsync(m => m.Id == id);
            if (attachment == null) {
                return NotFound();
            }
            if (
                leadId == null &&
                quoteId == null &&
                contactId == null &&
                billId == null &&
                companyId == null &&
                productId == null &&
                sourceId == null &&
                salesOrderId == null &&
                purchaseOrderId == null &&
                invoiceId == null &&
                rmaId == null &&
                inventoryItemId == null &&
                incomingShipmentId == null
            ) {
                return BadRequest(new {
                    Error = "You need to include somthing like leadId=... in the querystring. You can't just straight up delete an attachment because it could be connected to multiple things"
                });
            }

            //delete the attachment before deleting the reference to it so we don't end up with an orphan ghost attachment
            await attachment.Delete(_context, configuration);

            if (leadId != null) {
                var itemAttachment = await _context.LeadAttachments.FirstOrDefaultAsync(item => item.LeadId == leadId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.LeadAttachments.Remove(itemAttachment);
            } else if (quoteId != null) {
                var itemAttachment = await _context.QuoteAttachments.FirstOrDefaultAsync(item => item.QuoteId == quoteId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.QuoteAttachments.Remove(itemAttachment);
            } else if (contactId != null) {
                var itemAttachment = await _context.ContactAttachments.FirstOrDefaultAsync(item => item.ContactId == contactId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.ContactAttachments.Remove(itemAttachment);
            } else if (billId != null) {
                var itemAttachment = await _context.BillAttachments.FirstOrDefaultAsync(item => item.BillId == billId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.BillAttachments.Remove(itemAttachment);
            } else if (companyId != null) {
                var itemAttachment = await _context.CompanyAttachments.FirstOrDefaultAsync(item => item.CompanyId == companyId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.CompanyAttachments.Remove(itemAttachment);
            } else if (productId != null) {
                var itemAttachment = await _context.ProductAttachments.FirstOrDefaultAsync(item => item.ProductId == productId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.ProductAttachments.Remove(itemAttachment);
            } else if (sourceId != null) {
                var itemAttachment = await _context.SourceAttachments.FirstOrDefaultAsync(item => item.SourceId == sourceId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.SourceAttachments.Remove(itemAttachment);
            } else if (salesOrderId != null) {
                var itemAttachment = await _context.SalesOrderAttachments.FirstOrDefaultAsync(item => item.SalesOrderId == salesOrderId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.SalesOrderAttachments.Remove(itemAttachment);
            } else if (purchaseOrderId != null) {
                var itemAttachment = await _context.PurchaseOrderAttachments.FirstOrDefaultAsync(item => item.PurchaseOrderId == purchaseOrderId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.PurchaseOrderAttachments.Remove(itemAttachment);
            } else if (rmaId != null) {
                var itemAttachment = await _context.RmaAttachments.FirstOrDefaultAsync(item => item.RmaId == rmaId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.RmaAttachments.Remove(itemAttachment);
            } else if (invoiceId != null) {
                var itemAttachment = await _context.InvoiceAttachments.FirstOrDefaultAsync(item => item.InvoiceId == invoiceId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.InvoiceAttachments.Remove(itemAttachment);
            } else if (inventoryItemId != null) {
                var itemAttachment = await _context.InventoryItemAttachments.FirstOrDefaultAsync(item => item.InventoryItemId == inventoryItemId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.InventoryItemAttachments.Remove(itemAttachment);
            } else if (incomingShipmentId != null) {
                var itemAttachment = await _context.IncomingShipmentAttachments.FirstOrDefaultAsync(item => item.IncomingShipmentId == incomingShipmentId && item.AttachmentId == id);
                if (itemAttachment != null)
                    _context.IncomingShipmentAttachments.Remove(itemAttachment);
            }

            await _context.SaveChangesAsync();

            return Ok(attachment);
        }

        private bool AttachmentExists(int id) {
            return _context.Attachments.Any(e => e.Id == id);
        }
    }
}