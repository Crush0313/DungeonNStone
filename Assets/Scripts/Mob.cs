using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] protected string animalName;
    [SerializeField] protected int hp;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float runSpeed;

    protected bool isAction;
    protected bool isWalking;
    protected bool isRunning;
    protected bool isDead = false;

    [SerializeField] protected float walkTime;
    [SerializeField] protected float runTime;
    [SerializeField] protected float waitTime;
    protected float currentTime;

    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;

    protected NavMeshAgent nav;
    protected Vector3 destination;

    protected AudioSource theAudio;
    [SerializeField] protected AudioClip[] SE_Normal;
    [SerializeField] protected AudioClip SE_Hurt;
    [SerializeField] protected AudioClip SE_Dead;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();//기본적으로 리지드바디를 잠궈버림
        theAudio = GetComponent<AudioSource>();

        //액션 : 대기
        currentTime = waitTime;
        isAction = true;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            ElaspeTime();
            Move();
            //Rotation();
        }
    }
    private void Move()
    {
        if (isWalking || isRunning)
        {

            //rigid.MovePosition(transform.position + (transform.forward * nav.speed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
        }

    }

    protected void ElaspeTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                _Reset();
        }
    }
    protected virtual void _Reset()
    {
        isWalking = false;
        isRunning = false;
        isAction = true;
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        nav.speed = walkSpeed;
        nav.ResetPath();
        destination.Set(Random.Range(-0.2f, 0.2f), 0, Random.Range(0.5f, 1f));

    }
    */
    public void Damage(int _dmg/*, Vector3 _targetPos*/)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
                Dead();
            else
            {
                Debug.Log("아야");   
                /*
                anim.SetTrigger("Hurt");
                PlaySE(SE_Hurt);
                //Run(_targetPos);
                */
            }
        }
    }
    protected void Dead()
    {
        Destroy(this.gameObject);
        /*
        anim.SetTrigger("Dead");
        PlaySE(SE_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        */

    }
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
        int _random = Random.Range(0, 3); //일상 사운드 3개
        PlaySE(SE_Normal[_random]);
    }
    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
    */
}
