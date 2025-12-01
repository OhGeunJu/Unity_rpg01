using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    // 점 애니메이션용
    private float dotTimer = 0f;
    private int dotCount = 1;

    private void Start()
    {
        if (loadingBar != null)
            loadingBar.value = 0f;

        if (string.IsNullOrEmpty(LoadingSceneLoader.nextSceneName))
        {
            Debug.LogError("다음에 로드할 씬 이름이 설정되지 않았습니다.");
            return;
        }

        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return null;

        string sceneName = LoadingSceneLoader.nextSceneName;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            // 슬라이더 갱신
            if (loadingBar != null)
                loadingBar.value = progress;

            // 점 애니메이션 (0.4초마다 변경)
            dotTimer += Time.deltaTime;
            if (dotTimer >= 0.4f)
            {
                dotTimer = 0f;
                dotCount++;
                if (dotCount > 3)
                    dotCount = 1;
            }

            // 텍스트 갱신: 로딩 중 + 점 + 퍼센트
            if (loadingText != null)
            {
                string dots = new string('.', dotCount);
                int percent = Mathf.RoundToInt(progress * 100f);
                loadingText.text = $"로딩 중{dots} {percent}%";
            }

            // 로드 완료 후 잠시 대기 후 진입
            if (progress >= 1f)
            {
                timer += Time.deltaTime;
                if (timer >= 0.5f)
                {
                    op.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
