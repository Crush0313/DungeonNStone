using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라에 붙여야 카메라가 보는 곳을 공격함
public class CloseWeaponController : MonoBehaviour
{
    protected bool isAttack = false;
    public float attackDelayDmg;
    public float attackDelayAll;

    public Status status;
    public Animator WeaponAnim;
    protected RaycastHit hitInfo;
    [SerializeField] protected LayerMask layerMask;

    // Update is called once per frame
    public void TryAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    void Attack()
    {
        if (chkObj())
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.transform.tag == "Mob")
            {
                //SoundManager.instance.PlaySE("Animal_Hit");
                hitInfo.transform.GetComponent<Mob>().GetDamage(status.Dmg);
            }
        }
    }


    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        WeaponAnim.SetTrigger("Atk");

        yield return new WaitForSeconds(attackDelayDmg);
        SoundManager.instance.PlaySE("Atk_Sword");
        Attack();
        yield return new WaitForSeconds(attackDelayAll);

        yield return new WaitForSeconds(attackDelayAll - attackDelayDmg);
        isAttack = false; //재공격 가능
    }

    bool chkObj()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 8, layerMask))
            return true;
        else
            return false;
    }

}
