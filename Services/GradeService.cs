using System;
using System.Collections.Generic;
using System.Linq;
using EduConnect.Interfaces;
using EduConnect.Models;

namespace EduConnect.Services
{
    public class GradeService : IGradeService
    {
        public void SubmitGrade(GradeRecord grade)
        {
        }

        public List<GradeRecord> GetGradesForCourse(Guid courseId)
        {
            return new List<GradeRecord>();
        }

        public List<GradeRecord> GetGradesForStudent(Guid studentId)
        {
            return new List<GradeRecord>();
        }

        public decimal ComputeCGPA(Guid studentId)
        {
            return 0.0m;
        }
    }
}