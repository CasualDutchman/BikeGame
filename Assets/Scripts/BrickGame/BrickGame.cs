using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickGame : Game {

    public GameObject ball;
    public GameObject bouncer;

    public GameObject prefabBrick;

    Vector3 originBall;

    Vector2 ballVelocity;
    Vector2 bouncerLocation;
    float bounce;

    public float maxScreen = 7.5f;

    public float bouncerSpeed = 2;
    public float ballSpeed = 3;

    public Color[] colors;

    protected override void OnPlay() {
        base.OnPlay();
        originBall = ball.transform.localPosition;
        bouncerLocation = new Vector2(bouncer.transform.position.x, bouncer.transform.position.y);
        bounce = bouncerLocation.x;

        SetupBricks();
    }

    protected override void Reset() {
        base.Reset();

        SetupBricks();

        ResetBall();
    }

    Vector2Int[] rows = new Vector2Int[] { new Vector2Int(1, 3), new Vector2Int(2, 4), new Vector2Int(3, 5) };
    Vector2Int[] brickLines = new Vector2Int[] { new Vector2Int(2, 5), new Vector2Int(3, 6), new Vector2Int(4, 7) };

    void SetupBricks() {
        int[] line = new int[Random.Range(rows[currentProfile.gameDifficulty].x, rows[currentProfile.gameDifficulty].y)];

        for (int i = 0; i < line.Length; i++) {
            line[i] = Random.Range(brickLines[currentProfile.gameDifficulty].x, brickLines[currentProfile.gameDifficulty].y);
        }

        int colorPattern = 2;// Random.Range(0, 3);

        for (int y = 0; y < line.Length; y++) {
            for (int x = 0; x < line[y]; x++) {
                GameObject go = Instantiate(prefabBrick, transform);
                float xOffset = (line[y] * 1.8f) * 0.5f * -1f;
                go.transform.position = new Vector3(0.9f + xOffset + 1.8f * x, 5.2f - 0.7f * y, 0);

                switch (colorPattern) {
                    default: case 0: go.GetComponent<SpriteRenderer>().color = colors[y]; break;
                    case 1: go.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)]; break;
                    case 2: go.GetComponent<SpriteRenderer>().color = colors[(float)x >= line[y] * 0.5f ? Mathf.Abs((line[y] - 1) - x) : x]; break;
                }
            }
        }
    }

    protected override void OnUpdate () {
        bounce = Mathf.Clamp(bounce + (ControllerInput.GetHorizontal() * Time.deltaTime * GetBouncerSpeed()), -maxScreen, maxScreen);

        bouncer.transform.position = new Vector3(bounce, bouncerLocation.y, 0);

        if(ballVelocity.magnitude <= 0) {
            if (ControllerInput.PressButtonDown()) {
                StartBall();
            }
        }

        if(Mathf.Abs(ball.transform.position.x) > 2 || ball.transform.position.y > 8) {
            //ResetBall();
        }

        ControllerInput.PressMenu(ballVelocity.magnitude <= 0);
    }

    protected override void OnFixedUpdate() {
        if (ballVelocity.magnitude > 0) {
            ball.transform.position += new Vector3(ballVelocity.x, ballVelocity.y, 0) * Time.fixedDeltaTime * ballSpeed;
        }
    }

    float GetBouncerSpeed() {
        switch (bikeSpeed) {
            case BikeSpeed.TOO_SLOW: return 0;
            case BikeSpeed.SLOW: return bouncerSpeed * 0.3f;
            default: case BikeSpeed.GOOD: return bouncerSpeed;
            case BikeSpeed.FAST: return bouncerSpeed * 3;
            case BikeSpeed.TOO_FAST: return 0;
        }
    }

    void StartBall() {
        ballVelocity = new Vector2(Random.Range(-2f, 2f), Random.Range(0.5f, 2f)).normalized;
        ball.transform.parent = null;
    }

    public void OnDeleteBrick() {
        if (transform.childCount - 1 <= 0) {
            Reset();
        }
    }

    public void Fail() {
        ResetBall();
    }

    void ResetBall() {
        ballVelocity = Vector2.zero;
        ball.transform.SetParent(bouncer.transform);
        ball.transform.localPosition = originBall;
    }

    int strangeBounces = 0;

    public void BounceBall(Vector2 normal, bool b) {
        if (b) {
            ballVelocity = (ball.transform.position - bouncer.transform.position).normalized;
        } 
        else {
            ballVelocity = Vector2.Reflect(ballVelocity, normal);
            if (Mathf.Abs(ballVelocity.y) < 0.1f) {
                strangeBounces++;
                if(strangeBounces >= 3) {
                    ballVelocity.y *= 3f;
                    strangeBounces = 0;
                }
            }else {
                strangeBounces = 0;
            }
        }
    }
}
