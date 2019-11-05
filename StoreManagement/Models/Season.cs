namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Season")]
    public partial class Season
    {
        public Season()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeasonID { get; set; }

        public int? Year { get; set; }

        public int? Period { get; set; }
    }
}
