using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    public class CategoryModel
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsDisplayed { get; set; }

        public CategoryModel(Category category)
        {
            CategoryId = category.CategoryId;
            Name = category.Name;
            IsDisplayed = category.IsDisplayed;
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
