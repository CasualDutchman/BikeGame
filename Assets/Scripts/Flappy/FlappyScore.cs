using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyScore : MonoBehaviour {

    public FlappyGame game;

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Player"))) {
            game.AddScore();
        }
    }
}
