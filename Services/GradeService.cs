using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class GradeService : IGradeService
    {
        private List<GradeRecord> _grades = new();

        public void SubmitGrade(GradeRecord grade)
        {
            // Validation
            if (grade.Marks < 0 || grade.Marks > 100)
                throw new ArgumentException("Marks must be between 0 and 100.");
            if (grade.CreditHours <= 0)
                throw new ArgumentException("Credit hours must be positive.");
            if (string.IsNullOrWhiteSpace(grade.CourseTitle))
                throw new ArgumentException("Course title is required.");

            // Check if grade already exists for this student and course
            var existing = _grades.FirstOrDefault(g => 
                g.StudentId == grade.StudentId && g.CourseId == grade.CourseId);
            
            if (existing != null)
            {
                // Update existing grade
                existing.Marks = grade.Marks;
                existing.CourseTitle = grade.CourseTitle;
                existing.CreditHours = grade.CreditHours;
            }
            else
            {
                // Add new grade
                _grades.Add(grade);
            }

            // Update student's CGPA
            UpdateStudentCGPA(grade.StudentId);
        }

        public List<GradeRecord> GetGradesForCourse(Guid courseId)
        {
            return _grades
                .Where(g => g.CourseId == courseId)
                .OrderBy(g => g.StudentId)
                .ToList();
        }

        public List<GradeRecord> GetGradesForStudent(Guid studentId)
        {
            return _grades
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.CourseTitle)
                .ToList();
        }

        public decimal ComputeCGPA(Guid studentId)
        {
            var studentGrades = GetGradesForStudent(studentId);
            if (!studentGrades.Any())
                return 0.0m;

            decimal totalGradePoints = 0;
            int totalCreditHours = 0;

            foreach (var grade in studentGrades)
            {
                totalGradePoints += grade.GradePoints * grade.CreditHours;
                totalCreditHours += grade.CreditHours;
            }

            return totalCreditHours > 0 ? totalGradePoints / totalCreditHours : 0.0m;
        }

        private void UpdateStudentCGPA(Guid studentId)
        {
            var cgpa = ComputeCGPA(studentId);
            var student = SeedData.Students.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                student.CGPA = cgpa;
            }
        }
    }
}