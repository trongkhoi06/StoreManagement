namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Row")]
    public partial class Row
    {
        [Key]
        public int RowPK { get; set; }

        [Required]
        [StringLength(50)]
        public string RowID { get; set; }
    }
}
