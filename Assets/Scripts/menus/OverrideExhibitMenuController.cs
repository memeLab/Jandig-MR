using System;
using UnityEngine;

using UnityEngine.UI;
public class OverrideExhibitMenuController : MonoBehaviour
{

    public NameLocalExhibitController previousScreen;

    public Button YesButton;
    public Button CancelButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        YesButton.onClick.AddListener(OnConfirmOverride);
        CancelButton.onClick.AddListener(OnCancelOverride);
    }

    public void OnCancelOverride()
    {
        
        this.gameObject.SetActive(false);
        this.previousScreen.OnFailCallback();
    }
    public void OnConfirmOverride()
    {
        this.gameObject.SetActive(false);
        this.previousScreen.OnSuccessCallback();
    }
}
