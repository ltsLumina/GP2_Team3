using System;
using UnityEngine;

public class HideWeaponOnStart : MonoBehaviour
{
    [SerializeField] GameObject weapon;

    public void ShowWeapon()
    {
        weapon.SetActive(true);
        Destroy(this);
    }
    
    void Start()
    {
        weapon.SetActive(false);
    }
    
}
