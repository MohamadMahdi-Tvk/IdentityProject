﻿using System.ComponentModel.DataAnnotations;

namespace IdentityProject.UI.Models.Dto.Account
{
    public class TwoFactorLoginDto
    {
        [Required]
        public string Code { get; set; }

        public bool IsPersistent { get; set; }

        public string Provider { get; set; }
    }
}
