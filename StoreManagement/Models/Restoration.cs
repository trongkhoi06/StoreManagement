namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Restoration")]
    public partial class Restoration
    {
        public Restoration()
        {
        }

        public Restoration(string restorationID, string userID, string comment)
        {
            RestorationID = restorationID;
            DateCreated = DateTime.Now;
            IsReceived = false;
            UserID = userID;
            Comment = comment;
        }

        [Key]
        public int RestorationPK { get; set; }

        [Required]
        [StringLength(100)]
        public string RestorationID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsReceived { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public string Comment { get; set; }
    }
}
