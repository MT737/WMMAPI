using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
    public class AddVendorModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
                
        [Required]
        public bool IsDisplayed { get; set; }

        public Vendor ToDB(Guid userId)
        {
            return new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                IsDisplayed = IsDisplayed,
                IsDefault = false
            };
        }
    }
}
