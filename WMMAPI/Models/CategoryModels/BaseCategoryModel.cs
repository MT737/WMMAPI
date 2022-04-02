using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Models.CategoryModels
{
    public abstract class BaseCategoryModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public bool IsDisplayed { get; set; }
    }
}
