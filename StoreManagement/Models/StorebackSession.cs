namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RestoringSession")]
    public partial class StorebackSession
    {
        public StorebackSession()
        {
        }

        public StorebackSession(string userID, int issuePK)
        {
            UserID = userID;
            IssuePK = issuePK;
            ExecutedDate = DateTime.Now;
        }

        [Key]
        public int StorebackSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public int IssuePK { get; set; }
    }
}
