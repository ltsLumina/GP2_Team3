using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void UpdateFollow()
    {
        if (GameManager.Instance.Player.Health.IsDead) return;

        Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 rotation = Input.mousePosition - playerScreenPosition;
        float lookAngle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        lookAngle -= 45;
        transform.rotation = Quaternion.Euler(0, -lookAngle, 0);
    }

#if false
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Mouse Position: {Input.mousePosition}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Player Position: {transform.position}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Screen Player Position: {mainCamera.WorldToScreenPoint(transform.position)}");

        GUI.Label(new Rect(10, 70, 300, 20), $"MainCameras: {GameObject.FindGameObjectsWithTag("MainCamera").Length}");
        GUI.Label(new Rect(10, 90, 300, 20), $"Cameras: {FindObjectsByType<Camera>(FindObjectsSortMode.None).Length}");
        //GUI.Label(new Rect(10, 50, 100, 20), $"Rotation: {}");
    }
#endif
}
