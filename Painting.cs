function SG_PrecomputePaintColors()
{
	for(%i=0;%i<64;%i++)
	{
		%color = getColorIDTable(%i);
		%colR = mFloatLength(getWord(%color, 0) * 255, 0);
		%colG = mFloatLength(getWord(%color, 1) * 255, 0);
		%colB = mFloatLength(getWord(%color, 2) * 255, 0);
		
		%paintW = getMin(%colR, getMin(%colG, %colB));
		%colR -= %paintW; %colG -= %paintW; %colB -= %paintW;
		%paintY = getMin(%colR, %colG); %colR -= %paintY; %colG -= %paintY;
		%paintC = getMin(%colG, %colB); %colG -= %paintC; %colB -= %paintC;
		%paintM = getMin(%colR, %colB); %colR -= %paintM; %colB -= %paintM;
		%paintB = 255 - (%paintW + %paintY + %paintC + %paintM + %colR + %colG + %colB);
		
		$SG_PaintAmt[%i] = %colR SPC %paintY SPC %colG SPC %paintC
			SPC %colB SPC %paintM SPC %paintW SPC %paintB;
	}
}