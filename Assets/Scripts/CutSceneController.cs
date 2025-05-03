using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class CutSceneLine
{
    public string text;
    public AudioClip audioClip;
}

[System.Serializable]
public class CutSceneItem
{
    public Sprite image;
    public CutSceneLine[] lines; // Her görsel için birden fazla yazı+ses olabilir
}

public class CutSceneController : MonoBehaviour
{
    public Image cutsceneImage;
    public TextMeshProUGUI text;
    public AudioSource audioSource;
    public CutSceneItem[] cutsceneItems;
    public float displayTime = 5f; // Her metnin süresi
    public KeyCode skipKey = KeyCode.Space;

    private int itemIndex = 0;
    private int lineIndex = 0;
    private bool isWaitingForNext = false;

    private void Start()
    {
        StartCoroutine(PlayCurrentLine());
    }

    private void Update()
    {
        if (isWaitingForNext && Input.GetKeyDown(skipKey))
        {
            StopAllCoroutines();
            ShowNextLine();
        }
    }

    IEnumerator PlayCurrentLine()
    {
        isWaitingForNext = false;

        if (itemIndex >= cutsceneItems.Length)
        {
            LoadNextScene();
            yield break;
        }

        CutSceneItem currentItem = cutsceneItems[itemIndex];

        if (lineIndex == 0)
            cutsceneImage.sprite = currentItem.image;

        if (lineIndex < currentItem.lines.Length)
        {
            var currentLine = currentItem.lines[lineIndex];
            text.text = currentLine.text;

            if (currentLine.audioClip != null)
            {
                audioSource.clip = currentLine.audioClip;
                audioSource.Play();
            }

            isWaitingForNext = true;
            yield return new WaitForSeconds(displayTime);
            ShowNextLine();
        }
        else
        {
            itemIndex++;
            lineIndex = 0;
            StartCoroutine(PlayCurrentLine());
        }
    }

    private void ShowNextLine()
    {
        lineIndex++;
        StartCoroutine(PlayCurrentLine());
    }

    private void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex == 1 ? 2 : 0);
    }
}