using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
{
    public class CategoryModel
    {
        public Guid CategoryId { get; set; }

        public string Name { get; set; }

        public bool IsDisplayed { get; set; }

        public CategoryModel(Category category)
        {
            CategoryId = category.CategoryId;
            Name = category.Name;
            IsDisplayed = category.IsDisplayed;
        }
    }
}
