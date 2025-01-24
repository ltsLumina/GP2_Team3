using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Teleport destination;

    public bool isDestination = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isDestination)
        {
            if (destination == null)
                Debug.Log("No destination assigned");
            Debug.Log("Teleporting");

            CharacterController controller = GameManager.Instance.Player.GetComponent<CharacterController>();
            controller.enabled = false;
            GameManager.Instance.Player.transform.position = destination.transform.position;
            controller.enabled = true;
            destination.isDestination = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && isDestination)
            isDestination = false;
    }
}
