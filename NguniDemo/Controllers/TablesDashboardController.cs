using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Controllers
{
    public class TablesDashboardController : Controller
    {
        // GET: TablesDashboard

        // GET: Subjects
        public ActionResult Index()
        {
            TablesViewModel model = new TablesViewModel();
            TableTypeService gradeService = new TableTypeService();
            TablesService subjectService = new TablesService();

            model.TableTypes = gradeService.GetAllTableTypes();
            model.Tables = subjectService.GetAllTables();
            return View(model);
        }
    }
}