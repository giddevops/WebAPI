using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerActionRelatePieceParts : IScannerAction {

        // public static List<string>

        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ScannerLabelTypeId { get; set; }

        public bool Active { get; set; }

        public async Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context) {
            var data = await context.ScannerLabelTypes
                .Where(item => item.Variables.Any(
                    item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && item2.ObjectField == "Id"
                )).ToListAsync();
            return data;
        }

        public async Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context) {
            return await context.ScannerLabelTypes
                .Where(item =>
                    item.Variables.Any(item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.InventoryItem" && item2.ObjectField == "Id")
                    || item.Variables.Any(item2 => item2.ObjectName == "GidIndustrial.Gideon.WebApi.Models.User" && item2.ObjectField == "Id")
                ).ToListAsync();
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

        public async Task<ScanResponse> CheckIfAllRequiredPartsArePresentAndValid(AppDBContext context, ScanGroup scanGroup) {
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

            var inventoryItems = await context.InventoryItems
                .Include(item => item.Product)
                    .ThenInclude(item => item.ProductType)
                .Where(item => inventoryItemIds.Contains((int)item.Id)).ToListAsync();

            var nonPieceParts = inventoryItems.Where(item => item.Product.ProductType.IsPiecePart == false).ToList();
            var pieceParts = inventoryItems.Where(item => item.Product.ProductType.IsPiecePart == true).ToList();

            if (nonPieceParts.Count == 0) {
                return new ScanResponse {
                    Message = "Need one non-piece part",
                    Code = "PIECE_PART_MISSING"
                };
            } else if (nonPieceParts.Count > 1) {
                return new ScanResponse {
                    Message = "Only one non-piece part is allowed. This one is the parent",
                    Code = "TOO_MANY_NON_PIECE_PARTS"
                };
            }
            if (pieceParts.Count == 0) {
                return new ScanResponse {
                    Message = "Need at least one piece part",
                    Code = "PIECE_PART_REQUIRED"
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

            var inventoryItems = await context.InventoryItems
                .Include(item => item.Product)
                    .ThenInclude(item => item.ProductType)
                .Where(item => inventoryItemIds.Contains((int)item.Id)).ToListAsync();

            var nonPieceParts = inventoryItems.Where(item => item.Product.ProductType.IsPiecePart == false).ToList();
            var pieceParts = inventoryItems.Where(item => item.Product.ProductType.IsPiecePart == true).ToList();

            //first remove any relationships that exist for the children
            var piecePartIds = pieceParts.Select(item => item.Id).ToList();
            var itemsToRemove = context.InventoryItemRelatedInventoryItems.Where(item => piecePartIds.Contains(item.ChildInventoryItemId));
            context.InventoryItemRelatedInventoryItems.RemoveRange(itemsToRemove);
            await context.SaveChangesAsync();

            var nonPiecePart = nonPieceParts.First();

            var newRelationships = pieceParts.Select(item => new InventoryItemRelatedInventoryItem {
                ChildInventoryItemId = item.Id,
                ParentInventoryItemId = nonPiecePart.Id
            });
            context.InventoryItemRelatedInventoryItems.AddRange(newRelationships);
            await context.SaveChangesAsync();

            return null;
        }

        public async Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup) {
            var inventoryItemIdString = scannerLabel.GetVariableValueByObjectNameAndObjectField("GidIndustrial.Gideon.WebApi.Models.InventoryItem", "Id");
            if(inventoryItemIdString == null)
                return null;
            var inventoryItemId = Convert.ToInt32(inventoryItemIdString);
            var inventoryItem = await context.InventoryItems
                .Include(item => item.Product)
                .ThenInclude(item => item.ProductType)
                .FirstOrDefaultAsync(item => item.Id == inventoryItemId);
            if (inventoryItem == null) {
                return new ScanResponse {
                    Message = "An inventory item was not found with the id matching the id on the label",
                    Code = ScanCode.INVENTORY_ITEM_NOT_FOUND
                };
            }
            if (inventoryItem.Product.ProductType == null) {
                return new ScanResponse {
                    Message = "The product for this inventory item doesn't have a product type specified so the system can't tell if it's a piece part.  Add a Product Type to the Product in Gideon in order to fix this error",
                    Code = ScanCode.PRODUCT_DOES_NOT_HAVE_PRODUCT_TYPE
                };
            }
            if (inventoryItem.Product.ProductType.IsPiecePart == null) {
                return new ScanResponse {
                    Message = "The product type for this inventory item's product doesn't specify whether or not it's a piece part.  The system can't do any relating without having this specified.",
                    Code = ScanCode.IS_PIECE_PART_NOT_SPECIFIED_ON_PRODUCT
                };
            }
            if (inventoryItem.Product.ProductType.IsPiecePart == true) {
                return null;
            }

            var existingInventoryItemIds = scanGroup.Scans
                .Select(item => item.ScannerLabel.GetVariableValueByObjectNameAndObjectField("GidIndustrial.Gideon.WebApi.Models.InventoryItem", "Id"))
                // .Select(item => item.Result)
                .ToList()
                .Where(item => item != null)
                .Select(item => (int?)Convert.ToInt32(item));
            var existingInventoryItems = await context.InventoryItems
                .Include(item => item.Product)
                .ThenInclude(item => item.ProductType)
                .Where(item => item.Product.ProductType != null)
                .Where(item => existingInventoryItemIds.Contains(item.Id))
                .ToListAsync();

            if (existingInventoryItems.FirstOrDefault(item => item.Product.ProductType.IsPiecePart == false) != null) {
                return new ScanResponse {
                    Message = "The item you scanned is not a piece part, and a non piece part item has already been scanned.",
                    Code = ScanCode.SECOND_NON_PIECE_PART_SCANNED
                };
            }
            return null;
        }
    }
    class ScannerActionRelatePiecePartsDBConfiguration : IEntityTypeConfiguration<ScannerActionRelatePieceParts> {
        public void Configure(EntityTypeBuilder<ScannerActionRelatePieceParts> modelBuilder) {

        }
    }
}