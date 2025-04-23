namespace BandTogether;

/// <summary>
/// This static class is used to persist a few settings about the startup state of the application.
/// </summary>
public static class GlobalSettings
{
    public static messages CachedMessages { get; set; } = new messages();
    public static setList CachedSetList { get; set; } = new setList();
    public static bool StartupComplete { get; set; }
}