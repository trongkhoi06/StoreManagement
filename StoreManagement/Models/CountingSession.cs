namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CountingSession")]
    public partial class CountingSession
    {
        public CountingSession()
        {

        }

        public CountingSession(int identifiedItemPK, double countedQuantity, string userID)
        {
            CountedQuantity = countedQuantity;
            ExecutedDate = DateTime.Now;
            IdentifiedItemPK = identifiedItemPK;
            UserID = userID;
        }

        [Key]
        public int CountingSessionPK { get; set; }

        public double CountedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int IdentifiedItemPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }

}
