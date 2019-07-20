datablock StaticShapeData(StargateDHDAbydosShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Dial/Dial_Abydos.dts"; };

datablock StaticShapeData(StargateDHDGenericShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Dial/Dial_Generic.dts"; };

package DHD
{
	function Player::activateStuff(%pl)
	{
		%eye = %pl.getEyePoint();
		%dir = %pl.getEyeVector();
		%mask = $Typemasks::fxBrickAlwaysObjectType
			| $Typemasks::PlayerObjectType
			| $Typemasks::StaticShapeObjectType
			| $Typemasks::TerrainObjectType
			| $Typemasks::VehicleObjectType;
		%ray = containerRaycast(%eye, vectorAdd(%eye, vectorScale(%dir, 8)), %mask, %pl);
		if(isObject(%obj = firstWord(%ray)) && %obj.isDHD)
		{
			%pl.playThread(2, "activate");
			%hit = vectorSub(getWords(%ray, 1, 3), %obj.getPosition());
			%for = %obj.getForwardVector();
			%up = %obj.getUpVector();
			%right = vectorCross(%for, %up);
			%ringUp = vectorAdd(vectorScale(%up, 0.900969), vectorScale(%for, 0.433884));
			%ringFor = vectorAdd(vectorScale(%for, 0.900969), vectorScale(%up, -0.433884));
			%coordX = vectorDot(%hit, %right);
			%coordY = vectorDot(%hit, %ringFor) + 0.16;
			%ang = mAtan(%coordX, %coordY);
			%button = mFloatLength(%ang/(2*$PI)*19,0) + 0;
			%dist = vectorLen(%coordX SPC %coordY);
			if(%dist < 0.95 && %dist > 0.55)
				%ring = 2;
			else if(%dist < 0.50 && %dist > 0.3)
				%ring = 1;
			else if(%dist < 0.25)
				%ring = 0;
			else %ring = -1;
			if(%button < 0) %button += 19;
			if(%ring > 0 && %obj.gate.gateState == 0)
			{
				if((%words = getWordCount(%obj.dialSequence)) >= 9) return;
				if(%ring == 2)
				{
					%obj.ObjectLightAnim("SYM_A_"@%button, 1, 0, 0.2, 0.8, 0.1, 0.6, 0, 0);
					//%obj.setNodeColor("SYM_A_"@%button, "1 0.7 0 1");
				}
				if(%ring == 1)
				{
					%obj.ObjectLightAnim("SYM_B_"@%button, 1, 0, 0.2, 0.8, 0.1, 0.6, 0, 0);
					//%obj.setNodeColor("SYM_B_"@%button, "1 0.7 0 1");
					%button += 19;
				}
				%map = %obj.DHDButtonMap;
				%symbol = getWord(%map, %button);
				%sequence = "1 2 3 6 7 8 4 5 0";
				for(%i=0;%i<%words;%i++)
					if(getWord(%obj.dialSequence, %i) == %symbol) return;
				if(%symbol == %obj.gate.pointOfOrigin
					&& getWordCount(%obj.dialSequence) >= 6)
						%obj.gate.ChevronLightAnim(0, 1, 0);
				else
				{
					for(%i=0;%i<9;%i++)
						if(!%obj.gate.ChevL[getWord(%sequence, %i)].isNodeLitLight)
							break;
					%obj.gate.ChevronLightAnim(getWord(%sequence, %i), 1, 0);
				}
				%obj.dialSequence = trim(%obj.dialSequence SPC %symbol);
				serverPlay3D("DHDButton"@(getWordCount(%obj.dialSequence)%7)+1@"_Sound", %obj.getPosition());
			}
			else if(%ring == 0)
			{
				if(%obj.dialSequence $= "" && %obj.gate.isOpened && !isObject(%obj.gate.masterGate))
				{
					cancel(%obj.gate.DialSched);
					for(%i=0;%i<3;%i++)
					{
						%obj.gate.stopAudio(%i);
						cancel(%obj.gate.GateRollSched[%i]);
					}
					%obj.gate.CloseStargate();
					
					//temporary
					//%obj.setNodeColor("ALL", "0.2 0.1 0 1");
					//%obj.setNodeColor("Button", "0.5 0 0 1");
				}
				else if(%obj.gate.gateState == 0 && getWordCount(%obj.dialSequence) > 0)
				{
					%addr = %obj.dialSequence;
					%obj.dialSequence = "";
					if(getWordCount(%addr) < 7)
					{
						for(%i=0;%i<19;%i++)
						{
							%obj.ObjectLightAnim("SYM_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
							%obj.ObjectLightAnim("SYM_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						}
						//%obj.setNodeColor("ALL", "0.2 0.1 0 1");
						//%obj.setNodeColor("Button", "0.5 0 0 1");
						for(%i=0;%i<9;%i++) %obj.gate.ChevronLightAnim(%i, 0, 1);
						for(%i=0;%i<3;%i++)
						{
							%obj.stopAudio(%i);
							%obj.schedule(0, "playAudio", %i, DialFail_Sound);
						}
						return;
					}
					if(getWord(%addr, getWordCount(%addr) - 1) != %obj.gate.pointOfOrigin)
					{
						for(%i=0;%i<19;%i++)
						{
							%obj.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
							%obj.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						}
						//%obj.setNodeColor("ALL", "0.2 0.1 0 1");
						//%obj.setNodeColor("Button", "0.5 0 0 1");
						for(%i=0;%i<9;%i++) %obj.gate.ChevronLightAnim(%i, 0, 1);
						for(%i=0;%i<3;%i++)
						{
							%obj.stopAudio(%i);
							%obj.schedule(0, "playAudio", %i, DialFail_Sound);
						}
						return;
					}
					%mainAddr = getWords(%addr, 0, getWordCount(%addr) - 2);
					if(%mainAddr $= %obj.gate.address)
					{
						for(%i=0;%i<19;%i++)
						{
							%obj.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
							%obj.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						}
						for(%i=0;%i<9;%i++) %obj.gate.ChevronLightAnim(%i, 0, 1);
						for(%i=0;%i<3;%i++)
						{
							%obj.stopAudio(%i);
							%obj.schedule(0, "playAudio", %i, DialFail_Sound);
						}
						return;
					}
					%gates = StargateNetwork.getCount();
					for(%i=0;%i<%gates;%i++)
					{
						%gate = StargateNetwork.getObject(%i);
						if(%gate.address $= %mainAddr && %gate.gateState <= 1)
						{
							%obj.ObjectLightAnim("BUTTON", 1, 0, 0.5, 0.5, 0, 0, 0, 0);
							%obj.gate.setStargateState(2);
							%gate.setStargateState(2);
							%gate.dhd.dialSequence = "";
							for(%i=0;%i<9;%i++)
								if(%obj.gate.ChevL[%i].isNodeLitLight)
								{
									%gate.ChevronLockAnim(%i, 1);
									serverPlay3D(ChevronLocked_Sound, %gate.getPosition());
								}
								else %gate.ChevronLockAnim(%i, 0);
							%obj.gate.schedule(400, "OpenStargate", %gate, %gate, %pl.client);
							%gate.schedule(400, "OpenStargate", %obj.gate);
							return;
						}
					}
					for(%i=0;%i<3;%i++)
					{
						%obj.stopAudio(%i);
						%obj.schedule(0, "playAudio", %i, DialFail_Sound);
					}
					for(%i=0;%i<19;%i++)
					{
						%obj.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						%obj.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
					}
					for(%i=0;%i<9;%i++) %obj.gate.ChevronLightAnim(%i, 0, 1);
				}
			}
		}
		else Parent::activateStuff(%pl);
	}
};
activatePackage("DHD");

function MakeDHD(%obj, %trans, %type)
{
	%data = StargateDHDGenericShape;
	%map = "4 20 9 5 32 13 19 22 7 38 6 29 24 34 12 37 11 21 3"
		@" 25 36 8 27 31 15 35 30 16 28 10 33 39 26 23 18 14 17 2";
	switch$(%obj.worldName)
	{
		case "Abydos":
			%data = StargateDHDAbydosShape;
			%map = setWord(%map, 5, 1);
	}
	(%dhd = new StaticShape()
	{
		datablock = %data.getID();
		DHDButtonMap = %map;
		isDHD = 1;
	}).setTransform(%trans);
	%dhd.setNodeColor("ALL", "0.2 0.1 0 1");
	%dhd.setNodeColor("Button", "0.5 0 0 1");
	%obj.dhd = %dhd; %dhd.gate = %obj;
	return %dhd;
}