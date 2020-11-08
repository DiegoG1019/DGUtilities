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
	public static class LoadedLists
	{
		public interface ILoadedList { }
		
		public static IEnumerable<IDynamic> AllItems
        {
            get
            {
                AllLists.Clean();
                foreach (var list in AllLists)
					if(list.TryGetTarget(out IEnumerable o))
						foreach(var i in o)
							yield return ((KeyValuePair<ID, IDynamic>)i).Value;
			}
        }

		private static WeakList<IEnumerable> AllLists { get; set; } = new WeakList<IEnumerable>();

		public class LoadedList<T> : ILoadedList, IEnumerable where T : IDynamic
		{
			public string ListName { get; private set; }
			private Queue<ID> FreeIDs { get; } = new Queue<ID>();
			private Dictionary<ID, T> Items { get; } = new Dictionary<ID, T>();
			private readonly string typeofT = typeof(T).Name;
			public T this[ID index] => Items[index];

#warning Implement this
			//The proper implementation for this would simply be to add some sort of internal wrapper to objects held within the list that adds some kind of async timer, and when the timer reaches 0, they get added to this list along with a warning.
			//Mostly for debug purposes
			private List<T> OldItemsList => throw new NotImplementedException();//{ get; } = new List<T>();
			public IEnumerable<T> OldItems => OldItemsList;

			private ID AssignID(T holder)
			{
				Log.Verbose($"Next {typeofT} Loaded List available ID requested");
				ID newid;
				if (FreeIDs.Count > 0)
				{
					newid = FreeIDs.Dequeue();
					newid.Activate(holder, this);
					holder.ID = newid;
					Log.Verbose($"Granted the ID: \"{newid}\" to holder of Type: \"{typeofT}\"");
					return newid;
				}
				Log.Verbose($"No free IDs available in {typeofT} Loaded List, creating a new one.");
				newid = new ID(Items.Count);
				newid.Activate(holder, this);
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
			public LoadedList(string name)
            {
				ListName = name;
				AllLists.Add(this);
			}
			public ID Add(T item)
			{
				var id = AssignID(item);
				Items.Add(id, item);
				return id;
			}

			public void Remove(ID id)
			{
				Log.Debug("Removing object of ID {0} from {1} Loaded List, and adding its now released ID to the freeIDs Queue", id, typeofT);
				RevokeID(id);
				Items.Remove(id);
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