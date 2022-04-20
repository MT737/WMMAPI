using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Services.CategoryServices.CategoryModels
{
    public abstract class BaseCategoryModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }
    }
}
