using Empire_Rewritten.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
    public static class AIExtensions
    {
        public static bool IsAIPlayer(this Faction faction)=>GetAIPlayer(faction)!=null;

        public static AIPlayer GetAIPlayer(this Faction faction) => UpdateController.GetUpdateController.FactionController.GetAIPlayer(faction);
    }
}
