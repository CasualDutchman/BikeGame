using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour {

    public static ProfileManager instance;

    bool changed = false;

    public void Awake() {
        if(Time.time > 1) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public List<Profile> profiles;
    public int currentProfileInUse = 0;

	void Start () {
        LoadProfiles();

        if (profiles.Count == 0) {
            AddProfile(new Profile("eerste", 1, 45, 0, 1));
        }
    }

    void LoadProfiles() {
        profiles = new List<Profile>();
        if (PlayerPrefs.HasKey("ProfileCount")) {
            int profileCount = PlayerPrefs.GetInt("ProfileCount");
            for (int i = 0; i < profileCount; i++) {
                Profile p = JsonUtility.FromJson<Profile>(PlayerPrefs.GetString("Profile" + i));
                p.SetIndex(i);
                profiles.Add(p);
            }

            currentProfileInUse = PlayerPrefs.GetInt("ProfileInUse");

            Debug.Log("Loaded Profiles");
        }
    }
	
    void OnDestroy() {
        if (profiles != null && profiles.Count > 0 && changed) {
            PlayerPrefs.SetInt("ProfileCount", profiles.Count);
            for (int i = 0; i < profiles.Count; i++) {
                string json = JsonUtility.ToJson(profiles[i]);
                PlayerPrefs.SetString("Profile" + i, json);
            }

            Debug.Log("Saved profiles");
        }

        PlayerPrefs.SetInt("ProfileInUse", currentProfileInUse);
    }

    public Profile GetCurrentProfile() {
        return profiles[currentProfileInUse];
    }

    public void AddProfile(Profile p) {
        p.SetIndex(profiles.Count);
        profiles.Add(p);
        changed = true;
    }

    public void ChangeProfile(int index, Profile newP) {
        newP.SetIndex(index);
        profiles[index] = newP;
        changed = true;
    }

    public void RemoveProfile(int index) {
        if (profiles.Count == 1)
            return;

        if (index == currentProfileInUse) {
            currentProfileInUse = 0;
        }

        profiles.RemoveAt(index);
        for (int i = 0; i < profiles.Count; i++) {
            profiles[i].SetIndex(i);
        }

        changed = true;
    }

    [AddComponentMenu("Clear")]
    public void ClearPlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }
}
