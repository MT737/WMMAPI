using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
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
