using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameAsset", menuName = "Game/GameAsset", order = 1)]
public class GameAsset : ScriptableObject {

    //menu
    public Sprite menuSprite;
    public string gameName;
    [TextArea]
    public string gameDesc;
    [TextArea]
    public string gameExplain;

    public ExplainButton[] explainButtons;

    public int order = 0;
    public bool isGame = true;

    //game
    public GameObject gameObject;
}
