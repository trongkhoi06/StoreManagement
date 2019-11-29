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
        public Demand()
        {
        }

        [Key]
        public int DemandPK { get; set; }

        [Required]
        [StringLength(100)]
        public string DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public int TotalDemand { get; set; }

        public DateTime DateCreated { get; set; }

        public int ConceptionPK { get; set; }

        [Required]
        public string ReceiveDivision { get; set; }
    }
}
