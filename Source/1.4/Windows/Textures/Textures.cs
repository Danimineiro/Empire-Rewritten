using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace Empire_Rewritten.Windows.Textures
{
    [StaticConstructorOnStartup]
    public static class Tex
    {
        public static readonly Texture2D TradeArrow = ContentFinder<Texture2D>.Get("UI/Widgets/TradeArrow");
        public static readonly Texture2D InConstruction = ContentFinder<Texture2D>.Get("UI/Filler/ConstructingBig");
        public static readonly Texture2D LockedBuildingSlot = ContentFinder<Texture2D>.Get("UI/Facilities/LockedBuildingSlot");
        public static readonly Texture2D AddFacility = ContentFinder<Texture2D>.Get("UI/Facilities/AddFacility");
        public static readonly Texture2D QuestionMark = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
    }
}
