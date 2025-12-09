using ExhibitClasses;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameLocalExhibitController : MonoBehaviour
{
  
    private Exhibit exhibit;

    public MenuVisibilityScript menuController;
    public GameObject parentScreen;
    public GameObject confirmationScreen;
    public GameObject onSaveScreen;
    public GameObject onSaveErrorScreen;
    public GameObject onCancelScreen;
    public GameObject exhibitNameInput;
    public Button confirmRemixButton;
    public Button cancelRemixButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirmRemixButton.onClick.AddListener(OnConfirmRemixButton);
        cancelRemixButton.onClick.AddListener(OnCancelRemix);
    }

    public void SetExhibit(Exhibit exhibit)
    {
        this.exhibit = exhibit;
        this.exhibitNameInput.GetComponentInChildren<TMP_InputField>().text = exhibit.slug;
    }


    public void OnConfirmRemixButton()
    {
        string exhibitName = this.exhibitNameInput.GetComponentInChildren<TMP_InputField>().text;
        this.exhibit.slug = exhibitName;
        LocalExhibits localExhibits = LocalExhibitManager.loadLocalExhibits();
        if (localExhibits.GetLocalExhibitInfo(exhibitName) == null)
        {
            this.goToEditExhibit(exhibitName);
        } else
        {
            // An exhibit with this name already exists
            // Show Confirmation Screen
            parentScreen.SetActive(false);
            confirmationScreen.SetActive(true);
        }
    }
    private void goToEditExhibit(string exhibitName)
    {
        Debug.Log("Valid exhibit name");
        this.parentScreen.SetActive(false);
        this.onSaveScreen.SetActive(true);
        this.onSaveScreen.GetComponent<LocalExhibitMenuController>().onExitScreen = onCancelScreen;
        this.onSaveScreen.GetComponent<LocalExhibitMenuController>().startExhibit(this.exhibit);
        this.onSaveScreen.GetComponent<LocalExhibitMenuController>().editMode();
        this.menuController.HideMenu();
    }

    public void OnSuccessCallback()
    {
        string exhibitName = this.exhibitNameInput.GetComponentInChildren<TMP_InputField>().text;
        this.goToEditExhibit(exhibitName);
    }
    public void OnFailCallback()
    {
        parentScreen.SetActive(true);
    }
    public void OnCancelRemix()
    {
        this.parentScreen.SetActive(false);
        this.onSaveScreen.GetComponent<LocalExhibitMenuController>().SetExhibit(null);
        this.onCancelScreen.SetActive(true);
    }
}
