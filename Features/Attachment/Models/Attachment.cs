using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Attachment {

        public static readonly HttpClient HttpClient = new HttpClient();

        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public long Size { get; set; }
        public int? AttachmentTypeId { get; set; }

        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }

        public string Uri { get; set; }
        public Boolean Confirmed { get; set; }
        public string ContentType { get; set; }

        [NotMapped]
        public string UploadUrl { get; set; }

        [NotMapped]
        public string ViewingUrl { get; set; }

        public string OfficialFilename { get; set; }

        public List<LeadAttachment> LeadAttachments { get; set; }
        public List<QuoteAttachment> QuoteAttachments { get; set; }
        public List<ContactAttachment> ContactAttachments { get; set; }
        public List<CompanyAttachment> CompanyAttachments { get; set; }
        public List<ProductAttachment> ProductAttachments { get; set; }
        public List<SourceAttachment> SourceAttachments { get; set; }
        public List<InventoryItemAttachment> InventoryItemAttachments { get; set; }
        public List<SalesOrderAttachment> SalesOrderAttachments { get; set; }
        public List<RmaAttachment> RmaAttachments { get; set; }
        public List<BillAttachment> BillAttachments { get; set; }
        public List<InvoiceAttachment> InvoiceAttachments { get; set; }
        public List<Repair> Repairs { get; set; }
        public List<IncomingShipmentAttachment> IncomingShipmentAttachments { get; set; }

        public async Task UploadAttachment(byte[] data) {
            var content = new ByteArrayContent(data);
            content.Headers.Add("x-ms-blob-type", "BlockBlob");
            content.Headers.Add("Content-Type",this.ContentType);
            var response = await HttpClient.PutAsync(this.UploadUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) {
                throw new Exception("Received an invalid status code " + response.StatusCode + " message was " + responseString);
            }
        }

        public static async Task<Attachment> CreateNewAuthorizedAttachment(string azureStorageConnectionString, long fileSize, string fileName, string contentType, int? attachmentTypeId) {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
            await container.CreateIfNotExistsAsync();

            string officialFilename = Guid.NewGuid().ToString() + "/" + fileName;

            CloudBlockBlob blob = container.GetBlockBlobReference(officialFilename);

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;
            // sasConstraints.

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            var newAttachment = new Attachment {
                Size = fileSize,
                Name = fileName,
                OfficialFilename = officialFilename,
                ContentType = contentType,
                AttachmentTypeId = attachmentTypeId,
                Confirmed = false,
                UpdatedAt = new DateTime(),
                Uri = blob.Uri.ToString(),
                UploadUrl = blob.Uri + sasBlobToken
            };
            return newAttachment;
        }


        public async Task<byte[]> GetBody(string blobStorageConnectionString) {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");

            CloudBlockBlob blob = container.GetBlockBlobReference(this.OfficialFilename);
            await blob.FetchAttributesAsync();

            byte[] returnData = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(returnData, 0);

            return returnData;
        }

        public async Task Delete(AppDBContext _context, IConfiguration configuration) {
            //first make sure this attachment is referenced by only one item.
            //to do this, find all references. If the number of references == 1, go ahead and delete it in the cloud
            int totalNumReferences =
            _context.LeadAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.QuoteAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.ContactAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.CompanyAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.ProductAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.SalesOrderAttachments.Where(item => item.AttachmentId == this.Id).Count() +
            _context.PurchaseOrderAttachments.Where(item => item.AttachmentId == this.Id).Count() +
            _context.RmaAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.BillAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.InvoiceAttachments.Where(l => l.AttachmentId == this.Id).Count() +
            _context.InventoryItemAttachments.Where(item => item.AttachmentId == this.Id).Count() +
            _context.IncomingShipmentAttachments.Where(item => item.AttachmentId == this.Id).Count();


            if (totalNumReferences == 0) {
                throw new Exception("Error - the total number of references to this attachment is 0. It should never be below 1");
            }
            if (totalNumReferences == 1) {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureBlobStorage"));
                //Create the blob client object.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //Get a reference to a container to use for the sample code, and create it if it does not exist.
                CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
                await container.CreateIfNotExistsAsync();

                CloudBlockBlob blob = container.GetBlockBlobReference(this.OfficialFilename);
                await blob.DeleteAsync();

                _context.Attachments.Remove(this);
                await _context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// set db configuration
    /// </summary>
    class AttachmentDBConfiguration : IEntityTypeConfiguration<Attachment> {
        public void Configure(EntityTypeBuilder<Attachment> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.CreatedBy)
                .WithMany()
                .HasForeignKey(l => l.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
