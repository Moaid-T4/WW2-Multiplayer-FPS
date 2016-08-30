using UnityEngine;

public static class Tools
{
    public static string RemoveSpaces(string text)
    {
        string cleanText = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != ' ')
                cleanText += text[i];
        }
        return cleanText;
    }

    public static string GetValue(string text)
    {
        string value = "";
        bool passedMarker = false;
        for (int i = 0; i < text.Length; i++)
        {
            if (passedMarker)
                value += text[i];
            if (text[i] == '=' && !passedMarker)
                passedMarker = true;
        }
        return value;
    }

    public static bool IntToBool(int value)
    {
        if (value == 1)
            return true;
        return false;
    }

    public static int BoolToInt(bool value)
    {
        if (value == true)
            return 1;
        return 0;
    }

    public static void KeyCodeToAxis(KeyCode positive, KeyCode negative, ref float axis, float axisDelta)
    {
        if (Input.GetKey(positive) || Input.GetKey(negative))
        {
            if (Input.GetKey(positive))
            {
                axis = Mathf.Clamp(axis, 0, 1);
                axis = Mathf.MoveTowards(axis, 1, axisDelta);
            }
            if (Input.GetKey(negative))
            {
                axis = Mathf.Clamp(axis, -1, 0);
                axis = Mathf.MoveTowards(axis, -1, axisDelta);
            }
        }
        else
            axis = Mathf.MoveTowards(axis, 0, axisDelta);
    }

    public static float GetRandom(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    public static float GetRandomRelativeDamage(Vector2 range, float penetration, float maxPenetration)
    {
        return Random.Range(range.x, range.y) * (penetration / maxPenetration);
    }

    public static void ReArrangeHits(ref RaycastHit[] allHits, bool ascending)
    {
        RaycastHit tempHit;
        bool switched = true;
        while (switched)
        {
            switched = false;
            for (int i = 0; i < allHits.Length; i++)
            {
                if (ascending && i < allHits.Length - 1 && allHits[i].distance > allHits[i + 1].distance)
                {
                    tempHit = allHits[i];

                    allHits[i] = allHits[i + 1];

                    allHits[i + 1] = tempHit;
                    switched = true;
                }

                if (!ascending && i < allHits.Length - 1 && allHits[i].distance < allHits[i + 1].distance)
                {
                    tempHit = allHits[i];

                    allHits[i] = allHits[i + 1];

                    allHits[i + 1] = tempHit;
                    switched = true;
                }
            }
        }
    }

    public static float FixAngle(float angle)
    {
        if (angle > 180 && angle < 360)
            return angle - 360;
        return angle;
    }
}