using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExhibitClasses;
public class OpenExhibitButtonController : MonoBehaviour
{
    public GameObject exhibit_controller_prefab;
    public GameObject parentScreen;
    public GameObject nameLocalExhibitScreen;
    public Button openExhibitButton;

    private Exhibit exhibit;

    // Start is called before the first frame update
    void Start()
    {
        openExhibitButton.onClick.AddListener(OnExhibitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParentScreen(GameObject parentScreen)
    {
        this.parentScreen = parentScreen;
    }

    public void SetNameLocalExhibitScreen(GameObject nameLocalExhibitScreen)
    {
        this.nameLocalExhibitScreen = nameLocalExhibitScreen;
    }

    public void SetExhibit(Exhibit exhibit)
    {
        this.exhibit = exhibit;

        GameObject label = transform.Find("Content").Find("Background").Find("Elements").Find("Text").Find("Label").gameObject;
        label.GetComponent<TextMeshProUGUI>().text = exhibit.slug;
    }

    private void OnExhibitButtonClick()
    {
        this.parentScreen.SetActive(false);
        this.nameLocalExhibitScreen.GetComponent<NameLocalExhibitController>().SetExhibit(this.exhibit);
        this.nameLocalExhibitScreen.SetActive(true);
    }
}