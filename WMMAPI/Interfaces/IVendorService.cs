using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface IVendorService
    {
        void DeleteVendor(Guid absorbedId, Guid absorbingId, Guid userId);
        void AddVendor(Vendor vendor);
        Vendor Get(Guid id, Guid userId);
        IList<Vendor> GetList(Guid userId);
        void ModifyVendor(Vendor vendor);
        void CreateDefaults(Guid userId);
    }
}