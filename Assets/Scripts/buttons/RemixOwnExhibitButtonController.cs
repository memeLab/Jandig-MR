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
        string userInfoJson = System.IO.File.ReadAllText(Application.persistentDataPath + "/userInfo.json");
        userInfo = JsonUtility.FromJson<UserInfo>(userInfoJson);
        remixOwnExhibitButton.onClick.AddListener(OnRemixOwnExhibitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRemixOwnExhibitButtonClick() {
        onRemixOwnExhibitNextScreen.GetComponent<ListARExhibitsController>().Activate("https://dev.jandig.app/api/v1/exhibits/?owner=" + userInfo.user_profile_id);
        parentScreen.SetActive(false);
    }
}
