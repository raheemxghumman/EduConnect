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
            _all.Add(n);
            OnNewNotification?.Invoke(n);
        }

        public void AddNotification(string message, NotificationType type, Guid userId)
        {
            var notification = new Notification
            {
                Message = message,
                NotificationType = type,
                UserId = userId,
                Timestamp = DateTime.Now
            };
            AddNotification(notification);
        }

        public List<Notification> GetForUser(Guid userId)
        {
            return _all
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        public int GetUnreadCount(Guid userId)
        {
            return _all.Count(n => n.UserId == userId && !n.IsRead);
        }

        public void MarkAsRead(Guid notificationId)
        {
            var notification = _all.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }

        public void MarkAllAsRead(Guid userId)
        {
            foreach (var notification in _all.Where(n => n.UserId == userId && !n.IsRead))
            {
                notification.IsRead = true;
            }
        }
    }
}