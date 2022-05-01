using NguniDemo.Models;
using NguniDemo.Repositories;
using NguniDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguniDemo.Areas.Admin.Controllers
{
    public class TableTypeController : Controller
    {
        // GET: Admin/TableType
        TableTypeService accomodationTypeService = new TableTypeService();
        // GET: Dashboard/AccomodationTypes
        public ActionResult Index(string searchTerm, int? page)
        {
            TableTypeListingModel model = new TableTypeListingModel();

            int recordSize = 5;
            page = page ?? 1;
            model.SearchTerm = searchTerm;


            model.TableTypes = accomodationTypeService.SearchTableTypes(searchTerm, page.Value, recordSize);

            var totalRecords = accomodationTypeService.SearchTableTypeCount(searchTerm);
            model.Pager = new Pager(totalRecords, page, recordSize);
            return View(model);
        }

        [HttpGet]
        public ActionResult Action(int? ID)
        {
            TableTypeActionModel model = new TableTypeActionModel();

            if (ID.HasValue)  //we are trying to edit a record
            {
                var accomodationType = accomodationTypeService.GetTableTypeByID(ID.Value);

                model.ID = accomodationType.TabletypeId;
                model.Name = accomodationType.Name;
                model.Description = accomodationType.Description;
            }
            return PartialView("_Action", model);
        }


        [HttpPost]
        public JsonResult Action(TableTypeActionModel model)
        {
            JsonResult json = new JsonResult();

            var result = false;

            if (model.ID > 0)
            {
                var accomodationType = accomodationTypeService.GetTableTypeByID(model.ID);

                accomodationType.Name = model.Name;
                accomodationType.Description = model.Description;

                result = accomodationTypeService.UpdateTableType(accomodationType);
            }
            else
            {
                TableType accomodationType = new TableType();

                accomodationType.Name = model.Name;
                accomodationType.Description = model.Description;

                result = accomodationTypeService.SaveTableType(accomodationType);
            }


            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Types." };
            }
            return json;
        }


        [HttpGet]
        public ActionResult Delete(int ID)
        {
            TableTypeActionModel model = new TableTypeActionModel();

            var accomodationType = accomodationTypeService.GetTableTypeByID(ID);

            model.ID = accomodationType.TabletypeId;


            return PartialView("_Delete", model);
        }


        [HttpPost]
        public JsonResult Delete(TableType model)
        {
            JsonResult json = new JsonResult();

            var result = false;

            var accomodationType = accomodationTypeService.GetTableTypeByID(model.TabletypeId);



            result = accomodationTypeService.DeleteTableType(accomodationType);

            if (result)
            {
                json.Data = new { Success = true };
            }
            else
            {
                json.Data = new { Success = false, Message = "Unable to perform action on Accomodation Types." };
            }
            return json;
        }
    }
}