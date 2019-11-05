namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Size")]
    public partial class Size
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SizeID { get; set; }

        [Column("Size")]
        public int? Size1 { get; set; }

        public int? ConceptionCode { get; set; }
    }
}
