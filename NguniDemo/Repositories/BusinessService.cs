using NguniDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NguniDemo.Repositories
{
    public class BusinessService
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetTableType(int roomId)
        {
            var roomBuilding = (from rb in db.Table
                                where rb.TableId == roomId
                                select rb.TableTypes.Name).FirstOrDefault();
            return roomBuilding;
        }
       
        public static decimal GetTableCapacity(int roomId)
        {
            var roomCapacity = (from rb in db.Table
                                where rb.TableId == roomId
                                select rb.TableCapacity).FirstOrDefault();
            return roomCapacity;
        }
        
       
        public static Int32 GetNumberHours(DateTime Check_in, DateTime Check_Out)
        {
            return ((Check_Out.Date - Check_in.Date).Days);
        }
        public static decimal calcTotalRoomCost(TableReservation roomBooking)
        {
            return GetNumberHours(roomBooking.CheckInTime, roomBooking.CheckOutTime);
        }
        public static bool dateLessOutChecker(TableReservation roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInTime >= roomBooking.CheckOutTime)
            {
                check = true;
            }
            return check;
        }
        public static bool dateLessChecker(TableReservation roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInTime < DateTime.Now)
            {
                check = true;
            }
            return check;
        }
        public static bool roomChecker(TableReservation roomBooking)
        {
            bool check = false;
            var outDate = (from r in db.TableReservations
                           where r.TableId == roomBooking.TableId
                           select r.CheckOutTime
                         ).FirstOrDefault();
            if (roomBooking.CheckInTime >= outDate)
            {
                check = true;
            }
            return check;
        }
        public static void UpdateTablesAvailable(int roomId)
        {
            var roomTypeId = (from rb in db.Table
                              where rb.TableId == roomId
                              select rb.TabletypeId).FirstOrDefault();

            var roomsAvail = (from rb in db.TableTypes
                              where rb.TabletypeId == roomTypeId
                              select rb).FirstOrDefault();
            roomsAvail.TableAvailable -= 1;
            db.Entry(roomsAvail).State = EntityState.Modified;
            db.SaveChanges();

        }
    }
}