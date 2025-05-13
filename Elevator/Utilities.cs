
public static class Utilities
{
    public static string FormatFloorName(int internalFloor)
    {
        if (internalFloor < 0)
        {
            return $"Floor B{Math.Abs(internalFloor)}";
        }
        else
        {
            return $"Floor {internalFloor + 1}";
        }
    }

    public static bool TryParseFloorInput(string input, int basementsCount, int floorsCount, out int internalFloor)
    {
        internalFloor = 0;

        if (input.StartsWith("B", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(input[1..], out int basementLevel))
        {
            if (basementLevel >= 1 && basementLevel <= basementsCount)
            {
                internalFloor = -basementLevel;
                return true;
            }
            return false;
        }

        if (int.TryParse(input, out int regularFloor))
        {
            if (regularFloor >= 1 && regularFloor <= floorsCount)
            {
                internalFloor = regularFloor - 1;
                return true;
            }
        }

        return false;
    }
}
