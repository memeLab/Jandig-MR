using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ExhibitClasses;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform loginBackplateTf = transform.Find("CanvasRoot").Find("LoginBackplate");
        loginBackplateTf.gameObject.SetActive(false);

        Transform unloggedBackplateTf = transform.Find("CanvasRoot").Find("UnloggedBackplate");
        unloggedBackplateTf.gameObject.SetActive(false);

        Transform loggedBackplateTf = transform.Find("CanvasRoot").Find("LoggedBackplate");
        loggedBackplateTf.gameObject.SetActive(false);

        Transform remixBackplateTf = transform.Find("CanvasRoot").Find("RemixBackplate");
        remixBackplateTf.gameObject.SetActive(false);

        Transform listARExhibitsBackplateTf = transform.Find("CanvasRoot").Find("ListARExhibitsBackplate");
        listARExhibitsBackplateTf.gameObject.SetActive(false);

        Transform nameLocalExhibitBackplateTf = transform.Find("CanvasRoot").Find("NameLocalExhibitBackplate");
        nameLocalExhibitBackplateTf.gameObject.SetActive(false);

        Transform localExhibitMenuBackplateTf = transform.Find("CanvasRoot").Find("LocalExhibitMenuBackplate");
        localExhibitMenuBackplateTf.gameObject.SetActive(false);

        Transform listLocalExhibitsBackplateTf = transform.Find("CanvasRoot").Find("ListLocalExhibitsBackplate");
        listLocalExhibitsBackplateTf.gameObject.SetActive(false);

        Transform OverrideLocalExhibitBackplateTf = transform.Find("CanvasRoot").Find("OverrideLocalExhibitBackplate");
        OverrideLocalExhibitBackplateTf.gameObject.SetActive(false);

        if (System.IO.File.Exists(Application.persistentDataPath + "/userInfo.json"))
        {
            loggedBackplateTf.gameObject.SetActive(true);
        } else
        {
            unloggedBackplateTf.gameObject.SetActive(true);
        }
        CheckDefaultExhibit();

    }
    public void CheckDefaultExhibit()
    {
        LocalExhibits exhibits = LocalExhibitManager.loadLocalExhibits();
        if (exhibits.defaultExhibitSlug != null)
        { 
            Debug.Log("Loading default exhibit: " + exhibits.defaultExhibitSlug);
            LocalExhibitManager.loadLocalExhibit(exhibits.defaultExhibitSlug);
        }
        else {             Debug.Log("No default exhibit found."); }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void HandleOpenLoginForm()
    {
        Transform unloggedBackplateTf = transform.Find("CanvasRoot").Find("UnloggedBackplate");
        unloggedBackplateTf.gameObject.SetActive(false);

        Transform loginBackplateTf = transform.Find("CanvasRoot").Find("LoginBackplate");
        loginBackplateTf.gameObject.SetActive(true);
    }

}
