using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player & Checkpoints")]
    private Transform player;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    public Vector2 lostCurrencyPosition;

    private bool pausedGame = false;

    private void Awake()
    {
        // 싱글톤
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // 테스트용 씬 리스타트 (M키)
        if (Input.GetKeyDown(KeyCode.M))
            RestartScene();

        // 일시정지 (G키)
        if (Input.GetKeyDown(KeyCode.G))
        {
            pausedGame = !pausedGame;
            PauseGame(pausedGame);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬이 로드될 때마다 초기화 코루틴 실행
        StartCoroutine(SceneInitRoutine());
    }

    private IEnumerator SceneInitRoutine()
    {
        // 씬 내 오브젝트들(Awake/OnEnable/Start) 초기화 끝날 때까지 한 프레임 대기
        yield return null;

        // 1) 새 씬의 체크포인트/플레이어 다시 찾기
        checkpoints = FindObjectsOfType<Checkpoint>(true);

        Player newPlayer = FindObjectOfType<Player>();

        if (newPlayer != null)
        {
            player = newPlayer.transform;
        }
        else
        {
            // 이 씬에는 플레이어가 없는 씬(예: 로비, 메인 메뉴)일 수 있음
            player = null;
        }

        // 2) 저장된 데이터 기준으로 각 시스템 다시 세팅
        SaveManager.Instance.LoadGame();

        // 3) 잃어버린 화폐가 있다면 다시 생성
        SpawnLostCurrencyIfNeeded();
    }

    // 기존 Start, StartRoutine은 이제 필요 없으면 제거해도 됩니다.
    // 필요하다면 Start에서는 아무 것도 안 해도 됩니다.


// ───────────────────────────────────────────
// 씬 재시작
// ───────────────────────────────────────────
public void RestartScene()
    {
        SaveManager.Instance.SaveGame();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // ───────────────────────────────────────────
    // 잃은 화폐가 있다면 Instantiate
    // (PlayerStatsSave.Load() 이후 호출)
    // ───────────────────────────────────────────
    public void SpawnLostCurrencyIfNeeded()
    {
        if (lostCurrencyAmount > 0)
        {
            GameObject lostObj = Instantiate(
                lostCurrencyPrefab,
                lostCurrencyPosition,
                Quaternion.identity
            );

            lostObj.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;

            // 생성 후 내부 값 초기화
            lostCurrencyAmount = 0;
        }
    }

    // ───────────────────────────────────────────
    // 체크포인트 관련 기능
    // ───────────────────────────────────────────

    public void TeleportToCheckpoint(string id)
    {
        foreach (Checkpoint c in checkpoints)
        {
            if (c.id == id)
            {
                player.position = c.transform.position;
                return;
            }
        }
    }

    // 플레이어 위치 기준 가장 가까운 활성 체크포인트 찾기
    public Checkpoint GetClosestActiveCheckpoint()
    {
        float minDist = Mathf.Infinity;
        Checkpoint closest = null;

        foreach (var cp in checkpoints)
        {
            if (!cp.activationStatus) continue;

            float dist = Vector2.Distance(player.position, cp.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = cp;
            }
        }

        return closest;
    }

    // ───────────────────────────────────────────
    // 게임 일시정지
    // ───────────────────────────────────────────
    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
