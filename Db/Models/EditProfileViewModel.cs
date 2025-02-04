﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Db.Models
{
    public class EditProfileViewModel
    {
        [MaxLength(50, ErrorMessage = "Användarnamnet får inte vara längre än 50 tecken")]
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "Ange din e-mail i giltigt format")]
        public string? Email { get; set; }
        [MaxLength(100, ErrorMessage = "Adressen får inte vara längre än 100 tecken")]
        public string? Address { get; set; }

		[RegularExpression(@"^\d{3}-\d{7}$", ErrorMessage = "Telefonnumret måste vara i formatet 073-1234567")]
		public string? TelefonNummer { get; set; }

        public bool IsPublic { get; set; }

        public IFormFile? ProfilBild { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
