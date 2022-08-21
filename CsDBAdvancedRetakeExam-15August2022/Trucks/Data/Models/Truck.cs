using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        public Truck()
        {
            this.ClientsTrucks = new List<ClientTruck>();
        }

        [Key]
        public int Id { get; set; }

       // [StringLength(8)]
        [MinLength(8)]
        [MaxLength(8)]
        //[Required]          //WARNING
        [RegularExpression(@"([A-Z]{2}\d{4}[A-Z]{2})")]
        public string RegistrationNumber { get; set; }

      //  [StringLength(17)]
        [MinLength(17)]
        [MaxLength(17)]
        [Required]
        public string VinNumber { get; set; }

        [Range(950, 1420)]
        public int TankCapacity { get; set; }


        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        public MakeType MakeType { get; set; }

        [Required]
        [ForeignKey(nameof(Despatcher))]
        public int DespatcherId { get; set; }
        public virtual Despatcher Despatcher { get; set; }

        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}


//•	DespatcherId – integer, foreign key(required)
//•	Despatcher – Despatcher 
//•	ClientsTrucks – collection of type ClientTruck
