using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;

public class SceneController : SingletonMonoBehaviour<SceneController>
{
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject loginForm;

    [SerializeField]
    private GameObject exhibitList;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        loginForm.SetActive(false);
        exhibitList.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleStartButtonDown()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    public void HandleLoginOption()
    {
        Debug.Log("********** HANDLING LOGIN OPTION ***********");
        mainMenu.SetActive(false);
        loginForm.SetActive(true);
    }

    public void HandleListExhibitsOption()
    {
        Debug.Log("********** HANDLING  LIST EXHIBITS OPTION ***********");
    }
}
