// Not used
using System;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;

namespace GidIndustrial.Gideon.WebApi.Libraries {
    public class LogRequestsMiddleware {
        private readonly RequestDelegate _next;

        public LogRequestsMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            await _next(context);
            var dbContext = (AppDBContext)context.RequestServices.GetService(typeof(AppDBContext));
            var userId = GidIndustrial.Gideon.WebApi.Models.User.GetId(context.User);
            dbContext.AccessLogItems.Add(new AccessLogItem {
                Url = context.Request.Method + " " + context.Request.Path,
                UserId = userId
            });
            await dbContext.SaveChangesAsync();


            //First, get the incoming request
            // var request = await FormatRequest(context.Request);

            //Copy a pointer to the original response body stream
            // var originalBodyStream = context.Response.Body;

            // //Create a new memory stream...
            // using (var responseBody = new MemoryStream()) {
            //     //...and use that for the temporary response body
            //     context.Response.Body = responseBody;

            //     //Continue down the Middleware pipeline, eventually returning to this class
            //     await _next(context);

            //     //Format the response from the server
            //     // var response = await FormatResponse(context.Response);

            //     //TODO: Save log to chosen datastore

            //     //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            //     // await responseBody.CopyToAsync(originalBodyStream);
            // }
        }

        // private async Task<string> FormatRequest(HttpRequest request) {
        //     var body = request.Body;

        //     //This line allows us to set the reader for the request back at the beginning of its stream.
        //     request.EnableRewind();

        //     //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
        //     var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        //     //...Then we copy the entire request stream into the new buffer.
        //     await request.Body.ReadAsync(buffer, 0, buffer.Length);

        //     //We convert the byte[] into a string using UTF8 encoding...
        //     var bodyAsText = Encoding.UTF8.GetString(buffer);

        //     //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
        //     request.Body = body;

        //     return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        // }

        // private async Task<string> FormatResponse(HttpResponse response) {
        //     //We need to read the response stream from the beginning...
        //     response.Body.Seek(0, SeekOrigin.Begin);

        //     //...and copy it into a string
        //     string text = await new StreamReader(response.Body).ReadToEndAsync();

        //     //We need to reset the reader for the response so that the client can read it.
        //     response.Body.Seek(0, SeekOrigin.Begin);

        //     //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
        //     return $"{response.StatusCode}: {text}";
        // }
    }
}