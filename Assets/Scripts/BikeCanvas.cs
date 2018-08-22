using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BikeCanvas : MonoBehaviour {

    //game
    public RectTransform speedGaugeImage;
    public Text speedText;
    public Text gameTime;
    public Image gameTimer;

    //explainscreen
    public GameObject explainScreen;
    public Text explainTitle;
    public Text explainText;
    public Transform explainButtonsParent;
    public Image bikeTimer;
    public GameObject playButton;

    //change profile menu
    public GameObject changeProfile;
    public ControllerScrollSection scrollSection;
    public GameObject profilePrefab;

    public Text showProfileName, showProfileResistance, showProfileDifficultyLevel, showProfileTime, showProfileDefaultSpeed;

    //profileLeft
    public Text profileName, profileResistance, profileDifficultyLevel, profileTime, profileDefaultSpeed;
}
