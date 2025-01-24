using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerActionUpdateWorkLog : IScannerAction {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ScannerLabelTypeId { get; set; }
        public int? WorkLogItemActivityOptionId { get; set; }
        

        // public ScannerLabelType UserScannerLabelType { get; set; }
        // public int? UserScannerLabelTypeId { get; set; }

        // public ScannerLabelType InventoryItemScannerLabelType { get; set; }
        // public int? InventoryItemScannerLabelTypeId { get; set; }

        // public string EventName { get; set; }
        public bool Active { get; set; }


        public async Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context) {
            return await context.ScannerLabelTypes
                .Where(item => item.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA)
                .Where(item =>
                    item.Variables.Any(item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && item2.ObjectField == "Id") ||
                    item.Variables.Any(item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.User" && item2.ObjectField == "Id")
                ).ToListAsync();
        }

        public async Task<ScanResponse> CheckIfAllRequiredPartsArePresentAndValid(AppDBContext context, ScanGroup scanGroup) {
            var inventoryItemScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var scannedInventoryItemIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => inventoryItemScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();
            
            var inventoryItemsNotPieceParts = await context.InventoryItems
                .Where(item => item.Product.ProductType.IsPiecePart == false)
                .Where(item => scannedInventoryItemIds.Contains((int)item.Id))
                .ToListAsync();

            if (inventoryItemsNotPieceParts.Count == 0) {
                return new ScanResponse {
                    Message = "Need one non piece part inventory item to update the work log",
                    Code = "NON_PIECE_PART_INVENTORY_ITEM_MISSING"
                };
            }

            var userScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.User" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var scannedUserIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => userScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();

            if (scannedUserIds.Count == 0) {
                return new ScanResponse {
                    Message = "Need a user Id in order to update the work log",
                    Code = "USER_ID_ITEM_MISSING"
                };
            }

            return null;
        }

        public bool CheckIfScanGroupIsComplete(ScanGroup scanGroup) {
            var isComplete = true;
            //the only way that this is complete is with an appropriate end scan
            var endScan = scanGroup.Scans.FirstOrDefault(item => item.EndScannerLabelId != null);
            if (endScan == null) {
                isComplete = false;
            }
            return isComplete;
        }

        public async Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context) {
            return new List<ScannerLabelType> { };
        }

        public async Task<ScanResponse> Commit(AppDBContext context, ScanGroup scanGroup, ScannerStation scannerStation) {
            var inventoryItemScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var scannedInventoryItemIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => inventoryItemScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();
            
            var inventoryItemsNotPieceParts = await context.InventoryItems
                .Where(item => item.Product.ProductType.IsPiecePart == false)
                .Where(item => scannedInventoryItemIds.Contains((int)item.Id))
                .ToListAsync();


            var userScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.User" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();
            var scannedUserIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => userScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();
            
            var userId = scannedUserIds.First();

            foreach(var inventoryItem in inventoryItemsNotPieceParts){
                var salesOrderOrRmaIdData = await inventoryItem.GetDefaultSalesOrderIdOrRmaId(context);
                context.WorkLogItems.Add(new WorkLogItem{
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = userId,
                    PerformedById = userId,
                    RmaId = salesOrderOrRmaIdData.RmaId,
                    SalesOrderId = salesOrderOrRmaIdData.SalesOrderId,
                    WorkLogItemActivityOptionId = this.WorkLogItemActivityOptionId,
                    InventoryItemId = inventoryItem.Id,
                    StartDateTime = scanGroup.Scans.First().CreatedAt,
                    EndDateTime = scanGroup.Scans.Last().CreatedAt
                });
            }
            await context.SaveChangesAsync();

            return null;
        }

        public async Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup) {
            var userIdString = scannerLabel.GetVariableValueByObjectNameAndObjectField("GidIndustrial.Gideon.WebApi.Models.User", "Id");
            if (userIdString == null)
                return null;

            var userId = Convert.ToInt32(userIdString);
            var user = await context.Users
                .FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null) {
                return new ScanResponse {
                    Message = "A user was not found with the id matching the id on the label",
                    Code = ScanCode.INVENTORY_ITEM_NOT_FOUND
                };
            }

            var existingUserIds = scanGroup.Scans
                .Select(item => item.ScannerLabel.GetVariableValueByObjectNameAndObjectField("GidIndustrial.Gideon.WebApi.Models.User", "Id"))
                //.Select(item => item.Result)
                .ToList()
                .Where(item => item != null)
                .Select(item => (int?)Convert.ToInt32(item))
                .ToList();

            if (existingUserIds.Count > 0) {
                return new ScanResponse {
                    Message = "A user has already been scanned. Adding another would be ambiguous",
                    Code = ScanCode.ONLY_ONE_USER_ALLOWED
                };
            }

            return null;
        }
    }
    class ScannerActionUpdateWorkLogDBConfiguration : IEntityTypeConfiguration<ScannerActionUpdateWorkLog> {
        public void Configure(EntityTypeBuilder<ScannerActionUpdateWorkLog> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            // modelBuilder.HasOne(item => item.UserScannerLabelType).WithMany().HasForeignKey(item => item.UserScannerLabelTypeId);
            // modelBuilder.HasOne(item => item.InventoryItemScannerLabelType).WithMany().HasForeignKey(item => item.InventoryItemScannerLabelTypeId);
        }
    }
}