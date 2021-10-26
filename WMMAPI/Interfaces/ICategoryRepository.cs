using System;
using System.Collections.Generic;
using WMMAPI.Database.Models;

namespace WMMAPI.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        void Absorption(Guid absorbedId, Guid absorbingId, Guid userId);
        void CreateDefaults(Guid userId);
        bool DefaultsExist(Guid userId);
        Category Get(Guid id, Guid userId);
        decimal GetCategorySpending(Guid categoryId, Guid userId);
        int GetCount(Guid userId);
        Guid GetId(string name, Guid userId);
        List<Category> GetList(Guid userId);
        bool IsDefault(Guid entityId, Guid userId);
        bool NameExists(string desiredCategoryName, Guid categoryId, Guid userId);
        bool UserOwnsCategory(Guid categoryId, Guid userId);
    }
}