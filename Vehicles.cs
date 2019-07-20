return;

datablock WheeledVehicleData(MALPVehicle : JeepVehicle)
{
	rideable = 0;
	uiName = "M.A.L.P.";
};

// Unmanned Aerial Vehicle Vehicle? No thanks.
datablock WheeledVehicleData(UAVehicle : StuntPlaneVehicle)
{
	rideable = 0;
	uiName = "U.A.V.";
};

package SGVehicles
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
		if(isObject(%obj = firstWord(%ray)) && (%obj.getType() & $Typemasks::VehicleObjectType))
		{
			if(%obj.getDatablock().getID() == MALPVehicle.getID())
			{
				if(isObject(%cl = %pl.getControllingClient()))
					%cl.schedule(0, "setControlObject", %obj);
				return;
			}
		}
		Parent::activateStuff(%pl);
	}
	
	function MALPVehicle::onAdd(%data, %this)
	{
		Parent::onAdd(%data, %this);
		for(%i=0;%i<4;%i++)
		{
			%this.setWheelSpring(%i, JeepSpring);
			%this.setWheelTire(%i, JeepTire);
		}
		%this.setScale("0.5 0.5 0.5");
	}
	
	function UAVehicle::onAdd(%data, %this)
	{
		Parent::onAdd(%data, %this);
		%this.setScale("0.5 0.5 0.5");
	}
	
	function MALPVehicle::onTrigger(%data, %this, %trig, %tog)
	{
		if(%trig != 2 && %tog && isObject(%cl = %this.getControllingClient()) && isObject(%pl = %cl.player))
			%cl.setControlObject(%pl);
	}
};
activatePackage("SGVehicles");