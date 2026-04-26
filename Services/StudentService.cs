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
        public List<Student> GetAll()
        {
            return new List<Student>();
        }

        public Student? GetById(Guid id)
        {
            return null;
        }

        public void Add(Student entity)
        {
        }

        public void Update(Student entity)
        {
        }

        public void Delete(Guid id)
        {
        }

        public List<Student> Search(string term)
        {
            return new List<Student>();
        }
    }
}