registerInputEvent("fxDtsBrick", "WormholeIncoming", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "WormholeOutgoing", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "WormholeShutdown", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "onValidIDC", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "onValidIDC_D", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "onValidIDC_O", "Self fxDtsBrick");
registerInputEvent("fxDtsBrick", "onConfirmIDC", "Self fxDtsBrick");
registerOutputEvent("fxDtsBrick", "MessageAllInRange", "string 200 255", 0);
registerOutputEvent("fxDtsBrick", "CheckIDC", "string 200 255", 1);

function fxDtsBrick::WormholeIncoming(%brick, %cl)
{
	$InputTarget_Self = %brick;
	%brick.processInputEvent("IncomingWormhole", %cl);
	%brick.processInputEvent("WormholeIncoming", %cl);
}

function fxDtsBrick::WormholeOutgoing(%brick, %cl)
{
	$InputTarget_Self = %brick;
	%brick.processInputEvent("WormholeOutgoing", %cl);
}

function fxDtsBrick::WormholeShutdown(%brick, %cl)
{
	$InputTarget_Self = %brick;
	%brick.processInputEvent("WormholeShutdown", %cl);
}

function fxDtsBrick::ReceivedValidIDC(%brick, %idc, %cl)
{
	%brick.lastValidIDC = %idc;
	$InputTarget_Self = %brick;
	%brick.processInputEvent("onValidIDC", %cl);
}

function fxDtsBrick::ReceivedValidIDC_D(%brick, %idc, %cl)
{
	%brick.lastValidIDC = %idc;
	$InputTarget_Self = %brick;
	%brick.processInputEvent("onValidIDC_D", %cl);
}

function fxDtsBrick::ReceivedValidIDC_O(%brick, %idc, %cl)
{
	%brick.lastValidIDC = %idc;
	$InputTarget_Self = %brick;
	%brick.processInputEvent("onValidIDC_O", %cl);
}

function fxDtsBrick::CheckIDC(%brick, %list, %cl)
{
	%idc = %brick.lastValidIDC;
	%words = getWordCount(%list);
	$InputTarget_Self = %brick;
	for(%i=0;%i<%words;%i++)
		if(getWord(%list, %i) $= %idc)
		{ %brick.processInputEvent("onConfirmIDC", %cl); return; }
}

function fxDtsBrick::MessageAllInRange(%brick, %msg)
{
	if(%brick.lastValidIDC !$= "")
		%msg = strReplace(%msg, "[IDC]", %brick.lastValidIDC);
	MessageAllInRange(%brick.getPosition(), %msg);
}

function MessageAllInRange(%pos, %msg)
{
	%gates = StargateNetwork.getCount();
	for(%i=0;%i<%gates;%i++)
	{
		%gate = StargateNetwork.getObject(%i);
		if(isObject(%pair = %gate.linkedGate)
			|| isObject(%pair = %gate.masterGate))
		{
			%gateDist[%i] = getMin(vectorDist(%pos, %gate.getPosition()),
				vectorDist(%pos, %pair.getPosition()));
		}
	}
	
	%clients = ClientGroup.getCount();
	for(%i=0;%i<%clients;%i++)
	{
		%this = ClientGroup.getObject(%i);
		if(!%this.hasSpawnedOnce) %loc = "0 0 0";
		else %loc = %this.getControlObject().getEyePoint();
		if(vectorDist(%pos, %loc) < 256)
			%this.chatMessage(%msg);
		else
		{
			for(%j=0;%j<%gates;%j++)
			{
				if(%gateDist[%j] $= "") continue; %sumDist = vectorDist(%loc, StargateNetwork.getObject(%j).getPosition()) + %gateDist[%j];
				if(%sumDist < 256) { %this.chatMessage(%msg); break; }
			}
		}
	}
}