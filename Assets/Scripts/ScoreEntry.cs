using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour {
    public TextMeshProUGUI rank;
    public TextMeshProUGUI name;
    public TextMeshProUGUI score;
    public RawImage background;

    // Start is called before the first frame update
    public void Initialize(string rank, string name, string score, bool withBackground) {
        this.rank.text = rank;
        this.name.text = name;
        this.score.text = score;
        background.enabled = withBackground;
    }
    
    
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}