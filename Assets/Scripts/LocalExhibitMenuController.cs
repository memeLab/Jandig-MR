using ExhibitClasses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class LocalExhibitMenuController : MonoBehaviour
{

    public GameObject onExitScreen;

    public Button editButton;
    public Button makeDefaultButton;
    public Button enableAudioDescriptionButton;
    public Button disableAudioDescriptionButton;
    public Button saveButton;
    public Button saveExitButton;
    public Button exitWithoutSaveButton;
    public Button exitButton;

    private Exhibit exhibit = null;

    public GameObject exhibitControllerPrefab;

    private SpawnObjectsScript objectSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<SpawnObjectsScript>();
        this.exhibit = new Exhibit();
        saveButton.onClick.AddListener(onSaveButton);
        saveExitButton.onClick.AddListener(onSaveExitButton);
        exitWithoutSaveButton.onClick.AddListener(onExitWithoutSaveButton);
        enableAudioDescriptionButton.onClick.AddListener(enableAudioDescription);
        disableAudioDescriptionButton.onClick.AddListener(disableAudioDescription);
        editButton.onClick.AddListener(editExhibit);
        exitButton.onClick.AddListener(onExitButton);
        makeDefaultButton.onClick.AddListener(makeDefaultExhibit);

        checkDefaultExhibitButton();

    }
    public void editMode()
    {
        editButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        saveExitButton.gameObject.SetActive(true);
        exitWithoutSaveButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(false);
    }
    public void viewMode()
    {
        editButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
        saveExitButton.gameObject.SetActive(false);
        exitWithoutSaveButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);
    }

    public void checkDefaultExhibitButton()
    {
        LocalExhibits localExhibits = LocalExhibitManager.loadLocalExhibits();
        if (localExhibits.defaultExhibitSlug == this.exhibit.slug)
        {
            makeDefaultButton.gameObject.SetActive(false);
        }
        else
        {
            makeDefaultButton.gameObject.SetActive(true);
        }
    }
    public void enableAudioDescription()
    {
        // TODO: implementar funcionalidade
        enableAudioDescriptionButton.gameObject.SetActive(false);
        disableAudioDescriptionButton.gameObject.SetActive(true);
    }
    public void disableAudioDescription()
    {
        // TODO: implementar funcionalidade
        disableAudioDescriptionButton.gameObject.SetActive(false);
        enableAudioDescriptionButton.gameObject.SetActive(true);
    }
    public void editExhibit()
    {
        editButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        saveExitButton.gameObject.SetActive(true);
        exitWithoutSaveButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(false);
        ObjectVisibilityController.EnableEditorComponents();
    }
    public void onExitButton() {
        ObjectVisibilityController.ClearObjects();
        Exit();
    }
    public void makeDefaultExhibit()
    {
        LocalExhibitManager.saveDefaultLocalExhibit(this.exhibit.slug);
        makeDefaultButton.gameObject.SetActive(false);
    }

    private void Exit() {
        this.exhibit.name = null;
        // Hide this menu
        this.transform.gameObject.SetActive(false);
        this.onExitScreen.SetActive(true);
    }

    public void SetExhibit(Exhibit exhibit)
    {
        this.exhibit = exhibit;
    }
    public void startExhibit(Exhibit exhibit)
    {
        this.exhibit = exhibit;
        createExhibitController();
        checkDefaultExhibitButton();
    }

    private void createExhibitController()
    {
        Debug.Log("spawn artworks chamado");

        int offset_x = 0;
        float offset_z = 0.5f;

        ObjectVisibilityController.ClearObjects();

        if (exhibitControllerPrefab == null)
        {
            Debug.LogError("exhibitControllerPrefab está nulo!");
            return;
        }

        GameObject exhibitController = Instantiate(exhibitControllerPrefab, new Vector3(offset_x, 0, offset_z), Quaternion.identity);
        if (exhibitController == null)
        {
            Debug.LogError("Falha ao instanciar exhibitControllerPrefab!");
            return;
        }

        ExhibitControllerScript script = exhibitController.GetComponent<ExhibitControllerScript>();
        if (script == null)
        {
            Debug.LogError("ExhibitControllerScript não encontrado no exhibit_controller!");
            return;
        }

        if (this.exhibit == null)
        {
            Debug.LogError("Exhibit está nulo!");
            return;
        }

        script.url = "https://dev.jandig.app/api/v1/exhibits/" + this.exhibit.id + "/";
        script.shouldSave = true;
        Debug.Log($"URL definida no ExhibitControllerScript: {script.url}");
    }


    public void onSaveButton()
    {
        // chamada sem await — mantém comportamento anterior
        _ = createAndSaveSpatialAnchors(true);
    }
    public async void onSaveExitButton()
    {
        // Aguarda createAndSaveSpatialAnchors terminar antes de continuar
        await createAndSaveSpatialAnchors(false);
        onExitWithoutSaveButton();
    }
    public void onExitWithoutSaveButton()
    {
        editButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
        saveExitButton.gameObject.SetActive(false);
        exitWithoutSaveButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);
        ObjectVisibilityController.ClearObjects();
        LocalExhibits localExhibits = LocalExhibitManager.loadLocalExhibits();

        ExhibitInfo exhibit_info = localExhibits.GetLocalExhibitInfo(this.exhibit.slug);
        ExhibitEntry exhibitEntry = new ExhibitEntry();
        exhibitEntry.key = this.exhibit.slug;
        exhibitEntry.value = exhibit_info;
        SpawnObjectsScript.OpenLocalExhibit(objectSpawner, exhibitEntry);
        ObjectVisibilityController.DisableEditorComponents();
    }
    public async Task createAndSaveSpatialAnchors(bool remove_anchors_after_save = true)
    {
        GameObject[] all_artworks = GameObject.FindGameObjectsWithTag("Artwork");
        foreach (GameObject artwork in all_artworks)
        {
            OVRSpatialAnchor anchor = artwork.AddComponent<OVRSpatialAnchor>();
            await anchor.WhenLocalizedAsync();
            ArtworkDataController controller = artwork.GetComponent<ArtworkDataController>();
            ArtworkData data = controller.GetArtworkData();
            data.anchor_uuid = anchor.Uuid.ToString();
            controller.SetArtworkData(data);
        }

        ArtworkData[] artworks = LocalExhibitManager.getCurrentUpdatedArtworkData();
        LocalExhibitManager.saveLocalExhibit(artworks, this.exhibit.name, this.exhibit.slug);
        if (remove_anchors_after_save)
        {
            foreach (GameObject artwork in all_artworks)
            {
                OVRSpatialAnchor anchor = artwork.GetComponent<OVRSpatialAnchor>();
                if (anchor != null)
                {
                    Destroy(anchor);
                }
            }
        }
    }

}
