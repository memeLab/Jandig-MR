using System.Collections;
using System.Collections.Generic;
using System.IO;
using ThreeDISevenZeroR.UnityGifDecoder;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GifController : MonoBehaviour
{
    public enum State
    {
        None,
        Loading,
        Playing,
        Pause,
    }
    // Target row image
    [SerializeField]
    private RawImage m_rawImage;

    // GIF URL
    [SerializeField]
    public string m_gifUrl;

    // Decoded GIF texture list
    private List<Texture> m_gifTextureList;

    private List<float> m_gifDelayList;

    // Delay time
    private float m_delayTime;

    // Texture index
    private int m_gifTextureIndex;
    public State nowState
    {
        get;
        private set;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (m_rawImage == null)
        {
            m_rawImage = GetComponent<RawImage>();
        }
        nowState = State.Loading;
        if (m_gifUrl != null)
        {
            StartCoroutine(DownloadFile(m_gifUrl, Path.GetFileName(m_gifUrl)));
        }
        else
        {
            Debug.LogError("GIF URL is nothing.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (nowState)
        {
            case State.None:
                break;

            case State.Loading:
                break;

            case State.Playing:
                if (m_rawImage == null || m_gifTextureList == null || m_gifTextureList.Count <= 0)
                {
                    return;
                }
                if (m_delayTime > Time.time)
                {
                    return;
                }
                // Change texture
                m_gifTextureIndex++;
                if (m_gifTextureIndex >= m_gifTextureList.Count)
                {
                    m_gifTextureIndex = 0;
                }
                m_rawImage.texture = m_gifTextureList[m_gifTextureIndex];
                m_delayTime = Time.time + m_gifDelayList[m_gifTextureIndex];
                break;

            case State.Pause:
                break;

            default:
                break;
        }
    }
    private IEnumerator DownloadFile(string url, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            Debug.Log("File already exists in the path: " + filePath);
            StartCoroutine(LoadGifTextures(filePath));
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
                    StartCoroutine(LoadGifTextures(filePath));
                }
            }
        }
    }
    private IEnumerator LoadGifTextures(string path)
    {
        var frames = new List<Texture>();
        var frameDelays = new List<float>();
        byte[] file_bytes = File.ReadAllBytes(path);
        using (var gifStream = new GifStream(file_bytes))
        {
            while (gifStream.HasMoreData)
            {
                switch (gifStream.CurrentToken)
                {
                    case GifStream.Token.Image:
                        var image = gifStream.ReadImage();
                        var frame = new Texture2D(
                            gifStream.Header.width,
                            gifStream.Header.height,
                            TextureFormat.ARGB32, false);

                        frame.SetPixels32(image.colors);
                        frame.Apply();

                        frames.Add(frame);
                        frameDelays.Add(image.SafeDelaySeconds); // More about SafeDelay below
                        break;

                    case GifStream.Token.Comment:
                        var commentText = gifStream.ReadComment();
                        Debug.Log(commentText);
                        break;

                    default:
                        gifStream.SkipToken(); // Other tokens
                        break;
                }
            }
        }
        m_gifTextureList = frames;
        m_gifDelayList = frameDelays;
        nowState = State.Playing;
        yield break;
    }
}
