using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.Utilities.Collections
{
    [Serializable]
    public class Pair<T1, T2>
    {
        public T1 ObjectA { get; set; }
        public T2 ObjectB { get; set; }
        public Pair() { }
        public Pair(T1 valueA, T2 valueB)
        {
            ObjectA = valueA;
            ObjectB = valueB;
        }
        public void Deconstruct(out T1 objectA, out T2 objectB) { objectA = ObjectA; objectB = ObjectB; }
        public static (IEnumerable<T1> EnumerableA, IEnumerable<T2> EnumerableB) SeparateEnumerable(IEnumerable<Pair<T1, T2>> pairs)
        {
            var lista = new List<T1>();
            var listb = new List<T2>();
            foreach(var i in pairs)
            {
                lista.Add(i.ObjectA);
                listb.Add(i.ObjectB);
            }
            return (lista, listb);
        }
    }
    [Serializable]
    public class Triplet<T1, T2, T3>
    {
        public T1 ObjectA { get; set; }
        public T2 ObjectB { get; set; }
        public T3 ObjectC { get; set; }
        public Triplet() { }
        public Triplet(T1 valueA, T2 valueB, T3 valueC)
        {
            ObjectA = valueA;
            ObjectB = valueB;
            ObjectC = valueC;
        }

        public void Deconstruct(out T1 objectA, out T2 objectB, out T3 objectC) { objectA = ObjectA; objectB = ObjectB; objectC = ObjectC; }
        public static (IEnumerable<T1> EnumerableA, IEnumerable<T2> EnumerableB, IEnumerable<T3> EnumerableC) SeparateEnumerable(IEnumerable<Triplet<T1, T2, T3>> triplets)
        {
            var lista = new List<T1>();
            var listb = new List<T2>();
            var listc = new List<T3>();
            foreach (var i in triplets)
            {
                lista.Add(i.ObjectA);
                listb.Add(i.ObjectB);
                listc.Add(i.ObjectC);
            }
            return (lista, listb, listc);
        }
    }
}
