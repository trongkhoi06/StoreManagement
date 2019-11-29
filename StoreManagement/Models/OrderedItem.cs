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

        public OrderedItem(int orderPK, Client_Accessory_OrderedQuantity_Comment temp)
        {
            OrderedQuantity = temp.OrderedQuantity;
            Comment = temp.Comment;
            OrderPK = orderPK;
            AccessoryPK = temp.AccessoryPK;
        }

        [Key]
        public int OrderedItemPK { get; set; }

        public int OrderedQuantity { get; set; }

        public string Comment { get; set; }

        public int OrderPK { get; set; }

        public int AccessoryPK { get; set; }

        public int? UnitPK { get; set; }
    }
}
