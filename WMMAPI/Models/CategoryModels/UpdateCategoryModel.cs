using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    public class UpdateCategoryModel
    {
        [Required]
        public Guid CategoryId { get; set; }
                
        [Required, StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        public bool IsDisplayed { get; set; }

        public Category ToDB(Guid userId)
        {
            return new Category
            {
                CategoryId = CategoryId,
                UserId = userId,
                Name = Name,
                IsDefault = false, //Placeholder
                IsDisplayed = IsDisplayed
            };
        }
    }
}
