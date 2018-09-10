using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LodeProtocol))]
public class BikeManager : MonoBehaviour {

    public static BikeManager instance;

    public static float TOO_SLOW_SPEED = 10;
    public static float SLOW_SPEED = 10;
    public static float GOOD_SPEED = 10;
    public static float FAST_SPEED = 10;
    public static float TOO_FAST_SPEED = 10;

    public bool useProtocol = true;

    LodeProtocol protocol;

    public float bikeSpeed;

    public float nonProtocolSpeed = 40;

    public void Awake() {
        if (Time.time > 1) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start () {
        if (useProtocol) {
            protocol = GetComponent<LodeProtocol>();
            if (protocol == null) {
                protocol = gameObject.AddComponent<LodeProtocol>();
            }

            protocol.Connect();

            protocol.ResponseReceived += ProtocolGetSpeed;
            protocol.AddCall(LodeProtocol.GET_POWER);
        }
    }

    void Update() {
        if (!useProtocol) {
            if (Input.GetKey(KeyCode.Period)) {
                nonProtocolSpeed = Mathf.Clamp(nonProtocolSpeed + Time.deltaTime * 3f, 0, 200);
            }
            else if (Input.GetKey(KeyCode.Comma)) {
                nonProtocolSpeed = Mathf.Clamp(nonProtocolSpeed - Time.deltaTime * 3f, 0, 200);
            }

            bikeSpeed = Mathf.Clamp(nonProtocolSpeed, 0, 200);
        }
    }

    private void ProtocolGetSpeed(Protocol usedProt, string data) {
        switch (usedProt.command) {
            case LodeProtocol.GET_POWER:
                try {
                    bikeSpeed = int.Parse(data) / 10;
                } catch (System.Exception) {
                    string t = "";
                    foreach (char c in data.ToCharArray()) {
                        t += (int)c + " ";
                    }
                    Debug.LogWarning("test: " + data + "|" + data.Length + "|" + t + "|" + data.ToCharArray().Length);
                    Debug.Log("protocol " + usedProt.command);
                }
                break;
        }
    }
}
