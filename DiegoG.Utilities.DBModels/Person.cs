using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DiegoG.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiegoG.Utilities.DBModels
{
    public class Person
    {
        public int Id { get; set; }
        [Required, Column("FirstNames")]
        public NameCollection FirstNames { get; set; } = new();

        [Required, Column("LastNames")]
        public NameCollection LastNames { get; set; } = new();

#nullable enable
        [Required, Column("NickNames")]
        public NameCollection? NickNames { get; set; } = new();
#nullable disable

        [NotMapped]
        public string FirstName => FirstNames.First;

        [NotMapped]
        public string FirstAndLastNames => $"{FirstNames.First} {LastNames.First}";

        [NotMapped]
        public string FullName => $"{FirstNames.Flatten()} '{NickNames.First}' {LastNames.Flatten()}";

        public List<PhoneNumber> PhoneNumbers { get; set; } = new();

        public List<EmailAddress> Emails { get; set; } = new();

        public List<Address> Addresses { get; set; } = new();
    }
}
