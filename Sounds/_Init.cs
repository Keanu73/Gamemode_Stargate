datablock AudioDescription(AudioLoudClose3D : AudioClose3D)
{
	referenceDistance = 10;
	maxDistance = 150;
	coneVector = 3;
};

datablock AudioDescription(AudioLoudCloseLooping3D : AudioLoudClose3D)
{ isLooping = 1; };

datablock AudioProfile(ChevronLocked_Sound)
{
	filename = "./ChevronLocked.wav";
	description = AudioLoudClose3D;
	preload = false;
};

datablock AudioProfile(DialFail_Sound : ChevronLocked_Sound){filename = "./DialFail.wav";};

datablock AudioProfile(DHDButton1_Sound : DialFail_Sound){filename = "./DHDButton1.wav";};
datablock AudioProfile(DHDButton2_Sound : DialFail_Sound){filename = "./DHDButton2.wav";};
datablock AudioProfile(DHDButton3_Sound : DialFail_Sound){filename = "./DHDButton3.wav";};
datablock AudioProfile(DHDButton4_Sound : DialFail_Sound){filename = "./DHDButton4.wav";};
datablock AudioProfile(DHDButton5_Sound : DialFail_Sound){filename = "./DHDButton5.wav";};
datablock AudioProfile(DHDButton6_Sound : DialFail_Sound){filename = "./DHDButton6.wav";};
datablock AudioProfile(DHDButton7_Sound : DialFail_Sound){filename = "./DHDButton7.wav";};

datablock AudioProfile(EventHorizon_Sound : DialFail_Sound)
{
	filename = "./EventHorizon.wav";
	description = AudioLoudCloseLooping3D;
};

datablock AudioProfile(GateRoll_Sound : DialFail_Sound){filename = "./GateRoll.wav";};
datablock AudioProfile(GateRollLoop_Sound : EventHorizon_Sound){filename = "./GateRollLoop.wav";};

datablock AudioProfile(IrisClose_Sound : DialFail_Sound){filename = "./IrisClose.wav";};
datablock AudioProfile(IrisImpact_Sound : DialFail_Sound){filename = "./IrisImpact.wav";};
datablock AudioProfile(IrisOpen_Sound : DialFail_Sound){filename = "./IrisOpen.wav";};

datablock AudioProfile(ObjectEntrance_Sound : DialFail_Sound){filename = "./ObjectEntrance.wav";};

datablock AudioProfile(WormholeOpened_Sound : DialFail_Sound){filename = "./WormholeOpened.wav";};
datablock AudioProfile(WormholeClosed_Sound : DialFail_Sound){filename = "./WormholeClosed.wav";};