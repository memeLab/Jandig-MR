using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    public GameObject parentScreen;
    public GameObject onBackScreen;
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnBackButtonClick()
    {
        parentScreen.SetActive(false);
        onBackScreen.SetActive(true);
    }
}
