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

        public Demand(string demandID, double totalDemand, int conceptionPK, int workplacePK, string userID)
        {
            DemandID = demandID;
            TotalDemand = totalDemand;
            ConceptionPK = conceptionPK;
            IsOpened = true;
            UserID = userID;
            WorkplacePK = workplacePK;
            DateCreated = DateTime.Now;
        }

        [Key]
        public int DemandPK { get; set; }

        [Required]
        [StringLength(100)]
        public string DemandID { get; set; }

        public double TotalDemand { get; set; }

        public DateTime DateCreated { get; set; }

        public int ConceptionPK { get; set; }

        public bool IsOpened { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public int WorkplacePK { get; set; }
    }
}
