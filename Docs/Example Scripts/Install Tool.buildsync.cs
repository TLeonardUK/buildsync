public class InstallTool : ScriptLaunchMode
{
	private string ExeName = "SDK_Manager.exe";
	
	public InstallTool()
	{
		Name = "Open Tool";			
		IsAvailable = true;
	}
	
	public override bool Install(ScriptBuild Build)
	{
		return true;
	}
	
	public override bool Launch(ScriptBuild Build)
	{
        string ExePath = Path.Combine(Build.Directory, ExeName);
		return ScriptUtils.RunAndWait(ExePath, Build.Directory, "") == 0;
	}
}	