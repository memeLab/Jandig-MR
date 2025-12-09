
using System.Collections;
using System.IO;
using UnityEngine;
using UnityGLTF;
using UnityEngine.Networking;
using System.Runtime.ExceptionServices;

public class SoundController : MonoBehaviour
{

    // 3D Object URL
    [SerializeField]
    public string m_url;

    [SerializeField]
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
     
        if (m_url != null && m_url != "")
        {
            StartCoroutine(DownloadFile(m_url, Path.GetFileName(m_url)));
        }
        else
        {
            Debug.Log("3D Object URL is nothing.");
            return;
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is not assigned or found on the GameObject.");
            }
        }
    }

    private IEnumerator DownloadFile(string url, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            Debug.Log("File already exists in the path: " + filePath);
             StartCoroutine(LoadSoundFile(filePath));
            yield break;
        }
        else
        {
            Debug.Log("File not exists in the path, downloading from " + url);
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
                    StartCoroutine(LoadSoundFile(filePath));
                }
            }
        }
    }
    private IEnumerator LoadSoundFile(string path)
    {

        Debug.Log("Loading audio file from path: " + path);

        //// Create audioclip from path
        System.Uri uri = new System.Uri(path);

        ////// Define the extension and audio type
        string extension = Path.GetExtension(path).ToLower();
        AudioType audio_type;
        switch (extension)
        {
            case ".wav":
                audio_type = AudioType.WAV;
                break;
            case ".mp3":
                audio_type = AudioType.MPEG;
                break;
            case ".ogg":
                audio_type = AudioType.OGGVORBIS;
                break;
            default:
                Debug.LogError("Unsupported audio format: " + extension);
                yield break;
        }
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(uri, audio_type);

        ////// Send the request and wait for a response
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Audio file load error: " + request.error);
            yield break;
        }

        // Create AudioClip from downloaded data
        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
        audioSource.clip = audioClip;
        audioSource.Play();

    }


}
