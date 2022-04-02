using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
    public abstract class BaseVendorModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public bool IsDisplayed { get; set; }
    }
}
