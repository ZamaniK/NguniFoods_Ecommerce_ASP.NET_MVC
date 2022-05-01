using NguniDemo.Models;
using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class NguniTablesController : Controller
    {
        // GET: NguniTables
        // GET: SubjectDashboard
        TableTypeService gradeService = new TableTypeService();
        TablesService subjectsService = new TablesService();

        //// GET: Accomodations
        public ActionResult Index(int gradeID, int? subjectID)
        {
            TablesDashboardVM model = new TablesDashboardVM();

            model.TableTypes = gradeService.GetTableTypeByID(gradeID);
            model.Tables = subjectsService.GetAllTableTypes(gradeID);


            model.SelectedTableID = subjectID.HasValue ? subjectID.Value : model.Tables.First().TableId;
            return View(model);
        }

        public ActionResult Details(int id)
        {
            TablesDetailsViewModel model = new TablesDetailsViewModel();

            model.Tables = subjectsService.GetTableByID(id);
            return View(model);
        }

        public ActionResult TableBooking()
        {
             ApplicationDbContext db = new ApplicationDbContext();
            var rooms = db.Table.Include(r => r.TableTypes);
            return View(rooms.ToList());
        }
        public ActionResult TableBooking1(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var rooms = db.Table.Include(r => r.TableTypes);
            return View(rooms.ToList().Where(r => r.TableId == id));
        }

        // GET: Rooms
        public ActionResult Index2()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var rooms = db.Table.Include(r => r.TableTypes);
            return View(rooms.ToList());
        }
    }
}