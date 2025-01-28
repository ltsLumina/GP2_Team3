using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtPlayer : MonoBehaviour
{
    Player player;

    void Start() => player = FindAnyObjectByType<Player>();

#if UNITY_EDITOR
    void OnEnable()
    {
        if (!Application.isPlaying) EditorApplication.update += Update;
    }

    void OnDisable()
    {
        if (!Application.isPlaying) EditorApplication.update -= Update;
    }
#endif

    // Update is called once per frame
    void Update()
    {
        switch (Application.isPlaying)
        {
            case true: // play mode
                transform.up = transform.position - player.transform.position;
                break;

            case false when Application.isEditor: // edit mode 
            {
                if (SceneView.lastActiveSceneView)
                {
                    Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
                    transform.up = transform.position - sceneViewCamera.transform.position;
                }

                break;
            }
        }
    }
}
