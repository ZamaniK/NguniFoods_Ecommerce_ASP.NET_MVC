using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.Repositories
{
    public class TableTypeService
    {
        public IEnumerable<TableType> GetAllTableTypes()
        {
            var context = new ApplicationDbContext();
            return context.TableTypes.ToList();
        }

        public IEnumerable<TableType> SearchTableTypes(string searchTerm, int page, int recordSize)
        {
            var context = new ApplicationDbContext();
            var grades = context.TableTypes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                grades = grades.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            var skip = (page - 1) * recordSize;

            return grades.OrderBy(t => t.TabletypeId).Skip(skip).Take(recordSize).ToList();
        }
        public int SearchTableTypeCount(string searchTerm)
        {
            var context = new ApplicationDbContext();
            var grades = context.TableTypes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                grades = grades.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()));
            }


            return grades.Count();
        }

        public TableType GetTableTypeByID(int ID)
        {
            var context = new ApplicationDbContext();

            return context.TableTypes.Find(ID);
        }

        public bool SaveTableType(TableType accomodationType)
        {
            var context = new ApplicationDbContext();

            context.TableTypes.Add(accomodationType);
            return context.SaveChanges() > 0;
        }

        public bool UpdateTableType(TableType accomodationType)
        {
            var context = new ApplicationDbContext();

            context.Entry(accomodationType).State = System.Data.Entity.EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteTableType(TableType accomodationType)
        {
            var context = new ApplicationDbContext();

            context.Entry(accomodationType).State = System.Data.Entity.EntityState.Deleted;
            return context.SaveChanges() > 0;
        }
    }
}
