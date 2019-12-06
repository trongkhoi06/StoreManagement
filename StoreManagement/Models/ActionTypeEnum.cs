namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ActionTypeEnum")]
    public partial class ActionTypeEnum
    {
        public ActionTypeEnum()
        {
        }

        [Key]
        [StringLength(50)]
        public string Action { get; set; }
    }
}
