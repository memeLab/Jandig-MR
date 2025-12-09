using System.Collections.Generic;
using UnityEngine;
using static OVRSpatialAnchor;
namespace ExhibitClasses
{
    public enum ArtworkType
    {
        Gif,
        Video,
        MarkerGif,
        MarkerVideo,
        GLB,
        Sound
    }

    [System.Serializable]
    public class ArtworkData
    {
        public ArtworkType type;       // e.g., "gif", "video", "markerGif", "markerVideo", "glb", "sound"
        public string sourceUrl;  // The URL for the content (e.g., video, GLB, GIF)
        public string markerUrl;  // Optional (only for marker-based artworks)
        public string audioDescriptionUrl; // Optional audio description URL
        public string soundUrl; // Optional sound URL for extra object sounds. E.g., glb with sound effects
        public string anchor_uuid;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
    public class UnboundObjectAnchor
    {
        public UnboundAnchor anchor;
        public GameObject anchored_object;

    }
    [System.Serializable]
    public class ExhibitInfo
    {
        public string name;
        public int id;
        public ArtworkData[] artworks;
    }

    [System.Serializable]
    public class ExhibitEntry
    {
        public string key;
        public ExhibitInfo value;
    }
    [System.Serializable]
    public class Augmented
    {
        public string source;
    }

    [System.Serializable]
    public class Marker
    {
        public string source;
    }

    [System.Serializable]
    public class Artwork
    {
        public string title;
        public Marker marker;
        public Augmented augmented;
    }

    [System.Serializable]
    public class Exhibit
    {
        public int id;
        public int owner;
        public string name;
        public string slug;
        public Artwork[] artworks;
        public Augmented[] augmenteds;
    }

    [System.Serializable]
    public class ListExhibitResponse
    {
        public int count;
        public string next;
        public string previous;
        public Exhibit[] results;
    }
    [System.Serializable]
    public class UserInfo
    {
        public string userId;
        public string username;
        public string userProfileId;
    }

    [System.Serializable]
    public class LocalExhibits
    {
        public string defaultExhibitSlug;
        public List<ExhibitEntry> exhibits = new List<ExhibitEntry>();
        public ExhibitInfo GetLocalExhibitInfo(string exhibitSlug)
        {
            foreach (var exhibit in exhibits)
            {
                if (exhibit.key == exhibitSlug) { return exhibit.value; }
            }
            return null;
        }

        public void SetLocalExhibitInfo(ExhibitEntry entry)
        {
            foreach(var exhibit in exhibits)
            {
                if (exhibit.key == entry.key)
                {
                    exhibit.value = entry.value;
                    return;
                }
            }

            exhibits.Add(entry);

        }

    }
}