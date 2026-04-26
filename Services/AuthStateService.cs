using System;
using EduConnect.Models;

namespace EduConnect.Services
{
    /// <summary>
    /// SRP: Maintains the current authenticated user state and broadcasts auth changes to the UI.
    /// </summary>
    public class AuthStateService
    {
        public AuthState State { get; private set; } = new();
        public event Action? OnAuthChanged;

        public bool Login(string email, string password)
        {
            var user = SeedData.Users.Find(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) && u.PasswordHash == password);
            if (user == null) return false;
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
