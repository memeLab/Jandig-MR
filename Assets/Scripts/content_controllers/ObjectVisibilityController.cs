using ExhibitClasses;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
public class ObjectVisibilityController : MonoBehaviour
{
    static List<GameObject> editor_components = new List<GameObject>();
    static List<GameObject> active_artworks = new List<GameObject>();
    static bool is_editing = false;
    public static void ClearObjects()
    {
        // Remover todos os GameObjects com a tag "Artwork" antes de criar novos
        GameObject[] artworks = GameObject.FindGameObjectsWithTag("Artwork");
        Debug.Log($"Encontrados {artworks.Length} objetos com a tag 'Artwork' para destruir.");
        foreach (GameObject artwork in artworks)
        {
            Debug.Log($"Destruindo: {artwork.name}");
            Destroy(artwork);
        }
        editor_components.Clear();
        active_artworks.Clear();
    }
    public static void EnableEditorComponents()
    {
        foreach (GameObject o in editor_components)
        {
            o.SetActive(true);
        }
        foreach (GameObject artwork in active_artworks)
        {
            OVRSpatialAnchor spatial_anchor = artwork.GetComponent<OVRSpatialAnchor>();
            if (spatial_anchor != null)
            {
                Destroy(spatial_anchor);
            }
        }
        is_editing = true;
    }
    public static void DisableEditorComponents()
    {
        foreach (GameObject o in editor_components)
        {
            o.SetActive(false);
        }
        is_editing = false;
    }

    public static void registerEditableObject(GameObject obj)
    {
        if (obj == null) return;

        List<GameObject> children = new List<GameObject>();
        obj.GetChildGameObjects(children);
        foreach (GameObject child in children) { 
            if (child.CompareTag("EditOnly"))
            {
                editor_components.Add(child);
            }
        }
        active_artworks.Add(obj);
    }

    public void Update()
    {
        GameObject player_head = GameObject.Find("CenterEyeAnchor");
        if (player_head == null) return;

        foreach (GameObject artwork in active_artworks)
        {
            if (artwork == null) continue;
            float distance = Vector3.Distance(player_head.transform.position, artwork.transform.position);
            bool shouldBeVisible = distance < 3.0f; // Exemplo: tornar visível se estiver a menos de 3 metros
            if (is_editing)
            {
                shouldBeVisible = true; // Sempre visível em modo de edição
            }
            artwork.SetActive(shouldBeVisible);
        }
    }


}
