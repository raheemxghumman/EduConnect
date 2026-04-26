using System;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class AuthStateService
    {
        public AuthState State { get; private set; } = new();

        public event Action? OnAuthChanged;

        public bool Login(string email, string password)
        {
            // Will be implemented in Phase 3
            return false;
        }

        public void Logout()
        {
            // Will be implemented in Phase 3
        }
    }
}