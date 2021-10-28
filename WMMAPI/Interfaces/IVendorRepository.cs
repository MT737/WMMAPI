using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface IVendorRepository : IBaseRepository<Vendor>
    {
        void Absorption(Guid absorbedId, Guid absorbingId, Guid userId);
        void CreateDefaults(Guid userId);
        bool DefaultsExist(Guid userId);
        Vendor Get(Guid id, Guid userId);
        decimal GetAmount(Guid vendorID, Guid userID);
        int GetCount(Guid userId);
        Guid GetID(string name, Guid userID);
        IList<Vendor> GetList(Guid userId);
        bool IsDefault(Guid vendorId, Guid userId);
        bool NameExists(string desiredName, Guid vendorId, Guid userId);
        bool UserOwnsVendor(Guid vendorId, Guid userID);
    }
}