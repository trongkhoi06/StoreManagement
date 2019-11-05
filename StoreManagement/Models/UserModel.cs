namespace StoreManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserModel : DbContext
    {
        public UserModel()
            : base("name=UserModel")
        {
        }

        public virtual DbSet<Accessory> Accessories { get; set; }
        public virtual DbSet<ArrangingSession> ArrangingSessions { get; set; }
        public virtual DbSet<Box> Boxes { get; set; }
        public virtual DbSet<Conception> Conceptions { get; set; }
        public virtual DbSet<Conception_Accessory> Conception_Accessory { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Demand> Demands { get; set; }
        public virtual DbSet<DemandedItem> DemandedItems { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<IdentifiedItem> IdentifiedItems { get; set; }
        public virtual DbSet<IdentifyingSession> IdentifyingSessions { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderedItem> OrderedItems { get; set; }
        public virtual DbSet<Pack> Packs { get; set; }
        public virtual DbSet<PackedItem> PackedItems { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<RequestedItem> RequestedItems { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SystemUser> SystemUsers { get; set; }
        public virtual DbSet<UnstoredBox> UnstoredBoxes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
