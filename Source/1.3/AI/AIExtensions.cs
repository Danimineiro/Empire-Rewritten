using Empire_Rewritten.Controllers;
using RimWorld;

namespace Empire_Rewritten.AI
{
    public static class AIExtensions
    {
        public static BasePlayer GetPlayer(this Faction faction)
        {
            return UpdateController.CurrentWorldInstance.FactionController.PlayerOfFaction(faction);
        }
    }
}