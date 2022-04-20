using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;
using WMMAPI.Services.CategoryServices.CategoryModels;

namespace WMMAPI.Interfaces
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void DeleteCategory(Guid absorbedId, Guid absorbingGuid, Guid userId);
        CategoryModel Get(Guid id, Guid userId);
        IList<CategoryModel> GetList(Guid userId);
        void ModifyCategory(Category category);
    }
}