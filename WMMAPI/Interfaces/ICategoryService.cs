using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;
using WMMAPI.Models.CategoryModels;

namespace WMMAPI.Interfaces
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void DeleteCategory(Guid absorbedId, Guid absorbingGuid, Guid userId);
        CategoryModel Get(Guid id, Guid userId);
        List<CategoryModel> GetList(Guid userId);
        void ModifyCategory(Category category);
    }
}