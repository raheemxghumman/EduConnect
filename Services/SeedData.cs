using System;
using System.Collections.Generic;
using EduConnect.Models;

namespace EduConnect.Services
{
    public static class SeedData
    {
        public static List<Person> Users { get; } = new();
        public static List<Student> Students { get; } = new();
        public static List<Course> Courses { get; } = new();
        public static List<Faculty> Faculty { get; } = new();

        static SeedData()
        {
            var admin = new Admin { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), FullName = "Dr. Admin", Email = "admin@edu.pk", PasswordHash = "admin123" };
            var ayesha = new Faculty { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), FullName = "Dr. Ayesha Khan", Email = "ayesha@edu.pk", PasswordHash = "faculty123" };
            var bilal = new Faculty { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), FullName = "Prof. Bilal Ahmed", Email = "bilal@edu.pk", PasswordHash = "faculty123" };

            var wania = new Student { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), FullName = "Wania Rahman", Email = "wania@student.edu.pk", PasswordHash = "student123", Semester = 3, CGPA = 3.6m };
            var roman = new Student { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), FullName = "Roman Fatima", Email = "roman@student.edu.pk", PasswordHash = "student123", Semester = 2, CGPA = 3.2m };
            var abdul = new Student { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), FullName = "Abdulraheem", Email = "raheem@student.edu.pk", PasswordHash = "student123", Semester = 4, CGPA = 3.4m };
            var sara = new Student { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), FullName = "Sara Malik", Email = "sara@student.edu.pk", PasswordHash = "student123", Semester = 1, CGPA = 2.9m };
            var ali = new Student { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), FullName = "Ali Hassan", Email = "ali@student.edu.pk", PasswordHash = "student123", Semester = 2, CGPA = 3.0m };

            Users.AddRange(new Person[] { admin, ayesha, bilal, wania, roman, abdul, sara, ali });
            Faculty.AddRange(new[] { ayesha, bilal });
            Students.AddRange(new[] { wania, roman, abdul, sara, ali });

            var cs101 = new Course { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Code = "CS-101", Title = "Introduction to Programming", CreditHours = 3, MaxCapacity = 30, AssignedFacultyId = ayesha.Id };
            var cs201 = new Course { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Code = "CS-201", Title = "Data Structures", CreditHours = 3, MaxCapacity = 30, AssignedFacultyId = ayesha.Id };
            var cs401 = new Course { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Code = "CS-401", Title = "Artificial Intelligence", CreditHours = 3, MaxCapacity = 2, AssignedFacultyId = ayesha.Id };
            var cs284 = new Course { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Code = "CS-284", Title = "Web Engineering", CreditHours = 3, MaxCapacity = 25, AssignedFacultyId = bilal.Id };
            var se301 = new Course { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Code = "SE-301", Title = "Software Design Patterns", CreditHours = 3, MaxCapacity = 20, AssignedFacultyId = bilal.Id };
            Courses.AddRange(new[] { cs101, cs201, cs401, cs284, se301 });

            ayesha.AssignedCourses.AddRange(new[] { cs101, cs201, cs401 });
            bilal.AssignedCourses.AddRange(new[] { cs284, se301 });

            cs101.Enrolled.AddRange(new[] { wania, roman, abdul });
            cs201.Enrolled.AddRange(new[] { wania, sara });
            cs401.Enrolled.AddRange(new[] { wania, roman });
            wania.Enrollments.AddRange(new[] { cs101, cs201, cs401 });
            roman.Enrollments.AddRange(new[] { cs101, cs401 });
            abdul.Enrollments.Add(cs101);
            sara.Enrollments.Add(cs201);
        }
    }
}
