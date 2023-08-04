using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public SceneMV sceneMV;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            sceneMV.sceneMV(2);
        // 원하는 코드 작성
    }
}
