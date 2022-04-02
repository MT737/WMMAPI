using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    public class UpdateCategoryModel : BaseCategoryModel
    {
        [Required]
        public Guid Id { get; set; }

        public Category ToDB(Guid userId)
        {
            return new Category
            {
                UserId = userId,
                Id = Id,
                Name = Name,
                IsDefault = false, //Placeholder
                IsDisplayed = IsDisplayed
            };
        }
    }
}
