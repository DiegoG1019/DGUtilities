using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Personal
{
    public class EmailAddress
    {
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get => EmailField;
            init
            {
                var vals = value.Split('@');
                if (vals.Length != 2)
                    throw new ArgumentException("Invalid Email Address: " + value, nameof(value));
                Username = vals[0];
                Host = vals[1];
            }
        }

        [DataType(DataType.EmailAddress)]
        private string EmailField { get; init; }

        public string Username { get; private set; }
        public string Host { get; private set; }
    }
}
