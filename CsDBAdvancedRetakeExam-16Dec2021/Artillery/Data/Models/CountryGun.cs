using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Artillery.Data.Models
{
    public class CountryGun
    {
        [ForeignKey(nameof(Country))]
        public int CountryId{ get; set; }
        public virtual Country Country { get; set; }

        [ForeignKey(nameof(Gun))]
        public int GunId{ get; set; }
        public virtual Gun Gun{ get; set; }
    }
}
//•	CountryId – Primary Key integer, foreign key (required)
//•	GunId – Primary Key integer, foreign key (required)
