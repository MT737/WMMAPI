using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.CategoryServices.CategoryModels
{
    //TODO: Lots of similarities with VendorModel. Consider a base class?
    public class CategoryModel : BaseCategoryModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        public CategoryModel(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            IsDisplayed = category.IsDisplayed;
            IsDefault = category.IsDefault;
        }
    }
}
