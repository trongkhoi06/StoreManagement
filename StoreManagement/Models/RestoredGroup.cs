namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RestoredGroup")]
    public partial class RestoredGroup : IEquatable<RestoredGroup>
    {
        public RestoredGroup(double groupQuantity, int restoredItemPK, int unstoredBoxPK)
        {
            GroupQuantity = groupQuantity;
            RestoredItemPK = restoredItemPK;
            UnstoredBoxPK = unstoredBoxPK;
        }

        [Key]
        public int RestoredGroupPK { get; set; }

        public double GroupQuantity { get; set; }

        public int RestoredItemPK { get; set; }

        public int UnstoredBoxPK { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as RestoredGroup);
        }

        public bool Equals(RestoredGroup other)
        {
            return other != null &&
                   RestoredItemPK == other.RestoredItemPK;
        }

        public override int GetHashCode()
        {
            return 696051745 + RestoredItemPK.GetHashCode();
        }
    }
}
