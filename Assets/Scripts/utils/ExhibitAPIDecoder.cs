using UnityEngine;
using ExhibitClasses;

using System.Collections.Generic;

using Newtonsoft.Json.Linq;
public class ExhibitAPIDecoder : MonoBehaviour
{

    public static List<ArtworkData> DecodeSounds(JArray sounds)
    {
        List<ArtworkData> artworkDataList = new List<ArtworkData>();
        foreach (JObject sound in sounds)
        {
            string source = sound["file"].ToString();

            ArtworkData artworkData = new ArtworkData();
            artworkData.sourceUrl = source;
            artworkData.type = ArtworkType.Sound;

            artworkDataList.Add(artworkData);
        }
        return artworkDataList;
    }
    public static List<ArtworkData> DecodeObjects(JArray augmenteds)
    {
        List<ArtworkData> artworkDataList = new List<ArtworkData>();
        foreach (JObject augmented in augmenteds)
        {
            string source = augmented["source"].ToString();

            ArtworkType? type = DetermineAugmentedType(source);
            if (type == null)
            {
                Debug.LogWarning("Skipping object with unsupported type: " + source);
                continue;
            }

            ArtworkData artworkData = new ArtworkData();
            artworkData.sourceUrl = source;
            artworkData.type = type.Value;

            if (augmented["audio_description"].Type != JTokenType.Null)
            {
                string soundUrl = augmented["audio_description"].ToString();
                artworkData.audioDescriptionUrl = soundUrl;
            }
            if (augmented["sound"].Type != JTokenType.Null)
            {
                string soundUrl = augmented["sound"]["file"].ToString();
                artworkData.soundUrl = soundUrl;
            }
            artworkDataList.Add(artworkData);
        }
        return artworkDataList;
    }
    public static List<ArtworkData> DecodeArtworks(JArray artworks)
    {
        List<ArtworkData> artworkDataList = new List<ArtworkData>();
        foreach (JObject artwork in artworks)
        {
            ArtworkData artworkData = new ArtworkData();
            artworkData.sourceUrl = artwork["augmented"]["source"].ToString();
            artworkData.markerUrl = artwork["marker"]["source"].ToString();

            ArtworkType? type = DetermineArtworkType(artworkData.sourceUrl);
            if (type == null)
            {
                Debug.LogWarning("Skipping artwork with unsupported type: " + artworkData.sourceUrl);
                continue;
            }
            artworkData.type = type.Value;

            if (artwork["augmented"]["audio_description"].Type != JTokenType.Null)
            {
                string soundUrl = artwork["augmented"]["audio_description"].ToString();
                artworkData.audioDescriptionUrl = soundUrl;
            }
            if (artwork["augmented"]["sound"].Type != JTokenType.Null)
            {
                string soundUrl = artwork["augmented"]["sound"]["file"].ToString();
                artworkData.soundUrl = soundUrl;
            }
            artworkDataList.Add(artworkData);
        }
        return artworkDataList;
    }

    // Returns null for an extension the app cannot play, so the caller skips that
    // one artwork instead of aborting the whole exhibit.
    private static ArtworkType? DetermineArtworkType(string sourceUrl)
    {
        if (sourceUrl.EndsWith(".gif"))
        {
            return ArtworkType.MarkerGif;
        }
        else if (sourceUrl.EndsWith(".mp4") || sourceUrl.EndsWith(".webm"))
        {
            return ArtworkType.MarkerVideo;
        }
        else if (sourceUrl.EndsWith(".glb"))
        {
            return ArtworkType.GLB;
        }
        else if (sourceUrl.EndsWith(".mp3") || sourceUrl.EndsWith(".wav"))
        {
            return ArtworkType.Sound;
        }
        return null;
    }

    private static ArtworkType? DetermineAugmentedType(string sourceUrl)
    {
        if (sourceUrl.EndsWith(".gif"))
        {
            return ArtworkType.Gif;
        }
        else if (sourceUrl.EndsWith(".mp4") || sourceUrl.EndsWith(".webm"))
        {
            return ArtworkType.Video;
        }
        else if (sourceUrl.EndsWith(".glb"))
        {
            return ArtworkType.GLB;
        }
        else if (sourceUrl.EndsWith(".mp3") || sourceUrl.EndsWith(".wav"))
        {
            return ArtworkType.Sound;
        }
        return null;
    }
}
