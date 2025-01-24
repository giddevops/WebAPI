using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public interface IScannerAction {

        int? Id { get; set; }
        DateTime? CreatedAt { get; set; }
        int? CreatedById { get; set; }
        int? ScannerLabelTypeId { get; set; }
        bool Active { get; set; }

        Task<List<ScannerLabelType>> GetDataScannerLabelTypesThatCannotBeShared(AppDBContext context);

        Task<List<ScannerLabelType>> GetPossibleDataScannerLabelTypes(AppDBContext context);

        bool CheckIfScanGroupIsComplete(ScanGroup scanGroup);

        
        Task<ScanResponse> CheckIfAllRequiredPartsArePresentAndValid(AppDBContext context, ScanGroup scanGroup);
        Task<ScanResponse> Commit(AppDBContext context, ScanGroup scanGroup, ScannerStation scannerStation = null);
        Task<ScanResponse> CheckIfDataLabelIsNotApplicableBecauseAlreadyPresentAndCantHaveMore(AppDBContext context, ScannerLabel scannerLabel, ScanGroup scanGroup);
        
    }
}