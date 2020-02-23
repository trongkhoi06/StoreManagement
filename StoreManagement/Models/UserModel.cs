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
        public virtual DbSet<AccessoryType> AccessoryTypes { get; set; }
        public virtual DbSet<ActionTypeEnum> ActionTypeEnums { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<AdjustingSession> AdjustingSessions { get; set; }
        public virtual DbSet<ArrangingSession> ArrangingSessions { get; set; }
        public virtual DbSet<Box> Boxes { get; set; }
        public virtual DbSet<CheckingSession> CheckingSessions { get; set; }
        public virtual DbSet<ClassifiedItem> ClassifiedItems { get; set; }
        public virtual DbSet<ClassifyingSession> ClassifyingSessions { get; set; }
        public virtual DbSet<Conception> Conceptions { get; set; }
        public virtual DbSet<ConceptionAccessory> ConceptionAccessories { get; set; }
        public virtual DbSet<ConfirmingSession> ConfirmingSessions { get; set; }
        public virtual DbSet<CountingSession> CountingSessions { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Demand> Demands { get; set; }
        public virtual DbSet<DemandedItem> DemandedItems { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DiscardingSession> DiscardingSessions { get; set; }
        public virtual DbSet<Entry> Entries { get; set; }
        public virtual DbSet<FailedItem> FailedItems { get; set; }
        public virtual DbSet<GroupItem_ArrangingSession> GroupItem_ArrangingSession { get; set; }
        public virtual DbSet<IdentifiedItem> IdentifiedItems { get; set; }
        public virtual DbSet<IdentifyingSession> IdentifyingSessions { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<IssuedGroup> IssuedGroups { get; set; }
        public virtual DbSet<KindRole> KindRoles { get; set; }
        public virtual DbSet<MovingSession> MovingSessions { get; set; }
        public virtual DbSet<ObjectKindEnum> ObjectKindEnums { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderedItem> OrderedItems { get; set; }
        public virtual DbSet<Pack> Packs { get; set; }
        public virtual DbSet<PackedItem> PackedItems { get; set; }
        public virtual DbSet<PassedItem> PassedItems { get; set; }
        public virtual DbSet<ReceivingSession> ReceivingSessions { get; set; }
        public virtual DbSet<Restoration> Restorations { get; set; }
        public virtual DbSet<RestoredGroup> RestoredGroups { get; set; }
        public virtual DbSet<RestoredItem> RestoredItems { get; set; }
        public virtual DbSet<StorebackSession> StorebackSessions { get; set; }
        public virtual DbSet<ReturningSession> ReturningSessions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Row> Rows { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<StoredBox> StoredBoxes { get; set; }
        public virtual DbSet<StoringSession> StoringSessions { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SystemUser> SystemUsers { get; set; }
        public virtual DbSet<TransferringSession> TransferringSessions { get; set; }
        public virtual DbSet<UnstoredBox> UnstoredBoxes { get; set; }
        public virtual DbSet<Verification> Verifications { get; set; }
        public virtual DbSet<Workplace> Workplaces { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
