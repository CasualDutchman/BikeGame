using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyPlane : MonoBehaviour {

    public FlappyGame game;

    public Sprite[] animationArray;
    public int fps;

    SpriteRenderer sr;

    bool play = true;
    float timer;
    int frame = 0;

	void Start () {
        sr = GetComponent<SpriteRenderer>();
        play = false;
	}
	
	void Update () {
        if (play) {
            Animate();
        }

        if(transform.position.y > 5.73f) {
            game.Fail();
            Stop();
        }
	}

    public void Play() {
        play = true;
    }

    public void Stop() {
        play = false;
    }

    void Animate() {
        timer += Time.deltaTime;
        if (timer > 1 / (float)fps) {
            frame++;
            if (frame >= animationArray.Length)
                frame = 0;

            sr.sprite = animationArray[frame];

            timer = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Fail"))) {
            game.Fail();
            Stop();
        }
    }
}
