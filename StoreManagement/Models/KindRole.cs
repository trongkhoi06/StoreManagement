namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KindRole")]
    public partial class KindRole
    {
        public KindRole()
        {
        }

        [Key]
        [StringLength(100)]
        public string KindRoleName { get; set; }

        public bool Sign { get; set; }
    }
}
