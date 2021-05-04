using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiegoG.Utilities.DBModels
{
    [Table("PhoneNumber")]
    public class PhoneNumber
    {
        public int Id { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Number
        {
            get => NumberField;
            set
            {
                var a = value.IndexOf(' ');
                CountryCode = value[..a];
                Line = value[(a + 1)..];
                NumberField = value;
            }
        }

        [DataType(DataType.PhoneNumber)]
        private string NumberField { get; set; }

        public string Comment { get; set; }

        public string CountryCode { get; private set; }
        public string Line { get; private set; }

        public PhoneNumber(string number) => Number = number;
        public PhoneNumber() { }
    }
}
