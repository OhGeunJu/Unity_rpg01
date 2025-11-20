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

    private void Start()
    {
        // 씬 내 모든 체크포인트 가져오기
        checkpoints = FindObjectsOfType<Checkpoint>(true);

        // 플레이어 Transform 가져오기
        player = PlayerManager.instance.player.transform;
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
