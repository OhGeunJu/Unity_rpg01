using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingSceneLoader
{
    // 다음에 로드할 실제 씬 이름을 저장
    public static string nextSceneName;

    // 외부(포탈 등)에서 호출하는 함수
    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        // 로딩 전용 씬으로 이동
        SceneManager.LoadScene("Loading");
    }
}
