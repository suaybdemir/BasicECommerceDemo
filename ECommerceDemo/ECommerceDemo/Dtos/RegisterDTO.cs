using Microsoft.AspNetCore.Mvc;

namespace ECommerceDemo.Dtos
{
    public class RegisterDTO
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }

}
