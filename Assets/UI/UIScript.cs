using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEditor;
public class UIScript : MonoBehaviour
{
    [SerializeField] private RayInteractor leftHandRayInteractor;
    private UIDocument document;

    RayInteractable menu;
    // Start is called before the first frame update
    private void OnEnable()
    {
        menu = GetComponent<RayInteractable>();
        document = GetComponent<UIDocument>();
        document.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            RayInteractor activeRayInteractor = leftHandRayInteractor;
            
            SurfaceHit? has_hit = activeRayInteractor.CollisionInfo; // Obtenha o raio do RayInteractor ativo
            if (has_hit == null)
            {
                Debug.Log("Nenhum objeto foi atingido pelo raio.");
                return invalidPosition;
            }
            SurfaceHit hit = has_hit.Value;
            
            Vector2 pixelUV = hit.Point;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= this.document.panelSettings.targetTexture.width;
            pixelUV.y *= this.document.panelSettings.targetTexture.height;

            var cursor = this.document.rootVisualElement.Q<VisualElement>("cursor");

            if (cursor != null)
            {
                cursor.style.left = pixelUV.x;
                cursor.style.top = pixelUV.y;
            }
            else
            {
                Debug.Log("No cursor");
            }

                return pixelUV;
        });
    }
}
