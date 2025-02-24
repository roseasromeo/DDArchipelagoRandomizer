namespace ArchipelagoRandomizer;

static class Debug
{
	public static void Log(string message)
	{
		Plugin.Logger.LogInfo(message);
	}

	public static void LogWarning(string message)
	{
		Plugin.Logger.LogWarning(message);
	}

	public static void LogError(string message)
	{
		Plugin.Logger.LogError(message);
	}
}