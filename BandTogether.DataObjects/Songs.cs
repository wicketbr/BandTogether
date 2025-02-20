using System.Text.Json.Serialization;

namespace BandTogether;

public class selectedSong
{
    public Guid songId { get; set; }
    public Guid songBookId { get; set; }
}

public class song
{
    public Guid id { get; set; }
    public Guid? songBookId { get; set; }
    public string? title { get; set; } = "";
    public string? artist { get; set; } = "";
    public string? key { get; set; } = "";
    public string? tempo { get; set; } = "";
    public string? timeSignature { get; set; } = "";
    public string? copyright { get; set; } = "";
    public DateTime? created { get; set; }
    public DateTime? updated { get; set; }
    public List<songPart>? parts { get; set; }
    public string? content { get; set; } = "";
    public bool? saveRequired { get; set; }
    public string? ccliNumber { get; set; }
}

public class songBook
{
    public Guid id { get; set; }
    public string name { get; set; } = "";
    public string? fileName { get; set; }
    public DateTime? created { get; set; }
    public DateTime? updated { get; set; }
    public List<song> songs { get; set; } = new List<song>();
    public bool? saveRequired { get; set; }
    public viewStyle? style { get; set; }
}

public class songKey
{
    public string key { get; set; } = "";
    public string label { get; set; } = "";
}

public class songLine
{
	public int index { get; set; }
	public string? text { get; set; }
}

public class songPart
{
    public string content { get; set; } = "";
    public string label { get; set; } = "";
    public int index { get; set; }
    public int startLine { get; set; }
	public int endLine { get; set; }
    public bool partOfPrevious { get; set; }
}

public class songPreferences
{
    public Guid? songBookId { get; set; }
    public Guid songId { get; set; }
    public Guid userId { get; set; }
    public string title { get; set; } = "";
    public int capo { get; set; }
}