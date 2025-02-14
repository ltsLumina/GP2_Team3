using NoSlimes;
using UnityEngine;

public class GameEnder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadScene((int)SceneIndexes.END_MENU);
        }
    }
}
