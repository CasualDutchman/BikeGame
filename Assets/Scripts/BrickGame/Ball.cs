using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public BrickGame game;

	void Start () {
		
	}
	
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Fail")))
            game.Fail();

        bool b = col.gameObject.layer.Equals(LayerMask.NameToLayer("Bouncer"));

        game.BounceBall(col.contacts[0].normal, b);

        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Brick"))) { 
            Destroy(col.gameObject);
            game.OnDeleteBrick();
        }
    }

}
