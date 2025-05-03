using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    public Camera photoCamera; // Sadece fotoğraf çekmek için geçici kamera
    public RenderTexture renderTexture;
    public RawImage photoDisplayUI; // Canvas'ta gösterilecek yer
    public KeyCode photoKey = KeyCode.Mouse0;

    public float displayDuration = 2f; // Fotoğrafı gösterme süresi

    private bool isPhotoBeingShown = false;

    void Update()
    {
        if (Input.GetKeyDown(photoKey) && !isPhotoBeingShown)
        {
            StartCoroutine(CapturePhoto());
        }
    }

    private IEnumerator CapturePhoto()
    {
        isPhotoBeingShown = true;

        // Fotoğrafı al
        yield return new WaitForEndOfFrame(); // Kare bitimini bekle

        photoCamera.Render(); // Kamera görüntüyü renderlasın

        RenderTexture.active = renderTexture;
        Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        photo.Apply();
        RenderTexture.active = null;

        // UI'da göster
        photoDisplayUI.texture = photo;
        photoDisplayUI.gameObject.SetActive(true);

        // Delilleri bu görselde kontrol etmek istersen burada yap (gelişmiş seviye)

        yield return new WaitForSeconds(displayDuration);

        photoDisplayUI.gameObject.SetActive(false);
        isPhotoBeingShown = false;
    }
}
