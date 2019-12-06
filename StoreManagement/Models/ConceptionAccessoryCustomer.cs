namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConceptionAccessory")]
    public partial class ConceptionAccessory
    {
        public ConceptionAccessory()
        {
        }

        [Key]
        public int ConceptionAccessoryPK { get; set; }

        public int ConceptionPK { get; set; }

        public int AccessoryPK { get; set; }
    }
}
