using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsGame : Game {

    public int numAsteroids = 10;
    public Transform[] presetAsteroids;
    public Transform[] asteroids;

    public Transform ship;

    public GameObject bulletPrefab;

    int asteroidCount;

    protected override void Setup() {
        SetupAsteroids();
    }

    void SetupAsteroids() {
        asteroids = new Transform[numAsteroids];

        for (int i = 0; i < asteroids.Length; i++) {
            if (i < presetAsteroids.Length) {
                asteroids[i] = presetAsteroids[i];
            }else {
                Transform newTra = presetAsteroids[Random.Range(0, presetAsteroids.Length)];
                asteroids[i] = Instantiate(newTra, newTra.parent);
            }

            float posX = (5.5f + Random.value * 4) * Random.Range(0, 10) < 5 ? 1f : -1f;
            float posY = 0;

            if (Random.Range(0, 10) < 3)
                posX = -9.5f + Random.value * (9.5f * 2f);

            if (posX > -5.5f && posX < 5.5f) {
                posY = (Random.value * 2) + Random.Range(0, 10) < 5 ? -2 : 4.5f;
            } else {
                posY = Random.value * 8.5f - 2f;
            }

            asteroids[i].position = new Vector3(posX, posY, 0);

            asteroids[i].GetComponent<Asteroid>().InitChildren(2);
            asteroids[i].GetComponent<Asteroid>().SetNewVelocity();
        }

        ship.position = new Vector3(0, 2.25f, 0);

        asteroidCount = numAsteroids * 13;
    }

    protected override void OnChangeProfile() {
        
    }

    protected override void OnPlay() {
        
    }

    protected override void Reset() {
        
    }

    protected override void OnUpdate() {
        if (ControllerInput.PressButtonDown()) {
            SpawnBullet();
        }
    }

    protected override void OnFixedUpdate() {
        
    }

    public void DepleteAstroidStack() {
        asteroidCount--;

        if(asteroidCount <= 0) {
            Debug.Log("Win");
        }
    }

    void SpawnBullet() {
        GameObject go = Instantiate(bulletPrefab, transform);
        go.GetComponent<AsteroidsBullet>().SetVelocity(ship.GetChild(0));
    }
}
