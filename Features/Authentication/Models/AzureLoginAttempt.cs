/**
The purpose of this class is simply to generate a random token to prevent csrf when doing open id authentication
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Controllers;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi {
    public class AzureLoginAttempt {
        public static int NonceValidMinutes = 10;
        public static bool CheckIfLoginAttemptValid(string nonce) {
            //filter out old login attempts
            LoginAttempts = LoginAttempts.Where(item => item.CreatedAt.AddMinutes(NonceValidMinutes) > DateTime.UtcNow).ToList();
            return LoginAttempts.FirstOrDefault(item => item.Nonce == nonce) != null ? true : false;
        }
        public static string CreateLoginAttempt() {
            var newLoginAttempt = new AzureLoginAttempt {
                CreatedAt = DateTime.UtcNow,
                Nonce = AuthenticationController.GenerateSecureToken()
            };
            LoginAttempts.Add(newLoginAttempt);
            //filter out old login attempts
            LoginAttempts = LoginAttempts.Where(item => item.CreatedAt.AddMinutes(NonceValidMinutes) > DateTime.UtcNow).ToList();
            return newLoginAttempt.Nonce;
        }

        public static List<AzureLoginAttempt> LoginAttempts = new List<AzureLoginAttempt> { };

        public DateTime CreatedAt { get; set; }
        public string Nonce { get; set; }
    }
}