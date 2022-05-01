using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.Repositories
{
    public class FoodTypeService
    {
        public IEnumerable<Food> GetAllGrades()
        {
            var context = new ApplicationDbContext();
            return context.Foods.ToList();
        }

        public IEnumerable<Food> SearchGrades(string searchTerm, int page, int recordSize)
        {
            var context = new ApplicationDbContext();
            var grades = context.Foods.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                grades = grades.Where(a => a.FoodType.ToLower().Contains(searchTerm.ToLower()));
            }

            var skip = (page - 1) * recordSize;

            return grades.OrderBy(t => t.FoodID).Skip(skip).Take(recordSize).ToList();
        }
        public int SearchGradeCount(string searchTerm)
        {
            var context = new ApplicationDbContext();
            var grades = context.Foods.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                grades = grades.Where(a => a.FoodType.ToLower().Contains(searchTerm.ToLower()));
            }


            return grades.Count();
        }

        public Food GetGradeByID(int ID)
        {
            var context = new ApplicationDbContext();

            return context.Foods.Find(ID);
        }

        public bool SaveGrade(Food grade)
        {
            var context = new ApplicationDbContext();

            context.Foods.Add(grade);
            return context.SaveChanges() > 0;
        }

        public bool UpdateGrade(Food grade)
        {
            var context = new ApplicationDbContext();

            var existingGrade = context.Foods.Find(grade.FoodID);
            context.Entry(existingGrade).State = System.Data.Entity.EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteGrade(Food grade)
        {
            var context = new ApplicationDbContext();

            var existingGrade = context.Foods.Find(grade.FoodID);
            context.Entry(existingGrade).State = System.Data.Entity.EntityState.Deleted;
            return context.SaveChanges() > 0;
        }
    }
}
