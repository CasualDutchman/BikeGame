using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public static int numAsteroidChildren = 3;

    public AsteroidsGame game;

    Vector3 velocity;
    int size;
    Transform[] children;

    float rotation;

    public bool active = true;

	void Start () {
		
	}
	
	void Update () {
        transform.position += velocity * Time.deltaTime;
        transform.eulerAngles += Vector3.back * rotation * Time.deltaTime;

        if (transform.position.x < -9.5f) {
            transform.position += Vector3.right * 19;
        }
        if (transform.position.x > 9.5f) {
            transform.position += Vector3.left * 19;
        }
        if (transform.position.y > 6.5f) {
            transform.position += Vector3.down * 8.5f;
        }
        if (transform.position.y < -2) {
            transform.position += Vector3.up * 8.5f;
        }

	}

    public void InitChildren(int newSize) {
        size = newSize;

        GetComponent<SpriteRenderer>().sortingOrder = 2 - size;

        if (newSize > 0) {
            children = new Transform[Asteroid.numAsteroidChildren];

            for (int i = 0; i < children.Length; i++) {
                Transform tra = game.presetAsteroids[Random.Range(0, game.presetAsteroids.Length)];
                children[i] = Instantiate(tra, tra.parent);

                children[i].localScale = Vector3.one * ScaleFromSize(size - 1);
                children[i].GetComponent<Asteroid>().InitChildren(size - 1);

                children[i].gameObject.SetActive(false);
            }
        }
    }

    float ScaleFromSize(int f) {
        switch (f) {
            case 0: return 0.25f;
            case 1: return 0.5f;
            default: case 2: return 1f;
        }
    }

    public void SetNewVelocity() {
        velocity = Random.insideUnitCircle.normalized * SpeedFromSize();
        float rand = Random.value * 180 * (1 - SpeedFromSize());
        rotation = rand * 0.5f;
    }

    float SpeedFromSize() {
        switch (size) {
            case 0: return 2f;
            case 1: return 1f;
            default: case 2: return 0.5f;
        }
    }

     [ContextMenu("Break")]
    public void Break() {
        gameObject.SetActive(false);

        if(active)
            game.DepleteAstroidStack();

        active = false;

        if (size == 0)
            return;

        for (int i = 0; i < children.Length; i++) {
            children[i].gameObject.SetActive(true);
            children[i].position = transform.position;
            children[i].GetComponent<Asteroid>().SetNewVelocity();
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Win"))) {
            Break();
            Destroy(col.gameObject);
        }
    }
}
