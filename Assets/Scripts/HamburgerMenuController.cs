using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamburgerMenuController : MonoBehaviour
{
    private bool menuOpened = false;
    public GameObject menuCanva;
    public GameObject centerEyeAnchor;

    // Start is called before the first frame update
    void Start()
    {
        menuCanva.SetActive(false);
        menuCanva.transform.Translate(0, 0, 30, centerEyeAnchor.transform);

        //Vector3 position;
        //Quaternion rotation;
        //centerEyeAnchor.transform.GetPositionAndRotation(out position, out rotation);

        //position.z += 30;
        //menuCanva.transform.SetPositionAndRotation(position, rotation);

        //menuCanva.transform.SetParent(centerEyeAnchor.transform);
        //menuCanva.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 30), new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleButtonDown() {
        if (menuOpened) {
            closeMenu();
        } else
        {
            openMenu();
        }
        
    }

    private void closeMenu()
    {
        if (!menuOpened)
        {
            // nothing to do
            return;
        }
        Debug.Log("------------------------------------------------------ Closing menu! ------------------------------------------------------------------");
        menuOpened = !menuOpened;
        menuCanva.SetActive(menuOpened);
    }

    private void openMenu()
    {
        if (menuOpened)
        {
            // nothing to do
            return;
        }

        Debug.Log("------------------------------------------------------ Opening menu! ------------------------------------------------------------------");
        menuOpened = !menuOpened;
        menuCanva.SetActive(menuOpened);
    }
}
