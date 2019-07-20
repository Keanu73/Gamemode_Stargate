function groundTexToIDX(%groundTex)
{
	for(%i=0;%i<$EnvGuiServer::GroundCount;%i++)
		if($EnvGuiServer::Ground[%i] $= %groundTex)
			return %i;
	return -1;
}

function skyTexToIDX(%skyTex)
{
	for(%i=0;%i<$EnvGuiServer::SkyCount;%i++)
		if($EnvGuiServer::Sky[%i] $= %skyTex)
			return %i;
	return -1;
}