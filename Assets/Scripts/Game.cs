using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameScreen { None, Explain, Profile, Game, End }

[RequireComponent(typeof(BikeManager))]
public class Game : MonoBehaviour {

    public static float TOO_SLOW_SPEED = 30;// below 30 is too slow
    public static float SLOW_SPEED = 40;    // between 30 - 40 is slow
    public static float GOOD_SPEED = 50;    // between 40 - 50 is good
    public static float FAST_SPEED = 60;    // bewteen 50 - 60 is fast
                                            // above 60 is too fast

    GameScreen gameScreen = GameScreen.None;

    BikeCanvas bc;

    Profile currentProfile;

    public GameAsset gameAsset;

    BikeManager bikeManager;

    protected BikeSpeed bikespeed;
    float bikeTimer = 0;

    RectTransform speedGaugeImage;
    Text speedText;

    float startTimer;
    bool canPlay = false;

    float gameTimer;

	void Start () {
        bikeManager = GetComponent<BikeManager>();

        SetupCanvas();

        UpdateProfile(ProfileManager.instance.GetCurrentProfile());

        Setup();

        gameScreen = GameScreen.Explain;
    }
	
    void UpdateProfile(Profile p) {
        currentProfile = p;

        bc.profileName.text = p.profileName;
        bc.profileResistance.text = currentProfile.resistance.ToString();
        bc.profileDifficultyLevel.text = ProfileManagerUI.difficultNames[currentProfile.gameDifficulty];
        bc.profileDefaultSpeed.text = currentProfile.targetSpeed.ToString();
        bc.profileTime.text = currentProfile.time.ToString() + " minuten";
    }

    void SetupCanvas() {
        GameObject canvas = Instantiate(Resources.Load<GameObject>("Canvas"));
        bc = canvas.GetComponent<BikeCanvas>();
        speedGaugeImage = bc.speedGaugeImage;
        speedText = bc.speedText;

        if (gameAsset != null) {
            bc.explainTitle.text = "Uitleg " + gameAsset.gameName;
            bc.explainText.text = gameAsset.gameExplain;

            for (int i = 0; i < gameAsset.explainButtons.Length; i++) {
                GameObject go = Instantiate(Resources.Load<GameObject>("Explain"));
                go.transform.GetChild(0).GetComponent<Image>().sprite = gameAsset.explainButtons[i].sprite;
                go.transform.GetChild(1).GetComponent<Text>().text = gameAsset.explainButtons[i].expliainText;
                go.transform.SetParent(bc.explainButtonsParent);
            }
        }

        bc.gameTime.text = "0:00";
        bc.gameTimer.fillAmount = 0;
    }

	void Update () {
        if(gameScreen == GameScreen.Explain) {
            if (bikespeed == BikeSpeed.GOOD) {
                startTimer += Time.deltaTime;
                if (startTimer >= 3) {
                    canPlay = true;
                    bc.playButton.SetActive(true);
                }
            }else {
                canPlay = false;
                bc.playButton.SetActive(false);

                startTimer -= Time.deltaTime * 0.5f;
            }

            startTimer = Mathf.Clamp(startTimer, 0f, 3f);
            bc.bikeTimer.fillAmount = startTimer / 3f;

            if (canPlay && ControllerInput.PressButtonDown()) {
                ChangeScreen(GameScreen.Game);
            }
            if (ControllerInput.PressButtonLeft()) {
                ChangeScreen(GameScreen.Profile);
            }
            if (ControllerInput.PressButtonRight()) {
                ControllerInput.PressMenu();
            }
        } 
        else if (gameScreen == GameScreen.Profile) {
            if (ControllerInput.PressArrowUp()) {
                bc.scrollSection.GoUp();
                UpdateProfileSelection();
            }
            if (ControllerInput.PressArrowDown()) {
                bc.scrollSection.GoDown();
                UpdateProfileSelection();
            }

            if (ControllerInput.PressButtonRight()) {
                ChangeScreen(GameScreen.Explain);
                startTimer = 0;
            }
            if (ControllerInput.PressButtonDown()) {
                ProfileManager.instance.currentProfileInUse = bc.scrollSection.GetSelected();
                UpdateProfile(ProfileManager.instance.GetCurrentProfile());

                ChangeScreen(GameScreen.Explain);
                startTimer = 0;
            }
        }
        else if(gameScreen == GameScreen.Game){
            OnUpdate();

            gameTimer += Time.deltaTime;

            int min = Mathf.FloorToInt(gameTimer / 60f);
            int sec = (int)gameTimer % 60;

            bc.gameTime.text = min + ":" + ((sec < 10) ? "0" + sec : sec.ToString());
            bc.gameTimer.fillAmount = gameTimer / (currentProfile.time * 60);

            if (gameTimer == currentProfile.time * 60) {
                EndGame();
            }
        }

        ControllerInput.ResetToggles();
    }

    protected void EndGame() {

    }

    void UpdateProfileSelection() {
        Profile p = ProfileManager.instance.profiles[bc.scrollSection.GetSelected()];
        bc.showProfileName.text = p.profileName;
        bc.showProfileResistance.text = p.resistance.ToString();
        bc.showProfileDifficultyLevel.text = ProfileManagerUI.difficultNames[p.gameDifficulty];
        bc.showProfileDefaultSpeed.text = p.targetSpeed.ToString();
        bc.showProfileTime.text = p.time.ToString() + " minuten";
    }

    void ChangeScreen(GameScreen screen) {
        gameScreen = screen;
        bc.explainScreen.SetActive(screen == GameScreen.Explain);
        bc.changeProfile.SetActive(screen == GameScreen.Profile);

        if (screen == GameScreen.Profile) {
            bc.scrollSection.Create(bc.profilePrefab, ProfileManager.instance.profiles.ToArray(), Method);
            UpdateProfileSelection();
        }
    }

    public bool Method(GameObject go, object game) {
        go.transform.GetChild(0).GetComponent<Text>().text = ((Profile)game).profileName;
        return true;
    }

    void FixedUpdate() {
        UpdateBikeSpeed();

        if (gameScreen == GameScreen.Game) {
            OnFixedUpdate();
        }
    }

    protected virtual void Setup() { }

    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }

    void UpdateBikeSpeed() {
        bikeTimer += Time.deltaTime;
        if (bikeTimer >= 0.1f) {
            bikeTimer = 0;
            bikespeed = GetBikeSpeed(bikeManager.bikeSpeed);
        }
        UpdateBikeUI();
    }

    void UpdateBikeUI() {
        if (bc != null) {
            speedText.text = bikeManager.bikeSpeed.ToString("F1");
            //float f = Mathf.Clamp((bikeManager.bikeSpeed - TOO_SLOW_SPEED) * 5.33333f, 0, 160);
            //speedGaugeImage.localEulerAngles = new Vector3(0, 0, -f);

            float nul = 0;

            if (bikeManager.bikeSpeed > currentProfile.targetSpeed - 15) {
                nul = bikeManager.bikeSpeed - (currentProfile.targetSpeed - 15);
                nul *= 0.0333f;
            }

            nul = Mathf.Clamp(nul, 0f, 1f);

            speedGaugeImage.localEulerAngles = new Vector3(0, 0, -(nul * 160));

            //targetspeed - 15 = 0;
            //targetspeed      = 80;
            //targetspeed + 15 = 160;
        }
    }

    BikeSpeed GetBikeSpeed(float f) {
        if (f < currentProfile.targetSpeed - 15) {
            return BikeSpeed.TOO_SLOW;
        } 
        else if (f < currentProfile.targetSpeed - 5) {
            return BikeSpeed.SLOW;
        } 
        else if (f < currentProfile.targetSpeed + 5 && f > currentProfile.targetSpeed - 5) {
            return BikeSpeed.GOOD;
        } 
        else if (f < currentProfile.targetSpeed + 15) {
            return BikeSpeed.FAST;
        } 
        else {
            return BikeSpeed.TOO_FAST;
        }
    }
}

public enum BikeSpeed { TOO_SLOW, SLOW, GOOD, FAST, TOO_FAST }
