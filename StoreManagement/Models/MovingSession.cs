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

        [Key]
        public int MovingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int StoredBoxPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
