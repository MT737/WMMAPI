using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.CategoryServices.CategoryModels
{
    public class AddCategoryModel : BaseCategoryModel
    {        
        public Category ToDB(Guid userId)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                IsDefault = false,
                IsDisplayed = IsDisplayed
            };
        }
    }
}
