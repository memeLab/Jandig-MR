using System.Collections;
using UnityEngine;
using ExhibitClasses;

public class ArtworkDataController : MonoBehaviour
{
    public ArtworkData artwork_data;
        
    public void SetArtworkData(ArtworkData data)
    {
        artwork_data = data;
    }
    public ArtworkData GetArtworkData()
    {
        return artwork_data;
    }

}
