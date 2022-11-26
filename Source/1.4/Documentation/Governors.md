# Governors

Governors are instances of GovernorHandler assigned to each settlement, which provide additional
functionality and access to the interlocking parts of the Governor system.

A governor is, in theory, a local ruler who manages a Settlement in your name. Just like your own Pawns,
they have Traits and Histories. Unlike your Pawns, these Traits and Histories do not only provide individual
bonuses and penalties, but affect the settlement as well. An incompetent Governor can quickly cause
bureaucratic nightmares.

## The GovernorManager Class

A GovernorManager consists of the following important fields:

###Public
- Pawn pawn: The pawn currently acting as the governor. The pawn provides the appearance, traits and skills used by the game.
- GovernorHistoryDef def: The Def which determines how the governor came into power.
	- bool appointed: True when the pawn has been manually appointed by the player, false if the pawn has been randomly
		generated as part of an election. Appointed pawns draw from a different pool of histories.
	- List<SettlementStatOffsets> (NOT IMPLEMENTED): Determines what settlement stats the history affects.

### Private
- int daysSinceLastElection: Tracks how long it's been since the last governor change. Frequent changes make for an unhappy populace.
- Settlement settlement: Provides quick access to the Settlement the governor manages.
- bool isColonist: Tracks whether the current Governor was originally a colony pawn. If yes, changing governors places the old one in a caravan.

In addition, take note of the following methods:

### Public
- void ReplaceGovernor() : This is your entry method for facilitating a governor change. The name implies that there is always a governor: A settlement without a Governor can only be caused by an error.
	Parameters:
	- Pawn pawn: If non-null, replaces the current Governor with the supplied pawn. If null, creates a random pawn.
	- bool appointed: Determines whether the new Governor has been appointed by the player, or elected by the populace.
	- GovernorHistoryDef def: The Def which holds the info about the governor's history, aka their rise to power. Basically a backstory which grants settlement stats instead of skillpoints.


## Other Interesting Tidbits
When you want to generate interesting Governor quotes, refer to the BadOpinionGenerator class.