using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RemixExhibitButtonController : MonoBehaviour
{
    public Button remixExhibitButton;
    public GameObject parentScreen;
    public GameObject onRemixExhibitNextScreen;

    // Start is called before the first frame update
    void Start()
    {
        remixExhibitButton.onClick.AddListener(OnRemixExhibitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnRemixExhibitButtonClick()
    {
        parentScreen.SetActive(false);
        onRemixExhibitNextScreen.SetActive(true);
    }
    }
