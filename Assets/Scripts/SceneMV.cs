using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMV : MonoBehaviour
{

    // 0 : Ÿ��Ʋ, 1 : ����, 2 : ����1
    public static string[] SceneName = { "Title", "Town", "DG1", "DG2", "DG3", "DG4", "DG5" };
    //�� �̵� �Լ�, �� �ڵ带 ���ڷ� �޾� �̵�
    public void sceneMV(int SceneCode)
    {
        SceneManager.LoadScene(SceneName[SceneCode]);
    }
}
