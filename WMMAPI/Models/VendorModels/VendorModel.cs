using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
    //TODO: Lots of similarities with CategoryModel. Consider a base class?
    public class VendorModel
    {
        [Required]
        public Guid VendordId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }

        public VendorModel(Vendor vendor)
        {
            VendordId = vendor.VendorId;
            Name = vendor.Name;
            IsDefault = vendor.IsDefault;
            IsDisplayed = vendor.IsDisplayed;
        }

        public Vendor ToDB(Guid userId)
        {
            return new Vendor
            {
                VendorId = VendordId,
                UserId = userId,
                IsDefault = false, //placeholder
                IsDisplayed = IsDisplayed
            };
        }
    }
}
