using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
    //TODO: Lots of similarities with CategoryModel. Consider a base class?
    public class VendorModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }

        public VendorModel(Vendor vendor)
        {
            Id = vendor.Id;
            Name = vendor.Name;
            IsDefault = vendor.IsDefault;
            IsDisplayed = vendor.IsDisplayed;
        }

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
