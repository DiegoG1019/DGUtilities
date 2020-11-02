using MagicGame;

namespace DiegoG.MonoGame
{
    public class EntityInfo
    {
        private enum EntitySpecies
        {
            Human
        }
        public string Name { get; private set; }
        public EntityClasses Class { get; private set; }
        public EntityRaces Race { get; private set; }
        public EntityOrders Order { get; private set; }
        private EntitySpecies _species { get; set; }
        public string Species => _species.ToString();
        private EntityInfo(EntityRaces race, EntityOrders order, EntitySpecies species)
        {
            Race = race;
            Order = order;
            _species = species;
        }
        public void Fill(string name, EntityClasses _class)
        {
            Name = name;
            Class = _class;
        }
        public override string ToString() => $"({Name}, {Race}, {Species}, {Order}, {Class})";
        //Species list

        public static class SpeciesList
        {
            /// <summary>
            /// Anthropomorphic, Living, Human
            /// </summary>
            public static EntityInfo Human => new EntityInfo(EntityRaces.Anthropomorphic, EntityOrders.Living, EntitySpecies.Human);
        }

    }
}
