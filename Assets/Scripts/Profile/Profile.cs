using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile {

    private int index;

    public string profileName;
    public int resistance;
    public int gameDifficulty;
    public int targetSpeed;
    public int time;
	
    public Profile(string _name, int _res, int _spee, int _dif, int _time) {
        profileName = _name;
        resistance = _res;
        targetSpeed = _spee;
        gameDifficulty = _dif;
        time = _time;
    }

    public void SetIndex(int i) {
        index = i;
    }

    public int GetIndex() {
        return index;
    }

}
