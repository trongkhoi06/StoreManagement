namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IssuedGroup")]
    public partial class IssuedGroup : IEquatable<IssuedGroup>
    {
        public IssuedGroup()
        {
        }

        public IssuedGroup(double issuedGroupQuantity, int issuePK, int demandedItemPK, int unstoredBoxPK, int itemPK, bool isRestored, int accessoryPK)
        {
            IssuedGroupQuantity = issuedGroupQuantity;
            IssuePK = issuePK;
            DemandedItemPK = demandedItemPK;
            UnstoredBoxPK = unstoredBoxPK;
            ItemPK = itemPK;
            IsRestored = isRestored;
            AccessoryPK = accessoryPK;
        }

        [Key]
        public int IssuedGroupPK { get; set; }

        public double IssuedGroupQuantity { get; set; }

        public int IssuePK { get; set; }

        public int DemandedItemPK { get; set; }

        public int? UnstoredBoxPK { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }

        public int AccessoryPK { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as IssuedGroup);
        }

        public bool Equals(IssuedGroup other)
        {
            return other != null &&
                   UnstoredBoxPK == other.UnstoredBoxPK &&
                   ItemPK == other.ItemPK &&
                   IsRestored == other.IsRestored;
        }

        public override int GetHashCode()
        {
            var hashCode = -1309090341;
            hashCode = hashCode * -1521134295 + UnstoredBoxPK.GetHashCode();
            hashCode = hashCode * -1521134295 + ItemPK.GetHashCode();
            hashCode = hashCode * -1521134295 + IsRestored.GetHashCode();
            return hashCode;
        }
    }
}
