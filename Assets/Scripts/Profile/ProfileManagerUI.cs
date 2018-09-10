using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProfileScreen { Main, Create, Delete, Edit }

public class ProfileManagerUI : MonoBehaviour {

    ProfileManager manager;

    ProfileScreen profileScreen = ProfileScreen.Main;

    public GameObject mainObject, deleteObject, editObject;

    public GameObject profileItemPrefab;
    public ControllerScrollSection scrollSection;

    //main
    public Text MainTextName, mainTextDifficulty, mainTextTargetSpeed, mainTextResistance, mainTextTime;

    //delete
    public Text deleteTextName;

    //edit
    public Transform letter1, letter2, letter3, letter4, letter5, letter6, editTextNumberDifficulty, editTextNumberTargetSpeed, editTextNumberResistance, editTextNumberTime;
    public RectTransform selectionBox;
    Transform[] components;

    void Start() {
        Cursor.visible = false;

        manager = ProfileManager.instance;

        scrollSection.Create(profileItemPrefab, ProfileManager.instance.profiles.ToArray(), Method);

        components = new Transform[] { letter1, letter2, letter3, letter4, letter5, letter6, editTextNumberDifficulty, editTextNumberTargetSpeed, editTextNumberResistance, editTextNumberTime };

        UpdateMainDetailScreen();
    }

    public bool Method(GameObject go, object game) {
        go.transform.GetChild(0).GetComponent<Text>().text = ((Profile)game).profileName;
        return true;
    }

    void Update() {
        if (profileScreen == ProfileScreen.Main) {
            MainControls();
        } else if (profileScreen == ProfileScreen.Delete) {
            DeleteControls();
        } else if (profileScreen == ProfileScreen.Edit || profileScreen == ProfileScreen.Create) {
            EditControls();
        }

        ControllerInput.ResetToggles();
    }

    void SetScreen(ProfileScreen screen) {
        mainObject.SetActive(screen == ProfileScreen.Main);
        deleteObject.SetActive(screen == ProfileScreen.Delete);
        editObject.SetActive(screen == ProfileScreen.Edit || screen == ProfileScreen.Create);

        profileScreen = screen;

        if (screen == ProfileScreen.Create) {
            SetupEditScreen(null);
        } else if (screen == ProfileScreen.Edit) {
            SetupEditScreen(ProfileManager.instance.profiles[scrollSection.GetSelected()]);
        }else if (screen == ProfileScreen.Delete) {
            Debug.Log(scrollSection.GetSelected());
            workingProfileIndex = scrollSection.GetSelected();
            deleteTextName.text = ProfileManager.instance.profiles[workingProfileIndex].profileName;
        }

        selection = 0;
        UpdateSelectionBox();
    }

    string letters = " abcdefghijklmnopqrstuvwxyz1234567890";
    //6 letters + difficulty + resistance + time = 9 components
    int selection = 0;
    int maxSelection = 10;

    int workingProfileIndex = -1;

    int[] componentIndexes = new int[10];

    public static string[] difficultNames = new string[] {"makkelijk", "normaal", "moeilijk"};

    int IndexFromLetter(string str, int index) {
        if (index >= str.Length) {
            return 0;
        }

        str = str.ToLower();

        for (int i = 0; i < letters.Length; i++) {
            if (str[index].Equals(letters[i])) {
                return i;
            }
        }
        return 0;
    }

    string GetName() {
        string str = "";
        for (int i = 0; i < 6; i++) {
            str += letters[componentIndexes[i]].ToString();
        }
        return str;
    }

    int GetDifficulty() {
        return componentIndexes[6];
    }

    int GetTargetSpeed() {
        return componentIndexes[7];
    }

    int GetResistance() {
        return componentIndexes[8];
    }

    int GetTime() {
        return componentIndexes[9];
    }

    void SetupEditScreen(Profile profile) {
        selection = 0;

        for (int i = 0; i < 6; i++) {
            componentIndexes[i] = profile == null ? 1 : IndexFromLetter(profile.profileName, i);
        }

        if (profile != null) {
            workingProfileIndex = profile.GetIndex();

            componentIndexes[6] = profile.gameDifficulty;
            componentIndexes[7] = profile.targetSpeed;
            componentIndexes[8] = profile.resistance;
            componentIndexes[9] = profile.time;
        }
        else {
            workingProfileIndex = -1;

            componentIndexes[6] = 1;
            componentIndexes[7] = 45;
            componentIndexes[8] = 2;
            componentIndexes[9] = 1;
        }

        UpdateComponents();
    }

    void NextComponent() {
        selection++;
        if (selection >= maxSelection)
            selection = 0;

        UpdateSelectionBox();
    }

    void PreviousComponent() {
        selection--;
        if (selection < 0)
            selection = maxSelection - 1;

        UpdateSelectionBox();
    }

    void UpdateSelectionBox() {
        RectTransform rect = components[selection].GetComponent<RectTransform>();

        selectionBox.position = rect.position;
        selectionBox.sizeDelta = rect.sizeDelta;
    }

    void UpdateComponents() {
        for (int i = 0; i < maxSelection; i++) {
            if (i < 6) {
                components[i].GetChild(0).GetComponent<Text>().text = letters[componentIndexes[i]].ToString();
            }else if (i == 6) {
                components[i].GetChild(0).GetComponent<Text>().text = difficultNames[componentIndexes[i]];
            }
            else {
                components[i].GetChild(0).GetComponent<Text>().text = componentIndexes[i].ToString();
            }
        }
    }

    void AddToComponent(int i) {
        componentIndexes[selection] += i;

        if(selection < 6) {
            if(componentIndexes[selection] >= letters.Length) {
                componentIndexes[selection] = 0;
            }else if (componentIndexes[selection] < 0) {
                componentIndexes[selection] = letters.Length - 1;
            }
        }else if (selection == 6) {//difficulty
            if (componentIndexes[selection] >= difficultNames.Length) {
                componentIndexes[selection] = 0;
            } else if (componentIndexes[selection] < 0) {
                componentIndexes[selection] = difficultNames.Length - 1;
            }
        } else if (selection == 7) {//target speed
            componentIndexes[selection] = Mathf.Clamp(componentIndexes[selection], 10, 90);
        } else if (selection == 8) {//resistance
            if (componentIndexes[selection] >= 8) {
                componentIndexes[selection] = 1;
            } else if (componentIndexes[selection] < 1) {
                componentIndexes[selection] = 7;
            }
        } else {//time
            componentIndexes[selection] = Mathf.Clamp(componentIndexes[selection], 1, 10);
        }

        UpdateComponents();
        //UpdateSelectionBox();
    }

    void UpdateMainDetailScreen() {
        Profile p = ProfileManager.instance.profiles[scrollSection.GetSelected()];
        MainTextName.text = p.profileName;
        mainTextDifficulty.text = difficultNames[p.gameDifficulty];
        mainTextTargetSpeed.text = p.targetSpeed.ToString();
        mainTextResistance.text = p.resistance.ToString();
        mainTextTime.text = p.time + " minuten";
    }

    //button input
    void MainControls() {
        if (ControllerInput.PressArrowUp()) {
            scrollSection.GoUp();
            UpdateMainDetailScreen();
        }
        if (ControllerInput.PressArrowDown()) {
            scrollSection.GoDown();
            UpdateMainDetailScreen();
        }
        if (ControllerInput.PressButtonUp()) {
            SetScreen(ProfileScreen.Delete);
        }
        if (ControllerInput.PressButtonDown()) {
            SetScreen(ProfileScreen.Edit);
        }
        if (ControllerInput.PressButtonLeft()) {
            SetScreen(ProfileScreen.Create);
        }

        ControllerInput.PressMenu();
    }

    void DeleteControls() {
        if (ControllerInput.PressButtonRight()) {
            SetScreen(ProfileScreen.Main);

            workingProfileIndex = -1;
        }

        if (ControllerInput.PressButtonDown()) {
            ProfileManager.instance.RemoveProfile(workingProfileIndex);
            scrollSection.UpdateList(ProfileManager.instance.profiles.ToArray());
            SetScreen(ProfileScreen.Main);

            workingProfileIndex = -1;
        }
    }

    void EditControls() {
        if (ControllerInput.PressButtonRight()) {
            SetScreen(ProfileScreen.Main);
        }

        if (ControllerInput.PressArrowRight()) {
            NextComponent();
        }
        if (ControllerInput.PressArrowLeft()) {
            PreviousComponent();
        }

        if (ControllerInput.PressArrowUp()) {
            AddToComponent(1);
        }
        if (ControllerInput.PressArrowDown()) {
            AddToComponent(-1);
        }

        if (ControllerInput.PressButtonDown()) {
            Profile p = new Profile(GetName(), GetResistance(), GetTargetSpeed(), GetDifficulty(), GetTime());
            if (profileScreen == ProfileScreen.Create) {
                ProfileManager.instance.AddProfile(p);
            }else {
                ProfileManager.instance.ChangeProfile(workingProfileIndex, p);
            }

            scrollSection.UpdateList(ProfileManager.instance.profiles.ToArray());

            SetScreen(ProfileScreen.Main);
        }
    }
}
