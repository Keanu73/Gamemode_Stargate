function TutorialDamage(%num, %vel)
{
	%group = Brickgroup_999999;
	%count = %group.NTObjectCount_TutorialDamage[%num];
	for(%i=0;%i<%count;%i++)
	{
		%brick = %group.NTObject_TutorialDamage[%num, %i];
		%brick.fakeKillBrick(%vel, 3600);
	}
}

function TutorialResetDamage(%num)
{
	%group = Brickgroup_999999;
	%count = %group.NTObjectCount_TutorialDamage[%num];
	for(%i=0;%i<%count;%i++)
	{
		%brick = %group.NTObject_TutorialDamage[%num, %i];
		%brick.respawn();
	}
}

function TutorialSetLights(%num, %light)
{
	%group = Brickgroup_999999;
	%count = %group.NTObjectCount_TutorialLight[%num];
	%data = $uiNameTable_Lights[%light];
	if(%data $= "") %data = -1;
	for(%i=0;%i<%count;%i++)
	{
		%brick = %group.NTObject_TutorialLight[%num, %i];
		%brick.setLight(%data);
	}
}

function TutorialShake(%pos, %type)
{
	if (%type == 1)
	{
		new Projectile()
		{
			datablock = TutorialBaseShakeHeavyProj;
			initialPosition = %pos;
			initialVelocity = "0 0 0";
		};
	}
	else
	{
		new Projectile()
		{
			datablock = TutorialBaseShakeProj;
			initialPosition = %pos;
			initialVelocity = "0 0 0";
		};
	}
}

datablock ExplosionData(TutorialBaseShake)
{
	cameraShakeFalloff = 1;
	camShakeAmp = "1.5 1.5 1.5";
	camShakeDuration = 4;
	camShakeFalloff = 20;
	camShakeFreq = "10 11 10";
	camShakeRadius = 100;
	explosionSphere = "data/shapes/empty.dts";
	shakeCamera = 1;
};

datablock ExplosionData(TutorialBaseShakeHeavy : TutorialBaseShake)
{
	camShakeAmp = "3 3 3";
	camShakeDuration = 8;
	camShakeFalloff = 20;
};

datablock ProjectileData(TutorialBaseShakeProj : HammerProjectile)
{
	lifetimeMS = 0;
	explodeOnDeath = 1;
	explosion = TutorialBaseShake;
};

datablock ProjectileData(TutorialBaseShakeHeavyProj : TutorialBaseShakeProj)
{
	lifetimeMS = 0;
	explodeOnDeath = 1;
	explosion = TutorialBaseShakeHeavy;
};

function StargateStartTutorial(%cl, %mode)
{
	if(!%mode)
	{
		if(isObject(TutorialBot))
			TutorialBot.kill();
		%bot = new AIPlayer(TutorialBot)
		{
			client = %cl;
			datablock = PlayerNoJet;
			position = "3 -75.25 1.5";
		};
		%bot.setAimVector("0 0 1");
		%bot.player = %bot;
		servercmdSit(%bot);
		schedule(2000, 0, "StargateStartTutorial", %cl, 1);
	}
	else if(isObject(%bot = TutorialBot) && %bot.client == %cl)
	{
		%cl.setControlObject(%bot);
		if(isObject(%pl = %cl.player))
		{
			if(isObject(%temp = %pl.tempbrick))
				%temp.delete();
			%pl.delete();
		}
		%cl.player = %bot;
		%cl.lastUpdateBodyColorsTime = 0;
		%cl.lastUpdateBodyPartsTime = 0;
		%cl.applyBodyColors();
		%cl.applyBodyParts();
		%bot.ClearTools();
		%bot.GiveDefaultEquipment();
		StargateTutorial(%bot, 0);
	}
}

function StargateTutorial(%obj, %stage, %last, %arg0, %arg1, %arg2)
{
	if(!isObject(%obj)) return;
	cancel($TutorialSched);
	%time = vectorDist(%last, %now = getSimTime()) / 1000;
	if(%stage == 0)
	{
		for(%i=0;%i<3;%i++) TutorialResetDamage(%i);
		$TutorialSched = schedule(3000, 0, "StargateTutorial", %obj, 1);
	}
	else if(%stage == 1)
	{
		TutorialSetLights(0, "");
		schedule(1000, 0, "TutorialSetLights", 0, "Eksi (AMB) 1/2 - White spectrum");
		TutorialShake(vectorAdd(%obj.getHackPosition(), "0 0 10"));
		$TutorialSched = schedule(10000, 0, "StargateTutorial", %obj, 2);
	}
	else if(%stage == 2)
	{
		TutorialDamage(0, "0 0 -2");
		TutorialSetLights(0, "");
		schedule(1000, 0, "TutorialSetLights", 0, "Eksi (AMB) 1/2 - White spectrum");
		TutorialShake(vectorAdd(%obj.getHackPosition(), "0 0 10"));
		$TutorialSched = schedule(10000, 0, "StargateTutorial", %obj, 3);
	}
	else if(%stage == 3)
	{
		TutorialDamage(1, "3 10 0");
		TutorialDamage(2, "0 0 -2");
		TutorialDamage(3, "0 0 -7");
		TutorialSetLights(0, "");
		schedule(1000, 0, "TutorialSetLights", 0, "Eksi (AMB) 1/2 - White spectrum");
		TutorialShake(vectorAdd(%obj.getHackPosition(), "0 0 10"));
		$TutorialSched = schedule(16, 0, "StargateTutorial", %obj, 4, %now, 8 + getRandom(0, 12) / 2);
	}
	else if(%stage >= 4)
	{
		if(%stage == 4)
		{
			%pos = %obj.getHackPosition();
			if(getWord(%pos, 1) >= -69)
			{
				talk("Stage incremented");
				%stage++;
			}
		}
		if(%time <= %arg0)
			$TutorialSched = schedule(16, 0, "StargateTutorial", %obj, %stage, %now, %arg0 - %time);
		else
		{
			TutorialSetLights(0, "");
			schedule(1000, 0, "TutorialSetLights", 0, "Eksi (AMB) 1/2 - White spectrum");
			TutorialShake(vectorAdd(%obj.getHackPosition(), "0 0 10"));
			$TutorialSched = schedule(16, 0, "StargateTutorial", %obj, %stage, %now, 8 + getRandom(0, 12) / 2);
		}
	}
}