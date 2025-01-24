using UnityEngine;

public class ShaderFPSLimiter : MonoBehaviour
{

    private Material material;

    [SerializeField] private float FPS = 7;
    private float timer;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {

        if(timer <= 0)
        {
            material.SetFloat("_LimitDeltaTime", Random.Range(0, 1f));
            timer = 1/FPS;
        }

        timer -= Time.deltaTime;
    }
}
