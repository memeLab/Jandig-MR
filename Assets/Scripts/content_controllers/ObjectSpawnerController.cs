using UnityEngine;

using ExhibitClasses;
public class SpawnObjectsScript : MonoBehaviour
{
    [SerializeField]
    private GameObject markerGifPrefab;

    [SerializeField]
    private GameObject markerVideoPrefab;

    [SerializeField]
    private GameObject gifPrefab;

    [SerializeField]
    private GameObject GLBPrefab;

    [SerializeField]
    private GameObject videoPrefab;

    [SerializeField]
    private GameObject soundPrefab;

    [SerializeField]
    private GameObject soundComponentPrefab;

    [SerializeField]
    private GameObject audioDescriptionComponentPrefab;

    public static void OpenLocalExhibit(SpawnObjectsScript objectSpawner, ExhibitEntry exhibitEntry)
    {
        ObjectVisibilityController.ClearObjects();
        Debug.Log("Opening local exhibit: " + exhibitEntry.key);

        // Spawn each artwork
        foreach (var art in exhibitEntry.value.artworks)
        {
            objectSpawner.spawnObject(art);
        }
        ObjectVisibilityController.DisableEditorComponents();
    }

    public void spawnObject(ArtworkData artworkData)
    {
        spawnObjectInPosition(artworkData, artworkData.position, artworkData.rotation);
    }
    public GameObject spawnObjectInPosition(ArtworkData artworkData, Vector3 position, Quaternion rotation)
    {
        GameObject element = null;
        switch (artworkData.type)
        {
            case ArtworkType.Gif:
                element = createGifPrefab(artworkData.sourceUrl);
                break;
            case ArtworkType.MarkerGif:
                element = createMarkerGifPrefab(artworkData.sourceUrl, artworkData.markerUrl);
                break;
            case ArtworkType.GLB:
                element = createGLBPrefab(artworkData.sourceUrl);
                break;
            case ArtworkType.MarkerVideo:
                element = createMarkerVideoPrefab(artworkData.sourceUrl, artworkData.markerUrl);
                break;
            case ArtworkType.Video:
                element = createVideoPrefab(artworkData.sourceUrl);
                break;
            case ArtworkType.Sound:
                element = createSoundPrefab(artworkData.sourceUrl);
                break;
            default:
                Debug.LogError("Unsupported artwork type: " + artworkData.type);
                break;
        }
        if (element != null && artworkData.soundUrl != null)
        {
            AddSoundToObject(element, artworkData.soundUrl);
        }
        if (element != null && artworkData.audioDescriptionUrl != null)
        {
            AddAudioDescriptionToObject(element, artworkData.audioDescriptionUrl);
        }
        
        if (position != null)
        {
            element.transform.position = position;
        }
        else
        {
            element.transform.position = new Vector3(0, 0, 0);
        }
        
        if (rotation != null)
        {
            element.transform.rotation = rotation;
        }
        else
        {
            element.transform.rotation = Quaternion.identity;
        }

        artworkData.position = element.transform.position;
        artworkData.rotation = element.transform.rotation;
        
        element.AddComponent<ArtworkDataController>();
        ArtworkDataController artworkDataController = element.GetComponent<ArtworkDataController>();
        artworkDataController.SetArtworkData(artworkData);

        ObjectVisibilityController.registerEditableObject(element);
        return element;
    }
    GameObject createGifPrefab(string source)
    {
        GameObject gifObject = Instantiate(gifPrefab);
        GifController gifController = gifObject.GetComponentInChildren<GifController>();
        if (gifController != null)
        {
            gifController.m_gifUrl = source;
        }
        return gifObject;
    }
    GameObject createMarkerGifPrefab(string gifUrl,string markerSource)
    {
        GameObject gifObject = Instantiate(markerGifPrefab);

        GifController gifController = gifObject.GetComponentInChildren<GifController>();
        MarkerController markerController = gifObject.GetComponentInChildren<MarkerController>();
        if (gifController != null)
        {
            gifController.m_gifUrl = gifUrl;
        }
        if (markerController != null)
        {
            markerController.m_markerURL = markerSource;
        }
        return gifObject;
    }

    GameObject createGLBPrefab(string source)
    {
        GameObject glbObject = Instantiate(GLBPrefab);
        Object3DController object3DController = glbObject.GetComponentInChildren<Object3DController>();
        if (object3DController != null)
        {
            object3DController.m_url = source;
        }
        return glbObject;
    }

    GameObject createMarkerVideoPrefab(string videoUrl, string markerSource)
    {
        GameObject videoObject = Instantiate(markerVideoPrefab);
        VideoController videoController = videoObject.GetComponentInChildren<VideoController>();
        MarkerController markerController = videoObject.GetComponentInChildren<MarkerController>();
        if (videoController != null)
        {
            videoController.m_video_url = videoUrl;
        }
        if (markerController != null)
        {
            markerController.m_markerURL = markerSource;
        }
        return videoObject;
    }

    GameObject createVideoPrefab(string videoUrl)
    {
        GameObject videoObject = Instantiate(videoPrefab);
        VideoController videoController = videoObject.GetComponentInChildren<VideoController>();
        if (videoController != null)
        {
            videoController.m_video_url = videoUrl;
        }
        return videoObject;
    }

    GameObject createSoundPrefab(string soundUrl)
    {
        GameObject soundObject = Instantiate(soundPrefab);
        SoundController soundController = soundObject.GetComponentInChildren<SoundController>();
        if (soundController != null)
        {
            soundController.m_url = soundUrl;
        }
        else
        {
            Debug.LogError("SoundController component not found in the SoundPrefab.");
        }
        return soundObject;

    }
    void AddSoundToObject(GameObject obj, string soundUrl)
    {
        GameObject soundComponent = Instantiate(soundComponentPrefab);
        SoundController soundController = soundComponent.GetComponentInChildren<SoundController>();
        if (soundComponent != null)
        {
            soundController.m_url = soundUrl;        
        }
        soundComponent.transform.SetParent(obj.transform, worldPositionStays: false);
    }
    void AddAudioDescriptionToObject(GameObject obj, string soundUrl)
    {
        GameObject soundComponent = Instantiate(audioDescriptionComponentPrefab);
        AudioDescriptionController audioDescriptionController = soundComponent.GetComponentInChildren<AudioDescriptionController>();
        if (soundComponent != null)
        {
            audioDescriptionController.m_url = soundUrl;
        }
        soundComponent.transform.SetParent(obj.transform, worldPositionStays: false);
    }
}
