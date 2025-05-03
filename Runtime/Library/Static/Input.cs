using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RealMethod
{
    static class Input
    {
        public static bool IsTouchOverUI(GraphicRaycaster HUD_graphicRaycaster, Vector2 touchPosition)
        {
            // Create a pointer event data from the touch position
            PointerEventData pointerEventData = new PointerEventData(null);
            pointerEventData.position = touchPosition;

            // Create a list to hold the results
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast using the GraphicRaycaster and pointer event data
            HUD_graphicRaycaster.Raycast(pointerEventData, results);

            // Check if we hit any UI elements
            return results.Count > 0;
        }
    }
}