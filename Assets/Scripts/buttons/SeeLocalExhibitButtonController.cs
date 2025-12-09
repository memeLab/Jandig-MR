using UnityEngine;
using UnityEngine.UI;

public class SeeLocalExhibitButtonController : MonoBehaviour
{
    public Button seeLocalExhibitButton;
    public GameObject parentScreen;
    public GameObject onSeeLocalExhibitNextScreen;

    // Start is called before the first frame update
    void Start()
    {
        seeLocalExhibitButton.onClick.AddListener(OnSeeLocalExhibitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnSeeLocalExhibitButtonClick()
    {
        parentScreen.SetActive(false);
        onSeeLocalExhibitNextScreen.GetComponent<ListLocalExhibitsController>().Activate();
    }
}
