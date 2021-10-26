﻿using System;
using System.ComponentModel.DataAnnotations;
using WMMAPI.Database.Models;

namespace WMMAPI.ViewModels
{
    public class UserViewModel
    {
        //Properties
        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(200)]
        public string FirstName { get; set; }

        [Required, StringLength(200)]
        public string LastName { get; set; }

        [Required]
        public string DOB { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; }


        //TODO: Research better ways to limit information returned
        public UserViewModel(User user)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DOB = user.DOB.ToString("d");
            EmailAddress = user.EmailAddress;
        }
    }
}
