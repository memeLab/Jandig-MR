

using UnityEngine;

public class AudioDescriptionController : SoundController
{
    public AudioDescriptionController()
    {
        this.shouldPlayOnLoad = false;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "CenterEyeAnchor")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("StartingAudioDescription");
            this.audioSource.Play();
        }
    }
}
