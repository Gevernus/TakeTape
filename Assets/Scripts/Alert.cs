using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : MonoBehaviour {
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public float blinkDuration;

    private float time;
    private CanvasGroup canvas;
    private float min;
    private float max;

    // Start is called before the first frame update
    void Start() {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        min = 0;
        max = 1;
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        canvas.alpha = Mathf.Lerp(min, max, curve.Evaluate(time / blinkDuration));
        if (time > blinkDuration) {
            float temp = max;
            max = min;
            min = temp;
            time = 0.0f;
        }
    }

    private void OnDisable() {
        time = 0;
        canvas.alpha = 0;
    }
}