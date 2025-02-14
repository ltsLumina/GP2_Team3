using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Teleport destination;

    public bool isDestination = false;

    private bool isCooldown;

    [SerializeField] private FMODUnity.EventReference teleportSFX;

    private void OnTriggerEnter(Collider other)
    {
        if (isCooldown)
            return;

        if (other.gameObject.tag == "Player" && !isDestination)
        {
            if (destination == null)
                Debug.Log("No destination assigned");
            Debug.Log("Teleporting");


            FMODUnity.RuntimeManager.PlayOneShotAttached(teleportSFX, GameManager.Instance.Player.gameObject);
            CharacterController controller = GameManager.Instance.Player.GetComponent<CharacterController>();
            controller.enabled = false;
            GameManager.Instance.Player.transform.position = destination.transform.position;
            controller.enabled = true;
            destination.isDestination = true;

            //Very bad quick fix
            isCooldown = true;
            Invoke(nameof(EndCooldown), 2f);
        }
    }

    private void EndCooldown() => isCooldown = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && isDestination)
            isDestination = false;
    }
}
