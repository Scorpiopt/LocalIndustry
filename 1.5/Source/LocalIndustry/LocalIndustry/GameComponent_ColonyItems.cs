using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LocalIndustry
{
    public class GameComponent_ColonyItems : GameComponent
    {
        private HashSet<Thing> colonyItemsHashset;
        private List<Thing> colonyItems;
        public static GameComponent_ColonyItems Instance;
        public GameComponent_ColonyItems(Game game)
        {
            Init();
        }

        public void Init()
        {
            colonyItems ??= new List<Thing>();
            colonyItemsHashset = colonyItems.ToHashSet();
            Instance = this;
        }

        public void Add(Thing thing)
        {
            if (!colonyItems.Contains(thing))
            {
                colonyItems.Add(thing);
                colonyItemsHashset.Add(thing);
            }
        }

        public bool ColonyCanUseIt(Thing t)
        {
            if (t.def.IsStuff) return true;

            if (t.def.IsApparel || t.def.IsWeapon)
            {
                return colonyItemsHashset.Contains(t);
            }
            return true;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref colonyItems, "colonyItems", LookMode.Reference);
            Init();
        }
    }
}
