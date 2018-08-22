using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {

    Settings settings;

    public RectTransform selectionBox;
    public Text volumeText, quitText;

    RectTransform[] components;

    string[] quitStrings = new string[] { "Naar windows", "Hele computer" };

    int selection = 0;
    int maxSelection = 2;

    void Start() {
        settings = Settings.instance;

        components = new RectTransform[] { volumeText.rectTransform, quitText.rectTransform };

        UpdateTextFields();
        UpdateSelectionBox();
    }

    void UpdateTextFields() {
        volumeText.text = settings.volume.ToString();
        quitText.text = quitStrings[settings.quitPC ? 1 : 0];
    }

    void Update() {
        if (ControllerInput.PressArrowRight()) {
            NextComponent();
        }
        if (ControllerInput.PressArrowLeft()) {
            PreviousComponent();
        }

        if (ControllerInput.PressArrowUp()) {
            AddToComponen(1);
        }
        if (ControllerInput.PressArrowDown()) {
            AddToComponen(-1);
        }

        ControllerInput.ResetToggles();

        ControllerInput.PressMenu();
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

    void AddToComponen(int i) {
        if (selection == 0) {
            settings.ChangeVolume(Mathf.Clamp(settings.volume + i * 3, 0, 100));
        } 
        else {
            settings.ChangeQuit(!settings.quitPC);
        }

        UpdateTextFields();
    }
}
