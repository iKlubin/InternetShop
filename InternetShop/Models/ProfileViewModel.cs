using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhotoUrl { get; set; }
    }

}
