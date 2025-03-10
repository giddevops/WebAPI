using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using DinkToPdf;
using WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using GidIndustrial.Gideon.WebApi.Libraries;
using System.Runtime.CompilerServices;
using SelectPdf;
using iTextSharp.text.pdf;
using Microsoft.Graph;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Crypto.Paddings;

namespace WebApi.Features.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("Quotes")]
    public class QuotesController : Controller
    {
        private readonly AppDBContext _context;
        // private readonly string SendgridApiKey;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        public string BaseGidQuoteUrl = "https://forms.gidindustrial.com/key/quote/";

        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate quotes
        private readonly ViewRender viewRenderer;

        public QuotesController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter)
        {
            _context = context;
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }

        // GET: Quotes
        [HttpGet]
        public ListResult GetQuotes(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] Boolean fetchRelated = false,
            [FromQuery] int? id = null,
            [FromQuery] int? contactId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? productId = null,
            [FromQuery] int? salesPersonId = null,
            [FromQuery] int? quoteStatusOptionId = null,
            [FromQuery] int? productTypeId = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] string companyName = null

        )
        {
            var query = from quote in _context.Quotes select quote;

            if (fetchRelated)
            {
                query = query
                    .Include(l => l.LineItems)
                        .ThenInclude(item => item.Product);
            }

            if (id != null)
                query = query.Where(l => l.Id == id);
            if (companyId != null)
                query = query.Where(l => l.CompanyId == companyId);
            if (contactId != null)
                query = query.Where(l => l.ContactId == contactId);
            if (productId != null)
                query = query.Where(l => l.LineItems.Any(li => li.ProductId == productId));
            if (salesPersonId != null)
                query = query.Where(item => item.SalesPersonId == salesPersonId);
            if (quoteStatusOptionId != null)
                query = query.Where(item => item.QuoteStatusOptionId == quoteStatusOptionId);
            if (productTypeId != null)
                query = query.Where(item => item.LineItems.Any(pLineItem => pLineItem.Product.ProductTypeId == productTypeId));
            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);
            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);
            if (!String.IsNullOrWhiteSpace(companyName))
                query = query.Where(item => EF.Functions.Like(item.Company.Name, companyName + "%"));

            switch (sortBy)
            {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Company.Name":
                    query = sortAscending ? query.OrderBy(item => item.Company.Name) : query.OrderByDescending(item => item.Company.Name);
                    break;
                case "SalesPerson.DisplayName":
                    query = sortAscending ? query.OrderBy(item => item.SalesPerson.DisplayName) : query.OrderByDescending(item => item.SalesPerson.DisplayName);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                // case "Source":
                //     query = sortAscending ? query.OrderBy(item => item.Source) : query.OrderByDescending(item => item.Source);
                //     break;
                case "Contact.FirstName":
                    query = sortAscending ? query.OrderBy(item => item.Contact.FirstName) : query.OrderByDescending(item => item.Contact.FirstName);
                    break;
                case "QuoteStatusOptionId":
                    query = sortAscending ? query.OrderBy(item => item.QuoteStatusOptionId) : query.OrderByDescending(item => item.QuoteStatusOptionId);
                    break;
                case "PartNumber":
                    query = sortAscending ? query.OrderBy(item => item.LineItems.FirstOrDefault().Product.PartNumber) : query.OrderByDescending(item => item.LineItems.FirstOrDefault().Product.PartNumber);
                    break;
                // case "ShippedProfit":
                //     query = sortAscending ? query.OrderBy(item => item.ShippedProfit) : query.OrderByDescending(item => item.ShippedProfit);
                //     break;
                case "Total":
                    query = sortAscending ? query.OrderBy(item => item.Total) : query.OrderByDescending(item => item.Total);
                    break;
                // case "Product.PartNumber":
                //     query = sortAscending ? query.OrderBy(item => item.LineItems.) : query.OrderByDescending(item => item.);
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }


            if (!String.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                int searchStringNumber;
                if (Int32.TryParse(searchString, out searchStringNumber))
                {

                }
                else
                {
                    searchStringNumber = 0;
                }
                query = query.Where(item =>
                    item.Id == searchStringNumber ||
                    EF.Functions.Like(item.Company.Name, searchString + '%') ||
                    EF.Functions.Like(item.Contact.FirstName, searchString + '%') ||
                    EF.Functions.Like(item.Contact.LastName, searchString + '%') ||
                    EF.Functions.Like(item.Phone, searchString + '%') ||
                    EF.Functions.Like(item.Email, searchString + '%') ||
                    item.LineItems.Any(lLineItem =>
                        EF.Functions.Like(lLineItem.ProductName, searchString + '%') ||
                        EF.Functions.Like(lLineItem.Product.PartNumber, searchString + '%') ||
                        lLineItem.Description.Contains(searchString)
                    )
                );
            }

            query = query
                .Include(q => q.Company)
                .Include(item => item.Contact);


            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = query.Count();

            return new ListResult
            {
                Items = query.Skip(skip).Take(perPage),
                Count = count
            };
        }


        [HttpGet("Create/AutoQuote")]
        public async Task<IActionResult> CreateAutoQuote([FromQuery] string PartNumber, [FromQuery] string Manufacturer, [FromQuery] string Category, [FromQuery] decimal Price, [FromQuery] int Quantity, [FromQuery] string PersonName, [FromQuery] string Email, [FromQuery] string Phone, [FromQuery] string CompanyName)
        {
            var matchingProduct = await Lead.MatchProduct(_context, PartNumber, Manufacturer, Category, false);
            var quote = new Quote
            {

                LineItems = new List<QuoteLineItem>{
                    new QuoteLineItem{
                        Price = Price,
                        ManufacturerName = Manufacturer,
                        ProductId = matchingProduct.Id,
                        ManufacturerId = matchingProduct.ManufacturerId,
                        Quantity = Quantity,
                        Description = "",
                        LineItemConditionTypeId = 1,
                        DiscountPercent = 0,
                        OutgoingLineItemWarrantyOptionId = 7,
                        OutgoingLineItemLeadTimeOptionId = 5
                        // Condition = 1
                    }
                },
                SalesPersonId = 2,
                CurrencyOptionId = 1,
                BillingAddress = new Address(),
                ShippingAddress = new Address(),
                Email = Email,
                ShippingAndHandlingFee = 0,
                ExpediteFee = 0,
                SalesTax = 0,
                WireTransferFee = 0,
                Expiration = DateTime.UtcNow.AddDays(30),



                Contact = new GidIndustrial.Gideon.WebApi.Models.Contact
                {
                    FirstName = PersonName.Split().First(),
                    LastName = PersonName.Split().Last(),
                    AutoInserted = true,
                    PhoneNumbers = new List<ContactPhoneNumber>{
                        new ContactPhoneNumber{
                            PhoneNumber = new PhoneNumber{
                                Number = Phone
                            }
                        }
                    },
                    EmailAddresses = new List<ContactEmailAddress>{
                        new ContactEmailAddress {
                            EmailAddress = new GidIndustrial.Gideon.WebApi.Models.EmailAddress{
                                Address = Email
                            }
                        }
                    },
                },
                Company = new Company
                {
                    Name = CompanyName,
                    AutoInserted = true,
                    PhoneNumbers = new List<CompanyPhoneNumber>{
                        new CompanyPhoneNumber{
                            PhoneNumber = new PhoneNumber{
                                Number = Phone
                            }
                        }
                    },
                    EmailAddresses = new List<CompanyEmailAddress>{
                        new CompanyEmailAddress {
                            EmailAddress = new GidIndustrial.Gideon.WebApi.Models.EmailAddress{
                                Address = Email
                            }
                        }
                    }
                }
            };

            return await this.PostQuote(quote);
        }



        // GET: Quotes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuote([FromRoute] int id, [FromQuery] bool forOnlineQuote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var query = from q in _context.Quotes select q;

                query = query
                    .Include(m => m.Notes)
                        .ThenInclude(l => l.Note)
                    .Include(m => m.BillingAddress)
                    .Include(m => m.ShippingAddress)
                    .Include(m => m.SalesOrders)
                    .Include(q => q.Company)
                    //.Include(q => q.Addresses)
                    //    .ThenInclude(a => a.Address)
                    .Include(q => q.Attachments)
                        .ThenInclude(q => q.Attachment)
                    .Include(q => q.LineItems)
                        .ThenInclude(li => li.Sources)
                            .ThenInclude(item => item.Source)
                                .ThenInclude(item => item.Currency)
                    .Include(q => q.LineItems)
                        .ThenInclude(li => li.Product)
                            .ThenInclude(j => j.Manufacturer)
                    .Include(q => q.LineItems)
                        .ThenInclude(li => li.Product)
                            .ThenInclude(j => j.ProductType)
                     .Include(q => q.EventLogEntries)
                        .ThenInclude(q => q.EventLogEntry)
                    .Include(item => item.CurrencyOption);

                if (forOnlineQuote)
                {
                    query = query
                        .Include(m => m.BillingAddress)
                            .ThenInclude(m => m.Country)
                        .Include(item => item.GidLocationOption)
                            .ThenInclude(item => item.DefaultShippingAddress)
                        .Include(item => item.GidLocationOption)
                            .ThenInclude(item => item.MainAddress)
                        .Include(m => m.ShippingAddress)
                            .ThenInclude(m => m.Country)
                        .Include(item => item.Contact)
                        .Include(item => item.LineItems)
                            .ThenInclude(item => item.Condition)
                        .Include(item => item.LineItems)
                            .ThenInclude(item => item.Warranty)
                        .Include(item => item.LineItems)
                            .ThenInclude(item => item.Service)
                        .Include(item => item.LineItems)
                            .ThenInclude(item => item.LeadTime)
                        .Include(item => item.CurrencyOption)
                        .Include(item => item.SalesPerson);
                }

                var quote = await query
                    .SingleOrDefaultAsync(m => m.Id == id);

                if (quote == null)
                {
                    return NotFound();
                }

                // Sort line items
                await quote.SortLineItems();

                // Fix line items
                foreach (QuoteLineItem li in quote.LineItems)
                {
                    if (li.Product == null)
                        li.Product = new Product();

                    if (li.Product.ProductType == null)
                        li.Product.ProductType = new ProductType();

                    if (li.Product.Manufacturer == null)
                        li.Product.Manufacturer = new Company();

                    if (li.ManufacturerName == null)
                        li.ManufacturerName = li.Product.Manufacturer.Name;
                }

                return Ok(quote);
            }
            catch (Exception ex)
            {
                string t = "";
            }

            return Ok(null);
        }


        // GET: Quotes/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetQuoteLineItems([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Quote quote = await _context.Quotes
                .Include(m => m.LineItems)
                    .ThenInclude(li => li.Sources)
                        .ThenInclude(s => s.Source)
                            .ThenInclude(item => item.Currency)
                // .Include(m => m.LineItems)
                //     .ThenInclude(item => item.Product)
                .Include(m => m.Company)
                .FirstOrDefaultAsync(l => l.Id == id);

            // Sort line items
            await quote.SortLineItems();

            return Ok(quote.LineItems);
        }

        /// <summary>
        /// Gets a link for the selected quote from DJ's api
        /// </summary>
        /// <returns></returns>
        // [HttpGet("{id}/QuoteLink")]
        public async Task<string> GetQuoteLink([FromRoute] int id)
        {
            var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id);
            if (quote == null)
            {
                return null;
            }
            var baseUrl = "https://forms.gidindustrial.com/quote/";
#if DEBUG
            baseUrl = "http://localhost:50068/quote/";
#endif
            if (!String.IsNullOrWhiteSpace(quote.QuoteFormLink))
            {
                return quote.QuoteFormLink;
            }
            string GidApiKey = this._configuration.GetValue<string>("GidIndustrialApiKey");

            string url = this.BaseGidQuoteUrl + quote.Id.ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Set("Api-Key", GidApiKey);
            request.Headers.Set("User-Agent", "bob");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)
            await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                quote.QuoteFormLink = baseUrl + await reader.ReadToEndAsync();
            }

            _context.Entry(quote).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return quote.QuoteFormLink;
        }


        /// <summary>
        /// This method generates a pdf of the quote
        /// </summary>
        /// <param name="id">The id of the quote</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateQuotePdf")]
        public async Task GenerateQuotePdf([FromRoute] int id)
        {

            var quote = await _context.Quotes
                .Include(q => q.Contact)
                .Include(item => item.Company)
                //.Include(q => q.Addresses)
                //    .ThenInclude(a => a.Address)
                .Include(item => item.PaymentTerm)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(j => j.Manufacturer)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Condition)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.LeadTime)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Warranty)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingMethod)
                .Include(q => q.SalesPerson)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);



            if (quote == null)
            {
                Response.StatusCode = 400;
                return;
            }

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Common\Views\gid-industrial-logo.png";
            ViewData["LogoUrlNew"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer.png";
            ViewData["LogoUrlNew2"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer2.png";
            ViewData["Font1"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Regular.ttf";
            ViewData["Font2"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Medium.ttf";
            ViewData["Font3"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-SemiBold.ttf";
            ViewData["Font4"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Bold.ttf";

            ViewData["IsGidEurope"] = await quote.CheckIfIsGidEurope(_context);

            // Sort quote line items
            await quote.SortLineItems();

            try
            {
                // Render html
                string quoteHtml = viewRenderer.Render("~/Features/Quote/Views/QuotePdfNew.cshtml", quote, ViewData);

                // Create initial pdf
                HtmlToPdf converter = new HtmlToPdf();
                //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
                SelectPdf.PdfDocument pdfDocument = converter.ConvertHtmlString(quoteHtml);

                // Remove last blank page
                if (pdfDocument.Pages.Count > 1 && pdfDocument.Pages[pdfDocument.Pages.Count - 1].ClientRectangle.Location.IsEmpty)
                {
                    // pdfDocument.RemovePageAt(pdfDocument.Pages.Count - 1);  
                }

                // Render t&c html
                string tcHtml = viewRenderer.Render("~/Features/Common/Views/TermsAndConditionsPdf.cshtml", quote.GidLocationOption, ViewData);

                // Create initial pdf
                SelectPdf.PdfDocument pdfTCDocument = converter.ConvertHtmlString(tcHtml);

                // Create original appeneded doc
                SelectPdf.PdfDocument pdfDocumentAppended = new SelectPdf.PdfDocument();

                // Remove blank page if needed

                pdfDocumentAppended.Append(pdfDocument);
                pdfDocumentAppended.Append(pdfTCDocument);

                byte[] dataReturn = pdfDocumentAppended.Save();

                /*
                byte[] dataOriginal = pdfDocument.Save();
                


                // Set vars
                byte[] dataTrimmed;
                SelectPdf.PdfDocument pdfDocument2;
                SelectPdf.PdfDocument pdfComplete;
                SelectPdf.PdfDocument pdfDocument2Appended = new SelectPdf.PdfDocument();

                if (pdfDocument.Pages.Count > 1)
                {
                    pdfDocument2 = converter.ConvertHtmlString(quoteHtml);
                    pdfDocument2.RemovePageAt(pdfDocument2.Pages.Count - 1);
                    pdfDocument2Appended.Append(pdfDocument2);
                    pdfDocument2Appended.Append(pdfTCDocument);
                    dataTrimmed = pdfDocument2.Save();
                    
                }
                else
                {
                    dataTrimmed = dataOriginal;
                    pdfDocument2 = pdfDocument;
                    pdfDocument2Appended.Append(pdfDocument2);
                    pdfDocument2Appended.Append(pdfTCDocument);
                }
                
                byte[] dataReturn;

                // Remove last page if blank
                using (PdfReader reader = new PdfReader(dataTrimmed))
                {
                    string page = PdfTextExtractor.GetTextFromPage(reader, pdfDocument2.Pages.Count);

                    if (page.Contains("Terms and Conditions"))
                    {
                        pdfComplete = pdfDocument2;
                        dataReturn = pdfDocument2Appended.Save();
                        
                    }
                    else
                    {
                        pdfComplete = pdfDocument;
                        dataReturn = pdfDocumentAppended.Save();
                        
                    }   
                }

                // Close documents
                pdfDocument.Close();
                pdfDocument2.Close();
                pdfDocumentAppended.Close();
                pdfDocument2Appended.Close();
                pdfTCDocument.Close();

                */

                //var doc = new HtmlToPdfDocument() {
                //    GlobalSettings = {
                //        ColorMode = ColorMode.Color,
                //        Orientation = Orientation.Portrait,
                //        PaperSize = PaperKind.A4,
                //    },
                //    Objects = {
                //        new ObjectSettings() {
                //            PagesCount = true,
                //            HtmlContent = quoteHtml,
                //            WebSettings = { DefaultEncoding = "utf-8" },
                //            // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                //        }
                //    }
                //};
                //byte[] data = PdfConverter.Convert(doc); 
                // Response.Headers.ContentType = "application/pdf";
                Response.ContentType = "application/pdf";
                await Response.Body.WriteAsync(dataReturn, 0, dataReturn.Length);
            }
            catch (Exception ex)
            {
                string t = "";
            }

            return;
        }


        /// <summary>
        /// This method generates a pdf of the quote
        /// </summary>
        /// <param name="id">The id of the quote</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateQuotePdfResponse")]
        public async Task<HttpResponse> GenerateQuotePdfResponse([FromRoute] int id)
        {

            var quote = await _context.Quotes
                .Include(q => q.Contact)
                .Include(item => item.Company)
                //.Include(q => q.Addresses)
                //    .ThenInclude(a => a.Address)
                .Include(item => item.PaymentTerm)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Condition)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.LeadTime)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Warranty)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingMethod)
                .Include(q => q.SalesPerson)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);



            if (quote == null)
            {
                Response.StatusCode = 400;
                return null;
            }

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await quote.CheckIfIsGidEurope(_context);

            // Sort quote line items
            await quote.SortLineItems();

            string quoteHtml = viewRenderer.Render("~/Features/Quote/Views/QuotePdf.cshtml", quote, ViewData);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = quoteHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);
            // Response.Headers.ContentType = "application/pdf";
            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            return Response;
        }



        // GET: Quotes/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query)
        {
            return _context.Quotes
                 .Where(quote => quote.Id == int.Parse(query))
                 .Select(quote => new
                 {
                     Id = quote.Id,
                     Name = quote.Id
                 });
        }

        /// <summary>
        /// This method generates a pdf of the quote
        /// </summary>
        /// <param name="id">The id of the quote</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateProFormaInvoicePdf")]
        public async Task GenerateProFormaInvoicePdf([FromRoute] int id, [FromQuery] List<int?> quoteLineItemIds)
        {

            var quote = await _context.Quotes
                .Include(q => q.Contact)
                .Include(item => item.Company)
                //.Include(q => q.Addresses)
                //    .ThenInclude(a => a.Address)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Condition)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Warranty)
                .Include(q => q.SalesPerson)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingMethod)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (quote == null)
            {
                Response.StatusCode = 400;
                return;
            }
            quote.LineItems = quote.LineItems.Where(item => quoteLineItemIds.Contains(item.Id)).ToList();

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await quote.CheckIfIsGidEurope(_context);
            string quoteHtml = viewRenderer.Render("~/Features/Quote/Views/ProFormaInvoicePdf.cshtml", quote, ViewData);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = quoteHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);
            // Response.Headers.ContentType = "application/pdf";
            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            return;
        }


        // PUT: Quotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuote([FromRoute] int id, [FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quote.Id)
            {
                return BadRequest();
            }

            var previousQuote = await _context.Quotes.AsNoTracking().Include(item => item.SalesPerson).FirstOrDefaultAsync(item => item.Id == id);


            if (quote.ShippingAddress != null && quote.ShippingAddress.Id != null)
                _context.Entry(quote.ShippingAddress).State = EntityState.Modified;
            else if (quote.ShippingAddress != null)
                _context.Entry(quote.ShippingAddress).State = EntityState.Added;

            if (quote.BillingAddress != null && quote.BillingAddress.Id != null)
                _context.Entry(quote.BillingAddress).State = EntityState.Modified;
            else if (quote.BillingAddress != null)
                _context.Entry(quote.BillingAddress).State = EntityState.Added;

            _context.Entry(quote).State = EntityState.Modified;

            var updatedEvent = "Updated";
            if (previousQuote.QuoteStatusOptionId != quote.QuoteStatusOptionId)
            {
                var status = await _context.QuoteStatusOptions.FirstOrDefaultAsync(item => item.Id == quote.QuoteStatusOptionId);
                updatedEvent += " Status - " + (status != null ? status.Value : "None");
            }


            if (previousQuote.SalesPersonId != quote.SalesPersonId)
            {
                var previousSalesPersonName = previousQuote.SalesPerson != null ? previousQuote.SalesPerson.DisplayName : "";
                var currentSalesPersonName = "";
                if (quote.SalesPersonId == null)
                {
                    currentSalesPersonName = "Nobody";
                }
                else
                {
                    var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == quote.SalesPersonId);
                    if (user == null)
                    {
                        currentSalesPersonName = "Nobody";
                    }
                    else
                    {
                        currentSalesPersonName = user.DisplayName;
                    }
                }
                updatedEvent += $" Changed Sales Person from {previousSalesPersonName} to {currentSalesPersonName}";
            }

            var newEventLogEntry = new EventLogEntry
            {
                Event = updatedEvent,
                CreatedAt = DateTime.UtcNow,
                OccurredAt = DateTime.UtcNow,
                CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
            };
            _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry
            {
                Quote = quote,
                EventLogEntry = newEventLogEntry
            });

            try
            {
                await quote.UpdateTotal(_context);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: Quotes
        [HttpPost]
        public async Task<IActionResult> PostQuote([FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (quote.QuoteStatusOptionId == null)
            {
                var pendingStatus = await _context.QuoteStatusOptions.FirstAsync(item => item.Value == "Pending");
                quote.QuoteStatusOptionId = pendingStatus.Id;
            }

            foreach (var li in quote.LineItems)
            {
                li.Product = null;
            }

            quote.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            _context.Quotes.Add(quote);

            //add to quote event log entry the fact that it was created and who created it
            _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry
            {
                Quote = quote,
                EventLogEntry = new EventLogEntry
                {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            //if this quote is coming from a lead, add to lead event log entries table
            if (quote.LeadId != null && quote.LeadId != 0)
            {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry
                {
                    LeadId = quote.LeadId,
                    EventLogEntry = new EventLogEntry
                    {
                        Event = "Converted to Quote",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
            }

            await quote.UpdateTotal(_context);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string t = "";
            }



            //get a link to quote from DJ's api
            quote.QuoteFormLink = await this.GetQuoteLink(quote.Id);

            return CreatedAtAction("GetQuote", new { id = quote.Id }, quote);
        }

        // POST: Quotes/SendQuote
        [HttpPost("SendQuote")]
        public async Task<IActionResult> SendQuote([FromBody] SendQuoteData sendQuoteData)
        {
            var emailParameters = sendQuoteData.EmailParameters;
            var quote = sendQuoteData.Quote;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null)
            {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var sentStatus = await _context.QuoteStatusOptions.FirstAsync(item => item.Value == "Sent");
            quote.QuoteStatusOptionId = sentStatus.Id;
            _context.Entry(quote).State = EntityState.Modified;

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
            {
                _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry
                {
                    QuoteId = sendQuoteData.Quote.Id,
                    EventLogEntry = new EventLogEntry
                    {
                        Event = "Sent Quote",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new
                {
                    Error = "Error sending email. Status code was wrong"
                });
            }

            return Ok(new
            {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        // POST: Quotes/SenProFormaInvoice
        [HttpPost("SendProFormaInvoice")]
        public async Task<IActionResult> SendProFormaInvoice([FromBody] SendQuoteData sendQuoteData)
        {
            var emailParameters = sendQuoteData.EmailParameters;
            var quote = sendQuoteData.Quote;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null)
            {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
            {
                _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry
                {
                    QuoteId = sendQuoteData.Quote.Id,
                    EventLogEntry = new EventLogEntry
                    {
                        Event = "Sent Pro Forma Invoice",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new
                {
                    Error = "Error sending email. Status code was wrong"
                });
            }
            return Ok(new
            {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        // DELETE: Quotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quote = await _context.Quotes
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .Include(item => item.ContactLogItems)
                    .ThenInclude(item => item.ContactLogItem)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (quote == null)
            {
                return NotFound();
            }

            foreach (var quoteAttachment in quote.Attachments.ToList())
            {
                await quoteAttachment.Attachment.Delete(_context, _configuration);
            }

            _context.ContactLogItems.RemoveRange(quote.ContactLogItems.Select(item => item.ContactLogItem));
            _context.Quotes.Remove(quote);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(quote);
        }

        private bool QuoteExists(int id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }
    }
}