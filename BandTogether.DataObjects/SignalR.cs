namespace BandTogether;

public enum signalRUpdateType
{
    admin,
    blankScreen,
    hideText,
    setList,
    song,
    songBook,
    unblankScreen,
    unhideText,
    videoPlayerPlabackTime,
    videoPlayerState,
    youTubePlaybackTime,
    youTubePlayState,
}

public class signalRUpdate
{
    public Guid? itemId { get; set; }
    public signalRUpdateType updateType { get; set; }
    public string message { get; set; } = "";
    public object? obj { get; set; }
    public string? objectAsString { get; set; }
}