using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAtkCall : MonoBehaviour
{
    public CloseWeaponController weaponController;

    public void Atk()
    {
        Debug.Log("АјАн");
        weaponController.Attack();
    }
}
