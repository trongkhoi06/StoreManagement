namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MovingSession")]
    public partial class MovingSession
    {
        public MovingSession()
        {
        }

        public MovingSession(StoredBox storedBox, Shelf shelf, string userID)
        {
            StartShelfPK = storedBox.ShelfPK;
            DestinationShelfPK = shelf.ShelfPK;
            StoredBoxPK = storedBox.StoredBoxPK;
            UserID = userID;
        }

        [Key]
        public int MovingSessionPK { get; set; }

        public int? StartShelfPK { get; set; }

        public int DestinationShelfPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int StoredBoxPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
