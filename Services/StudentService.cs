using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Exceptions;
using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class StudentService : IStudentService
    {
        private List<Student> _students = new(SeedData.Students);

        public List<Student> GetAll()
        {
            return _students;
        }

        public Student? GetById(Guid id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Student entity)
        {
            if (!entity.Validate(out string errorMessage))
                throw new ArgumentException(errorMessage);

            // Check for duplicate email
            if (_students.Any(s => s.Email.Equals(entity.Email, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A student with this email already exists.");

            _students.Add(entity);
        }

        public void Update(Student entity)
        {
            if (!entity.Validate(out string errorMessage))
                throw new ArgumentException(errorMessage);

            var existing = GetById(entity.Id);
            if (existing == null)
                throw new ArgumentException("Student not found.");

            // Check for duplicate email (excluding the current student)
            if (_students.Any(s => s.Id != entity.Id && s.Email.Equals(entity.Email, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A student with this email already exists.");

            // Update properties
            existing.FullName = entity.FullName;
            existing.Email = entity.Email;
            existing.PasswordHash = entity.PasswordHash;
            existing.Semester = entity.Semester;
            existing.CGPA = entity.CGPA;
        }

        public void Delete(Guid id)
        {
            var student = GetById(id);
            if (student == null)
                return;

            // Business rule: Cannot delete a student with active enrollments
            if (student.Enrollments.Any())
                throw new StudentHasActiveEnrollmentsException($"Student {student.FullName} has active enrollments and cannot be deleted.");

            _students.Remove(student);
        }

        public List<Student> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return _students;

            term = term.ToLower();
            return _students
                .Where(s => s.FullName.ToLower().Contains(term) ||
                           s.Email.ToLower().Contains(term) ||
                           s.Id.ToString().Contains(term))
                .ToList();
        }
    }
}