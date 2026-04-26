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
            // Create Admin
            var admin = new Admin
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FullName = "System Administrator",
                Email = "admin@educonnect.edu",
                PasswordHash = "admin123" // In real app, this would be hashed
            };
            Users.Add(admin);

            // Create Faculty
            var faculty1 = new Faculty
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                FullName = "Dr. Sarah Johnson",
                Email = "sarah.johnson@educonnect.edu",
                PasswordHash = "faculty123"
            };
            var faculty2 = new Faculty
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                FullName = "Prof. Michael Chen",
                Email = "michael.chen@educonnect.edu",
                PasswordHash = "faculty123"
            };
            var faculty3 = new Faculty
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                FullName = "Dr. Emily Williams",
                Email = "emily.williams@educonnect.edu",
                PasswordHash = "faculty123"
            };
            
            Users.Add(faculty1);
            Users.Add(faculty2);
            Users.Add(faculty3);
            Faculty.Add(faculty1);
            Faculty.Add(faculty2);
            Faculty.Add(faculty3);

            // Create Students
            var student1 = new Student
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                FullName = "Alice Brown",
                Email = "alice.brown@educonnect.edu",
                PasswordHash = "student123",
                Semester = 3,
                CGPA = 3.8m
            };
            var student2 = new Student
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                FullName = "Bob Smith",
                Email = "bob.smith@educonnect.edu",
                PasswordHash = "student123",
                Semester = 2,
                CGPA = 3.2m
            };
            var student3 = new Student
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                FullName = "Charlie Davis",
                Email = "charlie.davis@educonnect.edu",
                PasswordHash = "student123",
                Semester = 1,
                CGPA = 2.9m
            };
            var student4 = new Student
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                FullName = "Diana Miller",
                Email = "diana.miller@educonnect.edu",
                PasswordHash = "student123",
                Semester = 4,
                CGPA = 3.5m
            };
            var student5 = new Student
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                FullName = "Ethan Wilson",
                Email = "ethan.wilson@educonnect.edu",
                PasswordHash = "student123",
                Semester = 2,
                CGPA = 3.0m
            };
            
            Users.Add(student1);
            Users.Add(student2);
            Users.Add(student3);
            Users.Add(student4);
            Users.Add(student5);
            Students.Add(student1);
            Students.Add(student2);
            Students.Add(student3);
            Students.Add(student4);
            Students.Add(student5);

            // Create Courses
            var course1 = new Course
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Code = "CS101",
                Title = "Introduction to Computer Science",
                CreditHours = 3,
                MaxCapacity = 30,
                AssignedFacultyId = faculty1.Id
            };
            var course2 = new Course
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Code = "MATH201",
                Title = "Calculus II",
                CreditHours = 4,
                MaxCapacity = 25,
                AssignedFacultyId = faculty2.Id
            };
            var course3 = new Course
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Code = "PHYS101",
                Title = "University Physics",
                CreditHours = 4,
                MaxCapacity = 35,
                AssignedFacultyId = faculty3.Id
            };
            var course4 = new Course
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Code = "ENG102",
                Title = "Academic Writing",
                CreditHours = 3,
                MaxCapacity = 40,
                AssignedFacultyId = faculty1.Id
            };
            var course5 = new Course
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Code = "CS301",
                Title = "Data Structures",
                CreditHours = 3,
                MaxCapacity = 20,
                AssignedFacultyId = faculty2.Id
            };
            var course6 = new Course
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Code = "BUS202",
                Title = "Business Ethics",
                CreditHours = 3,
                MaxCapacity = 30,
                AssignedFacultyId = faculty3.Id
            };
            
            Courses.Add(course1);
            Courses.Add(course2);
            Courses.Add(course3);
            Courses.Add(course4);
            Courses.Add(course5);
            Courses.Add(course6);

            // Assign courses to faculty (add Course objects to their AssignedCourses list)
            faculty1.AssignedCourses.Add(course1);
            faculty1.AssignedCourses.Add(course4);
            faculty2.AssignedCourses.Add(course2);
            faculty2.AssignedCourses.Add(course5);
            faculty3.AssignedCourses.Add(course3);
            faculty3.AssignedCourses.Add(course6);
        }
    }
}