using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.AccountServices.AccountModels
{
    public class AddAccountModel
    {           
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsAsset { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public Account ToDB(Guid userId)
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = Name,
                IsAsset = IsAsset,
                IsActive = IsActive
            };
        }
    }
}
