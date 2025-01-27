#region
using UnityEngine;
#endregion

public class Hotbar : MonoBehaviour
{
    void Start()
    {
        if (transform != transform.root)
        {
            Debug.LogError("Hotbar must be a root object!", this);
        }
    }
}
