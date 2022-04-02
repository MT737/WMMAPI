using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Models.VendorModels
{
    public class DeleteVendorModel
    {
        [Required]
        public Guid AbsorbedVendor { get; set; }
        
        [Required]
        public Guid AbsorbingVendor { get; set; }
    }
}
