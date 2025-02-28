namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CheckingSession")]
    public partial class CheckingSession
    {
        public CheckingSession()
        {
        }

        public CheckingSession(double checkedQuantity, double unqualifiedQuantity, int identifiedItemPK, string userID, string comment)
        {
            CheckedQuantity = checkedQuantity;
            UnqualifiedQuantity = unqualifiedQuantity;
            ExecutedDate = DateTime.Now;
            IdentifiedItemPK = identifiedItemPK;
            UserID = userID;
            Comment = comment;
        }

        [Key]
        public int CheckingSessionPK { get; set; }

        public double CheckedQuantity { get; set; }

        public double UnqualifiedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int IdentifiedItemPK { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public string Comment { get; set; }
    }
}
