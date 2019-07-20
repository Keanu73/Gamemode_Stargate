package StargateWandering
{
	function GameConnection::createPlayer(%cl, %trans)
	{
		if($StargateSpawn[%cl.bl_id] !$= "")
			%trans = $StargateSpawn[%cl.bl_id];
		Parent::createPlayer(%cl, %trans);
		if(isObject(%pl = %cl.player))
			%pl.StargateLoop(getSimTime());
	}
	
	function servercmdFind(%cl, %targ)
	{
		if(%cl.isAdmin && isObject(%targ))
		{
			$Stargate[%cl.bl_id] = %Stargate[%targ.bl_id];
			$StargatePos[%cl.bl_id] = $StargatePos[%targ.bl_id];
			$StargateSpawn[%cl.bl_id] = $StargateSpawn[%targ.bl_id];
		}
		else if(%cl.isAdmin && isObject(%vict = findClientByName(%targ)))
		{
			$Stargate[%cl.bl_id] = %Stargate[%targ.bl_id];
			$StargatePos[%cl.bl_id] = $StargatePos[%vict.bl_id];
			$StargateSpawn[%cl.bl_id] = $StargateSpawn[%vict.bl_id];
		}
		Parent::servercmdFind(%cl, %targ);
	}
	
	function servercmdFetch(%cl, %targ)
	{
		if(%cl.isAdmin && isObject(%targ))
		{
			$Stargate[%targ.bl_id] = %Stargate[%cl.bl_id];
			$StargatePos[%targ.bl_id] = $StargatePos[%cl.bl_id];
			$StargateSpawn[%targ.bl_id] = $StargateSpawn[%cl.bl_id];
		}
		else if(%cl.isAdmin && isObject(%vict = findClientByName(%targ)))
		{
			$Stargate[%vict.bl_id] = %Stargate[%cl.bl_id];
			$StargatePos[%vict.bl_id] = $StargatePos[%cl.bl_id];
			$StargateSpawn[%vict.bl_id] = $StargateSpawn[%cl.bl_id];
		}
		Parent::servercmdFetch(%cl, %targ);
	}
	
	function servercmdMessageSent(%cl, %msg)
	{
		if(isFunction("SetMode") && SetMode(%cl, %msg) == 2) return;
		
		if(!%cl.hasSpawnedOnce) %pos = "0 0 0";
		else %pos = %cl.getControlObject().getEyePoint();
		
		%idcHash = sha1(%cl.bl_id);
		%hex = "0123456789ABCDEF";
		for(%i=0;%i<10;%i++)
		{
			%digit = 0;
			for(%j=0;%j<4;%j++)
				%digit = (%digit << 4) | striPos(%hex, getSubStr(%idcHash, %i * 4 + %j, 1));
			%realIDC = %realIDC @ mFloor(%digit / 6553.6);
		}
		if(%msg $= %realIDC || (%msg !$= "" && getSubStr(%msg, 0, strLen(%msg) - 1) $= %realIDC))
		{
			if(%msg !$= %realIDC)
				%suffix = getSubStr(%msg, strLen(%msg) - 1, 1);
			%gates = StargateNetwork.getCount();
			for(%i=0;%i<%gates;%i++)
			{
				%gate = StargateNetwork.getObject(%i);
				if(isObject(%pair = %gate.linkedGate)
					|| isObject(%pair = %gate.masterGate))
				{
					%dist = getMin(vectorDist(%pos, %gate.getPosition()),
						vectorDist(%pos, %pair.getPosition()));
					if(%dist <= 32)
					{
						initContainerRadiusSearch(%gate.getPosition(), 16, $Typemasks::fxBrickAlwaysObjectType);
						while(isObject(%brick = containerSearchNext()))
						{
							if(%suffix $= "")
								%brick.ReceivedValidIDC(%realIDC, %cl);
							else if(%suffix $= "D" && isObject(%gate.masterGate))
								%brick.ReceivedValidIDC_D(%realIDC, %cl);
							else if(%suffix $= "O" && isObject(%gate.linkedGate))
								%brick.ReceivedValidIDC_O(%realIDC, %cl);
						}
					}
				}
			}
			%cl.chatMessage("\c6Identification code transmitted!");
			return;
		}
		
		while((%temp = stripMLControlChars(%msg)) !$= %msg)
			%msg = %temp;
		%msg = ParseLinks(%msg);
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
			if(vectorDist(%pos, %loc) < 256)// || %cl.isAdmin || %this.isAdmin)
				commandToClient(%this, 'chatMessage', %this, '', '', "\c7"@%cl.clanPrefix@"\c3"@%cl.name@"\c7"@%cl.clanSuffix@"\c6: "@%msg, %cl.clanPrefix, %cl.name, %cl.clanSuffix, %msg);
			else
			{
				for(%j=0;%j<%gates;%j++)
				{
					if(%gateDist[%j] $= "") continue; %sumDist = vectorDist(%loc, StargateNetwork.getObject(%j).getPosition()) + %gateDist[%j];
					if(%sumDist < 256) { commandToClient(%this, 'chatMessage', %this, '', '', "\c7"@%cl.clanPrefix@"\c3"@%cl.name@"\c7"@%cl.clanSuffix@"\c6: "@%msg, %cl.clanPrefix, %cl.name, %cl.clanSuffix, %msg); break; }
				}
			}
		}
		echo(%cl.name@": "@%msg);
	}
	
	function servercmdPlantBrick(%cl)
	{
		if(isObject(%pl = %cl.player) && isObject(%temp = %pl.tempbrick))
		{
			%farthest = %temp.getFarthestPoint($StargatePos[%cl.bl_id]);
			%len = vectorLen(getWords(%farthest, 0, 1));
			if(%len > 256)
			{
				messageClient(%cl,'MsgPlantError_TooFar');
				return;
			}
		}
		Parent::servercmdPlantBrick(%cl);
	}
};
activatePackage("StargateWandering");

function ParseLinks(%msg)
{
	%words = getWordCount(%msg);
	for(%i=0;%i<%words;%i++)
	{
		%word = getWord(%msg, %i);
		if(getSubStr(%word, 0, 7) $= "http://")
			%msg = setWord(%msg, %i, "<a:"@(%word=strReplace(%word, "http://", ""))@">"@%word@"</a>");
		else if(getSubStr(%word, 0, 8) $= "https://")
			%msg = setWord(%msg, %i, "<a:"@(%word=strReplace(%word, "https://", ""))@">"@%word@"</a>");
		else if(getSubStr(%word, 0, 6) $= "ftp://")
			%msg = setWord(%msg, %i, "<a:"@(%word=strReplace(%word, "ftp://", ""))@">"@%word@"</a>");
	}
	return trim(%msg);
}

function Player::StargateLoop(%pl, %last)
{
	cancel(%pl.StargateLoop);
	%pl.StargateLoop = %pl.schedule(50, "StargateLoop");
	if(isObject(%cl = %pl.client))
	{
		%diff = vectorSub(%pl.getHackPosition(), $StargatePos[%cl.bl_id]);
		%len = vectorLen(getWords(%diff, 0, 1));
		if(%len >= 256)
		{
			%out = vectorNormalize(getWords(%diff, 0, 1));
			%pos = vectorAdd($StargatePos[%cl.bl_id], vectorScale(%out, 256));
			%pl.setTransform(getWords(%pos, 0, 1) SPC getWords(%pl.getTransform(), 2, 6));
			%oVel = vectorScale(%out, -vectorDot(%pl.getVelocity(), %out));
			%pl.addVelocity(%oVel);
		}
	}
}

function servercmdWhatsMyIDC(%cl)
{
	%idcHash = sha1(%cl.bl_id);
	%hex = "0123456789ABCDEF";
	for(%i=0;%i<10;%i++)
	{
		%digit = 0;
		for(%j=0;%j<4;%j++)
			%digit = (%digit << 4) | striPos(%hex, getSubStr(%idcHash, %i * 4 + %j, 1));
		%realIDC = %realIDC @ mFloor(%digit / 6553.6);
	}
	%cl.chatMessage("\c6Your IDC is \c4"@%realIDC@"\c6.");
}

function servercmdWhatsMyPlanet(%cl)
{
	%gateObj = $Stargate[%cl.bl_id];
	%worldName = %gateObj.worldName $= "" || %gateObj.worldName $= null ? "a Hidden System" : %gateObj.worldName;
	%address = %gateObj.address;
	%poo = %gateObj.pointOfOrigin;
	%cl.chatMessage("\c6You are on \c3"@%worldName@"\c6, with the address \c4"@%address@"\c6 and the point of origin glyph \c1"@%poo@"\c6.");
}

exec("./Bricks/_Init.cs");
exec("./Libraries/_Init.cs");
exec("./Sounds/_Init.cs");
exec("./BotMALP.cs");
exec("./DHD.cs");
exec("./Environ.cs");
exec("./Events.cs");
exec("./MALP_AI.cs");
exec("./Painting.cs");
exec("./Stargate.cs");
exec("./Vehicles.cs");

function getUnitVector()
{
	%elev = mAsin(getRandom() * 2 - 1);
	%azim = (getRandom() * 2 - 1) * $PI;
	%z = mSin(%elev); %l = mCos(%elev);
	%x = mSin(%azim) * %l;
	%y = mCos(%azim) * %l;
	return %x SPC %y SPC %z;
}

//Stargate outermost diameter: 8.88
//Stargate innermost diameter: 7.20

//Tollan gate outermost diameter: 8.50