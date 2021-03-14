using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DiegoG.Utilities.Personal
{
    public record PhoneNumber
    {
        [DataType(DataType.PhoneNumber)]
        public string Number
        {
            get => NumberField;
            init
            {
                var a = value.IndexOf(' ');
                CountryCode = value[..a];
                Line = value[(a + 1)..];
                NumberField = value;
            }
        }

        [DataType(DataType.PhoneNumber)]
        private string NumberField { get; init; }

        public string CountryCode { get; private set; }
        public string Line { get; private set; }
    }
}
