using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVisibilityScript : MonoBehaviour
{
    [SerializeField] private GameObject menu; // Assign in Inspector
    [SerializeField] private GameObject centerEye; // Assign in Inspector

    // Update is called once per frame
    void Update()
    {
        // Check if the Meta Quest menu button is pressed
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            // Toggle menu visibility
            if (menu != null)
            {
                menu.SetActive(!menu.activeSelf);
            }
            if (menu.activeSelf)
            {
                resetMenuPosition();
            }
            else
            {
                Debug.Log("Menu is now hidden.");
            }
        }
    }
    public void ShowMenu()
    {
        if (menu != null && !menu.activeSelf)
        {
            menu.SetActive(true);
            resetMenuPosition();
            Debug.Log("Menu is now visible.");
        }
    }
    public void HideMenu()
    {
        if (menu != null && menu.activeSelf)
        {
            menu.SetActive(false);
            Debug.Log("Menu is now hidden.");
        }
    }
    private void Start()
    {

        if (menu != null)
        {
            // Ensure the menu starts hidden
            menu.SetActive(true);
            // after 500 ms reset position
            Invoke("resetMenuPosition", 1f);
            
        }
        else
        {
            Debug.LogError("Menu GameObject is not assigned in the Inspector!");
        }
    }
    void resetMenuPosition()
    {
        if (menu != null && centerEye != null)
        {
            // Position the menu in front of the center eye by moving its position and rotation to where the center eye is looking
            menu.transform.position = centerEye.transform.position + centerEye.transform.forward * 0.7f; // Adjust the distance as needed
            menu.transform.rotation = Quaternion.LookRotation(centerEye.transform.forward, Vector3.up);
            Debug.Log("Menu position reset to center eye's position and rotation.");

        }
        else
        {
            Debug.LogError("Menu or Center Eye GameObject is not assigned in the Inspector!");
        }
    }
}
