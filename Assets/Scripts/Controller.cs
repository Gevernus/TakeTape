using System;
using UnityEngine;

public class Controller : MonoBehaviour {
    public float thrust;
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public AnimationCurve rotationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public float gravityChangeDuration;
    public GameManager gameManager;
    public GameObject upAlert;
    public GameObject downAlert;
    public Flash flesh;
    public AudioClip jump;
    public AudioClip collision;
    public AudioClip gravityInversion;
    private ParticleSystem rocketTrail;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource audio;
    private float targetGravityScale;
    private GameObject prevCollision;
    private float timer;
    private bool inverted;
    private bool changed;
    private GameObject restoreAlert;
    private float rotateTimer;
    private float rotateDuration;
    private float targetAngle;
    private float startAngle;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
        targetGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        timer = gravityChangeDuration;
        rotateDuration = 0.2f;
        rocketTrail = GetComponent<ParticleSystem>();
        rocketTrail.loop = true;
        rocketTrail.Play();
    }

    private void Update() {
        if (gameManager.IsActiveGame()) {
            CheckJump();
            if (gameManager.IsStarted()) {
                timer += Time.deltaTime;
                var transform1 = transform;
                var position = transform1.position;
                if (gameManager.IsInverted() != changed) {
                    if (gameManager.IsInverted()) {
                        upAlert.SetActive(true);
                        restoreAlert = upAlert;
                    }
                    else {
                        restoreAlert = downAlert;
                        downAlert.SetActive(true);
                    }

                    changed = gameManager.IsInverted();
                }

                RaycastHit2D hit = Physics2D.Raycast(new Vector2(position.x - 3f, position.y - 1f),
                    Vector2.down);
                if (hit.collider != null) {
                    if (!hit.collider.gameObject.Equals(prevCollision)
                        && (hit.collider.gameObject.tag.Equals("wall")
                            || hit.collider.gameObject.tag.Equals("gravityChange"))) {
                        gameManager.IncrementScore();
                        prevCollision = hit.collider.gameObject;

                        if (hit.collider.gameObject.tag.Equals("gravityChange")) {
                            targetGravityScale = -targetGravityScale;
                            timer = 0;
                        }
                    }
                }

                if (timer < gravityChangeDuration) {
                    float gravityScale = Mathf.Lerp(-targetGravityScale, targetGravityScale,
                        curve.Evaluate(timer / gravityChangeDuration));
                    rb.gravityScale = gravityScale;

                    if (!inverted && gravityScale < 0 || inverted && gravityScale > 0) {
                        inverted = !inverted;
                        audio.PlayOneShot(gravityInversion);
                        gameManager.ChangeBackground(gravityScale < 0);
                        var localScale = transform1.localScale;
                        localScale = new Vector3(localScale.x, -localScale.y);
                        transform1.localScale = localScale;
                        var shape = rocketTrail.shape;
                        shape.alignToDirection = !shape.alignToDirection;
                        transform1.RotateAround(transform1.position, Vector3.forward,
                            transform1.rotation.eulerAngles.z < 0
                                ? 2 * transform1.rotation.eulerAngles.z
                                : -2 * transform1.rotation.eulerAngles.z);
                        flesh.Blink(0.3f);
                        upAlert.SetActive(false);
                        downAlert.SetActive(false);
                        restoreAlert = null;
                    }
                }

                var rotation = transform1.rotation;
                startAngle = rotation.eulerAngles.z > 180
                    ? rotation.eulerAngles.z - 360
                    : rotation.eulerAngles.z;
                rotateTimer += Time.deltaTime;
                var velocity = rb.velocity;
                targetAngle = velocity.y > 0 && !inverted || velocity.y < 0 && inverted ? 30 : -70;
                rotateDuration = velocity.y > 0 && !inverted || velocity.y < 0 && inverted ? 0.3f : 1.5f;
                if (inverted) {
                    targetAngle = -targetAngle;
                }

                var x = velocity.y > 0 && !inverted || velocity.y < 0 && inverted
                    ? Mathf.Lerp(startAngle, targetAngle, rotateTimer / rotateDuration)
                    : Mathf.Lerp(startAngle, targetAngle, rotationCurve.Evaluate(rotateTimer / rotateDuration));

                transform1.rotation = Quaternion.identity;
                transform1.RotateAround(transform1.position, Vector3.forward, x);
            }
        }
    }

    private void CheckJump() {
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.IsActiveGame()) {
            rotateTimer = 0;
            if (!gameManager.IsStarted()) {
                gameManager.StartGame();
                rocketTrail.loop = false;
                rocketTrail.Stop();
                rb.gravityScale = targetGravityScale;
            }

            rocketTrail.Play();
            audio.PlayOneShot(jump);
            rb.velocity = new Vector2(0, thrust * rb.gravityScale);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (gameManager.IsActiveGame()) {
            flesh.Blink(0.08f);
            audio.PlayOneShot(collision);
            targetGravityScale = Mathf.Abs(targetGravityScale);
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            timer = gravityChangeDuration;
            transform.localScale = new Vector3(1, 1);
            restoreAlert = null;
            upAlert.SetActive(false);
            downAlert.SetActive(false);
            changed = false;
            inverted = false;
            var shape = rocketTrail.shape;
            shape.alignToDirection = false;
            gameManager.End();
        }
    }

    private void OnDisable() {
        upAlert.SetActive(false);
        downAlert.SetActive(false);
    }

    private void OnEnable() {
        if (restoreAlert != null) {
            restoreAlert.SetActive(true);
        }
    }
}