using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotoCapture : MonoBehaviour
{
    public Camera photoCamera;
    public RenderTexture renderTexture;

    public GameObject photoModeCanvas;       // Kamera modundayken gösterilen UI (nişangah vb.)
    public GameObject photoReviewPanel;      // Fotoğraf incelenme ekranı (Panel GameObject)
    public RawImage photoDisplay;            // Panel içindeki RawImage (fotoğraf burada gösterilir)
    public KeyCode enterPhotoModeKey = KeyCode.E;
    public KeyCode takePhotoKey = KeyCode.Mouse0;
    public KeyCode exitReviewKey = KeyCode.Space;

    private bool isInPhotoMode = false;
    private bool isReviewingPhoto = false;

    void Update()
    {
        // Fotoğraf moduna gir/çık
        if (Input.GetKeyDown(enterPhotoModeKey) && !isReviewingPhoto)
        {
            isInPhotoMode = !isInPhotoMode;
            photoModeCanvas.SetActive(isInPhotoMode);
            Cursor.visible = false;
        }

        // Fotoğraf çek
        if (isInPhotoMode && Input.GetKeyDown(takePhotoKey) && !isReviewingPhoto)
        {
            StartCoroutine(CaptureAndReviewPhoto());
        }

        // İnceleme ekranından çık ve sonraki levele geç
        if (isReviewingPhoto && Input.GetKeyDown(exitReviewKey))
        {
            ProceedToNextLevel();
        }
    }

    private IEnumerator CaptureAndReviewPhoto()
    {
        isReviewingPhoto = true;

        yield return new WaitForEndOfFrame();

        photoCamera.Render();
        RenderTexture.active = renderTexture;

        Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        photo.Apply();
        RenderTexture.active = null;

        // Kamera modunu kapat
        photoModeCanvas.SetActive(false);
        isInPhotoMode = false;

        // İnceleme ekranını aç, çekilen fotoğrafı göster
        photoReviewPanel.SetActive(true);
        photoDisplay.texture = photo;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ProceedToNextLevel()
    {
        // İnceleme panelini kapat
        photoReviewPanel.SetActive(false);
        isReviewingPhoto = false;

        // (Örnek olarak sahneyi değiştiriyoruz)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
