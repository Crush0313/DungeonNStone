using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMVbutton : MonoBehaviour
{
    public void SceneMove(int _mapCode)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<SceneMV>().sceneMV(_mapCode);
    }
}
