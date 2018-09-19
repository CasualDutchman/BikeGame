using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsShip : MonoBehaviour {

    public AsteroidsGame game;

    public SpriteRenderer spriteRenderer;
    public AnimationCurve curve;

    bool invincible = false;
    float timer;

	void Start () {

    }

    private void Update() {
        if (invincible) {
            timer += Time.deltaTime;
            if (timer >= 3) {
                invincible = false;
                timer = 0;

                Color color = spriteRenderer.color;
                color.a = 1;
                spriteRenderer.color = color;

                return;
            }

            Color col = spriteRenderer.color;
            col.a = curve.Evaluate(timer - Mathf.Floor(timer));
            spriteRenderer.color = col;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Fail")) && !invincible) {
            //game.Fail();
            //Stop();
            //Debug.Log("Hit");

            game.HitRock();

            invincible = true;
        }
    }
}
