using ExhibitClasses;
using System;
using System.Collections.Generic;
using UnityEngine;
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

    // Callback usado ao terminar a localização (segue padrão do sample oficial)
    private Action<bool, UnboundObjectAnchor> _onAnchorLocalized;

    private void Awake()
    {
        _onAnchorLocalized = OnLocalized;
    }
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
            AddSoundToObject(element, artworkData.audioDescriptionUrl);
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
        if (!string.IsNullOrEmpty(artworkData.anchor_uuid))
        {
            LoadAndLocalizeAnchorByUuid(element, artworkData.anchor_uuid);
        }
        return element;
    }
    public async void LoadAndLocalizeAnchorByUuid(GameObject artwork, string uuid)
    {
        
        OVRSpatialAnchor new_anchor = artwork.AddComponent<OVRSpatialAnchor>();
        
        await new_anchor.WhenLocalizedAsync();

        List<Guid> uuids = new List<Guid>();
        uuids.Add(Guid.Parse(uuid));
        // Buffer para LoadUnboundAnchorsAsync
        List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();
        var loaded_unbounds = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, _unboundAnchors);
        
        if (!loaded_unbounds.Success)
        {
            Debug.LogError($"Failed to load unbound anchors with {loaded_unbounds.Status}");
            return;
        }

        foreach (var unbound in loaded_unbounds.Value)
        {
            UnboundObjectAnchor unboundObjectAnchor = new UnboundObjectAnchor
            {
                anchor = unbound,
                anchored_object = artwork
            };

            if (unbound.Localized)
            {

                _onAnchorLocalized(true, unboundObjectAnchor);
            }
            else if (!unbound.Localizing)
            {
                // Inicia localização assíncrona; ao terminar o callback será chamado
                unbound.LocalizeAsync().ContinueWith(_onAnchorLocalized, unboundObjectAnchor);
            }
        }

    }
    private void OnLocalized(bool success, UnboundObjectAnchor unboundAnchor)
    {
        if (!success)
        {
            Debug.LogError("[SpatialAnchorController] Localization falhou para unbound anchor: " + unboundAnchor);
            return;
        }

        // Garante que exista um componente OVRSpatialAnchor neste GameObject para o Bind
        OVRSpatialAnchor anchor_component = unboundAnchor.anchored_object.GetComponent<OVRSpatialAnchor>() ?? gameObject.AddComponent<OVRSpatialAnchor>();

        // Faz o bind da UnboundAnchor ao componente (persistente do serviço)
        unboundAnchor.anchor.BindTo(anchor_component);

        // Atualiza UUID em memória (útil caso tenha vindo da operação de Load)
        ArtworkData artwork_data = unboundAnchor.anchored_object.GetComponent<ArtworkDataController>().GetArtworkData();
        artwork_data.anchor_uuid = unboundAnchor.anchor.Uuid.ToString();

        Debug.Log($"[SpatialAnchorController] Anchor localizada e ligada a este objeto. UUID = {artwork_data.anchor_uuid}");
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
}
