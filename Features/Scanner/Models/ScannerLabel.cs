using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.IO;
using QRCoder;
using System.Drawing;


namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerLabel {

        public ScannerLabel(){
            this.BarcodeGuid = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }
        
        public int? CreatedById { get; set; }
        [Required]
        public Guid? BarcodeGuid { get; set; }

        [Required]
        public int? ScannerLabelTypeId { get; set; }
        public ScannerLabelType ScannerLabelType { get; set; }

        public int? UserId { get; set; }

        public int? ScannerId { get; set; }

        public EndScannerLabel EndScannerLabel { get; set; }

        public List<ScannerLabelVariableValue> VariableValues { get; set; }

        public static async Task<ScannerLabel> GetScannerLabel<T>(T item, AppDBContext _context, System.Security.Claims.ClaimsPrincipal createdBy = null){
            var scannerLabelType = await ScannerLabelType.GetScannerLabelTypeForClass(typeof(T), _context, createdBy);
            var id = item.GetType().GetProperty("Id").GetValue(item, null).ToString();

            var scannerLabel = await _context.ScannerLabels.FirstOrDefaultAsync(label => 
                label.ScannerLabelTypeId == scannerLabelType.Id
                && label.VariableValues.Any(v => v.Value == id)
            );
            if(scannerLabel != null){
                return scannerLabel;
            }
            scannerLabel = new ScannerLabel{
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdBy != null ? GidIndustrial.Gideon.WebApi.Models.User.GetId(createdBy) : null,
                ScannerLabelTypeId = scannerLabelType.Id,
                VariableValues = new List<ScannerLabelVariableValue>{
                    new ScannerLabelVariableValue{
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = createdBy != null ? GidIndustrial.Gideon.WebApi.Models.User.GetId(createdBy) : null,
                        ScannerLabelTypeVariableId = scannerLabelType.Variables.First().Id,
                        Value = id
                    }
                }
            };
            _context.Add(scannerLabel);
            await _context.SaveChangesAsync();
            return scannerLabel;
        }

        public async Task<string> GetQRCodeDataUrl(){
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(this.Id.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var img = stream.ToArray();
                return "data:image/png;base64," + Convert.ToBase64String(img);
            }
        }

        public async Task<string> GetDataMatrixBarcodeDataUrl(){
            var barcode = Barcoder.DataMatrix.DataMatrixEncoder.Encode(this.Id.ToString());
            var renderer = new Barcoder.Renderer.Image.ImageRenderer();
            using (var stream = new MemoryStream())
            {
                renderer.Render(barcode, stream);
                var img = stream.ToArray();
                return "data:image/png;base64," + Convert.ToBase64String(img);
            }
        }

        public string GetVariableValueByObjectNameAndObjectField(string objectName, string objectField) {
            var variableIds = this.ScannerLabelType.Variables.Where(item => item.ObjectName == objectName && item.ObjectField == objectField).Select(item => item.Id);
            var variable = this.VariableValues.Where(item => variableIds.Contains(item.ScannerLabelTypeVariableId)).FirstOrDefault();
            if(variable == null){
                return null;
            }
            return variable.Value;
        }

    }
    class ScannerLabelDBConfiguration : IEntityTypeConfiguration<ScannerLabel> {
        public void Configure(EntityTypeBuilder<ScannerLabel> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasOne(item => item.ScannerLabelType).WithMany().HasForeignKey(item => item.ScannerLabelTypeId);

            modelBuilder.HasMany(item => item.VariableValues).WithOne().HasForeignKey(item => item.ScannerLabelId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.EndScannerLabel).WithOne(item => item.StartScannerLabel).HasForeignKey<EndScannerLabel>(item => item.StartScannerLabelId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    enum SpecialScannerLabels {
        END = 1
    }
}