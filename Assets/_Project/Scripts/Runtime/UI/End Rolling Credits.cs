using NoSlimes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Example : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChangeMenu();
        }
    }
    private void SceneChangeMenu()
    {
        SceneLoader.Instance.LoadScene((int)SceneIndexes.MAIN_MENU);
    }
}