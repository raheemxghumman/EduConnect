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
        public List<Course> GetAll()
        {
            return new List<Course>();
        }

        public Course? GetById(Guid id)
        {
            return null;
        }

        public void Add(Course entity)
        {
        }

        public void Update(Course entity)
        {
        }

        public void Delete(Guid id)
        {
        }

        public void EnrollStudent(Guid courseId, Guid studentId)
        {
        }

        public void DropCourse(Guid courseId, Guid studentId)
        {
        }

        public List<Course> GetForFaculty(Guid facultyId)
        {
            return new List<Course>();
        }

        public List<Course> GetAvailable()
        {
            return new List<Course>();
        }
    }
}