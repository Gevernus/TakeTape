using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Records records;
    public GameObject player;
    public int score;
    public float passSize = 20;
    public GameObject wall;
    public GameObject wallInverted;
    public TextMeshProUGUI scoreText;
    public float spawnTime;
    public float wallSpeed = 1f;
    public float speedAdd = 0.1f;
    public int minInvertedDelay;
    public int maxInvertedDelay;
    public float timeToInvert;
    public float minPosition = -1.3f;
    public float maxPosition = 1.3f;
    public AudioClip main;
    public AudioClip menu;
    public GameObject startHint;
    public GameObject retryHint;
    public SpriteRenderer background;
    public Sprite simpleBack;
    public Sprite invertedBack;
    public CameraShake shaker;
    public LayerMask normalMask;
    public LayerMask uiMask;
    public Camera camera;

    private float currentWallSpeed;
    private float delta;
    private float timeToInvertDelta;
    private readonly List<Transform> walls = new List<Transform>();
    private AudioSource audio;
    private float invertedCounter;
    private bool isInvertedWalls;
    private bool isPaused = true;
    private bool isEnded;
    private bool isStarted;
    private Vector3 startPosition;
    private int invertedDelay;

    void Start() {
        audio = GetComponent<AudioSource>();
        var position = player.transform.position;
        startPosition = new Vector3(position.x, position.y);
        audio.loop = true;
        audio.PlayOneShot(menu);
        currentWallSpeed = wallSpeed;
        timeToInvertDelta = timeToInvert;
        delta = spawnTime - 1;
        invertedDelay = Random.Range(minInvertedDelay, maxInvertedDelay);
        camera.cullingMask = uiMask;
    }

    void Update() {
        if (!isPaused && !isStarted) {
            if (isEnded) {
                retryHint.SetActive(true);
                startHint.SetActive(false);
            }
            else {
                startHint.SetActive(true);
                retryHint.SetActive(false);
            }
        }
        else {
            startHint.SetActive(false);
            retryHint.SetActive(false);
        }

        if (IsActiveGame()) {
            if (!audio.isPlaying) {
                audio.PlayOneShot(isStarted ? main : menu);
            }

            if (isStarted) {
                timeToInvertDelta += Time.deltaTime;
                if (timeToInvertDelta >= timeToInvert) {
                    delta += Time.deltaTime;
                }

                //TODO: create walls on start and reuse components
                if (delta > spawnTime / currentWallSpeed) {
                    var bottpos = Random.Range(minPosition, maxPosition);
                    delta = 0f;

                    var topWall = Instantiate(isInvertedWalls ? wallInverted : wall, transform);
                    var bottomWall = Instantiate(isInvertedWalls ? wallInverted : wall, transform);
                    topWall.transform.position = new Vector3(30, bottpos + passSize, 0);
                    var localScale = topWall.transform.localScale;
                    localScale = new Vector3(localScale.x, -localScale.y);
                    topWall.transform.localScale = localScale;
                    bottomWall.transform.position = new Vector3(30, bottpos, 0);
                    walls.Add(topWall.transform);
                    walls.Add(bottomWall.transform);
                    invertedCounter++;
                    if (invertedCounter >= invertedDelay) {
                        invertedDelay = Random.Range(minInvertedDelay, maxInvertedDelay);
                        invertedCounter = 0;
                        timeToInvertDelta = 0;
                        isInvertedWalls = !isInvertedWalls;
                        bottomWall.tag = "gravityChange";
                        topWall.tag = "gravityChange";
                    }
                }

                for (var i = 0; i < walls.Count; i++) {
                    walls[i].position += Time.deltaTime * currentWallSpeed * Vector3.left;
                }

                currentWallSpeed += speedAdd * Time.deltaTime;
            }
        }
    }

    public void IncrementScore() {
        score++;
        scoreText.text = score + "";
    }

    public void Pause(bool isPaused) {
        this.isPaused = isPaused;
        player.SetActive(!isPaused);
        camera.cullingMask = isPaused ? uiMask : normalMask;
        scoreText.enabled = !isPaused;
        audio.Stop();
        audio.PlayOneShot(isPaused ? menu : main);
    }

    public void End() {
        if (!isEnded) {
            audio.Stop();
            if (score > 0) {
                records.AddRecord(score + "");
            }

            isEnded = true;
            isStarted = false;
            shaker.shakeDuration = 0.4f;
        }
    }

    public void StartGame() {
        isStarted = true;
        audio.PlayOneShot(main);
    }

    public void Restart() {
        player.transform.position = startPosition;
        player.transform.rotation = Quaternion.identity;
        var particleSystem = player.GetComponent<ParticleSystem>();
        particleSystem.loop = true;
        particleSystem.Play();
        foreach (Transform child in transform) {
            if (child.gameObject.tag.Equals("wall") || child.gameObject.tag.Equals("gravityChange")) {
                DestroyWall(child);
            }
        }

        isEnded = false;
        isStarted = false;
        isPaused = true;
        invertedCounter = 0;
        delta = spawnTime - 1;
        isInvertedWalls = false;
        score = 0;
        scoreText.text = "";
        currentWallSpeed = wallSpeed;
        background.sprite = simpleBack;
    }

    public bool IsActiveGame() {
        return !isPaused && !isEnded;
    }

    public bool IsStarted() {
        return isStarted;
    }

    public bool IsPaused() {
        return isPaused;
    }

    public void DestroyWall(Transform wall) {
        walls.Remove(wall);
        Destroy(wall.gameObject);
    }

    public void ChangeBackground(bool inverted) {
        background.sprite = inverted ? invertedBack : simpleBack;
    }

    public bool IsInverted() {
        return isInvertedWalls;
    }
}