using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerActionUpdateLocation : IScannerAction {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ScannerLabelTypeId { get; set; }
        public bool Active { get; set; }

        public ScannerLabelType InventoryItemScannerLabelType { get; set; }
        public int? InventoryItemScannerLabelTypeId { get; set; }

        public async Task<ScanResponse> ValidateNewScanUpdateLocationOnly(AppDBContext context, ScanGroup scanGroup, ScannerLabel scannerLabel) {
            //First figure out if the item scanned is an inventory item
            var inventoryItemScannerlabelTypeVariableId = scannerLabel.ScannerLabelType.Variables
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).FirstOrDefault();
            //if the scanner label type doesn't have an InventoryItem variable, reject it
            if (inventoryItemScannerlabelTypeVariableId == null) {
                return new ScanResponse {
                    Message = "This event label only supports updating inventory item locations, and this label doesn't correspond with an inventory item.  You can only scan an inventory item for this label type",
                    Code = "WRONG_LABEL_TYPE"
                };
            }
            return null;
        }
        public bool HasEnoughDataToCompleteAction(AppDBContext context, ScanGroup scanGroup) {
            var inventoryItemScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var inventoryItemIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => inventoryItemScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();

            if (inventoryItemIds.Count > 0) {
                return true;
            }
            return false;
        }

        public async Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context) {
            return new List<ScannerLabelType> { };
        }

        public async Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context) {
            return await context.ScannerLabelTypes
                .Where(item => item.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA)
                .Where(item =>
                    item.Variables.Any(item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.User" && item2.ObjectField == "Id")
                ).ToListAsync();
        }

        public bool CheckIfScanGroupIsComplete(ScanGroup scanGroup) {
            var inventoryItemScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var inventoryItemIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => inventoryItemScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();
            if (inventoryItemIds.Count > 0) {
                return true;
            }
            return false;
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

            if (scannedInventoryItemIds.Count == 0) {
                return new ScanResponse {
                    Message = "Need one inventory item",
                    Code = "INVENTORY_ITEM_MISSING"
                };
            }
            return null;
        }

        public async Task<ScanResponse> Commit(AppDBContext context, ScanGroup scanGroup, ScannerStation scannerStation) {
            var inventoryItemScannerLabelTypeVariableIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
                .Where(variable => variable.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && variable.ObjectField == "Id")
                .Select(item => item.Id).ToList();

            var inventoryItemIds = scanGroup.Scans
                .Where(scan => scan.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.VariableValues)
                .Where(variableValue => inventoryItemScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId))
                .Select(item => item.Value)
                .Where(val => !String.IsNullOrWhiteSpace(val))
                .Select(item => Convert.ToInt32(item)).ToList();

            var inventoryItems = await context.InventoryItems.Where(item => inventoryItemIds.Contains((int)item.Id)).ToListAsync();
            inventoryItems.ForEach(inventoryItem => inventoryItem.GidSubLocationOptionId = scannerStation.GidSubLocationOptionId);
            await context.SaveChangesAsync();
            return null;
        }

        public async Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup) {
            return null;
        }
    }
    class ScannerActionUpdateLocationDBConfiguration : IEntityTypeConfiguration<ScannerActionUpdateLocation> {
        public void Configure(EntityTypeBuilder<ScannerActionUpdateLocation> modelBuilder) {
            modelBuilder.HasOne(item => item.InventoryItemScannerLabelType).WithMany().HasForeignKey(item => item.InventoryItemScannerLabelTypeId);
        }
    }
}