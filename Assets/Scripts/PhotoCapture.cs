using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotoCapture : MonoBehaviour
{
    public Camera photoCamera;
    public RenderTexture renderTexture;

    public GameObject photoModeCanvas;
    public GameObject photoReviewPanel;
    public RawImage photoDisplay;

    public KeyCode enterPhotoModeKey = KeyCode.E;
    public KeyCode takePhotoKey = KeyCode.Mouse0;
    public KeyCode exitReviewKey = KeyCode.Space;

    [Header("Photo Capture")]
    public float maxPhotoDistance = 20f;
    public List<GameObject> targetObjects;

    [Header("Photo Feedback")]
    public AudioClip shutterSound;
    public GameObject photoFlashEffect;

    [Header("Review Info")]
    public GameObject retryPanel;
    public TextMeshProUGUI retryInfoText;
    
    private bool isInPhotoMode = false;
    private bool isReviewingPhoto = false;

    private Camera playerCamera;
    private List<GameObject> missedEvidence = new List<GameObject>();

    private bool waitingForRetry = false;
    void Start()
    {
        playerCamera = Camera.main;
        Cursor.visible = false;
        retryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(enterPhotoModeKey) && !isReviewingPhoto)
        {
            isInPhotoMode = !isInPhotoMode;
            photoModeCanvas.SetActive(isInPhotoMode);
        }

        if (isInPhotoMode && Input.GetKeyDown(takePhotoKey) && !isReviewingPhoto)
        {
            StartCoroutine(CaptureAndReviewPhoto());
        }

        if (isReviewingPhoto && Input.GetKeyDown(exitReviewKey))
        {
            FinalizeReview();
        }
        
        if (waitingForRetry && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator CaptureAndReviewPhoto()
    {
        isReviewingPhoto = true;

        if (shutterSound) AudioSource.PlayClipAtPoint(shutterSound, transform.position);
        yield return new WaitForSeconds(0.1f);
        if (photoFlashEffect) photoFlashEffect.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();

        photoCamera.Render();
        RenderTexture.active = renderTexture;

        Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        photo.Apply();
        RenderTexture.active = null;

        photoModeCanvas.SetActive(false);
        isInPhotoMode = false;

        photoReviewPanel.SetActive(true);
        photoDisplay.texture = photo;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EvaluatePhoto();
    }

    private void EvaluatePhoto()
    {
        missedEvidence.Clear();

        foreach (GameObject obj in targetObjects)
        {
            if (!IsObjectSeen(obj))
            {
                missedEvidence.Add(obj);
            }
        }

        if (missedEvidence.Count == 0)
        {
            Debug.Log("📸 Perfect shot! All evidence captured.");
        }
        else
        {
            Debug.Log("❌ Missed objects: " + string.Join(", ", missedEvidence.ConvertAll(o => o.name)));
        }
    }

    private void FinalizeReview()
    {
        photoReviewPanel.SetActive(false);
        isReviewingPhoto = false;

        if (missedEvidence.Count == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            retryPanel.SetActive(true);
            retryInfoText.text = $"Eksik delil sayısı: {missedEvidence.Count}";
            waitingForRetry = true; // Bayrağı aç
        }
    }

    public bool IsObjectSeen(GameObject target)
    {
        if (target == null || playerCamera == null)
            return false;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null)
            return false;

        Vector3 viewportPos = playerCamera.WorldToViewportPoint(renderer.bounds.center);
        if (viewportPos.z <= 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
            return false;

        Vector3 direction = (renderer.bounds.center - playerCamera.transform.position).normalized;
        float distance = Vector3.Distance(playerCamera.transform.position, renderer.bounds.center);

        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject != target)
                return false;
        }

        return true;
    }
}
