using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyGame : Game {

    public GameObject failScreen;

    public Transform[] objects;
    public Transform[] foreGround;
    public Transform[] backGround;

    public Transform flappy;

    Vector3 flappyVelocity;

    public Vector2 minMaxUpDown;

    public UnityEngine.UI.Text scoreUI;

    float spacing = 5;
    float backgroundSpacing = 8.0f;
    float foregroundSpacing = 8.08f;

    float speedMultiplier = 1.0f;

    int score = 0;

    bool going = true;

    protected override void Setup() {
        Transform obj = objects[0];

        objects = new Transform[6];
        for (int i = 0; i < objects.Length; i++) {
            if (i == 0) {
                objects[i] = obj;
            } else {
                objects[i] = Instantiate(obj, obj.parent);
            }

            objects[i].position = new Vector3(3f + spacing * i, 2.75f, 0);

            ChangeObjectPosition(i, Random.value);
            ChangeDistanceBetween(i, currentProfile.gameDifficulty > 1 ? 1.6f : 2f);
        }

        obj = backGround[0];
        backGround = new Transform[3];
        for (int i = 0; i < backGround.Length; i++) {
            if (i == 0) {
                backGround[i] = obj;
            } else {
                backGround[i] = Instantiate(obj, obj.parent);
                backGround[i].position = backGround[i - 1].position + new Vector3(backgroundSpacing * 2, 0, 0);
            }
        }

        obj = foreGround[0];
        foreGround = new Transform[4];
        for (int i = 0; i < foreGround.Length; i++) {
            if (i == 0) {
                foreGround[i] = obj;
            } else {
                foreGround[i] = Instantiate(obj, obj.parent);
                foreGround[i].position = foreGround[i - 1].position + new Vector3(foregroundSpacing, 0, 0);
            }
        }

        scoreUI.text = score.ToString();

        going = false;
        speedMultiplier = 0f;

        failScreen.SetActive(false);
    }

    protected override void OnChangeProfile() {
        for (int i = 0; i < objects.Length; i++)
            ChangeDistanceBetween(i, currentProfile.gameDifficulty > 1 ? 1.6f : 2f);
    }

    protected override void OnPlay() {
        flappy.GetComponent<FlappyPlane>().Play();

        flappyVelocity.y = 3;
        going = true;
        speedMultiplier = 1.0f;
    }

    protected override void Reset() {
        for (int i = 0; i < objects.Length; i++) {
            objects[i].position = new Vector3(3f + spacing * i, 2.75f, 0);

            ChangeObjectPosition(i, Random.value);
            ChangeDistanceBetween(i, 2);
        }

        flappy.position = new Vector3(flappy.position.x, 2.15f, 0);
        flappyVelocity.y = 3;

        going = true;
        speedMultiplier = 1.0f;

        failScreen.SetActive(false);

        StartCoroutine("ResetScore");
    }

    protected override void OnUpdate() {
        if (going) {
            if(currentProfile.gameDifficulty > 0) {
                speedMultiplier += 0.01f * Time.deltaTime;
            }

            //speedMultiplier = GetSpeedMultiplier();

            if (ControllerInput.PressButtonDown()) {
                flappyVelocity.y = GetFlyUp();
            }

            flappyVelocity.y = Mathf.Clamp(flappyVelocity.y - 5 * Time.deltaTime, -2f, 100);

            flappy.position += flappyVelocity * Time.deltaTime;
            flappy.localEulerAngles = new Vector3(0, 0, flappyVelocity.y * 5f);
        }else {
            if (ControllerInput.PressButtonUp()) {
                Reset();
                flappy.GetComponent<FlappyPlane>().Play();
            }
        }

        UpdateForeAndBackGround();
    }

    protected override void OnFixedUpdate() {
        ObjectUpDate();
    }

    IEnumerator ResetScore() {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        score = 0;
        scoreUI.text = score.ToString();
        yield return 0;
    }

    public void AddScore() {
        if (going) {
            score++;
            scoreUI.text = score.ToString();
        }
    }

    float GetFlyUp() {
        switch (bikeSpeed) {
            case BikeSpeed.TOO_SLOW: return 1;
            case BikeSpeed.SLOW: return 1;
            default: case BikeSpeed.GOOD: return 3;
            case BikeSpeed.FAST: return 5;
            case BikeSpeed.TOO_FAST: return 5;
        }
    }

    public void Fail() {
        speedMultiplier = 0;
        going = false;
        failScreen.SetActive(true);
    }

    void UpdateForeAndBackGround() {
        for (int i = 0; i < backGround.Length; i++) {
            backGround[i].position += Vector3.left * 0.5f * Time.deltaTime * speedMultiplier;
        }

        if (backGround[0].position.x < -18) {
            PlaceBackGroundAtBack();
        }

        for (int i = 0; i < foreGround.Length; i++) {
            foreGround[i].position += Vector3.left * Time.deltaTime * speedMultiplier;
        }

        if (foreGround[0].position.x < -15) {
            PlaceForeGroundAtBack();
        }
    }

    void PlaceBackGroundAtBack() {
        Transform first = backGround[0];
        for (int i = 0; i < backGround.Length - 1; i++) {
            backGround[i] = backGround[i + 1];
        }

        backGround[backGround.Length - 1] = first;
        backGround[backGround.Length - 1].localPosition = backGround[backGround.Length - 2].localPosition + Vector3.right * backgroundSpacing * 2;
    }

    void PlaceForeGroundAtBack() {
        Transform first = foreGround[0];
        for (int i = 0; i < foreGround.Length - 1; i++) {
            foreGround[i] = foreGround[i + 1];
        }

        foreGround[foreGround.Length - 1] = first;
        foreGround[foreGround.Length - 1].localPosition = foreGround[foreGround.Length - 2].localPosition + Vector3.right * foregroundSpacing;
    }

    #region objects
    void ObjectUpDate() {
        for (int i = 0; i < objects.Length; i++) {
            objects[i].position += Vector3.left * Time.fixedDeltaTime * speedMultiplier;
        }

        if (objects[0].position.x < -11) {
            PlaceObjectAtBack();
        }
    }

    void ChangeObjectPosition(int index, float normalized) {
        Vector3 v3 = objects[index].position;
        v3.y = Mathf.Lerp(minMaxUpDown.x, minMaxUpDown.y, normalized);
        objects[index].position = v3;
    }

    void ChangeDistanceBetween(int index, float dis) {
        Transform up = objects[index].GetChild(0);
        Transform down = objects[index].GetChild(1);
        float begin = 2.37f;
        float half = dis * 0.5f;

        up.localPosition = new Vector3(0, begin + half, 0);
        down.localPosition = new Vector3(0, -begin - half, 0);
    }

    void PlaceObjectAtBack() {
        Transform first = objects[0];
        for (int i = 0; i < objects.Length - 1; i++) {
            objects[i] = objects[i + 1];
        }

        objects[objects.Length - 1] = first;
        objects[objects.Length - 1].position = objects[objects.Length - 2].position + Vector3.right * spacing;

        ChangeObjectPosition(objects.Length - 1, Random.value);
    }
    #endregion
}
