using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NguniDemo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public virtual DbSet<Admin> Admins { get; set; }

        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<FoodItem> FoodItems { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<TableType> TableTypes { get; set; }
        public virtual DbSet<Table> Table { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<TablePictures> TablePictures { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }
        public DbSet<VenueTime> VenueTimes1 { get; set; }
        public DbSet<VenueTimes> VenueTimes { get; set; }
        public DbSet<VenueBooking> VenueBookings { get; set; }



        public virtual DbSet<TableReservation> TableReservations { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Food>()
                .Property(e => e.FoodType)
                .IsUnicode(false);

            modelBuilder.Entity<Food>()
                .HasMany(e => e.FoodItems)
                .WithRequired(e => e.Food)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FoodItem>()
                .Property(e => e.FoodItemName)
                .IsUnicode(false);

            modelBuilder.Entity<FoodItem>()
                .Property(e => e.FoodType)
                .IsUnicode(false);

            modelBuilder.Entity<FoodItem>()
                .Property(e => e.ShortDesc)
                .IsUnicode(false);

            modelBuilder.Entity<FoodItem>()
                .Property(e => e.LongDesc)
                .IsUnicode(false);

            modelBuilder.Entity<FoodItem>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);
        }
    }
}
