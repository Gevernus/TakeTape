using UnityEngine;

public class Destroyer : MonoBehaviour {
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnCollisionEnter2D(Collision2D other) {
        gameManager.DestroyWall(other.transform);
    }
}