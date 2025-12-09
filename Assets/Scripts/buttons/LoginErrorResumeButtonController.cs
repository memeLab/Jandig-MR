using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginErrorResumeButtonController : MonoBehaviour
{
    public GameObject parentScreen;
    public GameObject onResumeScreen;
    public Button resumeButton;

    // Start is called before the first frame update
    void Start()
    {
        resumeButton.onClick.AddListener(OnResumeError);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnResumeError()
    {
        parentScreen.SetActive(false);
        onResumeScreen.SetActive(true);
    }
}
