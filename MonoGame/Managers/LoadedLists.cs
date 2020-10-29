using MagicGame.Objects.World.Audio;
using MagicGame.Objects.World.Entities;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using DiegoG.MonoGame.Exceptions;
using DiegoG.Utilities.Collections;

namespace DiegoG.MonoGame
{
	public static class Loaded
	{
		public interface ILoadedList<T> : IEnumerablePair<ID, T> where T : IDynamic
		{
			T this[ID index] { get; }
        }
		public interface ILoadedList : ILoadedList<IDynamic> { }
		
		public static IEnumerable<IDynamic> AllItems
        {
            get
            {
                AllLists.Clean();
                foreach (var list in AllLists)
                {
					if(list.TryGetTarget(out IEnumerable o))
						foreach(var i in o)
							yield return ((KeyValuePair<ID, IDynamic>)i).Value;
                }
			}
        }

		private static WeakList<IEnumerable> AllLists { get; set; } = new WeakList<IEnumerable>();

		public class LoadedList<T> : ILoadedList<T> where T : IDynamic
		{
			public string TypeName { get; private set; }
			private Queue<ID> FreeIDs { get; } = new Queue<ID>();
			private Dictionary<ID, T> Items { get; } = new Dictionary<ID, T>();
			private readonly string typeofT = typeof(T).Name;
			public T this[ID index] => Items[index];

			private ID NextID()
			{
				const string grantedid = "Granted the ID {0} of {1} Loaded List";
				Log.Verbose("Next {0} Loaded List available ID requested", typeofT);
				ID newid;
				if (FreeIDs.Count > 0)
				{
					newid = FreeIDs.Dequeue();
					newid = new ID(newid.Value, typeofT);
					Log.Verbose(grantedid, newid, typeofT);
					return newid;
				}
				Log.Verbose("No free IDs available in {0} Loaded List, creating a new one.", typeofT);
				newid = new ID(Items.Count, typeofT);
				Log.Verbose(grantedid, newid, typeofT);
				return newid;
			}

			public LoadedList()
            {
				AllLists.Add(this);
            }

			public ID Add(T v)
			{
				ID a = NextID();
				v.ID = a;
				Items.Add(a, v);
				return a;
			}

			public void Remove(ID id)
			{
				Log.Debug("Removing object of ID {0} from {1} Loaded List, and adding its now released ID to the freeIDs Queue", id, typeofT);
				Items.Remove(id);
				id.Active = false;
				id.HolderType = null;
				id.Value = 0;
				FreeIDs.Enqueue(id);
			}
			public int Count => Items.Count;
			public IEnumerator<KeyValuePair<ID, T>> GetEnumerator()
            {
				foreach (var i in Items)
					yield return new KeyValuePair<ID, T>(i.Key, i.Value);
            }
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
