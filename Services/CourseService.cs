using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Exceptions;
using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class CourseService : ICourseService
    {
        private List<Course> _courses = new(SeedData.Courses);

        public List<Course> GetAll()
        {
            return _courses;
        }

        public Course? GetById(Guid id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Course entity)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new ArgumentException("Course code is required.");
            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Course title is required.");
            if (entity.CreditHours <= 0)
                throw new ArgumentException("Credit hours must be positive.");
            if (entity.MaxCapacity <= 0)
                throw new ArgumentException("Max capacity must be positive.");

            // Check for duplicate code
            if (_courses.Any(c => c.Code.Equals(entity.Code, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A course with this code already exists.");

            _courses.Add(entity);
        }

        public void Update(Course entity)
        {
            var existing = GetById(entity.Id);
            if (existing == null)
                throw new ArgumentException("Course not found.");

            // Basic validation
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new ArgumentException("Course code is required.");
            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Course title is required.");
            if (entity.CreditHours <= 0)
                throw new ArgumentException("Credit hours must be positive.");
            if (entity.MaxCapacity <= 0)
                throw new ArgumentException("Max capacity must be positive.");

            // Check for duplicate code (excluding the current course)
            if (_courses.Any(c => c.Id != entity.Id && c.Code.Equals(entity.Code, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A course with this code already exists.");

            // Cannot reduce capacity below current enrollment count
            if (entity.MaxCapacity < existing.Enrolled.Count)
                throw new ArgumentException($"Cannot reduce capacity below current enrollment count ({existing.Enrolled.Count}).");

            // Update properties
            existing.Code = entity.Code;
            existing.Title = entity.Title;
            existing.CreditHours = entity.CreditHours;
            existing.MaxCapacity = entity.MaxCapacity;
            existing.AssignedFacultyId = entity.AssignedFacultyId;
            existing.IsActive = entity.IsActive;
        }

        public void Delete(Guid id)
        {
            var course = GetById(id);
            if (course == null)
                return;

            // Cannot delete a course with enrolled students
            if (course.Enrolled.Any())
                throw new InvalidOperationException($"Course {course.Code} has enrolled students and cannot be deleted.");

            _courses.Remove(course);
        }

        public void EnrollStudent(Guid courseId, Guid studentId)
        {
            var course = GetById(courseId);
            if (course == null)
                throw new ArgumentException("Course not found.");

            // Check if course is full
            if (course.Enrolled.Count >= course.MaxCapacity)
                throw new CourseFullException($"Course {course.Code} is full (capacity: {course.MaxCapacity}).");

            // Get student from StudentService (we'll need to inject it, but for now use SeedData)
            var student = SeedData.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                throw new ArgumentException("Student not found.");

            // Check if student is already enrolled
            if (course.Enrolled.Any(s => s.Id == studentId))
                throw new ArgumentException($"Student {student.FullName} is already enrolled in this course.");

            // Enroll student
            course.Enrolled.Add(student);
            student.Enrollments.Add(course);
        }

        public void DropCourse(Guid courseId, Guid studentId)
        {
            var course = GetById(courseId);
            if (course == null)
                throw new ArgumentException("Course not found.");

            var student = SeedData.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                throw new ArgumentException("Student not found.");

            // Remove student from course
            var enrolledStudent = course.Enrolled.FirstOrDefault(s => s.Id == studentId);
            if (enrolledStudent != null)
            {
                course.Enrolled.Remove(enrolledStudent);
                student.Enrollments.Remove(course);
            }
        }

        public List<Course> GetForFaculty(Guid facultyId)
        {
            return _courses
                .Where(c => c.AssignedFacultyId == facultyId)
                .ToList();
        }

        public List<Course> GetAvailable()
        {
            return _courses
                .Where(c => c.IsActive && c.EnrollmentStatus != EnrollmentStatus.Full)
                .ToList();
        }
    }
}