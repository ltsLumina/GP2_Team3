#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// A debug class to allow for easy testing of the game.
/// </summary>
public class DEV_Sandbox : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;

    int enemyCount = 5;
    
    void OnGUI()
    {
        if (SceneManagerExtended.ActiveSceneName != "Kit Sandbox") return; // only allow this to be drawn in the Kit Sandbox scene
     
        // draw a slider above enemy heads to show their health
        foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            var screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            GUI.HorizontalSlider(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 50, 100, 20), enemy.CurrentHealth, 0, enemy.MaxHealth);
        }
        
        GUILayout.BeginArea(new Rect(Screen.width - 110, 10, 100, 800));
        GUILayout.BeginVertical();

        if (GUILayout.Button("Spawn Enemy", GUILayout.Height(50)))
        {
            var enemy = Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            enemy.name = "Enemy";
        }
        
        // spawn a chosen amount of enemies
        // slider to choose how many enemies to spawn
        GUILayout.Label("Spawn Enemies");
        enemyCount = (int) GUILayout.HorizontalSlider(enemyCount, 1, 10);
        if (GUILayout.Button("Spawn", GUILayout.Height(25)))
        {
            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                enemy.name = "Enemy";
            }
        }

        // make player invincible
        if (GUILayout.Button("Invulnerable", GUILayout.Height(50)))
        {
            var player = FindAnyObjectByType<Player>();
            player.Health.ToggleInvincible();
        }

        if (GUILayout.Button("Kill Player", GUILayout.Height(50)))
        {
            var player = FindAnyObjectByType<Player>();
            player.Health.CurrentHealth = 0;
        }

        if (GUILayout.Button("Refill Mana", GUILayout.Height(50)))
        {
            var player = FindAnyObjectByType<Player>();
            player.Mana.CurrentMana = player.Mana.MaxMana;
        }

        if (GUILayout.Button("Level Up", GUILayout.Height(50))) { Experience.EDITOR_GainLevel(); }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

#endif