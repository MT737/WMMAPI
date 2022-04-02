using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
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
                IsDefault = false, //placeholder
                IsDisplayed = IsDisplayed
            };
        }
    }
}
