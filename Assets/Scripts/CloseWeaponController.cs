using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ī�޶� �ٿ��� ī�޶� ���� ���� ������
public class CloseWeaponController : MonoBehaviour
{
    protected bool isAttack = false;
    public float attackDelay;

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

    //�ִϸ��̼ǿ��� ȣ��
    public void Attack()
    {
        if (chkObj())
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.transform.tag == "Mob")
            {
                //SoundManager.instance.PlaySE("Hit");
                hitInfo.transform.GetComponent<Mob>().GetDamage(status.Dmg);
            }
        }
    }


    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        WeaponAnim.SetTrigger("Atk");
        SoundManager.instance.PlaySE("Atk_Sword");
        yield return new WaitForSeconds(attackDelay);
        isAttack = false; //����� ����
    }

    bool chkObj()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 8, layerMask))
            return true;
        else
            return false;
    }

}
