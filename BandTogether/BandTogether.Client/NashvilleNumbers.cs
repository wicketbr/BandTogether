namespace BandTogether;

public static class NashvilleNumbering
{
    private static string _key = "";
    private static string _keyType = "major";

    public static void Init(string key = "", string keyType = "major")
    {
        _key = FixKey(key);

        SetKeyType(keyType);
    }

    public static string Key {
        get {
            return _key;
        }
    }

    public static void SetKey(string key)
    {
        _key = FixKey(key);
    }

    public static void SetKeyType(string keyType = "major")
    {
        if (!String.IsNullOrWhiteSpace(keyType)) {
            if (keyType.ToLower() == "minor") {
                _keyType = "minor";
            } else {
                _keyType = "major";
            }
        } else {
            _keyType = "major";
        }
    }

    public static string ConvertChordToNashvilleNumber(string chord, string? key = "")
    {
        if (!String.IsNullOrWhiteSpace(key)) {
            SetKey(key);
        }

        string output = String.Empty;

        if (chord.Contains("/")) {
            var parts = chord.Split("/");

            foreach(var part in parts) {
                if (output != String.Empty) {
                    output += "/";
                }
                output += ConvertChordToNashvilleNumber(part, key);
            }

            return output;
        }

        var chordMatrix = KeyMatrixItemsForNashvilleNumbering[_key];

        var chordRoot = FixKey(chord);
        var remainder = chord.Substring(chordRoot.Length);

        int chordPosition = -1;
        foreach (var item in chordMatrix) {
            chordPosition++;

            if (item == chordRoot) {
                break;
            }
        }

        bool useFlats = chordRoot.Contains("♭") || chordRoot == "F";
        var nnItems = new List<string>();
        if (useFlats) {
            nnItems = new List<string> { "1", "♭2", "2", "♭3", "3", "4", "♭5", "5", "♭6", "6", "♭7", "7" };
        } else {
            nnItems = new List<string> { "1", "♯1", "2", "♯2", "3", "4", "♯4", "5", "♯5", "6", "♯6", "7" };
        }

        output = nnItems[chordPosition] + remainder;

        return output;
    }

    private static string FixKey(string? key)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(key)) {
            if (key.Length == 1) {
                return key;
            }

            string test1 = key.ToLower();
            string test2 = test1.Substring(0, 2);

            switch (test2) {
                case "ab":
                case "a♭":
                    output = "A♭";
                    break;
                case "am":
                    output = "A";
                    break;
                case "a#":
                case "a♯":
                    output = "A♯";
                    break;
                case "bb":
                case "b♭":
                    output = "B♭";
                    break;
                case "bm":
                    output = "B";
                    break;
                case "cb":
                case "c♭":
                    output = "C♭";
                    break;
                case "cm":
                    output = "C";
                    break;
                case "c#":
                case "c♯":
                    output = "C♯";
                    break;
                case "db":
                case "d♭":
                    output = "D♭";
                    break;
                case "dm":
                    output = "D";
                    break;
                case "d#":
                case "d♯":
                    output = "D♯";
                    break;
                case "eb":
                case "e♭":
                    output = "E♭";
                    break;
                case "em":
                    output = "E";
                    break;
                case "e#":
                case "e♯":
                    output = "E♯";
                    break;
                case "fb":
                case "f♭":
                    output = "F♭";
                    break;
                case "fm":
                    output = "F";
                    break;
                case "f#":
                case "f♯":
                    output = "F♯";
                    break;
                case "gb":
                case "g♭":
                    output = "G♭";
                    break;
                case "gm":
                    output = "G";
                    break;
                case "g#":
                case "g♯":
                    output = "G♯";
                    break;
            }

            if (output == "") {
                output = key.Substring(0, 1);
            }
        }

        return output;
    }

    public static string FormatChord(string chord, string? key = "", bool bassPart = false)
    {
        if (!String.IsNullOrWhiteSpace(key)) {
            SetKey(key);
        }

        string output = String.Empty;

        if (chord.Contains("/")) {
            var parts = chord.Split('/');
            return FormatChord(parts[0], key, false) + "/" + FormatChord(parts[1], key, true);
        }

        int offset = 0;

        string firstCharacter = chord.Substring(0, 1);
        string remainder = chord.Length > 1 ? chord.Substring(1) : String.Empty;

        if (firstCharacter == "♭" || firstCharacter == "b") {
            offset = -1;
        } else if (firstCharacter == "♯" || firstCharacter == "#") {
            offset = 1;
        }

        if (offset != 0) {
            firstCharacter = chord.Substring(1, 1);
            remainder = chord.Length > 2 ? chord.Substring(2) : String.Empty;
        }

        int position = 0;
        try {
            position = Convert.ToInt32(firstCharacter);
        } catch { }

        if (position > 0) {
            if (!String.IsNullOrWhiteSpace(_key)) {
                var chords = KeyMatrixItemsForNashvilleNumbering[_key];
                if (chords != null && chords.Any()) {
                    //position.Dump("Position");

                    //chords.Dump("chords");

                    int keyChordPosition = KeyChordPositions()[position - 1];
                    //keyChordPosition.Dump("keyChordPosition");

                    if (offset != 0) {
                        keyChordPosition += offset;

                        if (keyChordPosition < 0) {
                            keyChordPosition = 12 + keyChordPosition;
                        } else if (keyChordPosition > 11) {
                            keyChordPosition = 12 - keyChordPosition;
                        }
                    }

                    var chordForPosition = chords[keyChordPosition];

                    output = chordForPosition;

                    if (!bassPart) {
                        // Only add the minor character if the first character of the remainder is not an "m" or a "-" character.
                        string firstRemainingCharacter = !String.IsNullOrWhiteSpace(remainder) ? remainder.Substring(0, 1) : String.Empty;
                        if (firstRemainingCharacter == "m") {
                            // Can just skip this.
                        } else if (firstRemainingCharacter == "-") {
                            // Replace this with an "m" character.
                            remainder = "m" + remainder.Substring(1);
                        } else {
                            if (_keyType.ToLower() == "minor") {
                                switch (position) {
                                    case 1:
                                    case 4:
                                    case 5:
                                        output += "m";
                                        break;
                                }
                            } else {
                                switch (position) {
                                    case 2:
                                    case 3:
                                    case 6:
                                        output += "m";
                                        break;
                                }
                            }
                        }
                    }

                    output += remainder;
                }
            } else {
                output = firstCharacter + remainder;
            }
        }

        if (String.IsNullOrWhiteSpace(output)) {
            output = chord;
        }

        return output;
    }

    public static Dictionary<string, List<string>> KeyMatrixItemsForNashvilleNumbering {
        get {
            var output = new Dictionary<string, List<string>>();

            output.Add("A", new List<string> { "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯" });
            output.Add("B♭", new List<string> { "B♭", "B", "C", "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A" });
            output.Add("B", new List<string> { "B", "C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯", "A", "A♯" });
            output.Add("C♭", new List<string> { "C♭", "C", "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A", "B♭" });
            output.Add("C", new List<string> { "C", "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A", "B♭", "B" });
            output.Add("C♯", new List<string> { "C♯", "D", "D♯", "E", "E♯", "F♯", "G", "G♯", "A", "A♯", "B", "C" });
            output.Add("D♭", new List<string> { "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A", "B♭", "B", "C" });
            output.Add("D", new List<string> { "D", "D♯", "E", "F", "F♯", "G", "G♯", "A", "A♯", "B", "C", "C♯" });
            output.Add("E♭", new List<string> { "E♭", "E", "F", "G♭", "G", "A♭", "A", "B♭", "B", "C", "D♭", "D" });
            output.Add("E", new List<string> { "E", "F", "F♯", "G", "G♯", "A", "A♯", "B", "C", "C♯", "D", "D♯" });
            output.Add("F", new List<string> { "F", "G♭", "G", "A♭", "A", "B♭", "B", "C", "D♭", "D", "E♭", "E" });
            output.Add("F♯", new List<string> { "F♯", "G", "G♯", "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F" });
            output.Add("G♭", new List<string> { "G♭", "G", "A♭", "A", "B♭", "C♭", "C", "D♭", "D", "E♭", "E", "F" });
            output.Add("G", new List<string> { "G", "G♯", "A", "A♯", "B", "C", "C♯", "D", "D♯", "E", "F", "F♯" });
            output.Add("A♭", new List<string> { "A♭", "A", "B♭", "B", "C", "D♭", "D", "E♭", "E", "F", "G♭", "G" });

            return output;
        }
    }

    public static List<int> KeyChordPositions()
    {
        List<int> output = new List<int>();

        if (_keyType.ToLower() == "minor") {
            output = new List<int> { 0, 2, 3, 5, 7, 8, 10, 12 };
        } else {
            output = new List<int> { 0, 2, 4, 5, 7, 9, 11, 12 };
        }

        return output;
    }
}