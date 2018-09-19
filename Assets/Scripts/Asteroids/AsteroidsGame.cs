using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsGame : Game {

    public int numAsteroids = 10;
    public Transform[] presetAsteroids;
    public Transform[] asteroids;

    public float maxSpeed;
    public float rotationSpeed;

    public Transform ship;
    Rigidbody2D shipRigid;

    public GameObject bulletPrefab;

    GameObject asteroidParent;
    int asteroidCount;

    int lives = 3;
    public GameObject[] lifesImages;

    bool playing = true;
    public GameObject failScreen;

    bool shot;
    float shootTimer;
    public float rateOfFire = 0.7f;

    protected override void Setup() {
        shipRigid = ship.GetComponent<Rigidbody2D>();

        for (int i = 0; i < presetAsteroids.Length; i++) {
            presetAsteroids[i].gameObject.SetActive(false);
        }

        failScreen.SetActive(false);

        SetupAsteroids();
        SetupShip();
    }

    void SetupAsteroids() {
        if (asteroidParent != null) {
            Destroy(asteroidParent);
        }

        if(transform.childCount > 0) {
            for (int i = 0; i < transform.childCount; i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        asteroids = new Transform[numAsteroids];

        asteroidParent = new GameObject("AsteroidsParent");
        asteroidParent.transform.SetParent(presetAsteroids[0].parent);

        for (int i = 0; i < asteroids.Length; i++) {
            Transform newTra = presetAsteroids[Random.Range(0, presetAsteroids.Length)];
            asteroids[i] = Instantiate(newTra, asteroidParent.transform);
            asteroids[i].gameObject.SetActive(true);

            float posX = (5.5f + Random.value * 4) * Random.Range(0, 10) < 5 ? 1f : -1f;
            float posY = 0;

            if (Random.Range(0, 10) < 7)
                posX = -9.5f + Random.value * (9.5f * 2f);

            if (posX > -5.5f && posX < 5.5f) {
                posY = (Random.value * 2) + Random.Range(0, 10) < 5 ? -2 : 4.5f;
            } else {
                posY = Random.value * 8.5f - 2f;
            }

            asteroids[i].position = new Vector3(posX, posY, 0);

            asteroids[i].GetComponent<Asteroid>().InitChildren(2, asteroidParent.transform);
            asteroids[i].GetComponent<Asteroid>().SetNewVelocity(asteroids[i], Vector2.zero, 3f);
        }

        asteroidCount = numAsteroids * 13;

        asteroids = new Transform[asteroidCount];

        for (int i = 0; i < asteroidCount; i++) {
            asteroids[i] = asteroidParent.transform.GetChild(i);
        }
    }

    void SetupShip() {
        ship.position = new Vector3(0, 2.25f, 0);
    }

    protected override void OnChangeProfile() {
        
    }

    protected override void OnPlay() {
        
    }

    protected override void Reset() {
        
    }

    protected override void OnUpdate() {
        if (playing) {
            if (ControllerInput.PressButtonDown() && !shot) {
                shot = true;
                SpawnBullet();
            }

            if (shot) {
                shootTimer += Time.deltaTime;
                if (shootTimer >= rateOfFire) {
                    shot = false;
                    shootTimer = 0;
                }
            }

            ship.Rotate(Vector3.back * ControllerInput.GetHorizontal() * rotationSpeed * Time.deltaTime);

            if (ship.position.x < -9) {
                ship.position += Vector3.right * 18;
            }
            if (ship.position.x > 9) {
                ship.position += Vector3.left * 18;
            }
            if (ship.position.y > 6) {
                ship.position += Vector3.down * 7.5f;
            }
            if (ship.position.y < -1.5) {
                ship.position += Vector3.up * 7.5f;
            }

            for (int i = 0; i < asteroids.Length; i++) {
                if (asteroids[i].gameObject.activeSelf) {
                    asteroids[i].GetComponent<Asteroid>().UpdateAsteroid();
                }
            }
        } else {
            if (ControllerInput.PressButtonUp()) {
                failScreen.SetActive(false);
                playing = true;
                numAsteroids = 2;
                SetupAsteroids();
                SetupShip();
                shootTimer = 0;
                shot = false;
            }
        }
    }

    protected override void OnFixedUpdate() {
        if (playing) {
            if (ControllerInput.GetVertical() > 0) {
                shipRigid.AddForce(-ship.up.ToVector2() * maxSpeed, ForceMode2D.Force);
            }
        }
    }

    public void DepleteAstroidStack() {
        asteroidCount--;

        if(asteroidCount <= 0) {
            numAsteroids++;
            SetupAsteroids();
            SetupShip();
            shootTimer = 0;
            shot = false;
        }
    }

    public void HitRock() {
        lives--;
        lifesImages[lives].SetActive(false);

        if (lives <= 0) {
            playing = false;
            failScreen.SetActive(true);

        }
    }

    void SpawnBullet() {
        GameObject go = Instantiate(bulletPrefab, transform);
        go.GetComponent<AsteroidsBullet>().SetVelocity(ship.GetChild(0));
    }
}
