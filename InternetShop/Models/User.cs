using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    public class User
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
