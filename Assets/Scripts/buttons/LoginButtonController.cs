using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static LoginHandler;

public class LoginButtonController : MonoBehaviour
{

    [System.Serializable]
    public class LoginForm
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string access;
        public string refresh;

        public UserInfo GetUserInfo()
        {
            // the access token has 3 parts
            // the first is the algorithm
            // the second is the content
            // the third is the signature
            string[] parts = this.access.Split(".");
            Debug.Log("Middle part of token: " + parts[1]);
            // Reverse base64 URL to standard base64
            string incoming = parts[1].Replace('_', '/').Replace('-', '+');
            switch (parts[1].Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            string content = Encoding.UTF8.GetString(Convert.FromBase64String(incoming));
            return JsonUtility.FromJson<UserInfo>(content);
        }
    }

    [System.Serializable]
    public class UserInfo
    {
        public string username;
        public string user_profile_id;
    }

    public Button loginButton;
    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject parentScreen;
    public GameObject onLoginNextScreen;
    public GameObject onLoginErrorScreen;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLoginButtonClick()
    {
        loginButton.interactable = false;
        StartCoroutine(MakeRequest());
    }

    private IEnumerator MakeRequest()
    {
        string username = usernameInput.GetComponentInChildren<TMP_InputField>().text;
        string password = passwordInput.GetComponentInChildren<TMP_InputField>().text;

        LoginForm loginForm = new LoginForm { username = username, password = password };

        byte[] payload = Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginForm));

        UnityWebRequest request = new UnityWebRequest("https://dev.jandig.app/api/v1/auth/login/", "POST");
        request.timeout = 20; // Set timeout to 30 seconds

        request.uploadHandler = new UploadHandlerRaw(payload);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            Debug.Log("Response: " + loginResponse.access);
            UserInfo userInfo = loginResponse.GetUserInfo();
            Debug.Log("UserInfo: " + userInfo.username);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/userInfo.json", JsonUtility.ToJson(userInfo));
            
            // deactivate the current backplate
            parentScreen.SetActive(false);
            loginButton.interactable = true;
            onLoginNextScreen.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Error: " + request.error);
            loginButton.interactable = true;
            parentScreen.SetActive(false);

            // Pass error to text component on login error screen
            TextMeshProUGUI errorText = onLoginErrorScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (errorText != null)
            {
                errorText.text = "Login failed: " + request.error;
            }


            onLoginErrorScreen.SetActive(true);
        }
    }
}
