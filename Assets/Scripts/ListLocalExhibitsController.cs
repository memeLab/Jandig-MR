using System;
using System.Collections.Generic;
using System.IO;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.UI;
using ExhibitClasses;

public class ListLocalExhibitsController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject parentScreen;
    public GameObject onBackScreen;
    public GameObject localExhibitScreen;

    public GameObject openExhibitButtonPrefab;
    public GameObject backButton;
    public GameObject previousPageButton;
    public GameObject nextPageButton;

    private const int defaultExhibitsCacheOffsetIncrement = 5;

    private LocalExhibits localExhibits;
    private List<GameObject> exhibitButtons = new();

    // Track spawned artworks to destroy them later
    private readonly List<GameObject> spawnedArtworks = new();

    private SpawnObjectsScript objectSpawner;

    private int _offset = 0;
    public int ExhibitsCacheOffset
    {
        get => _offset;
        set
        {
            if (_offset != value)
            {
                _offset = value;
                OnOffsetChanged?.Invoke(_offset);
            }
        }
    }

    public event Action<int> OnOffsetChanged;

    private void Start()
    {
        objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<SpawnObjectsScript>();

        if (previousPageButton == null || nextPageButton == null)
        {
            Debug.LogError("Buttons are not assigned in the Inspector.");
            return;
        }

        OnOffsetChanged += HandleOffsetChange;

        backButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ClearButtons();
            ObjectVisibilityController.ClearObjects();
            parentScreen.SetActive(false);
            onBackScreen.SetActive(true);
        });

        previousPageButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (ExhibitsCacheOffset > 0)
                ExhibitsCacheOffset -= defaultExhibitsCacheOffsetIncrement;
        });

        nextPageButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (localExhibits != null && ExhibitsCacheOffset + defaultExhibitsCacheOffsetIncrement < localExhibits.exhibits.Count)
                ExhibitsCacheOffset += defaultExhibitsCacheOffsetIncrement;
        });
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        localExhibits =  LocalExhibitManager.loadLocalExhibits();
        HandleOffsetChange(0);
    }

    private void HandleOffsetChange(int newOffset)
    {
        if (localExhibits == null || localExhibits.exhibits.Count == 0)
        {
            Debug.Log("No local exhibits to show.");
            ClearButtons();
            return;
        }

        int endIndex = Mathf.Min(newOffset + defaultExhibitsCacheOffsetIncrement, localExhibits.exhibits.Count);
        RenderButtons(newOffset, endIndex);

        previousPageButton.GetComponent<Button>().interactable = newOffset > 0;
        nextPageButton.GetComponent<Button>().interactable = endIndex < localExhibits.exhibits.Count;
    }

    private void RenderButtons(int start, int end)
    {
        ClearButtons();
        Transform contentTransform = transform.Find("Scroll View/Viewport/Content");

        for (int i = start; i < end; i++)
        {
            ExhibitEntry entry = localExhibits.exhibits[i];
            GameObject exhibitButton = Instantiate(openExhibitButtonPrefab, contentTransform);

            var tmpText = exhibitButton.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmpText != null)
                tmpText.text = entry.key;
            else
                Debug.LogWarning($"No Text or TMP_Text component found in exhibitButton prefab for {entry.key}"); 

            // Hook up behavior to open exhibit
            exhibitButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                onOpenExhibit(entry);
            });

            exhibitButtons.Add(exhibitButton);
        }
    }
    private void onOpenExhibit(ExhibitEntry exhibitEntry)
    {
        Debug.Log($"Opening local exhibit: {exhibitEntry.key}");
        ObjectVisibilityController.ClearObjects();
        SpawnObjectsScript.OpenLocalExhibit(objectSpawner, exhibitEntry);
        Exhibit exhibit = new Exhibit();
        exhibit.slug = exhibitEntry.key;
        exhibit.name = exhibitEntry.value.name;
        localExhibitScreen.SetActive(true);
        localExhibitScreen.GetComponent<LocalExhibitMenuController>().onExitScreen = this.transform.gameObject;
        localExhibitScreen.GetComponent<LocalExhibitMenuController>().SetExhibit(exhibit);
        localExhibitScreen.GetComponent<LocalExhibitMenuController>().checkDefaultExhibitButton();
        localExhibitScreen.GetComponent<LocalExhibitMenuController>().viewMode();
        this.transform.gameObject.SetActive(false);
    }

    private void ClearButtons()
    {
        foreach (var go in exhibitButtons)
        {
            if (go != null) Destroy(go);
        }
        exhibitButtons.Clear();
    }

}
