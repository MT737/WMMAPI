using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.AccountModels
{
    public class UpdateAccountModel
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required, StringLength(100)]                
        public string Name { get; set; }

        //public bool IsAsset { get; set; } //TODO: Should this be alterable?

        [Required]
        public bool IsActive { get; set; }
        

        public Account ToDB(Guid userId)
        {
            return new Account
            {
                UserId = userId,
                AccountId = AccountId,
                Name = Name,
                IsAsset = false, //Using a placeholder for now //TODO: Should this be changeable?
                IsActive = IsActive
            };
        }
    }
}
