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
        public Row()
        {
        }

        public Row(string rowID, bool isActive, int floor, int col)
        {
            RowID = rowID;
            IsActive = isActive;
            Floor = floor;
            Col = col;
        }

        [Key]
        public int RowPK { get; set; }

        [Required]
        [StringLength(50)]
        public string RowID { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int? Floor { get; set; }

        public int? Col { get; set; }
    }
}
