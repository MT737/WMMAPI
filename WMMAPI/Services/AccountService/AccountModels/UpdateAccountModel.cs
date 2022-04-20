using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Entities;

namespace WMMAPI.Services.AccountServices.AccountModels
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
                Id = AccountId,
                Name = Name,
                //IsAsset = false, //TODO: Should this be changeable?
                IsActive = IsActive
            };
        }
    }
}
