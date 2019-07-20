function createBotMALP(%trans, %gate)
{
	%bot = new AIPlayer()
	{
		hasMalpCheck = 1;
		reportToGate = %gate;
		datablock = PlayerStandardArmor;
		position = getWords(%trans, 0, 2);
	};
	%bot.setTransform(%trans);
	%bot.setNodeColor("headskin", "1 0.8 0.6 1");
	%bot.setNodeColor("lhand", "1 0.8 0.6 1");
	%bot.setNodeColor("rhand", "1 0.8 0.6 1");
	%bot.setNodeColor("larm", "0.5 0.5 0.5 1");
	%bot.setNodeColor("rarm", "0.5 0.5 0.5 1");
	%bot.setNodeColor("chest", "0.5 0.5 0.5 1");
	%bot.setNodeColor("pants", "0.5 0.5 0.5 1");
	%bot.setNodeColor("lshoe", "0.2 0.2 0.2 1");
	%bot.setNodeColor("rshoe", "0.2 0.2 0.2 1");
	%bot.setFaceName("Male07Smiley");
	%bot.setDecalName("DrKleiner");
	%bot.BotMalpAI(getSimTime());
	%gate.MalpSafetyCheck(%bot);
	
	//temporary
	%bot.setMoveY(1);
}

function StaticShape::MalpSafetyCheck(%gate, %malp)
{
	if(!isObject(%malp))
	{
		if(isObject(%linked = %gate.linkedGate))
		{
			cancel(%gate.shutSched);
			%gate.weight[%linked] *= 0.5;
			%gate.shutSched = %gate.schedule(3000, "closeStargate");
			MessageAllInRange(%gate.getPosition(), "\c2"@%gate.worldName@" Autodialler\c6: MALP telemetry lost. Shutting down the Stargate.");
		}
		return;
	}
	%gate.MalpCheck = %gate.schedule(50, "MalpSafetyCheck", %malp);
}

function AIPlayer::BotMalpAI(%bot, %last)
{
	if(%bot.getState() $= "DEAD") return;
	if(!isObject(%gate = %bot.reportToGate))
	{ %bot.kill(); return; }
	%time = vectorDist(%last, %now = getSimTime());
	%bot.MalpAISched = %bot.schedule(50, "BotMalpAI", %now);
	if(%bot.hasMalpCheck && vectorDist(%bot.getHackPosition(), %gate.getPosition()) > 256)
	{
		if(isObject(%linked = %gate.linkedGate))
			%gate.weight[%linked] += 0.2;
		cancel(%bot.reportToGate.MalpCheck);
		%bot.hasMalpCheck = 0;
		%bot.schedule(3000, "kill");
	}
}