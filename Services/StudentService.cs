using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Exceptions;
using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    /// <summary>
    /// SRP: Manages student records only. ISP: Depends on IStudentService without grade operations. DIP: Pages consume the interface abstraction.
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly List<Student> _students = SeedData.Students;

        public List<Student> GetAll() => _students;
        public Student? GetById(Guid id) => _students.FirstOrDefault(s => s.Id == id);

        public void Add(Student entity)
        {
            if (!entity.Validate(out var errorMessage)) throw new ArgumentException(errorMessage);
            if (string.IsNullOrWhiteSpace(entity.PasswordHash)) throw new ArgumentException("Password is required.");
            if (_students.Any(s => s.Email.Equals(entity.Email, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("A student with this email already exists.");
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            _students.Add(entity);
            if (!SeedData.Users.Any(u => u.Id == entity.Id)) SeedData.Users.Add(entity);
        }

        public void Update(Student entity)
        {
            if (!entity.Validate(out var errorMessage)) throw new ArgumentException(errorMessage);
            if (string.IsNullOrWhiteSpace(entity.PasswordHash)) throw new ArgumentException("Password is required.");
            var existing = GetById(entity.Id) ?? throw new ArgumentException("Student not found.");
            if (_students.Any(s => s.Id != entity.Id && s.Email.Equals(entity.Email, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("A student with this email already exists.");
            existing.FullName = entity.FullName;
            existing.Email = entity.Email;
            existing.PasswordHash = entity.PasswordHash;
            existing.Semester = entity.Semester;
            existing.CGPA = entity.CGPA;
        }

        public void Delete(Guid id)
        {
            var student = GetById(id);
            if (student == null) return;
            if (student.Enrollments.Any()) throw new StudentHasActiveEnrollmentsException($"Student {student.FullName} has active enrollments and cannot be deleted.");
            _students.Remove(student);
            SeedData.Users.RemoveAll(u => u.Id == id);
        }

        public List<Student> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return _students;
            term = term.Trim().ToLowerInvariant();
            return _students.Where(s => s.FullName.ToLowerInvariant().Contains(term) || s.Email.ToLowerInvariant().Contains(term) || s.Id.ToString().Contains(term)).ToList();
        }
    }
}
