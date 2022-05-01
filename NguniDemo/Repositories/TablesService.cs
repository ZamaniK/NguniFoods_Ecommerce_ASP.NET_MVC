using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.Repositories
{
    public class TablesService
    {


        public IEnumerable<Table> GetAllTables()
        {
            var context = new ApplicationDbContext();
            return context.Table.ToList();
        }

        public IEnumerable<Table> GetAllTableTypes(int accomodationTypeID)
        {
            var context = new ApplicationDbContext();
            return context.Table.Where(x => x.TableId == accomodationTypeID).ToList();
        }

        public IEnumerable<Table> SearchTables(string searchTerm, int? accomodationTypeID, int page, int recordSize)
        {
            var context = new ApplicationDbContext();
            var accomodationPackages = context.Table.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                accomodationPackages = accomodationPackages.Where(a => a.TableName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (accomodationTypeID.HasValue && accomodationTypeID.Value > 0)
            {
                accomodationPackages = accomodationPackages.Where(a => a.TableId == accomodationTypeID.Value);
            }

            var skip = (page - 1) * recordSize;

            return accomodationPackages.OrderBy(x => x.TableId).Skip(skip).Take(recordSize).ToList();
        }


        public int SearchTableCount(string searchTerm, int? accomodationTypeID)
        {
            var context = new ApplicationDbContext();
            var accomodationPackages = context.Table.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                accomodationPackages = accomodationPackages.Where(a => a.TableName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (accomodationTypeID.HasValue && accomodationTypeID.Value > 0)
            {
                accomodationPackages = accomodationPackages.Where(a => a.TableId == accomodationTypeID.Value);
            }
            return accomodationPackages.Count();
        }

        public Table GetTableByID(int ID)
        {
            var context = new ApplicationDbContext();
            return context.Table.Find(ID);
        }

        public bool SaveAccomodationPackage(Table accomodationPackage)
        {
            var context = new ApplicationDbContext();

            context.Table.Add(accomodationPackage);
            return context.SaveChanges() > 0;
        }

        public bool UpdateTable(Table accomodationPackage)
        {
            var context = new ApplicationDbContext();
            var existingAccomodationPackage = context.Table.Find(accomodationPackage.TableId);
            context.TablePictures.RemoveRange(existingAccomodationPackage.TablePictures);
            context.Entry(existingAccomodationPackage).CurrentValues.SetValues(accomodationPackage);
            context.TablePictures.AddRange(accomodationPackage.TablePictures);
            return context.SaveChanges() > 0;
        }

        public bool DeleteTable(Table accomodationPackage)
        {
            var context = new ApplicationDbContext();

            var existingAccomodationPackage = context.Table.Find(accomodationPackage.TableId);
            context.TablePictures.RemoveRange(existingAccomodationPackage.TablePictures);
            context.Entry(existingAccomodationPackage).State = System.Data.Entity.EntityState.Deleted;
            return context.SaveChanges() > 0;
        }

        public List<TablePictures> GetPicturesByAccomodationPackageID(int accomodationPackageID)
        {
            var context = new ApplicationDbContext();
            return context.Table.Find(accomodationPackageID).TablePictures.ToList();
        }

    }
}