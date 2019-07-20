//Black magic created by Port.

function createRandContext(%seed)
{
	if(%seed $= "")
	{
		%seed = getRealTime();
	}
	$_RNGState[$_RNGCount++] = %seed;
	$_RNGSeed[$_RNGCount] = %seed;
	return $_RNGCount;
}

function rand(%id, %min, %max, %precision)
{
	%a = 7 * 47 + 1;
	%c = 100;
	%m = 48 * 48 - 1;
	%result = ($_RNGState[%id] = LCG($_RNGState[%id], %a, %c, %m)) / %m;
	if(%max $= "")
	{
		if(%min !$= "")
		{
			%result = %result * %min;
		}
	}
	else
	{
		%result = %min + %result * (%max - %min);
	}
	if(%precision !$= "")
	{
		%result = mFloatLength(%result, %precision);
	}
	return %result;
}

function getRandSeed(%id)
{
	return $_RNGSeed[%id];
}

function setRandSeed(%id, %seed)
{
	$_RNGState[%id] = $_RNGSeed[%id] = %seed;
}

function LCG(%state, %a, %c, %m)
{
	return(%a * %state + %c) % %m;
}