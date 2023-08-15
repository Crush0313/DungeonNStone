using AFPC;
using AquariusMax.UPF;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class DropItem
{
    public int Id;
    public int Value;
}

public class Mob : MonoBehaviour
{
    public int MaxHp;
    [SerializeField] public int hp;
    public int Dmg;
    [SerializeField] protected float walkSpeed;

    Transform playerTf;
    NavMeshAgent nav; //�⺻������ ������ٵ� ��Ź���
    [SerializeField] protected Animator anim;
    [SerializeField] protected AudioSource theAudio;
    Inventory inv;

    [SerializeField] protected AudioClip[] SE_Normal;
    [SerializeField] protected AudioClip SE_Hurt;
    [SerializeField] protected AudioClip SE_Dead;

    public enum CurrentState { idle, trace, attack };
    public CurrentState curState = CurrentState.idle;


    public float traceDist = 15.0f; // ���� �����Ÿ�
    public float attackDist = 3.2f; // ���� �����Ÿ�

    public float AtkCool = 4f;
    public float currentAtkCool = 0f;

    public int DropXp;
    public DropItem[] DropItemList; //10% ����, �ν����Ϳ� ���̷��� Ŭ������ ����
   


    // ��� ����
    public bool isDead = false;

    public void GetDamage(int dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            Dead();
        }
        else
            RandAnimTrigger("Hit1", "Hit2");
    }

    void Dead()
    {
        isDead = true;
        RandAnimTrigger("Die1", "Die2");
    }
    //�ִϸ��̼� Ŭ������ ȣ��, read only �ִϸ��̼��� ������ object���� �� ��ũ��Ʈ�� �־���� ��
    public void DestroySelf()
    {
        Debug.Log("�ֱ�");
        ExpDrop();
        ItemDrop();
        //        Destroy(gameObject);
        transform.parent.GetComponent<SpawnManager>().OrderRespawn();
        gameObject.SetActive(false);
    }

    public  void ExpDrop()
    {
        playerTf.GetComponent<Status>().ExpSum(DropXp);
    }

    public void ItemDrop()
    {
        Debug.Log("���� ���");
        for (int i = 0; i < DropItemList.Length; i++)
        {
            GiveItem(DropItemList[i].Id, DropItemList[i].Value);
        }

        inv.updateItemList();
        inv.stackableSettings();
    }
    public void GiveItem(int ID, int Chance)
    {
        if(Random.Range(1,11) <= Chance) //1234567 �� 7���� �۰ų� ����
            inv.addItemToInventory(ID, 1);
    }

    //�ִϸ��̼� Ŭ������ ȣ��
    public void GiveDamage()
    {
        Debug.Log("����");
        if (playerTf != null)
            playerTf.GetComponent<Player>().lifecycle.Damage(Dmg);
    }

    void RandAnimTrigger(string _Tr1, string _Tr2)
    {
        int r = Random.Range(0,2);

        if (r == 1)
            anim.SetTrigger(_Tr1);
        else
            anim.SetTrigger(_Tr2);    
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
    }

    // Use this for initialization
    void Start()
    {
        playerTf = Player.PlayerTF;
        inv = playerTf.GetComponent<PlayerInventory>().mainInventory;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = walkSpeed;

        hp = MaxHp;

        StartCoroutine(CheckState());
        StartCoroutine(CheckStateForAction());
    }

    //�Ÿ��� ���� ���� ��ȯ
    IEnumerator CheckState()
    {
        while (!isDead)
        {
            float dist = Vector3.Distance(playerTf.position, transform.position);
            
            //���ݰŸ�
            if (dist <= attackDist) 
                curState = CurrentState.attack;

            //�����Ÿ�
            else if (dist <= traceDist) 
                curState = CurrentState.trace;

            //�����Ÿ�
            else 
                curState = CurrentState.idle;

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator CheckStateForAction()
    {
        while (!isDead)
        {
            if(currentAtkCool > 0)
                currentAtkCool -= Time.deltaTime;

            switch (curState)
            {
                case CurrentState.idle: //����
                    Idle();
                    break;
                case CurrentState.trace: //����
                    Trace();
                    break;
                case CurrentState.attack: //����
                    Attack();
                    break;
            }

            yield return null;
        }
    }

    void Idle()
    {
        nav.isStopped = true;

        anim.SetBool("isTrace", false);
        anim.SetBool("isAtk", false);
    }
    void Trace()
    {
        nav.destination = playerTf.position;
        nav.isStopped = false;
        transform.LookAt(playerTf);

        anim.SetBool("isTrace", true);
        anim.SetBool("isAtk", false);
    }

    void Attack()
    {
        transform.LookAt(playerTf);

        if (currentAtkCool <= 0)
        {
            anim.SetTrigger("Atk");
            currentAtkCool = AtkCool; //�� �ʱ�ȭ
        }

        anim.SetBool("isTrace", false);
        anim.SetBool("isAtk", true);
    }

    /*
    public void Damage(int _dmg/*, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
                Dead();
            else
            {
                Debug.Log("�ƾ�");   
                /*
                anim.SetTrigger("Hurt");
                PlaySE(SE_Hurt);
                //Run(_targetPos);
                
            }
        }
    }
*/
    /*
    protected void Dead()
    {
        Destroy(this.gameObject);
        
        anim.SetTrigger("Dead");
        PlaySE(SE_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        

    }*/
    /*
    protected void Wait()
    {
        currentTime = waitTime;

    }
    protected void TryWalk()
    {
        currentTime = walkTime;
        isWalking = true;
        nav.speed = walkSpeed;
        anim.SetBool("Walking", isWalking);
    }

    protected void PlayRandomSE()
    {
        int _random = Random.Range(0, 3); //�ϻ� ���� 3��
        PlaySE(SE_Normal[_random]);
    }
    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
    */

    


}
