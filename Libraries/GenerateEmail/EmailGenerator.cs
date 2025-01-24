// Not used
using System;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;


namespace GidIndustrial.Gideon.WebApi.Libraries {
    public class EmailGenerator {
        public static IConfiguration configuration; // this is set on startup in the Startup.cs class

        public static SendGridClient GetNewSendGridClient() {
            return new SendGridClient(configuration.GetValue<string>("SENDGRID_API_KEY"));
        }

        public static async Task<SendGridMessage> GenerateEmail(AppDBContext _context, EmailGeneratorParameters parameters) {

            var msg = new SendGridMessage();

            if (String.IsNullOrWhiteSpace(parameters.To)) {
                throw new Exception("To field is required");
            }
            var toAddresses = parameters.To.Split(",");
            foreach (var toAddress in toAddresses) {
                var toAddressTrimmed = toAddress.Trim();
                msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(toAddressTrimmed));
            }



            if (String.IsNullOrWhiteSpace(parameters.From)) {
                throw new Exception("From field is required");
            }
            var fromName = "GID Industrial";
            if (!string.IsNullOrWhiteSpace(parameters.FromName))
                fromName = parameters.FromName;
            var from = new SendGrid.Helpers.Mail.EmailAddress(parameters.From, fromName);
            msg.From = from;

            msg.Subject = parameters.Subject;
            msg.HtmlContent = parameters.HtmlContent;

            if (!String.IsNullOrWhiteSpace(parameters.CC)) {
                var addresses = parameters.CC.Split(",");
                foreach (var address in addresses) {
                    var addressTrimmed = address.Trim();
                    msg.AddCc(new SendGrid.Helpers.Mail.EmailAddress(addressTrimmed));
                }
                // var cc = new SendGrid.Helpers.Mail.EmailAddress(parameters.CC);
                // msg.AddCc(cc);
            }

            if (!String.IsNullOrWhiteSpace(parameters.Bcc)) {
                var addresses = parameters.Bcc.Split(",");
                foreach (var address in addresses) {
                    var addressTrimmed = address.Trim();
                    msg.AddBcc(new SendGrid.Helpers.Mail.EmailAddress(addressTrimmed));
                }
                // var bcc = new SendGrid.Helpers.Mail.EmailAddress(parameters.Bcc);
                // msg.AddBcc(bcc);
            }

            //add any attachments to email
            //all attachments, include the pdf quote form are in the Attachments array, containing Id and Name
            //Need to first fetch the body of the attachment from azure, then add it to the message
            if (parameters.Attachments != null) {
                foreach (var a in parameters.Attachments) {
                    //Fetch attachment body
                    var attachment = await _context.Attachments.FirstOrDefaultAsync(dbAttachment => dbAttachment.Id == a.Id);
                    var attachmentBody = await attachment.GetBody(configuration.GetConnectionString("AzureBlobStorage"));
                    //add attachment to email
                    msg.AddAttachment(a.Name, Convert.ToBase64String(attachmentBody),
                      "application/pdf",
                      "attachment",
                      "PurchaseOrder");
                }
            }

            return msg;
        }
    }
}
