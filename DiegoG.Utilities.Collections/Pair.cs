using System;
using System.Collections.Generic;

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
        public static implicit operator (T1 ObjA, T2 ObjB)(Pair<T1, T2> p) => (p.ObjectA, p.ObjectB);

        public static implicit operator Pair<T1, T2>((T1 ObjA, T2 ObjB) p) => new(p.ObjA, p.ObjB);

        public static (IEnumerable<T1> EnumerableA, IEnumerable<T2> EnumerableB) SeparateEnumerable(IEnumerable<Pair<T1, T2>> pairs)
        {
            var lista = new List<T1>();
            var listb = new List<T2>();
            foreach (var i in pairs)
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
        public static implicit operator (T1 ObjA, T2 ObjB, T3 ObjC)(Triplet<T1, T2, T3> p) => (p.ObjectA, p.ObjectB, p.ObjectC);

        public static implicit operator Triplet<T1, T2, T3>((T1 ObjA, T2 ObjB, T3 ObjC) p) => new(p.ObjA, p.ObjB, p.ObjC);

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
    [Serializable]
    public class Quartet<T1, T2, T3, T4>
    {
        public T1 ObjectA { get; set; }
        public T2 ObjectB { get; set; }
        public T3 ObjectC { get; set; }
        public T4 ObjectD { get; set; }
        public Quartet() { }
        public Quartet(T1 valueA, T2 valueB, T3 valueC, T4 valueD)
        {
            ObjectA = valueA;
            ObjectB = valueB;
            ObjectC = valueC;
            ObjectD = valueD;
        }

        public void Deconstruct(out T1 objectA, out T2 objectB, out T3 objectC, out T4 objectD) { objectA = ObjectA; objectB = ObjectB; objectC = ObjectC; objectD = ObjectD; }
        public static implicit operator (T1 ObjA, T2 ObjB, T3 ObjC, T4 ObjD)(Quartet<T1, T2, T3, T4> p) => (p.ObjectA, p.ObjectB, p.ObjectC, p.ObjectD);

        public static implicit operator Quartet<T1, T2, T3, T4>((T1 ObjA, T2 ObjB, T3 ObjC, T4 ObjD) p) => new(p.ObjA, p.ObjB, p.ObjC, p.ObjD);

        public static (IEnumerable<T1> EnumerableA, IEnumerable<T2> EnumerableB, IEnumerable<T3> EnumerableC, IEnumerable<T4> EnumerableD) SeparateEnumerable(IEnumerable<Quartet<T1, T2, T3, T4>> quartets)
        {
            var lista = new List<T1>();
            var listb = new List<T2>();
            var listc = new List<T3>();
            var listd = new List<T4>();
            foreach (var i in quartets)
            {
                lista.Add(i.ObjectA);
                listb.Add(i.ObjectB);
                listc.Add(i.ObjectC);
                listd.Add(i.ObjectD);
            }
            return (lista, listb, listc, listd);
        }
    }
}
