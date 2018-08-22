using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings {

    private static string newGameName;
    public static string NewGameName { get { return newGameName; } set { newGameName = value; } }

    //settings
    private static Vector2 resolution;
    public static Vector2 Resolution { get { return resolution; } set { resolution = value; } }
}
