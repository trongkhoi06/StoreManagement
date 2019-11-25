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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckingSessionPK { get; set; }

        public int CheckedQuantity { get; set; }

        public int UnqualifiedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int IdentifiedItemPK { get; set; }

        public string UserID { get; set; }

        public string Comment { get; set; }

        public CheckingSession()
        {
            ExecutedDate = DateTime.Now;
        }

        public CheckingSession(int checkedQuantity, int unqualifiedQuantity, int identifiedItemPK, string userID)
        {
            CheckedQuantity = checkedQuantity;
            UnqualifiedQuantity = unqualifiedQuantity;
            ExecutedDate = DateTime.Now;
            IdentifiedItemPK = identifiedItemPK;
            UserID = userID;
        }
    }
}
