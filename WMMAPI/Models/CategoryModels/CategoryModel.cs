using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    //TODO: Lots of similarities with VendorModel. Consider a base class?
    public class CategoryModel
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }

        public CategoryModel(Category category)
        {
            CategoryId = category.CategoryId;
            Name = category.Name;
            IsDisplayed = category.IsDisplayed;
            IsDefault = category.IsDefault;
        }

        public Category ToDB(Guid userId)
        {
            return new Category
            {
                UserId = userId,
                CategoryId = CategoryId,
                Name = Name,
                IsDefault = false, //Placeholder
                IsDisplayed = IsDisplayed
            };
        }
    }
}
