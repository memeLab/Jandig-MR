using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class LoginHandler : MonoBehaviour
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
            string content = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
            return JsonUtility.FromJson<UserInfo>(content);
        }
    }

    [System.Serializable]
    public class UserInfo
    {
        public string username;
        public string user_profile_id;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleLogin()
    {
        Debug.Log("$$$$$$$$$$$$$$$$$$ HANDLING LOGIN $$$$$$$$$$$$$$$$$$$$");
        string username = transform.Find("UsernameInputField").Find("TextField").GetComponent<TMP_InputField>().text;
        string password = transform.Find("PasswordInputField").Find("TextField").GetComponent<TMP_InputField>().text;
        StartCoroutine(sendLoginPostRequest(new LoginForm { username = username, password = password }));
        Debug.Log("$$$$$$$$$$$$$$$$$$ FINISHING HANDLING LOGIN $$$$$$$$$$$$$$$$$$$$");
    }

    private IEnumerator sendLoginPostRequest(LoginForm loginForm)
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        string loginFormJson = JsonUtility.ToJson(loginForm);
        Debug.Log(loginFormJson);
        byte[] payload = Encoding.UTF8.GetBytes(loginFormJson);
        
        UnityWebRequest request = new UnityWebRequest("https://dev.jandig.app/api/v1/auth/login/", "POST");

        request.uploadHandler = new UploadHandlerRaw(payload);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            Debug.Log("Response: " + loginResponse.access);
            UserInfo userInfo = loginResponse.GetUserInfo();
            Debug.Log("UserInfo: " + userInfo.username);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/userInfo.json", JsonUtility.ToJson(userInfo));
        } else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
