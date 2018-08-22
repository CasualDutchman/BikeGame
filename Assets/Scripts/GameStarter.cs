using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

	void Start () {
        LoadGame();
        Destroy(gameObject);
    }

    void LoadGame() {
        if (!string.IsNullOrEmpty(GlobalSettings.NewGameName)) {
            print(GlobalSettings.NewGameName);
            GameAsset ga = Resources.Load<GameAsset>("Games/" + GlobalSettings.NewGameName);
            if(ga.gameObject != null) {
                GameObject go = Instantiate(ga.gameObject);
                if (ga.isGame) {
                    go.GetComponentInChildren<Game>().gameAsset = ga;
                }
            }
        }
    }
}
