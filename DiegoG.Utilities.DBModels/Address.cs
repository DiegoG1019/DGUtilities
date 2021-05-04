using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiegoG.Utilities.DBModels
{
    public class Address
    {
        public int Id { get; set; }
        public class Coordinates
        {
            public int Id { get; set; }
            [Range(double.MinValue, double.MaxValue)]
            public double X { get; set; }
            [Range(double.MinValue, double.MaxValue)]
            public double Y { get; set; }
            public override string ToString() => $"X:{X};Y:{Y}";
        }
        [StringLength(56, MinimumLength = 1), Required]
        public string Country { get; set; }

        [StringLength(56, MinimumLength = 1), Required]
        public string State { get; set; }

        [StringLength(56, MinimumLength = 1), Required]
        public string City { get; set; }

        [StringLength(60, MinimumLength = 1), Required]
        public string Street { get; set; }

        [StringLength(60, MinimumLength = 1), Required]
        public string Avenue { get; set; }

        [DataType(DataType.PostalCode), StringLength(10, MinimumLength = 1), Required]
        public string PostalCode { get; set; }
        public bool IsBuilding { get; set; }

        [StringLength(60, MinimumLength = 1), Required]
        public string RoomName { get; set; }

        [StringLength(300, MinimumLength = 1)]
        public string Comment { get; set; }
        public Coordinates GoogleMapCoordinates { get; set; }
    }
}
