using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ExhibitClasses;
public class ListARExhibitsController : MonoBehaviour
{

 
    public GameObject parentScreen;
    public GameObject onBackScreen;
    public GameObject nameLocalExhibitScreen;


    public GameObject openExhibitButtonPrefab;

    public GameObject backButton;
    public GameObject previousPageButton;
    public GameObject nextPageButton;

    //private ListExhibitResponse content;
    private List<GameObject> exhibitButtons = new();

    private void decrementExhibitsCacheOffset()
    {
        if (ExhibitsCacheOffset > 0 && ExhibitsCacheOffset - defaultExhibitsCacheOffsetIncrement >= 0)
        {
            Debug.Log("Decrementing cache offset");
            ExhibitsCacheOffset -= defaultExhibitsCacheOffsetIncrement;
        }
    }

    private List<Exhibit> exhibitsCache = new();
    private string nextExhibitsPageLink;

    private const int defaultExhibitsCacheOffsetIncrement = 5;

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

    private void refillExhibitsCache(string url, System.Action<ListExhibitResponse> onResult)
    {
        nextPageButton.GetComponent<Button>().interactable = false;
        nextPageButton.GetComponent<Button>().interactable = false;
        string updated_url = url.Replace("http://", "https://");
        Debug.Log("Refilling exhibits cache from URL: " + updated_url);
        StartCoroutine(MakeRequest(updated_url, result =>
        {
            exhibitsCache.AddRange(result.results);
            nextExhibitsPageLink = result.next;
            nextPageButton.GetComponent<Button>().interactable = true;
            nextPageButton.GetComponent<Button>().interactable = true;
            onResult?.Invoke(result);
        }));
    }

    private void incrementExhibitsCacheOffset() {
        if (exhibitsCache.Count > ExhibitsCacheOffset + defaultExhibitsCacheOffsetIncrement)
        {
            Debug.Log("Incrementing cache offset");
            ExhibitsCacheOffset += defaultExhibitsCacheOffsetIncrement;
        } else if (nextExhibitsPageLink != null && nextExhibitsPageLink != "")
        {
            refillExhibitsCache(nextExhibitsPageLink, result =>
            {
                if (exhibitsCache.Count > ExhibitsCacheOffset + defaultExhibitsCacheOffsetIncrement)
                {
                    Debug.Log("Incrementing cache offset with new fetched page");
                    ExhibitsCacheOffset += defaultExhibitsCacheOffsetIncrement;
                }
            });
        }
        Debug.Log("Nothing to do");
    }

    private void HandleOffsetChange(int newOffset)
    {
        Debug.Log("Handling offset change");
        // Clamp how many to show (max 5, but not beyond list end)
        int endIndex = Mathf.Min(newOffset + defaultExhibitsCacheOffsetIncrement, exhibitsCache.Count);

        RenderButtons(newOffset, endIndex);
    }

    private void RenderButtons(int start, int end) {
        Debug.Log("Rendering buttons");
        foreach (var exhibitButton in exhibitButtons)
        {
            Destroy(exhibitButton);
        }
        exhibitButtons.Clear();

        for (int i = start; i < end; i++)
        {
            Transform contentTransform = transform.Find("Scroll View").Find("Viewport").Find("Content");
            GameObject exhibitButton = Instantiate(openExhibitButtonPrefab, contentTransform);
            exhibitButton.GetComponent<OpenExhibitButtonController>().SetExhibit(exhibitsCache[i]);
            exhibitButton.GetComponent<OpenExhibitButtonController>().SetParentScreen(this.parentScreen);
            exhibitButton.GetComponent<OpenExhibitButtonController>().SetNameLocalExhibitScreen(this.nameLocalExhibitScreen);
            exhibitButtons.Add(exhibitButton);
        }

        if (end == exhibitsCache.Count && nextExhibitsPageLink != "")
        {
            nextPageButton.GetComponent<Button>().interactable = true;
        }else if (end < exhibitsCache.Count)
        {
            nextPageButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            nextPageButton.GetComponent<Button>().interactable = false;
        }

        previousPageButton.GetComponent<Button>().interactable = start > 0;
    }

    void Start()
    {
        if (previousPageButton == null || nextPageButton == null)
        {
            Debug.LogError("Buttons are not assigned in the Inspector.");
            return;
        }

        OnOffsetChanged += HandleOffsetChange;

        backButton.GetComponent<Button>().onClick.AddListener(() => {
            foreach (var exhibitButton in exhibitButtons)
            {
                Destroy(exhibitButton);
            }
            exhibitButtons.Clear();
            exhibitsCache.Clear();

            parentScreen.gameObject.SetActive(false);
            onBackScreen.gameObject.SetActive(true);
        });

        previousPageButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            decrementExhibitsCacheOffset();
        });

        nextPageButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            incrementExhibitsCacheOffset();
        });

    }

    public void Activate(string url)
    {
        gameObject.SetActive(true);
        refillExhibitsCache(url, result =>
        {
            Debug.Log("Filled for the first time the exhibits cache");
            HandleOffsetChange(0);
        });
    }

    private IEnumerator MakeRequest(string url, System.Action<ListExhibitResponse> onResult)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ListExhibitResponse content = JsonUtility.FromJson<ListExhibitResponse>(request.downloadHandler.text);
            onResult?.Invoke(content);
        }
        else { 
            Debug.LogError(request.error);
            onResult?.Invoke(null);
        }
    }
}
