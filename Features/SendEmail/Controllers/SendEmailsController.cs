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

namespace WebApi.Features.Controllers {
    
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("SendEmails")]
    public class SendEmailsController : Controller {
        private readonly AppDBContext _context;
        // private readonly string SendgridApiKey;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        public string BaseGidSendEmailUrl = "https://forms.gidindustrial.com/key/quote/";

        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate quotes
        private readonly ViewRender viewRenderer;

        public SendEmailsController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter) {
            _context = context;
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }

        // POST: SendEmails/SendSendEmail
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailData sendEmailData) {
            var emailParameters = sendEmailData.EmailParameters;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                // _context.SendEmailEventLogEntries.Add(new SendEmailEventLogEntry {
                //     SendEmailId = sendEmailData.SendEmail.Id,
                //     EventLogEntry = new EventLogEntry {
                //         Event = "Sent SendEmail",
                //         CreatedAt = DateTime.UtcNow,
                //         OccurredAt = DateTime.UtcNow,
                //         CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                //         UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                //     }
                // });
                // await _context.SaveChangesAsync();
            } else {
                return BadRequest(new {
                    Error = "Error sending email. Status code was wrong"
                });
            }
            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }
    }
}