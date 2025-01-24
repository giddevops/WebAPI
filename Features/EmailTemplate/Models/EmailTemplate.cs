using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class EmailTemplate {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Subject { get; set; }
        public string HtmlContent { get; set; }

        public int? EmailTemplateTypeId { get; set; }

        public void ReplaceVariables(AppDBContext _context, int? userId) {
            this.HtmlContent = EmailTemplate.ReplaceVariables(_context, userId, this.HtmlContent);
            this.Subject = EmailTemplate.ReplaceVariables(_context, userId, this.Subject);
        }

        public static string ReplaceVariables(AppDBContext _context, int? userId, string templateText) {
            var user = _context.Users
                .Include(item => item.DefaultGidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                        .ThenInclude(item => item.Country)
                .FirstOrDefault(theUser => theUser.Id == userId);

            if (user == null) {
                throw new Exception("User is null in email template");
            }

            Regex regex = new Regex(@"\[\[([a-z0-9_-]+?)\]\]", RegexOptions.IgnoreCase);
            templateText = regex.Replace(templateText, match => {
                switch (match.Groups[1].Value) {
                    case "UserDisplayName":
                        return String.IsNullOrWhiteSpace(user.DisplayName) ? user.FirstName : user.DisplayName;
                    case "UserFirstName":
                        return user.FirstName;
                    case "UserLastName":
                        return user.LastName;
                    case "UserWorkPhone":
                        return user.WorkPhone;
                    case "UserMobilePhone":
                        return user.MobilePhone;
                    case "UserJobTitle":
                        return user.JobTitle;

                    case "UserAddressAttention":
                        return user.DefaultGidLocationOption.MainAddress.Attention;
                    case "UserAddress1":
                        return user.DefaultGidLocationOption.MainAddress.Address1;
                    case "UserAddress2":
                        return user.DefaultGidLocationOption.MainAddress.Address2;
                    case "UserAddress3":
                        return user.DefaultGidLocationOption.MainAddress.Address3;
                    case "UserAddressCity":
                        return user.DefaultGidLocationOption.MainAddress.City;
                    case "UserAddressState":
                        return user.DefaultGidLocationOption.MainAddress.State;
                    case "UserAddressZipPostalCode":
                        return user.DefaultGidLocationOption.MainAddress.ZipPostalCode;
                    case "UserAddressCountry":
                        return user.DefaultGidLocationOption.MainAddress.Country.Value;

                    case "UserEmail":
                        return user.Email;
                    case "UserDefaultGidLocationCompanyName":
                        return user.DefaultGidLocationOption != null ? user.DefaultGidLocationOption.Value : "";
                    case "UserDefaultGidLocationAddress":
                        if (user.DefaultGidLocationOption == null || user.DefaultGidLocationOption.MainAddress == null)
                            return "";
                        return @"
<div>${ user.DefaultGidLocationOption.MainAddress.Name || ''}</div>
<div>${ user.DefaultGidLocationOption.MainAddress.Address1 || ''}</div>
<div>${ user.DefaultGidLocationOption.MainAddress.Address2 || ''}</div>
<div>${ user.DefaultGidLocationOption.MainAddress.Address3 || ''}</div>
<div>${ user.DefaultGidLocationOption.MainAddress.City || ''}, ${ user.DefaultGidLocationOption.MainAddress.State || ''} ${ user.DefaultGidLocationOption.MainAddress.ZipPostalCode || ''}</div>
";
                }
                return match.Groups[0].Value;
            });

            var ifThenRegex = new Regex(@"\[\?:([a-z0-9_-]+)=?([a-z0-9\-_]*?)\](.*?)\[\/\?]", RegexOptions.IgnoreCase);
            templateText = ifThenRegex.Replace(templateText, match => {
                var tagName = match.Groups[1].Value;
                var tagValue = match.Groups[2].Value;
                var interiorText = match.Groups[3].Value;

                switch (tagName) {
                    case "UserDefaultGidLocationOptionId":
                        if (user.DefaultGidLocationOptionId.ToString() == tagValue) {
                            return interiorText;
                        }
                        break;
                    case "UserWorkPhone":
                        if (!String.IsNullOrWhiteSpace(user.WorkPhone)) {
                            return interiorText;
                        }
                        break;
                    case "UserMobilePhone":
                        if (!String.IsNullOrWhiteSpace(user.MobilePhone)) {
                            return interiorText;
                        }
                        break;
                }
                return "";
            });

            return templateText;

        }
    }

}