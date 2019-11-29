namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Unit")]
    public partial class Unit
    {
        public Unit()
        {
        }

        [Key]
        public int UnitPK { get; set; }

        public string UnitName { get; set; }
    }
}
