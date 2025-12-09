using ExhibitClasses;
using System;
using System.IO;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class LocalExhibitManager : MonoBehaviour
{
    public static LocalExhibits loadLocalExhibits()
    {
        string path = Path.Combine(Application.persistentDataPath, "localExhibits.json");
        Debug.Log("Loading local exhibits from: " + path);

        if (!File.Exists(path))
        {
            Debug.LogWarning("No local exhibits file found.");
            return new LocalExhibits();
        }

        try
        {
            string json = File.ReadAllText(path);
            LocalExhibits localExhibits = JsonUtility.FromJson<LocalExhibits>(json);

            if (localExhibits == null)
            {
                Debug.LogWarning("File found but couldn't parse. Creating empty dataset.");
                return new LocalExhibits();
            }
            else
            {
                return localExhibits;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading local exhibits: " + ex.Message);
            return new LocalExhibits();
        }
    }

    public static void persistToDisk(LocalExhibits localExhibits)
    {
        System.IO.File.WriteAllText(Application.persistentDataPath + "/localExhibits.json", JsonUtility.ToJson(localExhibits));
    }
    public static ArtworkData[] getCurrentUpdatedArtworkData()
    {
        var artworkObjects = GameObject.FindGameObjectsWithTag("Artwork");
        var artworks = new ArtworkData[artworkObjects.Length];

        for (int i = 0; i < artworkObjects.Length; i++)
        {
            var controller = artworkObjects[i].GetComponent<ArtworkDataController>();
            if (controller != null)
            {
                Debug.Log("Getting artwork data for object: " + artworkObjects[i].name);
                artworks[i] = controller.GetArtworkData();
                artworks[i].position = artworkObjects[i].transform.position;
                artworks[i].rotation = artworkObjects[i].transform.rotation;
                artworks[i].scale = artworkObjects[i].transform.localScale;
            }
        }

        return artworks;
    }
    public static void saveLocalExhibit(ArtworkData[] artworks, string name, string slug)
    {
        LocalExhibits localExhibits = loadLocalExhibits();
        ExhibitInfo exhibitInfo = new ExhibitInfo
        {
            name = name,
            artworks = artworks
        };
        localExhibits.SetLocalExhibitInfo(new ExhibitEntry
        {
            key = slug,
            value = exhibitInfo
        });
        persistToDisk(localExhibits);
    }
    public static void saveDefaultLocalExhibit(string exhibitSlug)
    {
        LocalExhibits localExhibits = loadLocalExhibits();
        localExhibits.defaultExhibitSlug = exhibitSlug;
        persistToDisk(localExhibits);
    }
    public static void loadLocalExhibit(string exhibitSlug)
    {
        if (string.IsNullOrEmpty(exhibitSlug))
        {
            return;
        }
        ObjectVisibilityController.ClearObjects();
        LocalExhibits localExhibits = loadLocalExhibits();
        ExhibitInfo info = localExhibits.GetLocalExhibitInfo(exhibitSlug);
        if (info == null)
        {
            Debug.LogError("No local exhibit found with slug: " + exhibitSlug);
            return;
        }

        ExhibitEntry exhibit = new ExhibitEntry
        {
            key = exhibitSlug,
            value = info
        };
        SpawnObjectsScript objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<SpawnObjectsScript>();
        SpawnObjectsScript.OpenLocalExhibit(objectSpawner, exhibit);
        ObjectVisibilityController.DisableEditorComponents();
    }
}
