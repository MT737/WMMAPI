using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Services.VendorService.VendorModels
{
    public abstract class BaseVendorModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }
    }
}
