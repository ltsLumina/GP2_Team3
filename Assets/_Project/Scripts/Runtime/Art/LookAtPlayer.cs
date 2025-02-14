using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }
    void Update()
    {
        transform.up = transform.position - player.transform.position + new Vector3(0, -1.5f, 0);
    }
}
