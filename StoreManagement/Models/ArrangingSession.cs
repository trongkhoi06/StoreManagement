namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ArrangingSession")]
    public partial class ArrangingSession
    {
        public ArrangingSession()
        {

        }

        public ArrangingSession(int startBoxPK, int destinationBoxPK, string userID)
        {
            StartBoxPK = startBoxPK;
            DestinationBoxPK = destinationBoxPK;
            UserID = userID;
            ExecutedDate = DateTime.Now;
        }

        [Key]
        public int ArrangingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int StartBoxPK { get; set; }

        public int DestinationBoxPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
