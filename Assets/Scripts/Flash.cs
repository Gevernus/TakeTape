using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour {
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    
    private float blinkDuration;
    private float time;
    private CanvasGroup canvas;
    private float min;
    private float max;


    // Start is called before the first frame update
    void Start() {
        canvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        canvas.alpha = Mathf.Lerp(min, max, curve.Evaluate(time / blinkDuration));
        if (time > blinkDuration) {
            if (max < min) {
                max = min = 0;
            }

            float temp = max;
            max = min;
            min = temp;
            time = 0.0f;
        }
    }

    public void Blink(float duration) {
        canvas.alpha = 0;
        min = 0;
        max = 1;
        time = 0;
        blinkDuration = duration;
    }
}