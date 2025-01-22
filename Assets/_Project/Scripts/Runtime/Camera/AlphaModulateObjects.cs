using System.Collections.Generic;
using NoSlimes.Loggers;
using UnityEngine;

public class AlphaModulateObjects : LoggerMonoBehaviour
{
    // offset would be to offset position to say head height until this is decided, stay at 0
    Vector3 offset = Vector3.zero;

    private List<GameObject> objectsHit = new();
    
    public void RunAlphaModulation()
    {
        // Reset any lingering objects in the list, they'll be re-added before the frame is done if they're still being hit by the ray
        ResetAlphaModulatedObjects();
        
        Ray r = new Ray(Camera.main.transform.position, ((transform.position + offset) - Camera.main.transform.position).normalized);
        foreach (RaycastHit hit in Physics.SphereCastAll(r, 0.1f, Vector3.Distance(Camera.main.transform.position, transform.position + offset)))
        {
            if (hit.collider.gameObject.CompareTag("Environment"))
            {
                if (!objectsHit.Contains(hit.collider.gameObject))
                    objectsHit.Add(hit.collider.gameObject);
            }
        }
        
        // Set alpha modulation on objects
        ApplyAlphaModulatedObjects();
    }

    private void ResetAlphaModulatedObjects()
    {
        foreach (GameObject obj in objectsHit)
        {
            Logger.Log("Reseting alpha modulation on objects", this);
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            var color = objRenderer.material.color;
            color.a = 1f;
            objRenderer.material.color = color;
            //objRenderer.material.SetColor("_Color", 
            //    new Color(objRenderer.material.color.r, objRenderer.material.color.g, objRenderer.material.color.b, 1f)); // 
        }
        objectsHit.Clear();
    }

    private void ApplyAlphaModulatedObjects()
    {
        foreach (GameObject obj in objectsHit)
        {
            Logger.Log("Setting alpha modulation on objects", this);
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            var color = objRenderer.material.color;
            color.a = 1f;
            objRenderer.material.color = color;
            //objRenderer.material.SetColor("_Color", 
            //    new Color(objRenderer.material.color.r, objRenderer.material.color.g, objRenderer.material.color.b, 0.1f)); // 
        }
    }
}
