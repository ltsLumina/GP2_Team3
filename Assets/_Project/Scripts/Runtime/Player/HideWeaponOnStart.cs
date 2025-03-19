using UnityEngine;

public class HideWeaponOnStart : MonoBehaviour
{
    [SerializeField] GameObject weapon;

    public void ShowWeapon()
    {
        weapon.SetActive(true);
        GameManager.Instance.Player.HasWeapon = true;
        Destroy(this);
    }
    
    void Start()
    {
        weapon.SetActive(false);
    }
    
}
