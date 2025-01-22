#region
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
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
