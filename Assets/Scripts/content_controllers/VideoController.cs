using System.Collections;
using System.Collections.Generic;
using System.IO;
using ThreeDISevenZeroR.UnityGifDecoder;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
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
    private VideoPlayer m_video_player;

    [SerializeField]
    private AudioSource m_audio_source;

    // Video URL
    [SerializeField]
    public string m_video_url;
    
    [SerializeField]
    public GameObject m_video_backplate;

    // Start is called before the first frame update
    void Start()
    {
        if (m_video_player == null)
        {
            m_video_player = GetComponent<VideoPlayer>();
        }
        
        if (m_video_url != null)
        {
            StartCoroutine(DownloadFile(m_video_url, Path.GetFileName(m_video_url)));
        }
        else
        {
            Debug.LogError("Video URL is nothing.");
        }
    }

    private IEnumerator DownloadFile(string url, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            Debug.Log("File already exists in the path: " + filePath);
            StartCoroutine(playVideo(filePath));
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
                    StartCoroutine(playVideo(filePath));
                }
            }
        }
    }
    private IEnumerator playVideo(string path)
    {
        if (File.Exists(path))
        {
            RenderTexture renderTexture = new RenderTexture(1024, 1024, 0);
            renderTexture.Create();
            m_video_player.targetTexture = renderTexture;

            GameObject backplate = Instantiate(m_video_backplate, Vector3.zero, Quaternion.identity);
            backplate.transform.SetParent(transform, false);
            Shader shader = Shader.Find("URP/UI/Prerendered");
            if (shader == null)
            {
                Debug.LogError("Video Shader not found!!!!");
            }
            Material videoMat = new (shader);
            backplate.GetComponent<Renderer>().material = videoMat;
            backplate.GetComponent<Renderer>().material.mainTexture = renderTexture;
            m_video_player.url = path;
            m_video_player.Prepare();
            while (!m_video_player.isPrepared)
            {
                yield return null;
            }
            m_video_player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_video_player.SetTargetAudioSource(0, m_audio_source);
            m_video_player.Play();
        }
        else
        {
            Debug.LogError("Video file does not exist at path: " + path);
        }
    }
}
