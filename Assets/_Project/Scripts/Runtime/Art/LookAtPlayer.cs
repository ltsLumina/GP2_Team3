using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Player player;


    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = transform.position - player.transform.position;
    }
}
