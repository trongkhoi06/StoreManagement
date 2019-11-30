namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StoringSession")]
    public partial class StoringSession
    {
        public StoringSession()
        {
        }

        public StoringSession(int boxPK, string userID)
        {
            ExecutedDate = DateTime.Now;
            BoxPK = boxPK;
            UserID = userID;
        }

        [Key]
        public int StoringSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int BoxPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
