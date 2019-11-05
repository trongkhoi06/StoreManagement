namespace StoreManagement.Models
{
    using StoreManagement.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderedItem")]
    public partial class OrderedItem
    {
        public OrderedItem()
        {
        }

        public OrderedItem(int OrderPK, Client_Accessory_OrderedQuantity_Comment temp)
        {
            this.OrderPK = OrderPK;
            this.OrderedQuantity = temp.OrderedQuantity;
            this.Comment = temp.Comment;
            this.AccessoryPK = temp.AccessoryPK;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderedItemPK { get; set; }

        public int OrderedQuantity { get; set; }

        public string Comment { get; set; }

        public int OrderPK { get; set; }

        public int AccessoryPK { get; set; }
    }
}
