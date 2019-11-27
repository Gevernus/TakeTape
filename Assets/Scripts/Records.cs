using System;
using System.Collections.Generic;
using UnityEngine;

public class Records : MonoBehaviour {
    public Transform recordsPanel;
    public GameObject scoreEntry;
    private List<string> records;
    private string path;
    private CanvasGroup panel;
    private AudioSource audio;

    void Start() {
        path = Application.dataPath + "/records.txt";
        //TODO: real file path?
        //records = ScoreManager.ReadRecords(path);
        records = new List<string>();
        panel = GetComponent<CanvasGroup>();
        audio = GetComponent<AudioSource>();
    }

    private void Update() {
        if (panel.alpha > 0 && Input.GetKeyDown(KeyCode.Escape)) {
            Back();
        }
    }

    public void Initialize() {
        records.Sort((a, b) => Int32.Parse(b).CompareTo(Int32.Parse(a)));
        var enumerator = records.GetEnumerator();
        for (int i = 0; i < 10; i++) {
            var entry = Instantiate(scoreEntry, recordsPanel.position, Quaternion.identity, recordsPanel);
            if (enumerator.MoveNext() && enumerator.Current != null) {
                entry.GetComponent<ScoreEntry>()
                    .Initialize(getRankString(i + 1), "", enumerator.Current, i % 2 == 0);
            }
            else {
                entry.GetComponent<ScoreEntry>()
                    .Initialize(getRankString(i + 1), "", enumerator.Current, i % 2 == 0);
            }
        }

        enumerator.Dispose();
    }

    private void OnDestroy() {
        records.Sort((a, b) => Int32.Parse(b).CompareTo(Int32.Parse(a)));
        //ScoreManager.SaveRecords(path, records);
    }

    public void AddRecord(string score) {
        records.Add(score);
    }

    private string getRankString(int rank) {
        switch (rank) {
            default:
                return rank + "th";
            case 1:
                return "1st";
            case 2:
                return "2nd";
            case 3:
                return "3rd";
        }
    }

    public void Back() {
        audio.PlayOneShot(audio.clip);
        foreach (Transform child in recordsPanel.transform) {
            Destroy(child.gameObject);
        }

        panel.alpha = 0;
        panel.blocksRaycasts = false;
    }
}