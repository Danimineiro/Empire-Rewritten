using System;
using Empire_Rewritten.Controllers;
using Empire_Rewritten.Settlements;
using RimWorld;

namespace Empire_Rewritten.Player
{
    public class UserPlayer : BasePlayer
    {
        public UserPlayer(Faction faction) : base(faction) { }


        public override void MakeMove(FactionController factionController)
        {
            throw new NotImplementedException();
        }

        public override void MakeThreadedMove(FactionController factionController)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldExecute()
        {
            return false;
        }

        public override bool ShouldExecuteThreaded()
        {
            return false;
        }
    }
}