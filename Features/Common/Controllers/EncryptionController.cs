using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Text;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [Route("Encryptions")]
    public class EncryptionsController : Controller
    {
        // [HttpPost("Encrypt")]
        // public IActionResult Encrypt([FromBody] string ToEncrypt){
        //     return Ok(Encryption.EncryptData(ToEncrypt));
        // }
        // [HttpPost("Decrypt")]
        // public IActionResult Decrypt([FromBody] string Encrypted){
        //     return Ok(Encryption.DecryptString(Encrypted));
        // }
    }
}