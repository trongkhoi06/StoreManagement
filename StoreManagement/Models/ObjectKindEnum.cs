namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ObjectKindEnum")]
    public partial class ObjectKindEnum
    {
        public ObjectKindEnum()
        {

        }

        [Key]
        [StringLength(50)]
        public string Object { get; set; }
    }
}
