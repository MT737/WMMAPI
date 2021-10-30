using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        void Absorption(Guid absorbedId, Guid absorbingId, Guid userId);
        void AddCategory(Category category);
        void CreateDefaults(Guid userId);
        bool DefaultsExist(Guid userId);
        void DeleteCategory(Guid absorbedId, Guid absorbingGuid, Guid userId);
        Category Get(Guid id, Guid userId);
        decimal GetCategorySpending(Guid categoryId, Guid userId);
        int GetCount(Guid userId);
        Guid GetId(string name, Guid userId);
        List<Category> GetList(Guid userId);
        bool IsDefault(Guid entityId, Guid userId);
        void ModifyCategory(Category category);
        bool NameExists(Category category);
        bool UserOwnsCategory(Guid categoryId, Guid userId);
    }
}