using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    #region 싱글톤
    void Awake() //객체 생성 시 최초 실행
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    #endregion

    public string[] playSoundName;

    public AudioSource[] audioSorceEffects; //동시 재생 위해 배열로
    public AudioSource audioSourceBGM;

    public Sound[] effectSounds;
    public Sound[] BGMsound;

    private void Start()
    {
        playSoundName = new string[audioSorceEffects.Length]; //오디오 소스 수만큼 생성
    }

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++) //사운드 라이브러리에서 해당 이름의 사운드 찾기
        {
            if (_name == effectSounds[i].name)
            {
                Debug.Log("찾았음" + i);
                for (int j = 0; j < audioSorceEffects.Length; j++) //재생 중이지  않은 빈 오디오소스 찾기
                {
                    if (!audioSorceEffects[j].isPlaying)
                    {
                        Debug.Log("찾음" + j);
                        Debug.Log(_name);
                        Debug.Log(effectSounds[i].name);

                        playSoundName[j] = effectSounds[i].name;
                        audioSorceEffects[j].clip = effectSounds[i].clip; //클립 넣기
                        audioSorceEffects[j].Play(); //재생
                        return; //함수 종료
                    }
                }
                Debug.Log("모든 가용 audioSorce가 사용중입니다."); //빈 소스가 없음
                return;
            }
        }
        Debug.Log(name + "사운드가 SoundManager에 등록되지 않았습니다."); //등록된 이름의 사운드가 아님
    }
    public void StopAllSE()
    {
        for (int i = 0; i < audioSorceEffects.Length; i++) //순회하며 전부 스톱
        {
            audioSorceEffects[i].Stop();
        }
    }
    public void StopSE(string _name) //특정 사운드 정지
    {
        for (int i = 0; i < audioSorceEffects.Length; i++) //순회하며 전부 스톱
        {
            if (playSoundName[i] == _name) //사운드소스에 들어간 상태에서는 비교 못하니, 동일한 인덱스에 할당되는 문자열 변수 배열을 이용해 탐색
            {
                audioSorceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }
}
