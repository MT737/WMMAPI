using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.VendorService.VendorModels
{
    public class VendorModel : BaseVendorModel
    {
        [Required]
        public Guid Id { get; set; }
    
        [Required]
        public bool IsDefault { get; set; }
        
        public VendorModel(Vendor vendor)
        {
            Id = vendor.Id;
            Name = vendor.Name;
            IsDefault = vendor.IsDefault;
            IsDisplayed = vendor.IsDisplayed;
        }
    }
}
