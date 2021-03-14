using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DiegoG.Utilities.Collections;

namespace DiegoG.Utilities.Personal
{
    public record Person()
    {
#nullable enable
        public NameCollection FirstNames { get; init; }
        public NameCollection LastNames { get; init; }
        public NameCollection? NickNames { get; init; }
#nullable disable
        public string FirstAndLastNames => $"{FirstNames.First} {LastNames.First}";
        public string FullName => $"{FirstNames.Flatten()} '{NickNames.First}' {LastNames.Flatten()}";
        /// <summary>
        /// Tag, PhoneNumber
        /// </summary>
        public IReadOnlyDictionary<string, PhoneNumber> PhoneNumbers { get; init; }

        /// <summary>
        /// Tag, Email
        /// </summary>
        public IReadOnlyDictionary<string, EmailAddress> Emails { get; init; }
    }
}
