using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public CanvasGroup recordsPanel;
    public GameManager manager;
    public RawImage soundCross;
    public GameObject backgroundUI;

    private CanvasGroup menu;
    private AudioSource audio;

    void Start() {
        menu = GetComponent<CanvasGroup>();
        audio = GetComponent<AudioSource>();
    }

    public void Play() {
        audio.PlayOneShot(audio.clip);
        menu.alpha = 0;
        menu.interactable = false;
        backgroundUI.SetActive(false);
        if (!manager.IsStarted()) {
            manager.Restart();
        }

        manager.Pause(false);
    }

    public void OpenRecords() {
        audio.PlayOneShot(audio.clip);
        recordsPanel.GetComponent<Records>().Initialize();
        recordsPanel.alpha = 1;
        recordsPanel.blocksRaycasts = true;
    }

    public void ChangeSoundMode() {
        audio.PlayOneShot(audio.clip);
        AudioListener.pause = !AudioListener.pause;
        AudioListener.volume = AudioListener.pause ? 0 : 1;
        soundCross.enabled = !soundCross.enabled;
    }

    public void Exit() {
        audio.PlayOneShot(audio.clip);
        Application.Quit();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (manager.IsPaused()) {
                if (manager.IsStarted()) {
                    Play();
                }
            }
            else {
                backgroundUI.SetActive(true);
                manager.Pause(true);
                menu.interactable = true;
                menu.alpha = 1;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && !manager.IsActiveGame()) {
            Play();
        }
    }
}