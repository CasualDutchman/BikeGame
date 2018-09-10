using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsBullet : MonoBehaviour {

    Vector3 velocity;
	
    void Update() {
        if (transform.position.x < -9.5f) {
            Destroy(gameObject);
        }
        if (transform.position.x > 9.5f) {
            Destroy(gameObject);
        }
        if (transform.position.y > 6.5f) {
            Destroy(gameObject);
        }
        if (transform.position.y < -2) {
            Destroy(gameObject);
        }
    }

	void FixedUpdate () {
        transform.position += velocity * Time.fixedDeltaTime;
	}
    
    public void SetVelocity(Transform t) {
        transform.position = t.position;
        velocity = t.up * 2;
        transform.eulerAngles = t.eulerAngles;// new Vector3(0, 0, newRot.y);
    }
}
