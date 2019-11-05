namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Conception_Accessory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int C_A_RelationshipID { get; set; }

        public int ConceptionCode { get; set; }

        public int AccessoryID { get; set; }

        public virtual Accessory Accessory { get; set; }

        public virtual Conception Conception { get; set; }
    }
}
