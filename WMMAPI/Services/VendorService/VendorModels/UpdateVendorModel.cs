using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.VendorService.VendorModels
{
    public class UpdateVendorModel : BaseVendorModel
    {
        [Required]
        public Guid Id { get; set; }

        public Vendor ToDB(Guid userId)
        {
            return new Vendor
            {
                Id = Id,
                UserId = userId,
                Name = Name,
                IsDisplayed = IsDisplayed,
                IsDefault = false //placeholder
            };
        }
    }
}
