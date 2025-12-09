using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.IO;
using Unity.VisualScripting;
using ExhibitClasses;
using System.Collections.Generic;
public class ExhibitControllerScript : MonoBehaviour
{
    [SerializeField]
    public string url = "https://dev.jandig.app/api/v1/exhibits/1/";

    private SpawnObjectsScript objectSpawner;
    public bool shouldSave = false;

    void Start()
    {
        objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<SpawnObjectsScript>();
        if (url != null)
        {
            StartCoroutine(GetArtworks(url, shouldSave));
        }
    }

    IEnumerator GetArtworks(string url, bool shouldSave)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            JObject json = JObject.Parse(jsonResponse);
            JArray artworks = (JArray)json["artworks"];
            JArray augmenteds = (JArray)json["augmenteds"];
            JArray sounds = (JArray)json["sounds"];

            List<ArtworkData> mediaList = new List<ArtworkData>();
            mediaList.AddRange(ExhibitAPIDecoder.DecodeArtworks(artworks));
            mediaList.AddRange(ExhibitAPIDecoder.DecodeObjects(augmenteds));
            mediaList.AddRange(ExhibitAPIDecoder.DecodeSounds(sounds));

            int column = 0;
            int row = 0;
            foreach (ArtworkData artworkData in mediaList)
            {
                Vector3 initial_position = retrieveInitialPosition(column, row);
                
                objectSpawner.spawnObjectInPosition(artworkData, initial_position, Quaternion.identity);

                column++;
                if (column >= 5)
                {
                    column = 0;
                    row++;
                }
            }

            if (shouldSave)
            {
                LocalExhibitManager.saveLocalExhibit(mediaList.ToArray(), (string)json["name"], (string)json["slug"]);
                ObjectVisibilityController.EnableEditorComponents();
            }
        }
    }
    Vector3 retrieveInitialPosition(int column, int row)
    {
        float spacing = 1.2f; // 1 (size) + 0.2 (spacing)
        float initial_distance = 1.0f;
        float height = 0f;
        return new Vector3(column * spacing - 1, height, row*spacing + initial_distance);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
