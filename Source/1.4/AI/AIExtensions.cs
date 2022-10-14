using Empire_Rewritten.Controllers;
using RimWorld;

namespace Empire_Rewritten.AI
{
    public static class AIExtensions
    {
        public static bool IsAIPlayer(this Faction faction)
        {
            return GetAIPlayer(faction) != null;
        }

        public static AIPlayer GetAIPlayer(this Faction faction)
        {
            return UpdateController.CurrentWorldInstance.FactionController.GetAIPlayer(faction);
        }
    }
}