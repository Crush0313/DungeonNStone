using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mob;

public class SpawnManager : MonoBehaviour
{
    public float RespawnTime;
    GameObject GO_Mob;
    Mob _mob;

    public void OrderRespawn()
    {
        GO_Mob = transform.GetChild(0).gameObject;
        _mob = GO_Mob.GetComponent<Mob>();
        Invoke("RespawnMob", RespawnTime);
    }

    public void RespawnMob()
    {
        GO_Mob.SetActive(true);
        _mob.curState = CurrentState.idle;
        _mob.hp = _mob.MaxHp;
        _mob.isDead = false;
        //GO_Mob.transform.position = Vector3.zero;
    }
}
