#region
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public class ShaderFPSLimiter : MonoBehaviour
{
    readonly List<Material> sourceMaterials = new ();
    readonly List<Material> instancedMaterials = new ();

    [SerializeField] private float FPS = 7;
    [SerializeField] private bool mainMenu;
    private Material fullScreenMat;
    private float timer;

    private void Awake()
    {
        sourceMaterials.AddRange(Resources.LoadAll<Material>("LowFramerateShaders"));
        fullScreenMat = Resources.Load<Material>("LowFramerateShaders/FullScreenShader");

        foreach (var material in sourceMaterials)
        {
            var instancedMaterial = new Material(material);
            instancedMaterials.Add(instancedMaterial);
        }
    }

    private void OnEnable()
    {
        if (mainMenu)
        {
            fullScreenMat.SetFloat("_InMainMenu", 1);
        }
        else
        {
            fullScreenMat.SetFloat("_InMainMenu", 0);
        }

    }

    private void Update()
    {
    
        if(timer <= 0)
        {
            foreach (Material mat in sourceMaterials)
            {
                mat.SetFloat("_LimitDeltaTime", Random.Range(0, 1f));
            }
            timer = 1/FPS;
        }
    
        timer -= Time.deltaTime;
    }
}
