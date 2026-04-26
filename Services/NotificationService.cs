using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class NotificationService
    {
        public event Action<Notification>? OnNewNotification;

        private List<Notification> _all = new();

        public void AddNotification(Notification n)
        {
            // Will be implemented in Phase 3
        }

        public List<Notification> GetForUser(Guid userId)
        {
            return new List<Notification>();
        }

        public int GetUnreadCount(Guid userId)
        {
            return 0;
        }

        public void MarkAsRead(Guid notificationId)
        {
        }

        public void MarkAllAsRead(Guid userId)
        {
        }
    }
}