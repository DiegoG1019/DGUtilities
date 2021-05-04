using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.DBModels
{
    public class EmailAddress
    {
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get => EmailField;
            set
            {
                if(!TrySetEmail(value))
                    throw new ArgumentException("Invalid Email Address: " + value, nameof(value));
            }
        }

        public bool TrySetEmail(string email)
        {
            var vals = email.Split('@');

            if (vals.Length != 2 || !RegexUtilities.IsValidEmail(email))
                return false;

            EmailField = email;
            Username = vals[0];
            Host = vals[1];

            return true;
        }

        private string EmailField { get; set; }

        public string Comment { get; set; }

        public string Username { get; private set; }
        public string Host { get; private set; }

        public EmailAddress(string email) => Email = email;
        public EmailAddress() { }
    }
}
