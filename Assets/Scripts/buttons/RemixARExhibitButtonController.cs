using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RemixARExhibitButtonController : MonoBehaviour
{
    public GameObject parentScreen;
    public GameObject onRemixARExhibitNextScreen;

    public Button remixARExhibitButton;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnRemixARExhibitButtonClick()
    {
        onRemixARExhibitNextScreen.GetComponent<ListARExhibitsController>().Activate("https://dev.jandig.app/api/v1/exhibits/");
        parentScreen.SetActive(false);
    }
}
