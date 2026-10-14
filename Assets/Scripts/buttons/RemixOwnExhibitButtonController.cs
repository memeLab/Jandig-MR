using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static ListARExhibitsController;

public class RemixOwnExhibitButtonController : MonoBehaviour
{
    [System.Serializable]
    public class UserInfo
    {
        public string username;
        public string user_profile_id;
    }
    public GameObject parentScreen;
    public GameObject onRemixOwnExhibitNextScreen;

    public Button remixOwnExhibitButton;
    private UserInfo userInfo;

    // Start is called before the first frame update
    void Start()
    {
        remixOwnExhibitButton.onClick.AddListener(OnRemixOwnExhibitButtonClick);
    }

    // Reads the session on click instead of in Start(), so this button no longer
    // depends on MainMenuController hiding it before Start() runs for a
    // logged-out user, and it picks up a login made after the scene loaded.
    private UserInfo LoadUserInfo()
    {
        string path = Application.persistentDataPath + "/userInfo.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("No session file found, cannot list the user's own exhibits.");
            return null;
        }
        try
        {
            UserInfo info = JsonUtility.FromJson<UserInfo>(System.IO.File.ReadAllText(path));
            if (info == null || string.IsNullOrEmpty(info.user_profile_id))
            {
                Debug.LogWarning("Session file has no user profile id.");
                return null;
            }
            return info;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error reading session file: " + ex.Message);
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRemixOwnExhibitButtonClick() {
        userInfo = LoadUserInfo();
        if (userInfo == null)
        {
            return;
        }
        onRemixOwnExhibitNextScreen.GetComponent<ListARExhibitsController>().Activate("https://jandig.app/api/v1/exhibits/?owner=" + userInfo.user_profile_id);
        parentScreen.SetActive(false);
    }
}
