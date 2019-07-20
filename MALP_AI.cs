function createMALP(%trans)
{
	(%malp = new WheeledVehicle()
	{
		datablock = MALPVehicle;
	}).setTransform(%trans);
	
	(%ai = new AIPlayer()
	{
		malpLinkObj = %malp;
		datablock = PlayerStandardArmor;
		scale = "0.2 0.2 0.2";
	}).hideNode("ALL");
	
	%malp.malpLinkObj = %ai;
	%malp.mountObject(%ai, 0);
	%ai.setMoveY(0.1);
}

//tick-based function to ensure both still exist
//%bot.setMoveY and %bot.setMoveYaw