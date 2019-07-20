function fxDtsBrick::getFarthestPoint(%brick, %pos)   //45 탎 per call (average)
{
	%data = %brick.getDatablock();
	if(%brick.angleID & 1)
	{
		%size0 = %data.brickSizeY / 4;
		%size1 = %data.brickSizeX / 4;
	}
	else
	{
		%size0 = %data.brickSizeX / 4;
		%size1 = %data.brickSizeY / 4;
	}
	%size2 = %data.brickSizeZ / 10;
	%farthest = "0 0 0";
	%rel = vectorSub(%brick.getPosition(), %pos);   //Relative position.
	for(%i=0;%i<3;%i++) %farthest = setWord(%farthest, %i, mAbs(getWord(%rel, %i)) + %size[%i]);
	return %farthest;
}

function fxDtsBrick::getNearestPoint(%brick, %pos)   //52 탎 per call (average)
{
	%data = %brick.getDatablock();
	if(%brick.angleID & 1)
	{
		%size0 = %data.brickSizeY / 4;
		%size1 = %data.brickSizeX / 4;
	}
	else
	{
		%size0 = %data.brickSizeX / 4;
		%size1 = %data.brickSizeY / 4;
	}
	%size2 = %data.brickSizeZ / 10;
	%nearest = "0 0 0";
	%rel = vectorSub(%brick.getPosition(), %pos);   //Relative position.
	for(%i=0;%i<3;%i++) %nearest = setWord(%nearest, %i, getMax(mAbs(getWord(%rel, %i)) - %size[%i], 0));
	return %nearest;
}

function fxDtsBrick::getMaxDist(%brick, %pos)   //52 탎 per call (average)
{ return vectorLen(%brick.getFarthestPoint(%pos)); }

function fxDtsBrick::getMinDist(%brick, %pos)   //60 탎 per call (average)
{ return vectorLen(%brick.getNearestPoint(%pos)); }