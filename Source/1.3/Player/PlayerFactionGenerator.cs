using RimWorld;
using System.Linq;
using System.Reflection;

namespace Empire_Rewritten.Player
{
    internal static class PlayerFactionGenerator
    {
        /// <summary>
        /// Generate a player faction
        /// </summary>
        /// <returns></returns>
        public static Faction GeneratePlayerFaction()
        {
            Faction result = new Faction();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (FieldInfo field in Faction.OfPlayer.GetType().GetFields(bindingFlags))
            {
                foreach (FieldInfo f2 in result.GetType().GetFields(bindingFlags).Where(f => f.Name == field.Name))
                {
                    f2.SetValue(result, field.GetValue(Faction.OfPlayer));
                }
            }
            return result;
        }
    }
}
