using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void DeleteCategory(Guid absorbedId, Guid absorbingGuid, Guid userId);
        Category Get(Guid id, Guid userId);
        List<Category> GetList(Guid userId);
        void ModifyCategory(Category category);
    }
}