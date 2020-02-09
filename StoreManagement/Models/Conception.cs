namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Conception")]
    public partial class Conception
    {
        public Conception()
        {

        }

        public Conception(string conceptionCode, string description, int year, string season, int customerPK)
        {
            ConceptionCode = conceptionCode;
            Description = description;
            Year = year;
            Season = season;
            CustomerPK = customerPK;
            IsActive = true;
        }

        [Key]
        public int ConceptionPK { get; set; }

        [Required]
        [StringLength(50)]
        public string ConceptionCode { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string Season { get; set; }

        public bool IsActive { get; set; }

        public int CustomerPK { get; set; }
    }
}
