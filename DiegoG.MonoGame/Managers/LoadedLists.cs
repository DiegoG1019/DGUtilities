using DiegoG.Utilities;
using DiegoG.Utilities.Collections;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace DiegoG.MonoGame
{
    public static class LoadedLists
    {
        public interface ILoadedList : INotifyCollectionChanged, IEnumerable, IEquatable<ILoadedList>
        {
            Game Game { get; }
            string ListName { get; }
            int Count { get; }
            void Remove(ID id);
            object GetItem(ID id);
        }

        public static IEnumerable<IDynamic> AllItems
        {
            get
            {
                AllLists.Clean();
                foreach (var list in AllLists)
                {
                    if (list.TryGetTarget(out IEnumerable o))
                    {
                        foreach (var i in o)
                        {
                            yield return ((KeyValuePair<ID, IDynamic>)i).Value;
                        }
                    }
                }
            }
        }

        private static WeakList<IEnumerable> AllLists { get; set; } = new WeakList<IEnumerable>();

        public class LoadedList<T> : ILoadedList where T : IDynamic
        {
            public Game Game { get; private set; }
            public string ListName { get; private set; }

            private Queue<ID> FreeIDs { get; } = new Queue<ID>();
            /// <summary>
            /// T item, Timer LastUseTimer, bool IsOld, Stopwatch TotalAge
            /// </summary>
            private Dictionary<ID, Quartet<T, Timer, bool, Stopwatch>> Items { get; } = new();
            private readonly string typeofT = typeof(T).Name;

            /// <summary>
            /// Every item will be marked as old past 30 seconds since last use
            /// </summary>
            private static Timer DefaultTimer => new(30000);

            /// <summary>
            /// It's worth mentioning that this number will be different across diferent type arguments
            /// </summary>
            private static ulong AllInstancedTypeTListsCount;
            private ulong AllListsIndex { get; init; }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public T this[ID index]
            {
                get
                {
                    Items[index].ObjectB.Reset();
                    return Items[index].ObjectA;
                }
            }
            object ILoadedList.GetItem(ID index) => this[index];

            public IEnumerable<T> OldItems => from it in Items.Values
                                              where it.ObjectC
                                              select it.ObjectA;

            public bool Equals(ILoadedList obj)
            {
                if (obj is LoadedList<T> ll)
                {
                    return AllListsIndex == ll.AllListsIndex;
                }

                return false;
            }
            private ID AssignID(T holder)
            {
                Log.Verbose($"Next {typeofT} Loaded List available ID requested");
                ID newid;
                if (FreeIDs.Count > 0)
                {
                    newid = FreeIDs.Dequeue();
                    newid.Activate(holder);
                    holder.ID = newid;
                    Log.Verbose($"Granted the ID: \"{newid}\" to holder of Type: \"{typeofT}\"");
                    return newid;
                }
                Log.Verbose($"No free IDs available in {typeofT} Loaded List, creating a new one.");
                newid = new ID(Items.Count, this);
                newid.Activate(holder);
                holder.ID = newid;
                Log.Verbose($"Granted the ID: \"{newid}\" to holder of Type: \"{typeofT}\"");
                return newid;
            }

            private void RevokeID(ID id)
            {
                id.Deactivate();
                FreeIDs.Enqueue(id);
            }

            // This adds to a WeakList, meaning that the list will still be discarded if all other references are lost
            public LoadedList(string name, Game game)
            {
                Game = game;
                ListName = name;

                AllListsIndex = AllInstancedTypeTListsCount;
                AllInstancedTypeTListsCount++;

                AllLists.Add(this);
            }

            public ID Add(T item)
            {
                var id = AssignID(item);
                Log.Debug("Adding object of ID {0} from <{1}> \"{2}\" Loaded List", id, typeofT, ListName);
                var dtime = DefaultTimer;
                var stopw = new Stopwatch();
                Quartet<T, Timer, bool, Stopwatch> newitem = new(item, dtime, false, stopw);
                dtime.Elapsed += (s, a) => newitem.ObjectC = true;

                Items.Add(id, newitem);
                Game.Components.Add(item);
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, id));
                stopw.Start();
                return id;
            }
            public void Remove(ID id)
            {
                var itm = Items[id];
                Log.Debug("Removing object of ID {0} from <{1}> \"{2}\" Loaded List, and adding its now released ID to the freeIDs Queue. Total Age of the object: {3}", id, typeofT, ListName, itm.ObjectD.Elapsed);
                itm.ObjectB.Dispose();
                itm.ObjectD.Stop();
                Game.Components.Remove(itm.ObjectA);
                RevokeID(id);
                Items.Remove(id);
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, id));
            }
            public int Count => Items.Count;
            public IEnumerator<KeyValuePair<ID, T>> GetEnumerator()
            {
                foreach (var i in Items)
                {
                    yield return new KeyValuePair<ID, T>(i.Key, i.Value.ObjectA);
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}