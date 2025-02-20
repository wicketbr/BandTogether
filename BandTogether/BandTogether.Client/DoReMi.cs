namespace BandTogether;

public static class DoReMi
{
    public static string ConvertChordToNashvilleNumber(string chord, bool bassPart = false)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(chord)) {
            if (chord.Contains("/")) {
                var parts = chord.Split('/');
                return ConvertChordToNashvilleNumber(parts[0], false) + "/" + ConvertChordToNashvilleNumber(parts[1], true);
            }

            if (chord.Length == 1) {
                return chord;
            }

            int position = 0;
            int characters = 2;
            bool flat = false;
            bool sharp = false;

            string startingCharacters = (chord + "     ").Substring(0, 5).ToUpper();

            // Replace international items with English items.
            if (startingCharacters.Substring(0, 2) == "RÉ") {
                startingCharacters = "RE" + startingCharacters.Substring(2);
            } else if (startingCharacters.Substring(0, 2) == "SI") {
                startingCharacters = "TI" + startingCharacters.Substring(2);
            }

            if (startingCharacters.StartsWith("DO♭") || startingCharacters.StartsWith("DOB")) {
                characters = 3;
                position = 1;
                flat = true;
            } else if (startingCharacters.StartsWith("DO♯") || startingCharacters.StartsWith("DO#")) {
                characters = 3;
                position = 1;
                sharp = true;
            } else if (startingCharacters.StartsWith("DO")) {
                position = 1;
            } else if (startingCharacters.StartsWith("RE♭") || startingCharacters.StartsWith("REB")) {
                characters = 3;
                position = 2;
                flat = true;
            } else if (startingCharacters.StartsWith("RE♯") || startingCharacters.StartsWith("RE#")) {
                characters = 3;
                position = 2;
                sharp = true;
            } else if (startingCharacters.StartsWith("RE")) {
                position = 2;
            } else if (startingCharacters.StartsWith("MI♭") || startingCharacters.StartsWith("MIB")) {
                characters = 3;
                position = 3;
                flat = true;
            } else if (startingCharacters.StartsWith("MI♯") || startingCharacters.StartsWith("MI#")) {
                characters = 3;
                position = 3;
                sharp = true;
            } else if (startingCharacters.StartsWith("MI")) {
                position = 3;
            } else if (startingCharacters.StartsWith("FA♭") || startingCharacters.StartsWith("FAB")) {
                characters = 3;
                position = 4;
                flat = true;
            } else if (startingCharacters.StartsWith("FA♯") || startingCharacters.StartsWith("FA#")) {
                characters = 3;
                position = 4;
                sharp = true;
            } else if (startingCharacters.StartsWith("FA")) {
                position = 4;
            } else if (startingCharacters.StartsWith("SOL♭") || startingCharacters.StartsWith("SOLB")) {
                characters = 4;
                position = 5;
                flat = true;
            } else if (startingCharacters.StartsWith("SOL♯") || startingCharacters.StartsWith("SOL#")) {
                characters = 4;
                position = 5;
                sharp = true;
            } else if (startingCharacters.StartsWith("SOL")) {
                characters = 3;
                position = 5;
            } else if (startingCharacters.StartsWith("SO♭") || startingCharacters.StartsWith("SOB")) {
                characters = 3;
                position = 5;
                flat = true;
            } else if (startingCharacters.StartsWith("SO♯") || startingCharacters.StartsWith("SO#")) {
                characters = 3;
                position = 5;
                sharp = true;
            } else if (startingCharacters.StartsWith("SO")) {
                position = 5;
            } else if (startingCharacters.StartsWith("LA♭") || startingCharacters.StartsWith("LAB")) {
                characters = 3;
                position = 6;
                flat = true;
            } else if (startingCharacters.StartsWith("LA♯") || startingCharacters.StartsWith("LA#")) {
                characters = 3;
                position = 6;
                sharp = true;
            } else if (startingCharacters.StartsWith("LA")) {
                position = 6;
            } else if (startingCharacters.StartsWith("TI♭") || startingCharacters.StartsWith("TIB")) {
                characters = 3;
                position = 7;
                flat = true;
            } else if (startingCharacters.StartsWith("TI♯") || startingCharacters.StartsWith("TI#")) {
                characters = 3;
                position = 7;
                sharp = true;
            } else if (startingCharacters.StartsWith("TI")) {
                position = 7;
            }

            if (position > 0) {
                output = (flat ? "♭" : "") + (sharp ? "♯" : "") + position.ToString();

                string remainder = chord.Substring(characters).Trim();

                if (!bassPart) {
                    // Only add the minor character if the first character of the remainder is not an "m" or a "-" character.
                    string firstRemainingCharacter = !String.IsNullOrWhiteSpace(remainder) ? remainder.Substring(0, 1) : String.Empty;
                    if (firstRemainingCharacter == "m") {
                        // Can just skip this.
                    } else if (firstRemainingCharacter == "-") {
                        // Replace this with an "m" character.
                        remainder = "m" + remainder.Substring(1);
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

                output += remainder;
            }

        }

        if (String.IsNullOrWhiteSpace(output)) {
            output = chord;
        }

        return output;
    }
}