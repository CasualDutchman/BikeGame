using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameScreen { None, Explain, Profile, Game, End }

public class Game : MonoBehaviour {

    public static float TOO_SLOW_SPEED = 30;// below 30 is too slow
    public static float SLOW_SPEED = 40;    // between 30 - 40 is slow
    public static float GOOD_SPEED = 50;    // between 40 - 50 is good
    public static float FAST_SPEED = 60;    // bewteen 50 - 60 is fast
                                            // above 60 is too fast
    public bool debugGame = false;

    GameScreen gameScreen = GameScreen.None;

    BikeCanvas bc;

    protected Profile currentProfile;

    public GameAsset gameAsset;

    BikeManager bikeManager;

    protected BikeSpeed bikeSpeed;
    float bikeTimer = 0;

    RectTransform speedGaugeImage;
    Text speedText;

    float startTimer;
    bool canPlay = false;

    float gameTimer;

    string[] speedExplainText = new string[] { "Te traag!", "Te snel!" };
    string stopText = "Stoppen in {0}";
    float tooFastSlowTimer;
    float maxTooSlowTime = 15f;

	void Start () {
        Cursor.visible = false;

        if (!debugGame) {
            bikeManager = BikeManager.instance;

            SetupCanvas();

            UpdateProfile(ProfileManager.instance.GetCurrentProfile());
        }

        Setup();

        if (!debugGame) {
            ChangeScreen(GameScreen.Explain);
        }else {
            bikeSpeed = BikeSpeed.GOOD;
            gameScreen = GameScreen.Game;
            OnPlay();
        }
    }
	
    void UpdateProfile(Profile p) {
        currentProfile = p;

        bc.profileName.text = p.profileName;
        bc.profileResistance.text = currentProfile.resistance.ToString();
        bc.profileDifficultyLevel.text = ProfileManagerUI.difficultNames[currentProfile.gameDifficulty];
        bc.profileDefaultSpeed.text = currentProfile.targetSpeed.ToString();
        bc.profileTime.text = currentProfile.time.ToString() + " minuten";

        OnChangeProfile();
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
            if (bikeSpeed == BikeSpeed.GOOD) {
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
        else if(gameScreen == GameScreen.Game) {
            OnUpdate();

            if (!debugGame) {
                if (bikeSpeed == BikeSpeed.TOO_FAST || bikeSpeed == BikeSpeed.TOO_SLOW) {
                    if (!bc.tooSlowObject.activeSelf) {
                        bc.tooSlowObject.SetActive(true);
                    }

                    tooFastSlowTimer += Time.deltaTime;

                    Color col = bc.exclamationMark.color;
                    col.a = tooFastSlowTimer - Mathf.FloorToInt(tooFastSlowTimer);
                    bc.exclamationMark.color = col;

                    if (bikeSpeed == BikeSpeed.TOO_SLOW) {
                        bc.speedExplainText.text = speedExplainText[0];
                    } else {
                        bc.speedExplainText.text = speedExplainText[1];
                    }

                    bc.stopText.text = string.Format(stopText, (maxTooSlowTime - tooFastSlowTimer).ToString("F0"));

                    if (tooFastSlowTimer >= maxTooSlowTime) {
                        SceneManager.LoadScene(0);
                    }
                } else {
                    if (bc.tooSlowObject.activeSelf) {
                        tooFastSlowTimer = 0;
                        bc.tooSlowObject.SetActive(false);
                    }
                }

                gameTimer += Time.deltaTime;

                int min = Mathf.FloorToInt(gameTimer / 60f);
                int sec = (int)gameTimer % 60;

                bc.gameTime.text = min + ":" + ((sec < 10) ? "0" + sec : sec.ToString());
                bc.gameTimer.fillAmount = gameTimer / (currentProfile.time * 60);

                if (gameTimer >= currentProfile.time * 60) {
                    EndGame();
                }
            }
        } else if (gameScreen == GameScreen.End) {
            if (ControllerInput.PressButtonLeft()) {
                SceneManager.LoadScene(1);
            }
            if (ControllerInput.PressButtonDown()) {
                ChangeScreen(GameScreen.Game);
                gameTimer = 0;
            }

            ControllerInput.PressMenu();
        }

        ControllerInput.ResetToggles();
    }

    protected void EndGame() {
        ChangeScreen(GameScreen.End);

        bc.gameTime.text = currentProfile.time * 60 + "";
        bc.gameTimer.fillAmount = 1;

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
        bc.endScreen.SetActive(screen == GameScreen.End);

        if (screen == GameScreen.Profile) {
            bc.scrollSection.Create(bc.profilePrefab, ProfileManager.instance.profiles.ToArray(), Method);
            UpdateProfileSelection();
        }

        if (screen == GameScreen.Game)
            OnPlay();
    }

    public bool Method(GameObject go, object game) {
        go.transform.GetChild(0).GetComponent<Text>().text = ((Profile)game).profileName;
        return true;
    }

    void FixedUpdate() {
        if(!debugGame)
            UpdateBikeSpeed();

        if (gameScreen == GameScreen.Game) {
            OnFixedUpdate();
        }
    }

    protected virtual void Setup() { }
    protected virtual void Reset() { }

    protected virtual void OnChangeProfile() { }
    protected virtual void OnPlay() { }

    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }

    void UpdateBikeSpeed() {
        bikeTimer += Time.deltaTime;
        if (bikeTimer >= 0.1f) {
            bikeTimer = 0;
            bikeSpeed = GetBikeSpeed(bikeManager.bikeSpeed);
        }
        UpdateBikeUI();
    }

    void UpdateBikeUI() {
        if (bc != null) {
            speedText.text = bikeManager.bikeSpeed.ToString("F1");

            float nul = 0;
            if (bikeManager.bikeSpeed > currentProfile.targetSpeed - 20) {
                nul = bikeManager.bikeSpeed - (currentProfile.targetSpeed - 20);
                nul *= 0.025f;
            }
            nul = Mathf.Clamp(nul, 0f, 1f);

            speedGaugeImage.localEulerAngles = new Vector3(0, 0, -(nul * 160));
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
