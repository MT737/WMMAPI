﻿using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.CategoryModels
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
