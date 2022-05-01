using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguniDemo.Repositories
{
    public class DashboardService
    {
        public bool SavePicture(Picture picture)
        {
            var context = new ApplicationDbContext();

            context.Pictures.Add(picture);
            return context.SaveChanges() > 0;
        }


        public IEnumerable<Picture> GetPicturesByID(List<int> pictureIDs)
        {
            var context = new ApplicationDbContext();
            return pictureIDs.Select(x => context.Pictures.Find(x)).ToList();
        }

    }
}