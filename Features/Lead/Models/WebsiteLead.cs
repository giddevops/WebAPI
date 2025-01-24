// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models
// {
//     public class WebsiteLead
//     {
//         public int Id { get; set; }
//         public DateTime? CreatedAt { get; set; }
//         public string Notes { get; set; }
//         public int? Quality { get; set; }
//         public bool? AutoResponseSent { get; set; }
//         public string Address1 { get; set; }
//         public string Address2 { get; set; }
//         public string Address3 { get; set; }
//         public string City { get; set; }
//         public string PostalCode { get; set; }
//         public string Comments { get; set; }
//         public string Company { get; set; }
//         public string CountryCode { get; set; }
//         public string CountryName { get; set; }
//         public string CustomerType { get; set; }
//         public string DunsNumber { get; set; }
//         public string Email { get; set; }
//         public string FullName { get; set; }
//         public string Phone { get; set; }
//         public string Position { get; set; }
//         public string RegionName { get; set; }
//         public string Website { get; set; }
//         public string Category { get; set; }
//         public string Manufacturer { get; set; }
//         public string PartNumber { get; set; }
//         public int? Quantity { get; set; }
//         public int? RequiredDeliveryWeeks { get; set; }
//         public string Service { get; set; }
//         public string AdditionalData { get; set; }
//         public string BrowserName { get; set; }
//         public string BrowserVersion { get; set; }
//         public string IPAddress { get; set; }
//         public string Origin { get; set; }
//         public string Key { get; set; }
//         public string ReferrerUrl { get; set; }
//         public string SourceUrl { get; set; }
//         public string UserAgent { get; set; }
//         public string WebServerName { get; set; }
//         public string GeolocationCity { get; set; }
//         public string GeolocationCountryCode { get; set; }
//         public string GeolocationCountryName { get; set; }
//         public decimal GeolocationLatitude { get; set; }
//         public decimal GeolocationLongitude { get; set; }
//         public string GeolocationPostalCode { get; set; }
//         public string GeolocationRegionCode { get; set; }
//         public string GeolocationRegionName { get; set; }
//         public string GeolocationTimeZone { get; set; }

//         public Lead ToLead(){
//             return new Lead{
//                 CreatedAt = this.CreatedAt,
//                 NotesText = this.Notes,
//                 Quality = this.Quality,
//                 AutoResponseSent = this.AutoResponseSent,
//                 Address1 = this.Address1,
//                 Address2 = this.Address2,
//                 Address3 = this.Address3,
//                 City = this.City,
//                 Comments = this.Comments,
//                 CompanyName = this.Company,
//                 CountryName = this.CountryName,
//                 DunsNumber = this.DunsNumber,
//                 Email = this.Email,
//                 FullName = this.FullName,
//                 Phone = this.Phone,
//                 Position = this.Position,
//                 ZipPostalCode = this.PostalCode,
//                 RegionName = this.RegionName,
//                 Website = this.Website,
//                 Category = this.Category,
//                 Manufacturer = this.Manufacturer,
//                 PartNumber = this.PartNumber,
//                 Quantity = this.Quantity,
//                 Service = this.Service,
//                 AdditionalData = this.AdditionalData,
//                 BrowserName = this.BrowserName,
//                 IPAddress = this.IPAddress,
//                 OriginText = this.Origin,
//                 Key = this.Key,
//                 ReferrerUrl = this.ReferrerUrl,
//                 SourceUrl = this.SourceUrl,
//                 UserAgent = this.UserAgent,
//                 WebServerName = this.WebServerName,
//                 GeolocationCity = this.GeolocationCity,
//                 GeolocationCountryCode = this.GeolocationCountryCode,
//                 GeolocationCountryName = this.GeolocationCountryName,
//                 GeolocationLatitude = this.GeolocationLatitude,
//                 GeolocationLongitude = this.GeolocationLongitude,
//                 GeolocationPostalCode = this.GeolocationPostalCode,
//                 GeolocationRegionCode = this.GeolocationRegionCode,
//                 GeolocationRegionName = this.GeolocationRegionName,
//                 GeolocationTimeZone = this.GeolocationTimeZone
//             };

//             //countryId - CountryCode
//             //customerTypeId - CustomerType
//             //RequiredDeliveryTimeId - RequiredDeliveryWeeks
//         }
//     }
// }