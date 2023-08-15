using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMV : MonoBehaviour
{

    // 0 : Ÿ��Ʋ, 1 : ����, 2 : ����1
    public static string[] SceneName = { "Title", "Town", "DG1", "DG2", "DG3", "DG4", "DG5" };
    public Vector3[] ScenePos;
    public GameObject Player;

    private void Start()
    {
        if(SoundManager.instance.audioSourceBGM.clip==null)
            SoundManager.instance.PlayBGM("Title");
    }


    //�� �̵� �Լ�, �� �ڵ带 ���ڷ� �޾� �̵�
    public void sceneMV(int SceneCode)
    {
        SceneManager.LoadScene(SceneName[SceneCode]);
        if(SceneCode > 1)
            Player.transform.position = ScenePos[SceneCode];

        switch (SceneCode)
        {
            case 0:
                break;
            case 1:
                SoundManager.instance.PlayBGM("Town_Main");
                break;
            case 2:
                SoundManager.instance.PlayBGM("Battle");
                break;
            default:
                break;
        }
    }


}