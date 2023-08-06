using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeaponController : MonoBehaviour
{
    protected bool isAttack = false;
    public float attackDelayDmg;
    public float attackDelayAll;

    public Animator WeaponAnim;
    protected RaycastHit hitInfo;
    [SerializeField] protected LayerMask layerMask;

    void Update()
    {
        TryAttack();
    }

    // Update is called once per frame
    protected void TryAttack()
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
            if (hitInfo.transform.tag == "WeakAnimal")
            {
                //SoundManager.instance.PlaySE("Animal_Hit");
                //hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentCloseWeapon.damage, transform.position);
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
