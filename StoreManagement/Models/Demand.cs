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

        public Demand(string demandID, int startWeek, int endWeek, double totalDemand, int conceptionPK, string receiveDivision, string userID)
        {
            DemandID = demandID;
            StartWeek = startWeek;
            EndWeek = endWeek;
            TotalDemand = totalDemand;
            DateCreated = DateTime.Now;
            ConceptionPK = conceptionPK;
            ReceiveDivision = receiveDivision;
            IsOpened = true;
            UserID = userID;
        }

        [Key]
        public int DemandPK { get; set; }

        [Required]
        [StringLength(100)]
        public string DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public double TotalDemand { get; set; }

        public DateTime DateCreated { get; set; }

        public int ConceptionPK { get; set; }

        [Required]
        public string ReceiveDivision { get; set; }

        public bool IsOpened { get; set; }

        [Required]
        [StringLength(100)]
        public string UserID { get; set; }
    }
}
