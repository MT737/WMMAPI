using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;
using WMMAPI.Services.VendorService.VendorModels;

namespace WMMAPI.Interfaces
{
    public interface IVendorService
    {
        void DeleteVendor(Guid absorbedId, Guid absorbingId, Guid userId);
        void AddVendor(Vendor vendor);
        Vendor Get(Guid id, Guid userId);
        IList<VendorModel> GetList(Guid userId);
        void ModifyVendor(Vendor vendor);
    }
}