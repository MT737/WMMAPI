using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    public class AddCategoryModel
    {
        [Required]
        public string Name { get; set; }

        public Category ToDB(Guid userId)
        {
            return new Category
            {
                CategoryId = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                IsDefault = false,
                IsDisplayed = true
            };
        }
    }
}
