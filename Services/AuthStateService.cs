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
            // Find user in seed data
            var user = SeedData.Users.Find(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == password); // In real app, compare hashed passwords
            
            if (user == null)
                return false;

            State.CurrentUser = user;
            OnAuthChanged?.Invoke();
            return true;
        }

        public void Logout()
        {
            State.CurrentUser = null;
            OnAuthChanged?.Invoke();
        }
    }
}