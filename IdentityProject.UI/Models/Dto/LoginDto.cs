﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.UI.Models.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [DisplayName("Remember Me")]
        public bool IsPersistent { get; set; } = false;


        public string ReturnUrl { get; set; }
    }
}
