using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsExtended
{
    public static void SetDouble(string key, double data)
    {
        PlayerPrefs.SetString(key, data.ToString());
    }

    public static double GetDouble(string key, double defaultValue = 0)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;
        else
        {
            var temp = PlayerPrefs.GetString(key);
            return double.Parse(temp);
        }
    }
}
