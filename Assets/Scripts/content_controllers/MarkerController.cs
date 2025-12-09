
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MarkerController : MonoBehaviour
{

    // Target row image
    [SerializeField]
    private Image m_image;

    // Marker URL
    [SerializeField]
    public string m_markerURL;


    private void Start()
    {
        if (m_image == null)
        {
            m_image = GetComponent<Image>();
        }
        if (m_markerURL != null)
        {
            StartCoroutine(DownloadFile(m_markerURL, Path.GetFileName(m_markerURL)));
        }
        else
        {
            Debug.LogError("Marker URL is nothing.");
        }
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
    }

    private IEnumerator DownloadFile(string url, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            Debug.Log("File already exists in the path: " + filePath);
            StartCoroutine(LoadMarker(filePath));
            yield break;
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("File download error: " + www.error);
                }
                else
                {
                    File.WriteAllBytes(filePath, www.downloadHandler.data);
                    Debug.Log("File successfully downloaded and saved to: " + filePath);
                    StartCoroutine(LoadMarker(filePath));
                }
            }
        }
    }
    private IEnumerator LoadMarker(string path)
    {
        Debug.Log("Loading Marker image...");
        Debug.Log("Path: " + path);

        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new(2, 2);
        if (texture.LoadImage(imageData))
        {
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            if (m_image != null)
            {
                m_image.sprite = sprite;
                Debug.Log("Marker image successfully loaded and applied.");
                yield break;
            }
            else
            {
                Debug.LogError("Image component is null. Cannot apply the sprite.");
            }
        }
        else
        {
            Debug.LogError("Failed to load image data into texture.");
        }
    }
}