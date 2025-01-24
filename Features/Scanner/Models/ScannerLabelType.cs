using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerLabelType {
        public int? Id { get; set; }

        public int? CreatedById { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        public int? GidSubLocationOptionId { get; set; }

        public string Name { get; set; }

        public Boolean Locked { get; set; }

        [Column(TypeName = "decimal(16,2)")]
        public ScannerLabelTypeClass? ScannerLabelTypeClass { get; set; }

        public List<ScannerLabel> Labels { get; set; }
        public List<ScannerLabelTypeVariable> Variables { get; set; }


        public ScannerActionUpdateLocation ScannerActionUpdateLocation { get; set; }
        public ScannerActionRelatePieceParts ScannerActionRelatePieceParts { get; set; }
        public ScannerActionUpdateWorkLog ScannerActionUpdateWorkLog { get; set; }
        public ScannerActionUpdateSystemData ScannerActionUpdateSystemData { get; set; }

        public static async Task<ScannerLabelType> GetScannerLabelTypeForClass(Type type, AppDBContext _context, System.Security.Claims.ClaimsPrincipal createdBy = null ){
            var scannerLabelType = await _context.ScannerLabelTypes
                .Include(item => item.Variables)
                .FirstOrDefaultAsync(item => item.ScannerLabelTypeClass == GidIndustrial.Gideon.WebApi.Models.ScannerLabelTypeClass.DATA
                && item.Variables.Count == 1
                && item.Variables.Any(v => v.ObjectName == type.FullName && v.ObjectField == "Id"));

            if(scannerLabelType == null){
                //create it
                scannerLabelType = new ScannerLabelType{
                    CreatedAt = DateTime.UtcNow,
                    Name = type.FullName.Split(".").Last() + "Id",
                    Locked = true,
                    ScannerLabelTypeClass = GidIndustrial.Gideon.WebApi.Models.ScannerLabelTypeClass.DATA,
                    CreatedById = createdBy != null ? GidIndustrial.Gideon.WebApi.Models.User.GetId(createdBy) : null,
                    Variables = new List<ScannerLabelTypeVariable>{
                        new ScannerLabelTypeVariable{
                            CreatedAt = DateTime.UtcNow,
                            CreatedById = createdBy != null ? GidIndustrial.Gideon.WebApi.Models.User.GetId(createdBy) : null,
                            ObjectName = type.FullName,
                            ObjectField = "Id"
                        }
                    }
                };
                _context.Add(scannerLabelType);
                await _context.SaveChangesAsync();
            }
            if(scannerLabelType.Locked != true){
                scannerLabelType.Locked = true;
                await _context.SaveChangesAsync();
            }
            return scannerLabelType;
        }

        public bool IsUpdateLocationOnly() {
            return (
                (this.ScannerActionUpdateLocation != null && this.ScannerActionUpdateLocation.Active) &&
                (this.ScannerActionUpdateWorkLog == null || !this.ScannerActionUpdateWorkLog.Active) &&
                (this.ScannerActionRelatePieceParts == null || !this.ScannerActionRelatePieceParts.Active) &&
                (this.ScannerActionUpdateSystemData == null || !this.ScannerActionUpdateSystemData.Active)
            );
        }

        public async Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context) {
            var types = new List<ScannerLabelType> { };
            if (this.ScannerActionRelatePieceParts != null && this.ScannerActionRelatePieceParts.Active) {
                types.AddRange(await this.ScannerActionRelatePieceParts.GetDataScannerLabelTypesThatCannotBeShared(context));
            }
            if (this.ScannerActionUpdateWorkLog != null && this.ScannerActionUpdateWorkLog.Active) {
                types.AddRange(await this.ScannerActionUpdateWorkLog.GetDataScannerLabelTypesThatCannotBeShared(context));
            }
            if (this.ScannerActionUpdateLocation != null && this.ScannerActionUpdateLocation.Active) {
                types.AddRange(await this.ScannerActionUpdateLocation.GetDataScannerLabelTypesThatCannotBeShared(context));
            }
            if (this.ScannerActionUpdateSystemData != null && this.ScannerActionUpdateSystemData.Active) {
                types.AddRange(await this.ScannerActionUpdateSystemData.GetDataScannerLabelTypesThatCannotBeShared(context));
            }
            return types;
        }
        public async Task<bool> CheckIfDataLabelTypeIsValid(AppDBContext context, ScannerLabelType scannerLabelType) {
            var typeAllowed = (await this.GetPossibleDataScannerLabelTypes(context)).Select(item => item.Id).Contains(scannerLabelType.Id);
            return typeAllowed;
        }

        public async Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context) {
            var types = new List<ScannerLabelType> { };
            if (this.ScannerActionRelatePieceParts != null && this.ScannerActionRelatePieceParts.Active) {
                types.AddRange(await this.ScannerActionRelatePieceParts.GetPossibleDataScannerLabelTypes(context));
            }
            if (this.ScannerActionUpdateWorkLog != null && this.ScannerActionUpdateWorkLog.Active) {
                types.AddRange(await this.ScannerActionUpdateWorkLog.GetPossibleDataScannerLabelTypes(context));
            }
            if (this.ScannerActionUpdateLocation != null && this.ScannerActionUpdateLocation.Active) {
                types.AddRange(await this.ScannerActionUpdateLocation.GetPossibleDataScannerLabelTypes(context));
            }
            if (this.ScannerActionUpdateSystemData != null && this.ScannerActionUpdateSystemData.Active) {
                types.AddRange(await this.ScannerActionUpdateSystemData.GetPossibleDataScannerLabelTypes(context));
            }
            return types;
        }

        public async Task<bool> CheckIfDataTypeIsAllowed(AppDBContext context, ScannerLabelType scannerLabelType) {
            var typeAllowed = (await this.GetPossibleDataScannerLabelTypes(context)).Select(item => item.Id).Contains(scannerLabelType.Id);
            return typeAllowed;
        }

        public async Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup) {
            if (this.ScannerActionRelatePieceParts != null && this.ScannerActionRelatePieceParts.Active) {
                var result = await this.ScannerActionRelatePieceParts.CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(context, scannerLabel, scanGroup);
                if (result != null)
                    return result;
            }
            if (this.ScannerActionUpdateWorkLog != null && this.ScannerActionUpdateWorkLog.Active) {
                var result = await this.ScannerActionUpdateWorkLog.CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(context, scannerLabel, scanGroup);
                if (result != null)
                    return result;
            }
            if (this.ScannerActionUpdateLocation != null && this.ScannerActionUpdateLocation.Active) {
                var result = await this.ScannerActionUpdateLocation.CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(context, scannerLabel, scanGroup);
                if (result != null)
                    return result;
            }
            if (this.ScannerActionUpdateSystemData != null && this.ScannerActionUpdateSystemData.Active) {
                var result = await this.ScannerActionUpdateSystemData.CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(context, scannerLabel, scanGroup);
                if (result != null)
                    return result;
            }
            return null;
        }

        public int? GetVariableIdByObjectNameAndField(string objectName, string objectField) {
            var variable = this.Variables.FirstOrDefault(item => item.ObjectName == objectName && item.ObjectField == objectField);
            if (variable != null)
                return variable.Id;
            return null;
        }
    }
    class ScannerLabelTypeDBConfiguration : IEntityTypeConfiguration<ScannerLabelType> {
        public void Configure(EntityTypeBuilder<ScannerLabelType> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasIndex(item => new { item.ScannerLabelTypeClass, item.Name });

            modelBuilder.HasMany(item => item.Variables).WithOne().HasForeignKey(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Cascade);


            modelBuilder.HasOne(item => item.ScannerActionUpdateLocation).WithOne().HasForeignKey<ScannerActionUpdateLocation>(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.ScannerActionRelatePieceParts).WithOne().HasForeignKey<ScannerActionRelatePieceParts>(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.ScannerActionUpdateWorkLog).WithOne().HasForeignKey<ScannerActionUpdateWorkLog>(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.ScannerActionUpdateSystemData).WithOne().HasForeignKey<ScannerActionUpdateSystemData>(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    public enum ScannerLabelTypeClass {
        EVENT = 1,
        DATA = 2
    }
}