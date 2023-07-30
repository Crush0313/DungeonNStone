using UnityEngine.SceneManagement;

public class SceneMV
{
    // 0 : 타이틀, 1 : 마을, 2 : 던전1
    public static string[] SceneName = { "MainTitle", "Town", "DG1", "DG2", "DG3", "DG4", "DG5" };
    //씬 이동 함수, 씬 코드를 인자로 받아 이동
    public void sceneMV(int SceneCode)
    {
        SceneManager.LoadScene(SceneName[SceneCode]);
    }
}
