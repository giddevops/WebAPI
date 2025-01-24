using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerActionUpdateSystemData : IScannerAction {
        public int? Id { get; set; }


        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ScannerLabelTypeId { get; set; }

        public bool Active { get; set; }

        public List<ScannerActionUpdateSystemDataCommand> Commands { get; set; }



        public async Task<ScanResponse> CheckIfAllRequiredPartsArePresentAndValid(AppDBContext context, ScanGroup scanGroup) {
            //required parts are dynamic
            //need to require a label that is an ID for whatever object will be updated. So if the ScannerActionUpdateSystemData says update SalesOrder.SalesOrderStatusOptionId=1, Look for a SalesOrder.Id label


            // var applicableScannerLabelTypeVariableIds = scanGroup.Scans
            //     .Where(scan => scan.EndScannerLabelId == null)
            //     .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables)
            //     .Where(variable => variable.ObjectField == "Id")
            //     .Where(variable => this.Commands.Any(item => item.ObjectName == variable.ObjectName && item.ObjectField == "Id"))
            //     .Select(item => item.Id).ToList();

            // var scannedApplicableItems = scanGroup.Scans
            //     .Where(scan => scan.EndScannerLabelId == null)
            //     .SelectMany(scan => scan.ScannerLabel.VariableValues)
            //     .Where(variableValue => applicableScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId));

            var scannerLabelTypeVariables = scanGroup.Scans
                    .Where(item => item.EndScannerLabelId == null)
                .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables);

            foreach (var command in this.Commands) {
                var applicableObjectScannerLabelTypeVariableIds = scannerLabelTypeVariables
                    .Where(variable => variable.ObjectField == "Id")
                    .Where(variable =>
                        (command.ObjectName == variable.ObjectName)
                    )
                    .Select(item => item.Id).ToList();

                var scannedApplicableObjectItems = scanGroup.Scans
                    .Where(scan => scan.EndScannerLabelId == null)
                    .SelectMany(scan => scan.ScannerLabel.VariableValues)
                    .Where(variableValue => applicableObjectScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId));

                if (command.ValueScannerLabelTypeId != null) {
                    var matchingValueScannerLabel = scanGroup.Scans
                        .Where(scan => scan.ScannerLabel.ScannerLabelTypeId == command.ValueScannerLabelTypeId)
                        .FirstOrDefault();
                    if (matchingValueScannerLabel == null) {
                        return new ScanResponse {
                            Message = "The update system data command requires a label for the value but none was found",
                            Code = ScanCode.REQUIRED_LABEL_TYPE_MISSING
                        };
                    }
                }

                if (scannedApplicableObjectItems.Count() < 1) {
                    return new ScanResponse {
                        Message = "At least one label of Object Type " + command.ObjectName + " Id is required but was not found.",
                        Code = ScanCode.REQUIRED_LABEL_TYPE_MISSING
                    };
                }
            }
            return null;
        }

        public async Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup) {
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

        public async Task<ScanResponse> Commit(AppDBContext context, ScanGroup scanGroup, ScannerStation scannerStation) {

            foreach (var command in this.Commands) {
                //apply each commmand
                // command.ObjectName
                Type objectType = Type.GetType(command.ObjectName);
                var objectTableName = context.Model.FindEntityType(objectType).SqlServer().TableName;
                var fieldName = Utilities.CleanSqlColumnName(command.ObjectField);

                if (command.Type == ScannerActionUpdateSystemDataCommandType.UPDATE) {
                    //get the value which is either plain text or comes from a variable
                    var value = command.TextValue;
                    if (command.ValueScannerLabelTypeVariableId != null) {
                        var valueLabel = scanGroup.Scans.FirstOrDefault(item =>
                            item.ScannerLabel.ScannerLabelTypeId == command.ValueScannerLabelTypeVariableId
                            && item.ScannerLabel.VariableValues.Any(item2 => item2.ScannerLabelTypeVariableId == command.ValueScannerLabelTypeVariableId)
                        ).ScannerLabel;
                        var variableValue = valueLabel.VariableValues.FirstOrDefault(item => item.ScannerLabelTypeVariableId == command.ValueScannerLabelTypeVariableId);
                        value = variableValue.Value;
                    }

                    //Now get the objects that need to be updated
                    var scannerLabelTypeVariables = scanGroup.Scans
                            .Where(item => item.EndScannerLabelId == null)
                        .SelectMany(scan => scan.ScannerLabel.ScannerLabelType.Variables);
                    var applicableObjectScannerLabelTypeVariableIds = scannerLabelTypeVariables
                        .Where(variable => variable.ObjectField == "Id")
                        .Where(variable =>
                            (command.ObjectName == variable.ObjectName)
                        )
                        .Select(item => item.Id).ToList();
                    var scannedApplicableObjectItems = scanGroup.Scans
                        .Where(scan => scan.EndScannerLabelId == null)
                        .SelectMany(scan => scan.ScannerLabel.VariableValues)
                        .Where(variableValue => applicableObjectScannerLabelTypeVariableIds.Contains(variableValue.ScannerLabelTypeVariableId));
                    var objectIds = scannedApplicableObjectItems.Select(item => item.Value);

                    foreach (var objectId in objectIds) {
                        var query = $"UPDATE \"{objectTableName}\" SET \"{fieldName}\"= @value WHERE Id=@id";
                        var valueParam = new SqlParameter("@value", value);
                        var idParam = new SqlParameter("@id", objectId);
                        await context.Database.ExecuteSqlCommandAsync(query, valueParam, idParam);
                    }
                    await context.SaveChangesAsync();

                } else {
                    throw new NotImplementedException("Currently only update is supported");
                }
            }
            return null;
        }

        public async Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context) {
            return new List<ScannerLabelType> { };
        }

        public async Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context) {

            var possibleTypes = await context.ScannerLabelTypes
                .Where(item => item.ScannerLabelTypeClass == ScannerLabelTypeClass.DATA)
                .Where(scannerLabelType =>
                    // first check for any labels that have variables that match the ObjectName of any of the commands.  So if the command is update company.name, find any label with company.Id as a variable.
                    (this.Commands.Any(command => scannerLabelType.Variables.Any(variable => variable.ObjectName == command.ObjectName && variable.ObjectField == "Id")))
                    // now check for any data labels that serve as the value that the object will be updated to.  For exmaple if the command is to  update inventory item location, looks for any label types that specify a GidSubLocationOption.Id which will be used as the new value for the location.
                    || (this.Commands.Any(command => command.ValueScannerLabelTypeId == scannerLabelType.Id))
                )
                .ToListAsync();

            return possibleTypes;
        }
    }
    class ScannerActionUpdateSystemDataDBConfiguration : IEntityTypeConfiguration<ScannerActionUpdateSystemData> {
        public void Configure(EntityTypeBuilder<ScannerActionUpdateSystemData> modelBuilder) {
            // modelBuilder.HasOne(item => item.ValueScannerLabelType).WithMany().HasForeignKey(item => item.ValueScannerLabelTypeId);
        }
    }
}