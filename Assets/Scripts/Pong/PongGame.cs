using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongGame : Game {

    public Transform playerBrick, aiBrick;
    float bounce;

    public Transform ball;

    public Vector2 minMaxBrick;

    public float ballSpeed;

    public float bouncerSpeed = 2;
    float aiBouncerSpeed = 1;

    public TextMesh playerScore, aiScore;

    Vector2 ballVelocity;

    Vector3 initialBallPos;

    int score_player, score_ai;

    protected override void Setup() {
        base.Setup();

        bounce = playerBrick.position.y;
        initialBallPos = ball.position;

        playerScore.text = "0";
        aiScore.text = "0";

        aiBouncerSpeed = 1 + currentProfile.gameDifficulty * 0.5f;
    }

    protected override void Reset() {
        base.Reset();
    }

    protected override void OnUpdate() {
        bounce = Mathf.Clamp(bounce + (ControllerInput.GetVertical() * Time.deltaTime * GetBouncerSpeed()), minMaxBrick.x, minMaxBrick.y);
        playerBrick.position = new Vector3(playerBrick.position.x, bounce, 0);

        if (ballVelocity.magnitude <= 0) {
            if (ControllerInput.PressButtonDown()) {
                StartBall();
            }
        }

        if (ball.position.x > 0) {
            float d = ball.position.y - aiBrick.position.y;
            Vector3 move = Vector3.zero;
            move.y = aiBouncerSpeed * Mathf.Min(d, d > 0 ? 1f : -1f);
            Vector3 current = aiBrick.position;
            current += move * Time.deltaTime;
            current.y = Mathf.Clamp(current.y, minMaxBrick.x, minMaxBrick.y);
            aiBrick.position = current;
        }
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
        ballVelocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-0.3f, 0.3f)).normalized;
    }

    void ResetBall() {
        ballVelocity = Vector3.zero;
        ball.position = initialBallPos;
    }

    public void Win() {
        score_player++;
        playerScore.text = score_player.ToString();

        ResetBall();

        print("Win");
    }

    public void Fail() {
        score_ai++;
        aiScore.text = score_ai.ToString();

        ResetBall();

        print("Fail");
    }

    public void BounceBall(Vector3 pos, Vector3 normal, bool b) {
        if (b) {
            ballVelocity = (ball.transform.position - pos).normalized;
        } else {
            ballVelocity = Vector2.Reflect(ballVelocity, normal);
        }
    }
}
