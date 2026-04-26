using System.Collections.Generic;
using EduConnect.Models;

namespace EduConnect.Services
{
    public static class SeedData
    {
        public static List<Person> Users { get; } = new();
        public static List<Student> Students { get; } = new();
        public static List<Course> Courses { get; } = new();

        static SeedData()
        {
            // Will be populated in Phase 3
        }
    }
}