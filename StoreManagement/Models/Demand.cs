namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Demand")]
    public partial class Demand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Demand()
        {
            DemandedItems = new HashSet<DemandedItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public int DemandedQuantity { get; set; }

        public DateTime DateCreated { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DemandedItem> DemandedItems { get; set; }
    }
}
