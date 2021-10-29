using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.AccountModels
{
    public class AddAccountModel
    {   
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsAsset { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public Account ToDB()
        {
            return new Account
            {
                AccountId = Guid.NewGuid(),
                UserId = UserId,
                Name = Name,
                IsAsset = IsAsset,
                IsActive = IsActive
            };
        }
    }
}
