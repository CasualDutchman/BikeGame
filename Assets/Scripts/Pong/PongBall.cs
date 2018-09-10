using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour {

    public PongGame game;

    void Start() {

    }

    void Update() {

    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Fail"))) {
            if (col.transform.position.x > 0) {
                game.Win();
            } else {
                game.Fail();
            }
        }

        bool b = col.gameObject.layer.Equals(LayerMask.NameToLayer("Bouncer"));

        game.BounceBall(col.transform.position, col.contacts[0].normal, b);
    }
}
