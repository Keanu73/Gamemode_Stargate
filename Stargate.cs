AddDamageType("Stargate_IrisSlam", '%1 didn\'t rematerialize at the other end.', '%1 didn\'t rematerialize at the other end.', 1, 1);
AddDamageType("Stargate_WrongSide", '%1 entered the wrong side of a Stargate.', '%1 entered the wrong side of a Stargate.', 1, 1);
AddDamageType("Stargate_Backwards", '%1 went backwards through the wormhole.', '%1 went backwards through the wormhole.', 1, 1);
AddDamageType("Stargate_Kawoosh", '%1 stood in the way of the Kawoosh.', '%1 stood in the way of the Kawoosh.', 1, 1);
AddDamageType("Stargate_Telefrag", '%1 stood in a closing wormhole.', '%1 stood in a closing wormhole.', 1, 1);

datablock StaticShapeData(StargateOuterRingShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Ring/Outer.dts"; };

datablock StaticShapeData(StargateMiddleRingShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Ring/Middle.dts"; };

datablock StaticShapeData(StargateInnerRingShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Ring/Inner.dts"; };

datablock StaticShapeData(StargateChevronBottomShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Chev/Bottom.dts"; };

datablock StaticShapeData(StargateChevronFinShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Chev/Fin.dts"; };

datablock StaticShapeData(StargateChevronLightShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Milky_Way/Chev/Light.dts"; };

datablock StaticShapeData(StargateHorizonShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Horiz/Horizon.dts"; };

datablock StaticShapeData(StargateKawooshShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Horiz/Kawoosh.dts"; };

datablock StaticShapeData(StargateIrisShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Horiz/Iris.dts"; };

datablock StaticShapeData(StargateDestinyShape)
{ shapefile = "Add-Ons/Gamemode_Stargate/Destiny/Stargate/Stargate.dts"; };

function MakeStargate(%trans, %address, %origin, %name)
{
	if(!isObject(StargateNetwork))
		new SimSet(StargateNetwork);
	
	(%outer = new StaticShape()
	{
		address = %address;
		datablock = StargateOuterRingShape;
		pointOfOrigin = %origin;
		worldName = %name;
		gateClass = "Milky_Way";
	}).setTransform(%trans);
	%outer.setNodeColor("Outer", "0.3 0.3 0.3 1");
	
	(%middle = new StaticShape()
	{
		datablock = StargateMiddleRingShape;
	}).setTransform(%trans);
	%middle.setNodeColor("Middle", "0.3 0.3 0.3 1");
	
	(%inner = new StaticShape()
	{
		datablock = StargateInnerRingShape;
	}).setTransform(%trans);
	%inner.setNodeColor("Inner", "0.3 0.3 0.3 1");
	
	(%horiz = new StaticShape()
	{
		datablock = StargateHorizonShape;
	}).setTransform(%trans);
	%horiz.setNodeColor("ALL", "1 1 1 1");
	%horiz.hideNode("ALL");
	
	(%kawoosh = new StaticShape()
	{
		datablock = StargateKawooshShape;
	}).setTransform(%trans);
	%kawoosh.setNodeColor("ALL", "0 0.5 1 1");
	%kawoosh.hideNode("ALL");
	
	%outer.middle = %middle;
	%outer.inner = %inner;
	%outer.horiz = %horiz;
	%outer.kawoosh = %kawoosh;
	
	%pos = getWords(%trans, 0, 2);
	%for = %outer.getForwardVector();
	%up = %outer.getUpVector();
	%right = vectorCross(%for, %up);
	%hLen = vectorLen(getWords(%for, 0, 1));
	%vAng = mAtan(getWord(%for, 2), %hLen) * (180 / $PI);
	%hAng = mAtan(getWord(%for, 0), getWord(%for, 1)) * (180 / $PI);
	if(mAbs(vectorDot(%up, "0 0 1")) <= 0.999)
	{
		%hAng += 180; %vAng = 180 - %vAng;
		if(mAbs(vectorDot(%for, "0 0 1")) <= 0.999) %rot += 180;
	}
	for(%i=0;%i<9;%i++)
	{
		%offset = vectorAdd(vectorScale(%right, mSin(%i*$PI/4.5)*-4.2),
			vectorScale(%up, mCos(%i*$PI/4.5)*4.2));
		%trans = vectorAdd(%pos, %offset) SPC eulerToAxis(%vAng SPC -360*(%i/9)+%rot SPC -%hAng);
		
		(%shape = new StaticShape() {
			datablock = StargateChevronBottomShape;
		}).setTransform(%trans);
		%shape.setNodeColor("Bottom", "0.3 0.3 0.3 1");
		%shape.setNodeColor("LightStrips", "0.2 0.2 0.2 1");
		%outer.ChevB[%i] = %shape;
		
		(%shape = new StaticShape() {
			datablock = StargateChevronFinShape;
		}).setTransform(%trans);
		%shape.setNodeColor("LightBody", "0.3 0.3 0.3 1");
		%shape.setNodeColor("Fins", "0.3 0.3 0.3 1");
		%shape.setNodeColor("Detail", "0.3 0.3 0.3 1");
		%outer.ChevF[%i] = %shape;
		
		(%shape = new StaticShape() {
			datablock = StargateChevronLightShape;
		}).setTransform(%trans);
		%shape.setNodeColor("Light", "0.3 0 0 1");
		%outer.ChevL[%i] = %shape;
	}
	
	StargateNetwork.add(%outer);
	return %outer;
}

function MakeDestinyGate(%trans, %address, %origin, %name)
{
	if(!isObject(StargateNetwork))
		new SimSet(StargateNetwork);
	
	(%gate = new StaticShape()
	{
		address = %address;
		datablock = StargateDestinyShape;
		pointOfOrigin = %origin;
		worldName = %name;
		gateClass = "Destiny";
	}).setTransform(%trans);
	
	(%kawoosh = new StaticShape()
	{
		datablock = StargateKawooshShape;
	}).setTransform(%trans);
	%kawoosh.setNodeColor("ALL", "0 0.5 1 1");
	%kawoosh.hideNode("ALL");
	
	%gate.kawoosh = %kawoosh;
	
	StargateNetwork.add(%gate);
	return %gate;
}

function StaticShape::ChevronLockAnim(%obj, %num, %stayOn)
{
	%scale = getWord(%obj.getScale(), 2);
	if(isObject(%bottom = %obj.ChevB[%num]))
	{
		%rot = getWords(%bottom.getTransform(), 3, 6);
		if(%bottom.ChevronPosition !$= "") %pos = %bottom.ChevronPosition;
		else { %pos = %bottom.getPosition(); %bottom.ChevronPosition = %pos; }
		%dir = vectorNormalize(vectorSub(%obj.getPosition(), %pos));
		for(%i=0;%i<12;%i++)
		{
			cancel(%bottom.ChevLockASched[%i]); cancel(%bottom.ChevLockBSched[%i]);
			%bottom.ChevLockASched[%i] = %bottom.schedule(%i * 16, "setTransform", vectorAdd(%pos, vectorScale(%dir, 0.2 * %scale * (%i / 11))) SPC %rot);
			%bottom.ChevLockBSched[%i] = %bottom.schedule(%i * 16 + 1200, "setTransform", vectorAdd(%pos, vectorScale(%dir, 0.2 * %scale * (1 - %i / 11))) SPC %rot);
		}
	}
	if(isObject(%light = %obj.ChevL[%num]))
	{
		%rot = getWords(%light.getTransform(), 3, 6);
		if(%light.ChevronPosition !$= "") %pos = %light.ChevronPosition;
		else { %pos = %light.getPosition(); %light.ChevronPosition = %pos; }
		%dir = vectorNormalize(vectorSub(%obj.getPosition(), %pos));
		for(%i=0;%i<12;%i++)
		{
			cancel(%light.ChevLockASched[%i]); cancel(%light.ChevLockBSched[%i]);
			%light.ChevLockASched[%i] = %light.schedule(%i * 16, "setTransform", vectorAdd(%pos, vectorScale(%dir, -0.05 * %scale * (%i / 11))) SPC %rot);
			%light.ChevLockBSched[%i] = %light.schedule(%i * 16 + 1200, "setTransform", vectorAdd(%pos, vectorScale(%dir, -0.05 * %scale * (1 - %i / 11))) SPC %rot);
		}
	}
	%obj.schedule(384, "ChevronLightAnim", %num, 1, !%stayOn);
}

function StaticShape::ChevronLightAnim(%obj, %num, %on, %off)
{
	if(isObject(%bottom = %obj.ChevB[%num]))
		%bottom.ObjectLightAnim("LightStrips", %on, %off, 0.2, 0.8, 0.2, 0.5, 0.2, -0.2);
	if(isObject(%light = %obj.ChevL[%num]))
		%light.ObjectLightAnim("Light", %on, %off, 0.3, 0.7, 0, 0.7, 0, 0);
}

function StaticShape::ObjectLightAnim(%obj, %node, %on, %off, %r0, %r1, %g0, %g1, %b0, %b1)
{
	if(%on && %off)
	{
		for(%i=0;%i<63;%i++)
		{
			%value = mSin(%i / 62 * $PI);
			if(%i > 31 || !%obj.isNodeLit[%node])
				%obj.schedule(%i * 16, "setNodeColor", %node, %r0+%r1*%value SPC %g0+%g1*%value SPC %b0-%b1*%value SPC 1);
		}
		%obj.isNodeLit[%node] = 0;
	}
	else if(%on && !%obj.isNodeLit[%node])
	{
		for(%i=0;%i<32;%i++)
		{
			%value = mSin(%i / 62 * $PI);
			%obj.schedule(%i * 16, "setNodeColor", %node, %r0+%r1*%value SPC %g0+%g1*%value SPC %b0-%b1*%value SPC 1);
		}
		%obj.isNodeLit[%node] = 1;
	}
	else if(%off && %obj.isNodeLit[%node])
	{
		for(%i=0;%i<32;%i++)
		{
			%value = mSin((%i + 31) / 62 * $PI);
			%obj.schedule(%i * 16, "setNodeColor", %node, %r0+%r1*%value SPC %g0+%g1*%value SPC %b0-%b1*%value SPC 1);
		}
		%obj.isNodeLit[%node] = 0;
	}
}

function StaticShape::DialStargate(%obj, %address)
{
	%obj.setStargateState(1);
	%obj.DiallingSequence(%address, %address, -1, -1, 0, 0, 1);
}

function Gate_GetGoalAng(%obj, %goal)
{
	switch$(%obj.gateClass)
	{
		case "Milky_Way":
			return (%goal / -39) * 360;
		case "Destiny":
			return 9.457616760 + %goal * 7.1 + mFloor(%goal / 4) * 40;
	}
}

function StaticShape::DiallingSequence(%obj, %address, %origAddr, %last, %spinStart, %accelMode, %angVel, %spinDir)
{
	cancel(%obj.DialSched);
	if((%words = getWordCount(%address)) == 0) return;
	%goal = firstWord(%address);
	if(%last == -1)
	{
		%starting = 1;
		%last = getSimTime();
		%spinStart = %last;
		for(%i=0;%i<2;%i++)
		{
			%obj.playAudio(%i, GateRoll_Sound);
			%obj.GateRollSched[%i] = %obj.schedule(3392, "playAudio", %i, GateRollLoop_Sound);
		}
	}
	
	%obj.currSpin -= mFloor(%obj.currSpin / 360) * 360;
	%time = vectorDist(%last, %now = getSimTime()) / 1000;
	%goalAng = Gate_GetGoalAng(%obj, %goal - 1);
	%dist = %obj.currSpin - %goalAng;
	%dist -= mFloor(%dist / 360) * 360;

	switch$(%obj.gateClass)
	{
		case "Milky_Way":
			%spinObj = %obj.Middle;
		case "Destiny":
			%spinObj = %obj;
	}
	
	if(%obj.maxAngularVel < 0.5)
		%obj.maxAngularVel = 40;
	if(%obj.angularAccelTime > 4 || %obj.angularAccelTime < 0.01)
		%obj.angularAccelTime = 1;
	%angTopVel = %obj.maxAngularVel;
	%accelTime = %obj.angularAccelTime;
	
	//%angTopVel = 40;	//40
	//%accelTime = 1.0;	//1.5
	
	if(%obj.gateState > 1)
	{
		%accelMode = 2;
		%haltGate = 1;
	}
	
	if(%accelMode == 0)
	{
		%accelRate = (%angTopVel / %accelTime) * (%spinDir - 0.5) * 2;
		if(mAbs(%angVel + %accelRate * %time) > %angTopVel)
		{
			%velDiff = %angTopVel - mAbs(%angVel);
			%accelDiff = %velDiff / %accelRate;
			%coastTime = %time - %accelDiff;
			%newRot = (%accelRate / 2) * (%accelDiff * %accelDiff) + %angVel * %accelDiff + %obj.currSpin;
			%angVel = %angTopVel * (%spinDir - 0.5) * 2;
			%newRot += %angVel * %coastTime;
			%obj.currSpin = %newRot;
			%accelMode = 1;
		}
		else
		{
			%newRot = (%accelRate / 2) * (%time * %time) + %angVel * %time + %obj.currSpin;
			%angVel += %accelRate * %time; %obj.currSpin = %newRot;
		}
	}
	else if(%accelMode == 1)
	{
		if(%spinDir)
		{
			%deltaPos = 360 - (%obj.currSpin - %goalAng);
			if(%deltaPos < 0) %deltaPos += 360;
		}
		else
		{
			%deltaPos = %obj.currSpin - %goalAng;
			if(%deltaPos > 360) %deltaPos -= 360;
		}
		
		%accelRate = %angTopVel / %accelTime;
		%decelTime = mAbs(%angVel) / %accelRate;
		%decelDist = (mAbs(%angVel) * %decelTime) - (%accelRate * %decelTime * %decelTime / 2);
		
		%deltaTime = (%deltaPos - %decelDist) / mAbs(%angVel);
		if(%deltaTime < %time && %deltaTime > 0)
		{
			%obj.currSpin += %angVel * %deltaTime;
			%decelTime = %time - %deltaTime;
			%accelRate *= (%spinDir - 0.5) * 2;
			%newRot = (%accelRate / -2) * (%time * %time) + %angVel * %time + %obj.currSpin;
			%angVel -= %accelRate * %time;
			%obj.currSpin = %newRot;
			%accelMode = 2;
		}
		else %obj.currSpin += %angVel * %time;
	}
	else if(%accelMode == 2)
	{
		%accelRate = (%angTopVel / %accelTime) * (%spinDir - 0.5) * 2;
		if(mAbs(%angVel) - mAbs(%accelRate * %time) < 0)
		{
			%velDiff = mAbs(%angVel);
			%accelDiff = %velDiff / %accelRate;
			%newRot = (%accelRate / -2) * (%accelDiff * %accelDiff) + %angVel * %accelDiff + %obj.currSpin;
			%obj.currSpin = %newRot;
			%accelMode = 3;
			%angVel = 0;
			
			if(%haltGate)
			{
				for(%i=0;%i<2;%i++)
				{
					%obj.stopAudio(%i);
					cancel(%obj.GateRollSched[%i]);
				}
				return;
			}
		}
		else
		{
			%newRot = (%accelRate / -2) * (%time * %time) + %angVel * %time + %obj.currSpin;
			%angVel -= %accelRate * %time; %obj.currSpin = %newRot;
		}
	}
	else if(%accelMode == 3)
	{
		for(%i=0;%i<2;%i++)
		{
			%obj.stopAudio(%i);
			cancel(%obj.GateRollSched[%i]);
		}
		//%rot = ((%goal-1)/-39)*360; %obj.currSpin = %rot;
		%rot = %obj.currSpin;
		
		%for = %obj.getForwardVector();
		%hLen = vectorLen(getWords(%for, 0, 1));
		%vAng = mAtan(getWord(%for, 2), %hLen) * (180 / $PI);
		%hAng = mAtan(getWord(%for, 0), getWord(%for, 1)) * (180 / $PI);
		if(mAbs(vectorDot(%obj.getUpVector(), "0 0 1")) > 0.999) %hAng *= -1;
		%spinObj.setTransform(%obj.getPosition() SPC eulerToAxis(%vAng SPC -%rot SPC %hAng));
		
		%obj.DialSched = %obj.schedule(1520, "DiallingSequence", restWords(%address), %origAddr, -1, -1, 0, 0, !%spinDir);
		%obj.ChevronLockAnim(0, %words == 1);
		for(%i=0;%i<2;%i++)
			serverPlay3D(ChevronLocked_Sound, %obj.ChevF0.getPosition());
		if(%words > 1)
		{
			%sequence = "1 2 3 6 7 8 4 5";
			%word = getWordCount(%origAddr) - %words;
			%obj.schedule(1040, "ChevronLightAnim", getWord(%sequence, %word), 1, 0);
		}
		else
		{
			if(getWord(%origAddr, getWordCount(%origAddr) - 1) != %obj.pointOfOrigin
				|| getWordCount(%origAddr) <= 1)
			{
				if(isObject(%dhd = %obj.dhd))
				{
					%dhd.dialSequence = "";
					for(%i=0;%i<19;%i++)
					{
						%dhd.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						%dhd.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
					}
				}
				for(%i=0;%i<9;%i++)
					%obj.schedule(1920, "ChevronLightAnim", %i, 0, 1);
				%obj.setStargateState(0);
				return;
			}
			%mainAddr = getWords(%origAddr, 0, getWordCount(%origAddr) - 2);
			if(%mainAddr $= %obj.address)
			{
				if(isObject(%dhd = %obj.dhd))
				{
					%dhd.dialSequence = "";
					for(%i=0;%i<19;%i++)
					{
						%dhd.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
						%dhd.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
					}
				}
				for(%i=0;%i<9;%i++)
					%obj.schedule(1920, "ChevronLightAnim", %i, 0, 1);
				%obj.setStargateState(0);
				return;
			}
			%gates = StargateNetwork.getCount();
			for(%i=0;%i<%gates;%i++)
			{
				%gate = StargateNetwork.getObject(%i);
				if(%gate.address $= %mainAddr && %gate.gateState <= 1)
				{
					%obj.setStargateState(2);
					%gate.setStargateState(2);
					%gate.dhd.dialSequence = "";
					//for(%i=0;%i<9;%i++)
					//	if(%i == 0 || %obj.ChevL[%i].isNodeLitLight)
					//	{
					//		%gate.ChevronLockAnim(%i, 1);
					//		serverPlay3D(ChevronLocked_Sound, %gate.getPosition());
					//	}
					for(%i=0;%i<9;%i++)
						if(%i == 0 || %obj.ChevL[%i].isNodeLitLight)
						{
							%gate.schedule(1520, "ChevronLockAnim", %i, 1);
							schedule(1520, 0, "serverPlay3D", ChevronLocked_Sound, %gate.getPosition());
						}
						else %gate.schedule(1520, "ChevronLockAnim", %i, 0);
					%obj.schedule(1920, "OpenStargate", %gate, %gate);
					%gate.schedule(1920, "OpenStargate", %obj);
					return;
				}
			}
			
			for(%i=0;%i<3;%i++)
			{
				%obj.stopAudio(%i);
				%obj.schedule(1520, "playAudio", %i, DialFail_Sound);
			}
			if(isObject(%dhd = %obj.dhd))
			{
				%dhd.dialSequence = "";
				for(%i=0;%i<19;%i++)
				{
					%dhd.ObjectLightAnim("Sym_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
					%dhd.ObjectLightAnim("Sym_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
				}
			}
			for(%i=0;%i<9;%i++)
				%obj.schedule(1920, "ChevronLightAnim", %i, 0, 1);
			%obj.setStargateState(0);
			if(%obj.autodialOnShutdown)
			{
				cancel(%obj.autodialSched);
				StargateAutodial(%obj, 90);
			}
			return;
		}
		return;
	}
	
	%for = %obj.getForwardVector();
	%hLen = vectorLen(getWords(%for, 0, 1));
	%vAng = mAtan(getWord(%for, 2), %hLen) * (180 / $PI);
	%hAng = mAtan(getWord(%for, 0), getWord(%for, 1)) * (180 / $PI);
	if(mAbs(vectorDot(%obj.getUpVector(), "0 0 1")) > 0.999) %hAng *= -1;
	%spinObj.setTransform(%obj.getPosition() SPC eulerToAxis(%vAng SPC -%obj.currSpin SPC %hAng));
	
	%obj.DialSched = %obj.schedule(16, "DiallingSequence", %address, %origAddr, %now, %spinStart, %accelMode, %angVel, %spinDir);
}

function StaticShape::OpenStargate(%obj, %goal, %linked, %cl)
{
	cancel(%obj.DialSched);
	cancel(%obj.shutSched);
	cancel(%obj.hardShutSched);
	cancel(%obj.autodialSched);
	if(%obj.maxOpenTime > 0 && isObject(%linked))
	{
		%obj.shutSched = %obj.schedule((%obj.maxOpenTime * 1000) | 0, "CloseStargate");
		%obj.maxOpenTime = 0;
	}
	if(%obj.spawnMalpBotOnLock && isObject(%linked))   //Temporary
	{
		schedule(3000, 0, "createBotMALP", "0 12 2 0 0 1 3.14159", %obj);
	}
	for(%i=0;%i<3;%i++)
	{
		%obj.stopAudio(%i);
		cancel(%obj.GateRollSched[%i]);
	}
	%obj.schedule(0, "setStargateState", 2);
	%obj.horiz.setNodeColor("ALL", "1 1 1 1");
	%obj.hardShutSched = %obj.schedule(16, "HardShutStargate", 2314, getSimTime());
	for(%i=0;%i<3;%i++)
		%obj.schedule(0, "playAudio", %i, WormholeOpened_Sound);
	for(%i=0;%i<8;%i++)
		%obj.horiz.schedule(1008+32*%i, "unhideNode", "Layer"@(7-%i));
	
	%objFor = %obj.getForwardVector();
	%objUp = %obj.getUpVector();
	%objRight = vectorCross(%objFor, %objUp);
	%objPos = %obj.getPosition();
	%mask = $Typemasks::fxBrickAlwaysObjectType;
	%offset0 = vectorScale(%objUp, 0.05);
	%offset1 = vectorScale(%objRight, 0.05);
	%offset2 = vectorScale(%objUp, -0.05);
	%offset3 = vectorScale(%objRight, -0.05);
	%killKawoosh = 1;
	for(%i=0;%i<8;%i++)
	{
		for(%j=0;%j<24;%j++)
		{
			%killCount = 0;
			if(%i == 0 && %j != 0) continue;
			%mult = 0.45 * %i; //3.6 (radius of the wormhole) divided by 8 (number of raycast rings)
			%uVal = mSin(%j / 12 * $PI) * %mult; %rVal = mCos(%j / 12 * $PI) * %mult;
			%pos = vectorAdd(%objPos, vectorAdd(vectorScale(%objUp, %uVal), vectorScale(%objRight, %rVal)));
			for(%k=0;%k<4;%k++)
			{
				%end = vectorAdd(vectorAdd(%pos, %offset[%k]), vectorScale(%objFor, 0.5));
				%ray = containerRaycast(%pos, %end, %mask);
				if(!isObject(%hit = firstWord(%ray)) || !%hit.isColliding()) %killCount++;
			}
			if(%killCount == 4) { %killKawoosh = 0; break; }
		}
		if(!%killKawoosh) break;
	}
	
	if(!%killKawoosh)
		%obj.kawoosh.schedule(1232, "unhideNode", "ALL");
	%scale = getWord(%obj.getScale(), 2);
	for(%i=0;%i<120;%i++)
	{
		if(%i < 30) %colVal = mSin(%i/59*$PI);
		else if(!%killKawoosh && %i >= 90) %colVal = mSin((%i-60)/59*$PI);
		else %colVal = 1;
		if(!%killKawoosh)
		{
			%obj.kawoosh.schedule(1232+16*%i, "setScale", %scale SPC (mSin(%i/119*$PI)*%scale) SPC %scale);
			if(%i & 1) %obj.schedule(1232+16*%i, "SlayNearby", 0, mSin(%i/119*$PI)*13.32);
			%obj.kawoosh.schedule(1232+16*%i, "setNodeColor", "ALL", 1-%colVal SPC 1-%colVal/2@" 1 1");
		}
		%obj.horiz.schedule(1232+16*%i, "setNodeColor", "ALL", 1-%colVal SPC 1-%colVal/2@" 1 1");
	}
	if(!%killKawoosh)
	{
		%obj.kawoosh.schedule(3152, "hideNode", "ALL");
		for(%i=0;%i<16;%i++)
			%obj.horiz.schedule(3152+16*%i, "setNodeColor", "ALL", 1-mSin(%i/31*$PI) SPC 1-mSin(%i/31*$PI)/2@" 1 1");
	}
	for(%i=0;%i<2;%i++)
		%obj.schedule(4000, "playAudio", %i, EventHorizon_Sound);
	%obj.schedule(4000, "setStargateState", 3);
	%obj.SGWormholeSched = %obj.schedule(3152, "StargateWormhole", %goal, getSimTime());
	if(isObject(%linked))
	{
		cancel(%linked.shutSched);
		cancel(%linked.hardShutSched);
		initContainerRadiusSearch(%linked.getPosition(), 16, $Typemasks::fxBrickAlwaysObjectType);
		while(isObject(%brick = containerSearchNext()))
			%brick.WormholeIncoming(%cl);
		initContainerRadiusSearch(%obj.getPosition(), 16, $Typemasks::fxBrickAlwaysObjectType);
		while(isObject(%brick = containerSearchNext()))
			%brick.WormholeOutgoing(%cl);
		%obj.linkedGate = %linked;
		%linked.masterGate = %obj;
	}
}

function StaticShape::CloseStargate(%obj)
{
	%obj.setStargateState(4);
	cancel(%obj.shutSched);
	cancel(%obj.hardShutSched);
	cancel(%obj.autodialSched); %hop = %obj;
	while(isObject(%nextHop = %hop.redirectToGate))
		%hop.redirectToGate = "";
	if(isObject(%dhd = %obj.dhd))
	{
		%dhd.ObjectLightAnim("BUTTON", 0, 1, 0.5, 0.5, 0, 0, 0, 0);
		for(%i=0;%i<19;%i++)
		{
			%dhd.ObjectLightAnim("SYM_A_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
			%dhd.ObjectLightAnim("SYM_B_"@%i, 0, 1, 0.2, 0.8, 0.1, 0.6, 0, 0);
		}
	}
	for(%i=0;%i<9;%i++)
		%obj.schedule(1680, "ChevronLightAnim", %i, 0, 1);
	for(%i=0;%i<3;%i++)
		%obj.playAudio(%i, WormholeClosed_Sound);
	for(%i=0;%i<80;%i++)
	{
		%value = mSin((%i + 80)/159*$PI);
		%obj.horiz.schedule(16*%i, "setNodeColor", "ALL", 1-%value SPC 1-%value/2@" 1 1");
	}
	%obj.schedule(1296, "SlayNearby", 1);
	for(%i=0;%i<39;%i++)
		%obj.horiz.schedule(1296+16*%i, "setNodeColor", "ALL", "1 1 1 "@1-%i/39);
	%obj.horiz.schedule(1920, "hideNode", "ALL");
	%obj.schedule(1920, "setStargateState", 0);
	if(isObject(%linked = %obj.linkedGate))
	{
		%linked.CloseStargate();
		%linked.schedule(1920, "clearLinkedGate");
		%obj.schedule(1920, "clearLinkedGate");
	}
	initContainerRadiusSearch(%obj.getPosition(), 16, $Typemasks::fxBrickAlwaysObjectType);
	while(isObject(%brick = containerSearchNext()))
		%brick.schedule(1920, "WormholeShutdown", %cl);
	if(%obj.autodialOnShutdown)
	{
		cancel(%obj.autodialSched);
		%obj.autodialSched = schedule(4920, 0, "StargateAutodial", %obj, 90);
	}
}

function StaticShape::clearLinkedGate(%obj)
{ %obj.masterGate = ""; %obj.linkedGate = ""; }

function StaticShape::HardShutStargate(%obj, %timeLeft, %last)
{
	if(!isObject(%this)) return;
	cancel(%obj.hardShutSched);
	%time = vectorDist(%last, %now = getSimTime()) / 1000;
	%timeLeft -= %time; if(%timeLeft <= 0) { %obj.CloseStargate(); return; }
	%obj.hardShutSched = %obj.schedule(16, "HardShutStargate", %timeLeft, %now);
}

function StaticShape::StargateWormhole(%this, %dest, %last)
{
	if(!isObject(%this)) return;
	cancel(%this.SGWormholeSched);
	%scale = getWord(%this.getScale(), 2);
	%time = vectorDist(%last, %now = getSimTime()) / 1000;
	%this.SGWormholeSched = %this.schedule(48, "StargateWormhole", %dest, %now);
	%mask = $Typemasks::CorpseObjectType | $Typemasks::PlayerObjectType |
		$Typemasks::ProjectileObjectType | $Typemasks::VehicleObjectType;
	%playerMask = $Typemasks::CorpseObjectType | $Typemasks::PlayerObjectType;
	initContainerRadiusSearch(%loc = %this.getPosition(), 16 * %scale, %mask);
	while(isObject(%obj = containerSearchNext()))
	{
		%ignore = 0;
		for(%i=0;%i<%this.trackCount;%i++)
			if(%this.trackObj[%i] == %obj)
			{ %ignore = 1; break; }
		if(%ignore) continue;
		%this.trackObj[-1+%this.trackCount++] = %obj;
	}
	while(isObject(%nextHop = %dest.redirectToGate) && !%hoppedTo[%nextHop])
	{
		%hoppedTo[%nextHop] = 1;
		%dest = %nextHop;
	}
	%destFor = %dest.getForwardVector();
	%destUp = %dest.getUpVector();
	%destPos = %dest.getPosition();
	for(%i=0;%i<%this.trackCount;%i++)
	{
		%obj = %this.trackObj[%i];
		if(!isObject(%obj))
		{ %shift++; continue; }
		%type = %obj.getType();
		if(!(%type & $Typemasks::ProjectileObjectType) && isObject(%obj.getObjectMount())) continue;
		if(%type & %playerMask)
			%pos = %obj.getHackPosition();
		else if(%type & $Typemasks::VehicleObjectType)
			%pos = %obj.getWorldBoxCenter();
		else if(%type & $Typemasks::ProjectileObjectType)
			%pos = vectorAdd(%obj.getPosition(), vectorScale(%obj.getVelocity(), 0.08));
		if(vectorDist(%pos, %loc) > 16 * %scale)
		{ %shift++; continue; }
		else %this.trackObj[%i - %shift] = %this.trackObj[%i];
		if((%this.SGLastDetect[%obj] | 0) != (%last | 0))
		{
			%oPos = %pos;
			%this.SGLastVel[%obj] = %obj.getVelocity();
		}
		else %oPos = %this.SGLastPos[%obj];
		%dir = vectorSub(%pos, %loc);
		%oDir = vectorSub(%oPos, %loc);
		%for = %this.getForwardVector();
		%up = %this.getUpVector();
		%right = vectorCross(%for, %up);
		%rDir = vectorDot(%dir, %for) SPC vectorDot(%dir, %right) SPC vectorDot(%dir, %up);
		%roDir = vectorDot(%oDir, %for) SPC vectorDot(%oDir, %right) SPC vectorDot(%oDir, %up);
		if(getWord(%rDir, 0) / getWord(%roDir, 0) < 0)
		{
			%len = getWord(%rDir, 0) / (getWord(%rDir, 0) - getWord(%roDir, 0));
			%yDiff = getWord(%rDir, 1) - getWord(%roDir, 1);
			%zDiff = getWord(%rDir, 2) - getWord(%roDir, 2);
			%hit = "0 "@getWord(%rDir, 1) + %yDiff * %len SPC getWord(%rDir, 2) + %zDiff * %len;
			if(vectorLen(%hit) <= 3.6 * %scale)
			{
				if(getWord(%rDir, 0) <= 0)
				{
					if(isObject(%this.masterGate))
					{
						if(%type & %playerMask)
						{
							%obj.spawnTime = 0;
							%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_Backwards);
							if(%obj.getState() $= "DEAD") { %obj.delete(); }
						}
						else if(%type & $Typemasks::VehicleObjectType)
						{
							%mounts = %obj.getDatablock().numMountPoints;
							for(%i=0;%i<%mounts;%i++)
							{
								%passenger = %obj.getMountedObject(%i); %passenger.spawnTime = 0;
								%passenger.damage(%passenger, %passenger.position, 10000, $DamageType::Stargate_Backwards);
								if(%passenger.getState() $= "DEAD") { %passenger.delete(); }
							}
							
							%spawn = %obj.spawnBrick;
							if(isObject(%spawn))
								%spawn.respawnVehicle();
							else %obj.delete();
						}
						else if(%type & $Typemasks::ProjectileObjectType)
							%obj.delete();
					}
					else
					{
						%vel = %this.SGLastVel[%obj];
                        %rPos = vectorDot(%hit, %right);
                        %uPos = vectorDot(%hit, %up);
                        //%destHit = vectorAdd(vectorAdd(vectorScale(%destRight, %rPos), vectorScale(%destUp, %uPos)), %dest);

                        %destRight = vectorCross(%destFor, %destUp);
                        %destHit = vectorAdd(%destPos, vectorAdd(vectorAdd(vectorScale(%destFor, -getWord(%rDir, 0)), vectorScale(%destRight, -getWord(%rDir, 1))), vectorScale(%destUp, getWord(%rDir, 2))));

                        %index = mAbs(getWord(%destFor, 0)) < mAbs(getWord(%destFor, 1));
                        %destHit = setWord(%destHit, %index, getWord(%destPos, %index));

                        %objHit = vectorAdd(%this.getPosition(), vectorAdd(
                            vectorScale(%right, %rPos), vectorScale(%up, %uPos)));

						serverPlay3D(ObjectEntrance_Sound, %objHit);

                        %mask = $Typemasks::fxBrickAlwaysObjectType;
                        %ray = containerRaycast(%destHit, vectorAdd(%destHit, vectorScale(%destFor, 0.8)), %mask);
                        if(isObject(%ray) && %ray.isColliding())
						{
							if(%type & %playerMask)
							{
								%obj.spawnTime = 0;
								for(%j=0;%j<3;%j++) serverPlay3D(IrisImpact_Sound, %destHit);
								%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_IrisSlam);
								if(%obj.getState() $= "DEAD") { %obj.delete(); } continue;
							}
							else if(%type & $Typemasks::VehicleObjectType)
							{
								%mounts = %obj.getDatablock().numMountPoints;
								for(%i=0;%i<%mounts;%i++)
								{
									%passenger = %obj.getMountedObject(%i); %passenger.spawnTime = 0;
									%passenger.damage(%passenger, %passenger.position, 10000, $DamageType::Stargate_IrisSlam);
									if(%passenger.getState() $= "DEAD") { %passenger.delete(); }
								}
								
								for(%j=0;%j<5;%j++) serverPlay3D(IrisImpact_Sound, %destHit);
								%spawn = %obj.spawnBrick;
								if(isObject(%spawn))
									%spawn.respawnVehicle();
								else %obj.delete();
								continue;
							}
							else if(%type & $Typemasks::ProjectileObjectType)
							{ serverPlay3D(IrisImpact_Sound, %destHit); %obj.delete(); continue; }
						}
						else serverPlay3D(ObjectEntrance_Sound, %destHit);
						if(%obj.getClassName() $= "Player" && $EnvironMode)
						{
							if(!isObject(EnvGuiMaster))
								new ScriptObject(EnvGuiMaster)
								{ isEnvMaster = 1; isAdmin = 1; isSuperAdmin = 1; };
							%id = skyTexToIDX(%dest.skyTex);
							if(%id != -1) servercmdEnvGui_SetVar(EnvGuiMaster, "SkyIDX", %id);
							%id = groundTexToIDX(%dest.groundTex);
							if(%id != -1) servercmdEnvGui_SetVar(EnvGuiMaster, "GroundIDX", %id);
							if(%dest.dLightColor !$= "") servercmdEnvGui_SetVar(EnvGuiMaster, "DirectLightColor", %dest.dLightColor);
							if(%dest.aLightColor !$= "") servercmdEnvGui_SetVar(EnvGuiMaster, "AmbientLightColor", %dest.aLightColor);
							if(%dest.shadowColor !$= "") servercmdEnvGui_SetVar(EnvGuiMaster, "ShadowColor", %dest.shadowColor);
							if(%dest.sunAzim !$= "" || %dest.sunElev !$= "")
							{
								if(%dest.sunAzim !$= "") Sun.azimuth = %dest.sunAzim;
								if(%dest.sunElev !$= "") Sun.elevation = %dest.sunElev;
								Sun.sendUpdate();
							}
							Sky.visibleDistance = getMin($EnvGuiServer::VisibleDistance, 225);
							Sky.sendUpdate();
						}
						%destRight = vectorCross(%destFor, %destUp);
						%tele = vectorAdd(%destPos, vectorAdd(vectorAdd(
							vectorScale(%destFor, -getWord(%rDir, 0)),
							vectorScale(%destRight, -getWord(%rDir, 1))),
							vectorScale(%destUp, getWord(%rDir, 2))));
						if(%type & $Typemasks::PlayerObjectType)
							%offset = vectorSub(%obj.getHackPosition(), %obj.getPosition());
						else if(%type & $Typemasks::PlayerObjectType)
							%offset = vectorSub(%obj.getWorldBoxCenter(), %obj.getPosition());
						%tele = vectorSub(%tele, %offset);
						
						%oFor = %obj.getForwardVector();
						%fFor = vectorDot(%oFor, %for);
						%rFor = vectorDot(%oFor, %right);
						%uFor = vectorDot(%oFor, %up);
						%oFor = vectorAdd(vectorScale(%destFor, -%fFor), vectorScale(%destRight, -%rFor));
						%ang = mAtan(getWord(%oFor, 0), getWord(%oFor, 1));
						if(%type & $Typemasks::ProjectileObjectType)
						{
							%fVel = vectorDot(%vel, %for);
							%rVel = vectorDot(%vel, %right);
							%uVel = vectorDot(%vel, %up);
							%outVel = vectorAdd(vectorAdd(vectorScale(%destFor, -%fVel),
								vectorScale(%destRight, -%rVel)), vectorScale(%destUp, %uVel));
							
							%obj.setName("SGTeleport_OldProjectile");
							%proj = new Projectile(:SGTeleport_OldProjectile)
							{
								initialPosition = %tele;
								initialVelocity = %outVel;
							};
							MissionCleanup.add(%proj);
							%obj.delete();
							
							//if(%this.gateState == 2 || %this.gateState == 3)
							//{
							//	if(getTimeRemaining(%this.shutSched) <= 5000)
							//	{
							//		cancel(%this.shutSched);
							//		%this.shutSched = %this.schedule(5000, "CloseStargate");
							//	}
							//}
						}
						else if(!isObject(%obj.getObjectMount()))
						{
							if(%type & $Typemasks::VehicleObjectType)
							{
								%mounts = %obj.getDatablock().numMountPoints;
								for(%i=0;%i<%mounts;%i++)
								{
									%passenger = %obj.getMountedObject(%i);
									if(isObject(%cl = %passenger.client))
									{
										$Stargate[%cl.bl_id] = %this.linkedGate;
										$StargatePos[%cl.bl_id] = %destPos;
										%destAng = mAtan(getWords(%destFor, 0), getWords(%destFor, 1));
										$StargateSpawn[%cl.bl_id] = vectorAdd(%destPos, vectorScale(%destFor, 4))@" 0 0 1 "@%destAng;
									}
								}
							}
							
							%obj.setTransform(%tele@" 0 0 1 "@%ang);
							
							%fVel = vectorDot(%vel, %for);
							%rVel = vectorDot(%vel, %right);
							%uVel = vectorDot(%vel, %up);
							%obj.setVelocity(vectorAdd(vectorAdd(vectorScale(%destFor, -%fVel),
								vectorScale(%destRight, -%rVel)), vectorScale(%destUp, %uVel)));
							
							if(isObject(%cl = %obj.client))
							{
								$Stargate[%cl.bl_id] = %this.linkedGate;
								$StargatePos[%cl.bl_id] = %destPos;
								%destAng = mAtan(getWords(%destFor, 0), getWords(%destFor, 1));
								$StargateSpawn[%cl.bl_id] = vectorAdd(%destPos, vectorScale(%destFor, 4))@" 0 0 1 "@%destAng;
							}
							
							if(%this.gateState == 2 || %this.gateState == 3)
							{
								if(!(%this.dontCloseOnObjectEnter ||
									(%obj.getClassName() $= "AIPlayer" && %this.dontCloseOnBotEnter)))
								{
									cancel(%this.shutSched);
									%this.shutSched = %this.schedule(20000, "CloseStargate");
								}
							}
						}
					}
				}
				else
				{
					if(%type & $Typemasks::PlayerObjectType)
					{
						%obj.spawnTime = 0;
						%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_WrongSide);
						if(%obj.getState() $= "DEAD") { %obj.delete(); }
					}
					else if(%type & $Typemasks::VehicleObjectType)
					{
						%mounts = %obj.getDatablock().numMountPoints;
						for(%i=0;%i<%mounts;%i++)
						{
							%passenger = %obj.getMountedObject(%i);
							if(!isObject(%passenger)) continue; %passenger.spawnTime = 0;
							%passenger.damage(%passenger, %passenger.position, 10000, $DamageType::Stargate_WrongSide);
							if(%passenger.getState() $= "DEAD") { %passenger.delete(); }
						}
						
						%spawn = %obj.spawnBrick;
						if(isObject(%spawn))
							%spawn.respawnVehicle();
						else %obj.delete();
					}
					else if(%type & $Typemasks::ProjectileObjectType)
						%obj.delete();
				}
			}
		}
		if(isObject(%obj))
		{
			%this.SGLastDetect[%obj] = %now;
			%this.SGLastPos[%obj] = %pos;
			%this.SGLastVel[%obj] = %obj.getVelocity();
		}
		else %shift++;
	}
	%this.trackCount -= %shift;
}

function StaticShape::SlayNearby(%this, %mode, %len)
{
	%scale = getWord(%this.getScale(), 2);
	if(%mode == 0)
	{
		%loc = %this.getPosition();
		%for = %this.getForwardVector();
		%up = %this.getUpVector();
		%right = vectorCross(%for, %up);
		initContainerRadiusSearch(vectorAdd(%loc, vectorScale(%for, 10)), %scale * 16 + 8, $Typemasks::PlayerObjectType);
		while(isObject(%obj = containerSearchNext()))
		{
			%pos = %obj.getHackPosition();
			%dir = vectorSub(%pos, %loc);
			%objF = vectorDot(%dir, %for);
			%objU = vectorDot(%dir, %up);
			%objR = vectorDot(%dir, %right);
			%objOffset = vectorLen(%objU SPC %objR);
			if(%objOffset > 3.6 * %scale) continue;
			if(mAbs(%objF) <= 0.36)
			{
				%obj.spawnTime = 0;
				%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_Kawoosh);
				if(%obj.getState() $= "DEAD") { %obj.delete(); }
			}
			%objOffset /= 3.6 * %scale;
			%kLen = %len - (%objOffset * %objOffset * %len);
			if(%objF < %kLen && %objF > 0)
			{
				%obj.spawnTime = 0;
				%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_Kawoosh);
				if(%obj.getState() $= "DEAD") { %obj.delete(); }
			}
		}
	}
	else if(%mode == 1)
	{
		initContainerRadiusSearch(%loc = %this.getPosition(), %scale * 10, $Typemasks::PlayerObjectType);
		while(isObject(%obj = containerSearchNext()))
		{
			%pos = %obj.getHackPosition();
			if(vectorDist(%pos, %loc) > 3.6 * %scale) continue;
			%dir = vectorSub(%pos, %loc);
			if(mAbs(vectorDot(%dir, %this.getForwardVector())) <= 0.35)
			{
				%obj.spawnTime = 0;
				%obj.damage(%obj, %obj.position, 10000, $DamageType::Stargate_Telefrag);
				if(%obj.getState() $= "DEAD") { %obj.delete(); }
			}
		}
		cancel(%this.SGWormholeSched);
	}
}

function StaticShape::setStargateState(%obj, %state)
{
	if(%state == 0)
	{
		%obj.isOpened = 0;
		%obj.isOpening = 0;
		%obj.isClosed = 1;
		%obj.isClosing = 0;
		%obj.isDialing = 0;
		%obj.gateState = 0;
	}
	else if(%state == 1)
	{
		%obj.isOpened = 0;
		%obj.isOpening = 0;
		%obj.isClosed = 0;
		%obj.isClosing = 0;
		%obj.isDialing = 1;
		%obj.gateState = 1;
	}
	else if(%state == 2)
	{
		%obj.isOpened = 0;
		%obj.isOpening = 1;
		%obj.isClosed = 0;
		%obj.isClosing = 0;
		%obj.isDialing = 0;
		%obj.gateState = 2;
	}
	else if(%state == 3)
	{
		%obj.isOpened = 1;
		%obj.isOpening = 0;
		%obj.isClosed = 0;
		%obj.isClosing = 0;
		%obj.isDialing = 0;
		%obj.gateState = 3;
	}
	else if(%state == 4)
	{
		%obj.isOpened = 0;
		%obj.isOpening = 0;
		%obj.isClosed = 0;
		%obj.isClosing = 1;
		%obj.isDialing = 0;
		%obj.gateState = 4;
	}
}

function StaticShape::DeleteStargate(%obj)
{
	for(%i=0;%i<3;%i++)
		%obj.stopAudio(%i);
	for(%i=0;%i<9;%i++)
	{
		%obj.ChevB[%i].delete();
		%obj.ChevF[%i].delete();
		%obj.ChevL[%i].delete();
	}
	%obj.kawoosh.delete();
	%obj.horiz.delete();
	%obj.inner.delete();
	%obj.middle.delete();
	%obj.dhd.delete();
	%obj.delete();
}

function StaticShape::UpdateStargateTrans(%obj, %trans)
{
	%obj.setTransform(%trans);
	%obj.middle.setTransform(%trans);
	%obj.inner.setTransform(%trans);
	%obj.horiz.setTransform(%trans);
	%obj.kawoosh.setTransform(%trans);
	
	%pos = getWords(%trans, 0, 2);
	%for = %obj.getForwardVector();
	%up = %obj.getUpVector();
	%right = vectorCross(%for, %up);
	%hLen = vectorLen(getWords(%for, 0, 1));
	%vAng = mAtan(getWord(%for, 2), %hLen) * (180 / $PI);
	%hAng = mAtan(getWord(%for, 0), getWord(%for, 1)) * (180 / $PI);
	%radius = getWord(%obj.getScale(), 2) * 4.2;
	if(mAbs(vectorDot(%up, "0 0 1")) <= 0.999)
	{
		%hAng += 180; %vAng = 180 - %vAng;
		if(mAbs(vectorDot(%for, "0 0 1")) <= 0.999) %rot += 180;
	}
	for(%i=0;%i<9;%i++)
	{
		%offset = vectorAdd(vectorScale(%right, mSin(%i*$PI/4.5)*-%radius),
			vectorScale(%up, mCos(%i*$PI/4.5)*%radius));
		%trans = vectorAdd(%pos, %offset) SPC eulerToAxis(%vAng SPC -360*(%i/9)+%rot SPC -%hAng);
		
		%obj.ChevB[%i].setTransform(%trans);
		%obj.ChevF[%i].setTransform(%trans);
		%obj.ChevL[%i].setTransform(%trans);
		
		%obj.ChevB[%i].ChevronPosition = getWords(%trans, 0, 3);
		%obj.ChevL[%i].ChevronPosition = getWords(%trans, 0, 3);
	}
}

function StaticShape::UpdateStargateScale(%obj, %scalar)
{
	%scalar = getWord(%scalar, 0) * 1;
	%scale = %scalar SPC %scalar SPC %scalar;
	
	%obj.setScale(%scale);
	%obj.middle.setScale(%scale);
	%obj.inner.setScale(%scale);
	%obj.horiz.setScale(%scale);
	%obj.kawoosh.setScale(%scale);
	
	%pos = getWords(%trans, 0, 2);
	%for = %obj.getForwardVector();
	%up = %obj.getUpVector();
	%right = vectorCross(%for, %up);
	%hLen = vectorLen(getWords(%for, 0, 1));
	%vAng = mAtan(getWord(%for, 2), %hLen) * (180 / $PI);
	%hAng = mAtan(getWord(%for, 0), getWord(%for, 1)) * (180 / $PI);
	%radius = getWord(%obj.getScale(), 2) * 4.2;
	if(mAbs(vectorDot(%up, "0 0 1")) <= 0.999)
	{
		%hAng += 180; %vAng = 180 - %vAng;
		if(mAbs(vectorDot(%for, "0 0 1")) <= 0.999) %rot += 180;
	}
	for(%i=0;%i<9;%i++)
	{
		%offset = vectorAdd(vectorScale(%right, mSin(%i*$PI/4.5)*-%radius),
			vectorScale(%up, mCos(%i*$PI/4.5)*%radius));
		%trans = vectorAdd(%pos, %offset) SPC eulerToAxis(%vAng SPC -360*(%i/9)+%rot SPC -%hAng);
		
		%obj.ChevB[%i].setScale(%scale);
		%obj.ChevF[%i].setScale(%scale);
		%obj.ChevL[%i].setScale(%scale);
		
		%obj.ChevB[%i].setTransform(%trans);
		%obj.ChevF[%i].setTransform(%trans);
		%obj.ChevL[%i].setTransform(%trans);
		
		%obj.ChevB[%i].ChevronPosition = getWords(%trans, 0, 3);
		%obj.ChevL[%i].ChevronPosition = getWords(%trans, 0, 3);
	}
}

function StaticShape::copyEnvironment(%gate)
{
	%gate.skyTex = Sky.materialList;
	%gate.groundTex = GroundPlane.topTexture;
	%gate.dLightColor = Sun.color;
	%gate.aLightColor = Sun.ambient;
	%gate.shadowColor = Sun.shadowColor;
	%gate.sunAzim = Sun.azimuth;
	%gate.sunElev = Sun.elevation;
}

//Destiny gate symbols
//Symbol 1:  9.457616760

//Symbol 2: 16.557627498
//Delta:     7.100010738

//Symbol 5: 49.457612532
//Delta:    39.999995772

//MakeDHD(MakeStargate("0 0.25 4.5"), "0 20 1 1 0 0 0");
//for(%i=0;%i<9;%i++)%hit.ChevronLightAnim(%i,0,1);%hit.horiz.hideNode("ALL");for(%i=0;%i<2;%i++)%hit.playAudio(%i,WormholeClosed_Sound);

//%hit.DialStargate("27 7 15 32 12 30 1");	Abydos
//%hit.DialStargate("9 18 27 15 21 36 1");	Final Destination
//%hit.DialStargate("11 27 23 16 33 3 9 1");	Othala

//2 9 11 14 29 35
//%hit.DialStargate("6 0 0 0 0 0 5 0 1");	//Destiny - known symbols

//%hit.DialStargate("6 35 11 2 14 9 5 29 1");	//Destiny - assumed address

function MakeGates()
{
	if(!$MadeGates)
	{
		$MadeGates = 1;
		%earthGate = MakeStargate("0 0.25 5.3 "@eulerToAxis("0 0 0"), "28 26 5 36 11 29", 1, "Earth");
		
		%address[-1+%address++] = "27-7-15-32-12-30 1 Abydos";   //Canonical point of origin
		%address[-1+%address++] = "20-18-11-38-10-32 31 Apophis's_Base";
		%address[-1+%address++] = "20-2-35-8-26-15 36 Camelot";
		%address[-1+%address++] = "29-3-6-9-12-16 31 Castiana";
		%address[-1+%address++] = "29-18-19-20-21-22 20 Sahal";
		%address[-1+%address++] = "9-2-23-15-37-20 7 Chulak";   //Canonical point of origin
		%address[-1+%address++] = "14-21-16-11-30-7 20 Clava_Thessara_Infinatas_Alpha";
		%address[-1+%address++] = "26-20-35-32-23-4 9 Clava_Thessara_Infinatas_Beta";
		%address[-1+%address++] = "3-32-16-8-10-12 36 Destroyers";
		%address[-1+%address++] = "28-38-35-9-15-3 26 Edora";
		%address[-1+%address++] = "30-27-9-7-18-16 9 Euronda";
		%address[-1+%address++] = "29-8-18-22-4-25 4 Juna";
		%address[-1+%address++] = "6-16-8-3-26-25 26 Kallana";
		%address[-1+%address++] = "26-35-6-8-23-14 20 Kheb";
		%address[-1+%address++] = "18-2-30-12-26-33 22 K'Tau";   //Non-canonical
		%address[-1+%address++] = "24-12-32-7-11-34 4 Martin's_Homeworld";
		%address[-1+%address++] = "38-28-15-35-3-19 9 NID_Off-World_Base";
		%address[-1+%address++] = "28-8-16-33-13-31 15 P2X-555";
		%address[-1+%address++] = "38-9-28-15-35-3 37 P34-353J";   //Canonical point of origin
		%address[-1+%address++] = "19-8-4-37-26-16 4 P3W-451";
		%address[-1+%address++] = "9-26-34-37-17-21 15 P3X-118";
		%address[-1+%address++] = "3-28-9-35-24-15 14 P3X-562";   //Canonical point of origin
		%address[-1+%address++] = "25-8-18-29-4-22 37 P9C-372";
		%address[-1+%address++] = "12-36-23-18-7-27 32 PB5-926";
		%address[-1+%address++] = "35-3-31-29-5-17 37 Praclarush_Taonas";
		%address[-1+%address++] = "30-19-34-9-33-18 37 Sangreal_Planet";
		%address[-1+%address++] = "33-28-23-26-16-31 3 Tartarus";   //Non-canonical
		%address[-1+%address++] = "6-33-27-37-11-18 2 Tollan";   //Canonical point of origin
		%address[-1+%address++] = "4-29-8-22-18-25 10 Tollana";
		%address[-1+%address++] = "3-8-2-12-19-30 4 Vagon_Brei";
		
		//%address[-1+%address++] = "9-18-27-15-21-36 21";
		//%address[-1+%address++] = "11-27-23-16-33-3-9 1 Othala";
		//%address[-1+%address++] = "6-35-11-2-14-9-5-29 1 Destiny";
		
		%id = createRandContext(671);  //*Magick* Mage's BL_ID
		while(%address < 816)
		{
			%randAddr = "";
			for(%i=0;%i<6;%i++)
			{
				%symbol = mFloor(rand(%id, 2, 40));
				if(%symbol >= 40) %symbol = 39;
				%flag = 1;
				while(%flag)
				{
					%flag = 0;
					for(%j=0;%j<%i;%j++)
						if(getWord(%randAddr, %j) == %symbol)
						{
							%symbol = mFloor(rand(%id, 2, 40));
							if(%symbol >= 40) %symbol = 39;
							%flag = 1;
						}
				}
				if(strstr(%randAddr, %symbol) != -1)
				{
					%symbol = mCeil(rand(%id, 2, 40));
				}
				%randAddr = trim(trim(%randAddr) @ "-" @ trim(%symbol));
			}
			%randAddr = trim(%randAddr);
			%duplicate = 0;
			for(%i=0;%i<%address;%i++)
				if(firstWord(%address[%i]) $= firstWord(%randAddr))
					%duplicate = 1;
			if(%duplicate == 0)
				%address[-1+%address++] = %randAddr;
		}
		
		for(%y=0;%y<33;%y++)
		{
			%nVal = mFloatLength((%y - 16) * 443.405 * 2, 0) / 2;
			%width = 33 - mAbs(%y - 16);
			for(%x=0;%x<%width;%x++)
			{
				%eVal = (%x * 512) - ((%width - 1) * 256);
				if(%nVal == 0 && %eVal == 0) continue;
				
				%addrID = mFloor(rand(%id, 0, %address));
				if(%addrID >= %address) %addrID = %address - 1;
				%gateAddr = ltrim(strReplace(getWord(%addr = %address[%addrID], 0), "-", " "));
				if(getWord(%addr, 1) !$= "")
				{
					%origin = getWord(%addr, 1);
					rand(%id, 2, 40);
				}
				else
				{
					%origin = mFloor(rand(%id, 2, 40));
					if(%origin >= 40)
						%origin = 39;
				}
				%ang = mFloor(rand(%id, 0, 4)) * 90;
				%gateName = strReplace(getWord(%addr, 2), "_", " ");
				
				%shift = 0;
				for(%i=0;%i<%address;%i++)
					if(%i == %addrID) %shift++;
					else %address[%i - %shift] = %address[%i];
				%address -= %shift;
				
				%gate = MakeStargate(vectorAdd(%eVal SPC %nVal SPC 4.7,
					vectorScale(-mSin(%ang/180*$PI) SPC mCos(%ang/180*$PI), -0.25))
					SPC eulerToAxis("0 0 "@%ang), %gateAddr, %origin, %gateName);
				%dhd = MakeDHD(%gate, vectorAdd(%eVal SPC %nVal SPC 1.2,
					vectorScale(-mSin(%ang/180*$PI) SPC mCos(%ang/180*$PI), 20))
					SPC eulerToAxis("0 0 "@%ang));
				switch(%addrID)
				{
					case 0:
						%gate.groundTex = "./TTasphalt01.jpg";
						%gate.skyTex = "./Slate_Storm.dml";
						%gate.sunAzim = 0;
						%gate.sunElev = 90;
				}
			}
		}
		%earthGate.groundTex = "./cement-pebbled.jpg";
		%earthGate.skyTex = "./Blue2.dml";
		%earthGate.sunAzim = 0;
		%earthGate.sunElev = 30;
		%earthGate.autodialOnShutdown = 1;
		%earthGate.spawnMalpBotOnLock = 1;   //Temporary
		%earthGate.dontCloseOnObjectEnter = 0;
		%earthGate.dontCloseOnBotEnter = 1;
		schedule(15000, 0, "StargateAutodial", %earthGate, 90);
	}
}

function StargateAutodial(%gate, %openTime)
{
	if(!isObject(%gate)) return;
	if(%gate.gateState != 0) return;
	%gates = StargateNetwork.getCount();
	for(%i=0;%i<%gates;%i++)
	{
		%obj = StargateNetwork.getObject(%i);
		if(%obj.worldName !$= "")
		{
			if(%obj == %gate) continue;
			%list[-1+%list++] = %obj;
		}
	}
	if(%list > 0)
	{
		%gate.maxOpenTime = %openTime;
		%dialGate = %list[getRandom(0, %list - 1)];
		MessageAllInRange(%gate.getPosition(), "\c2"@trim(%gate.worldName SPC "Autodialler")@"\c6: Dialling to "@%dialGate.worldName@".");
		%gate.DialStargate(%dialGate.address SPC %gate.pointOfOrigin);
		for(%i=0;%i<3;%i++) %gate.lastAutodial[%i + 1] = %gate.lastAutodial[%i];
		%gate.lastAutodial0 = %dialGate;
	}
}

if(!isObject(StargateNetwork))
	new SimSet(StargateNetwork);
MakeGates();