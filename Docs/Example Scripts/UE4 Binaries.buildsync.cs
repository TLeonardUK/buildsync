public class LaunchEditor: ScriptLaunchMode
{
	[DisplayName("Launch in game mode?")]
	[Description("Causes the editor to launch in game mode rather than the full editor.")]
	[Category("Launch Settings")]
	public bool RunInGameMode { get; set; } = false;
	
	public LaunchEditor()
	{
		Name = "Launch Editor";			
		IsAvailable = true;
	}

	public override bool Install(ScriptBuild Build)
	{
        string SourceBinariesDirectory = Path.Combine(Build.Directory, "Binaries");
        string TargetBinariesDirectory = Path.Combine(Build.InstallLocation, "Binaries");

		if (!ScriptUtils.CreateJunction(SourceBinariesDirectory, TargetBinariesDirectory))
		{
			return false;
		}
		
		return true;
	}

	public override bool Launch(ScriptBuild Build)
	{
        string BinariesDirectory = Path.Combine(Build.InstallLocation, "Binaries");
		string ExePath = Path.Combine(BinariesDirectory, @"UE4Editor.exe");

		string CommandLine = "";
        if (RunInGameMode) 
        {
            CommandLine += "-game";
        }

		return ScriptUtils.Run(ExePath, BinariesDirectory, CommandLine);
	}
}	