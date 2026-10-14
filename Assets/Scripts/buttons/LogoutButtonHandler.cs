using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoutButtonHandler : MonoBehaviour
{
    public GameObject parentScreen;
    public GameObject onLogoutScreen;
    public Button logoutButton;

    // Start is called before the first frame update
    void Start()
    {
        logoutButton.onClick.AddListener(OnLogoutClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLogoutClick()
    {
        try
        {
            System.IO.File.Delete(Application.persistentDataPath + "/userInfo.json");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Could not remove the session file: " + ex.Message);
            return;
        }
        parentScreen.SetActive(false);
        onLogoutScreen.SetActive(true);
    }
}
