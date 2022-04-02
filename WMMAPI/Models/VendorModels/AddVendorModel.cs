using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.VendorModels
{
    public class AddVendorModel : BaseVendorModel
    {
        public Vendor ToDB(Guid userId)
        {
            return new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                IsDisplayed = IsDisplayed,
                IsDefault = false
            };
        }
    }
}
