using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiegoG.Utilities.DBModels
{
    public class Name
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Name(string n) { Value = n; }
        public Name() { }
    }
    [Table("Names")]
    public class NameCollection : ICollection<Name>, IEnumerable<Name>, ISerializable, IEnumerable<string>
    {
        public int Id { get; set; }

        [Required]
        public LinkedList<Name> NameList { get; set; } = new();

        [NotMapped, JsonIgnore]
        public string First => NameList.First?.Value?.Value;

        [NotMapped, JsonIgnore]
        public int Count => NameList.Count;

        [NotMapped, JsonIgnore]
        public bool IsReadOnly => false;

        public bool Contains(Name item) => NameList.Contains(item);
        public void CopyTo(Name[] array, int arrayIndex) => NameList.CopyTo(array, arrayIndex);

        public IEnumerator<Name> GetEnumerator() => NameList.GetEnumerator();
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            foreach (var n in NameList)
                yield return n.Value;
        }
        IEnumerator IEnumerable.GetEnumerator() => NameList.GetEnumerator();
        public void Add(Name item) => NameList.AddLast(item);
        public void Clear() => NameList.Clear();
        public bool Remove(Name item) => NameList.Remove(item);
        public void GetObjectData(SerializationInfo info, StreamingContext context) => NameList.GetObjectData(info, context);

        public NameCollection(params string[] names)
        {
            foreach (var n in names)
                NameList.AddLast(new Name(n));
        } 

        public NameCollection() { }
    }
}
