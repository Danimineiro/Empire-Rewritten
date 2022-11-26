using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_Rewritten
{
 /// <summary>
 /// Tells the code why a <see cref="Process"/> was cancelled.
 /// </summary>
    public enum CancelReason
    {
        // Process got cancelled by direct player input.
        CancelledByPlayer,
        //Process got cancelled by an Empire AI.
        CancelledByAI,
        //Process got cancelled by an Event.
        CancelledByEvent,
        // Process doesn't meet its requirements.
        InsufficientRequirements,
        // Process became impossible because the outcome is not logically possible, i.e. targetting a Pawn that doesn't exist.
        ImpossibleOutcome,
        // Process had to be cancelled because a code error occured.
        ErrorOccured,
        // Process was stopped by player actions, and it's a good thing
        SuccessfulIntervention,
        // Process was stopped due to lack of player action, and it's a bad thing
        FailedIntervention
    }
}
