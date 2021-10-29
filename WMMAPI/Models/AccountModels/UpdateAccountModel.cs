using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.AccountModels
{
    public class UpdateAccountModel
    {
        [Required]
        public Guid AccountId { get; set; }
                
        public string Name { get; set; }

        public bool IsAsset { get; set; }

        public bool IsActive { get; set; }
        

        public Account ToDB(Guid userId)
        {
            return new Account
            {
                UserId = userId,
                AccountId = AccountId,
                Name = Name,
                IsAsset = IsAsset, //TODO: Should this be changeable?
                IsActive = IsActive
            };
        }
    }
}
