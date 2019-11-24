namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConceptionAccessoryCustomer")]
    public partial class ConceptionAccessoryCustomer
    {
        [Key]
        public int ConceptionAccessoryPK { get; set; }

        public int? ConceptionPK { get; set; }

        public int? AccessoryPK { get; set; }

        public int CustomerPK { get; set; }

        public virtual Accessory Accessory { get; set; }

        public virtual Conception Conception { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
