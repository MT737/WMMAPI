using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
