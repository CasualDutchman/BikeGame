using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Menu : MonoBehaviour {

    GameAsset[] gameAssets;
    List<Transform> menuItems = new List<Transform>();

    public GameObject menuItemPrefab;

    public ControllerScrollSection scrollSection;

    void Start () {
        if (Time.time < 1) {
            LoadFirstTime();
        }

        LoadAllGames();
    }

    void LoadFirstTime() {
        Cursor.visible = false;
    }

    void LoadAllGames() {
        GameAsset[] ga = Resources.LoadAll<GameAsset>("Games");
        gameAssets = new GameAsset[ga.Length];
        for (int i = 0; i < ga.Length; i++) {
            GameAsset game = ga[i];
            if (gameAssets[game.order] == null) {
                gameAssets[game.order] = game;
            }
        }

        scrollSection.Create(menuItemPrefab, gameAssets, Method);
    }

    public bool Method(GameObject go, object game) {
        go.transform.GetChild(0).GetComponent<Image>().sprite = ((GameAsset)game).menuSprite;
        go.transform.GetChild(1).GetComponent<Text>().text = ((GameAsset)game).gameName;
        go.transform.GetChild(2).GetComponent<Text>().text = ((GameAsset)game).gameDesc;
        return true;
    }

    void StartGame() {
        if (gameAssets[scrollSection.GetSelected()].gameObject == null)
            return;

        GlobalSettings.NewGameName = gameAssets[scrollSection.GetSelected()].name;
        SceneManager.LoadScene(1);
    }
	
	void Update () {
        if (ControllerInput.PressArrowUp()) {
            scrollSection.GoUp();
        }

        if (ControllerInput.PressArrowDown()) {
            scrollSection.GoDown();
        }

        if (ControllerInput.PressButtonDown()) {
            StartGame();
        }

        if (ControllerInput.PressButtonRight()) {
            if (Settings.instance.quitPC && !Debug.isDebugBuild) {
#if !UNITY_EDITOR
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 0");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);
#endif

            }else {
                Application.Quit();
            }
        }

        ControllerInput.ResetToggles();
    }
}
