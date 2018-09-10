using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsShip : MonoBehaviour {

    Rigidbody2D rigid;

    public AsteroidsGame game;

    public float maxSpeed;
    public float rotationSpeed;

	void Start () {
        rigid = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        transform.Rotate(Vector3.back * ControllerInput.GetHorizontal() * rotationSpeed * Time.deltaTime);

        if (transform.position.x < -9) {
            transform.position += Vector3.right * 18;
        }
        if (transform.position.x > 9) {
            transform.position += Vector3.left * 18;
        }
        if (transform.position.y > 6) {
            transform.position += Vector3.down * 7.5f;
        }
        if (transform.position.y < -1.5) {
            transform.position += Vector3.up * 7.5f;
        }
    }

    void FixedUpdate() {
        if (ControllerInput.GetVertical() > 0) {
            rigid.AddForce(-transform.up.ToVector2() * maxSpeed, ForceMode2D.Force);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Fail"))) {
            //game.Fail();
            //Stop();
        }
    }
}
