using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour {

    public static Settings instance;

    public float volume = 100;
    public bool quitPC = true;

    public AudioMixer mixer;

    bool changed = false;

    public void Awake() {
        if (Time.time > 1) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start () {
        loadSettings();
    }
	
    void loadSettings() {
        if (PlayerPrefs.HasKey("Volume")) {
            volume = PlayerPrefs.GetFloat("Volume");
            quitPC = PlayerPrefs.GetInt("Quit") == 1;
            Debug.Log("Loaded settings");
        }else {
            changed = true;
        }

        UpdateMixer();
    }

    void OnDestroy() {
        if (changed) {
            PlayerPrefs.SetFloat("Volume", volume);
            PlayerPrefs.SetInt("Quit", quitPC ? 1 : 0);
            Debug.Log("Saved settings");
        }
    }

    public void ChangeVolume(float newVol) {
        volume = newVol;

        UpdateMixer();

        changed = true;
    }

    public void ChangeQuit(bool newBool) {
        quitPC = newBool;

        changed = true;
    }

    void UpdateMixer() {
        mixer.SetFloat("Volume", (100 - volume) * -0.8f);
    }
}
