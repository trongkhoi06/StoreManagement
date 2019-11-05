namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IdentifyingSession")]
    public partial class IdentifyingSession
    {
        public IdentifyingSession()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }
    }
}
