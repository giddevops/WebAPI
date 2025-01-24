using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScanGroup {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        [Required]
        public DateTime? LastScanOccurredAt { get; set; }

        public List<Scan> Scans { get; set; }

        public bool Open { get; set; }

        [Required]
        public int? ScannerId { get; set; }

        [NotMapped]
        public ScannerLabelType ScannerLabelType { get; set; }

        public async Task<ScannerLabelType> GetEventLabelType(AppDBContext context, ScannerStation scannerStation) {
            this.Scans = this.Scans.OrderBy(item => item.CreatedAt).ToList();
            var first = this.Scans.First();
            int? scannerEventLabelTypeId;
            if (first.ScannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.EVENT) {
                scannerEventLabelTypeId = first.ScannerLabel.ScannerLabelTypeId;
            } else {
                scannerEventLabelTypeId = scannerStation.DefaultScannerLabelId;
                // return await context.ScannerEvents.FirstOrDefaultAsync(item => item.ScannerEventLabelTypeId == scannerStation.DefaultScannerLabelTypeId);
            }
            var scannerEventLabelType = await context.ScannerLabelTypes
                .Include(item => item.ScannerActionUpdateLocation)
                .Include(item => item.ScannerActionRelatePieceParts)
                .Include(item => item.ScannerActionUpdateSystemData)
                    .ThenInclude(item => item.Commands)
                .Include(item => item.ScannerActionUpdateWorkLog)
                .FirstOrDefaultAsync(item => item.Id == scannerEventLabelTypeId);
            return scannerEventLabelType;
        }
        public List<int?> GetScannerLabelIdsOfType(int? scannerLabelTypeId) {
            return this.Scans.Where(item => item.ScannerLabel.ScannerLabelTypeId == scannerLabelTypeId).Select(item => item.ScannerLabel.ScannerLabelTypeId).ToList();
        }
        public List<int?> GetAllScannedDataLabelTypeIds() {
            return this.Scans.Where(item => item.ScannerLabel.ScannerLabelType.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA).Select(item => item.ScannerLabel.ScannerLabelTypeId).ToList();
        }
        public bool IsComplete() {
            var isComplete = true;
            if (this.ScannerLabelType.ScannerActionRelatePieceParts != null && this.ScannerLabelType.ScannerActionRelatePieceParts.Active == true) {
                if (!this.ScannerLabelType.ScannerActionRelatePieceParts.CheckIfScanGroupIsComplete(this)) {
                    isComplete = false;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateLocation != null && this.ScannerLabelType.ScannerActionUpdateLocation.Active == true) {
                if (!this.ScannerLabelType.ScannerActionUpdateLocation.CheckIfScanGroupIsComplete(this)) {
                    isComplete = false;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateWorkLog != null && this.ScannerLabelType.ScannerActionUpdateWorkLog.Active == true) {
                if (!this.ScannerLabelType.ScannerActionUpdateWorkLog.CheckIfScanGroupIsComplete(this)) {
                    isComplete = false;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateSystemData != null && this.ScannerLabelType.ScannerActionUpdateSystemData.Active == true) {
                if (!this.ScannerLabelType.ScannerActionUpdateSystemData.CheckIfScanGroupIsComplete(this)) {
                    isComplete = false;
                }
            }
            return isComplete;
        }
        public async Task<ScanResponse> CheckIfAllRequiredPartsArePresentAndValid(AppDBContext context) {
            if (this.ScannerLabelType.ScannerActionRelatePieceParts != null && this.ScannerLabelType.ScannerActionRelatePieceParts.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionRelatePieceParts.CheckIfAllRequiredPartsArePresentAndValid(context, this);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateLocation != null && this.ScannerLabelType.ScannerActionUpdateLocation.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateLocation.CheckIfAllRequiredPartsArePresentAndValid(context, this);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateWorkLog != null && this.ScannerLabelType.ScannerActionUpdateWorkLog.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateWorkLog.CheckIfAllRequiredPartsArePresentAndValid(context, this);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateSystemData != null && this.ScannerLabelType.ScannerActionUpdateSystemData.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateSystemData.CheckIfAllRequiredPartsArePresentAndValid(context, this);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }
        public async Task<ScanResponse> Commit(AppDBContext context, ScannerStation scannerStation) {
            if (this.ScannerLabelType.ScannerActionRelatePieceParts != null && this.ScannerLabelType.ScannerActionRelatePieceParts.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionRelatePieceParts.Commit(context, this, scannerStation);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateLocation != null && this.ScannerLabelType.ScannerActionUpdateLocation.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateLocation.Commit(context, this, scannerStation);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateSystemData != null && this.ScannerLabelType.ScannerActionUpdateSystemData.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateSystemData.Commit(context, this, scannerStation);
                if (result != null) {
                    return result;
                }
            }
            if (this.ScannerLabelType.ScannerActionUpdateWorkLog != null && this.ScannerLabelType.ScannerActionUpdateWorkLog.Active == true) {
                var result = await this.ScannerLabelType.ScannerActionUpdateWorkLog.Commit(context, this, scannerStation);
                if (result != null) {
                    return result;
                }
            }
            this.Open = false;
            await context.SaveChangesAsync();
            return null;
        }
    }
    class ScanGroupDBConfiguration : IEntityTypeConfiguration<ScanGroup> {
        public void Configure(EntityTypeBuilder<ScanGroup> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasMany(item => item.Scans).WithOne(item => item.ScanGroup).HasForeignKey(item => item.ScanGroupId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}