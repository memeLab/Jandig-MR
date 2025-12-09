using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldUI : MonoBehaviour
{
    VisualElement root;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Login").clicked += () => Debug.Log("wow");
    }
}
