using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Personal
{
    public class NameCollection : IEnumerable<string>
    {
        private readonly LinkedList<string> NameList = new();

        public string First => NameList.First.Value;

        public int Count => NameList.Count;

        public bool Contains(string item) => NameList.Contains(item);
        public void CopyTo(string[] array, int arrayIndex) => NameList.CopyTo(array, arrayIndex);

        public IEnumerator<string> GetEnumerator() => NameList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => NameList.GetEnumerator();

        public NameCollection(params string[] names)
        {
            foreach (var n in names)
                NameList.AddLast(n);
        } 

    }
}
