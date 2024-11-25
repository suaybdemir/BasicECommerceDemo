﻿using Microsoft.AspNetCore.Mvc;

namespace ECommerceDemo.Dtos
{
    public sealed record class LoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required bool RememberMe { get; set; }
    }
}
