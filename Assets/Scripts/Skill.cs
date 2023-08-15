
using UnityEngine;
using static UnityEngine.UI.Image;

public class Skill : MonoBehaviour
{
    public Status status;
    public ParticleSystem particle;
    public int skillRadius;
    public int Dmg;

    public float Cooltime;
    public float currnetCoolTime;

    // Update is called once per frame
    void Update()
    {
        if (currnetCoolTime > 0){
            currnetCoolTime -= Time.deltaTime;
        }

        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (status.ChkEss(31))
            {
                currnetCoolTime = Cooltime;
                particle.Play();
                SoundManager.instance.PlaySE("Slash");

                int layerMask = 1 << LayerMask.NameToLayer("Mob");
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, skillRadius, Vector3.up, 0, layerMask);
                foreach (var hit in hits)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    hit.collider.GetComponent<Mob>().GetDamage(Dmg);
                }
            }
        }
    }

}
