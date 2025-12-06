using System.Collections.Generic;
using UnityEngine;

public class CheckpointSave : MonoBehaviour
{
    private string filePath =>
        System.IO.Path.Combine(Application.persistentDataPath, "checkpoints.es3");

    [Header("현재 씬 체크포인트들 (자동 검색됨)")]
    [SerializeField] private Checkpoint[] checkpoints;

    private Transform player;

    private void Awake()
    {
        SaveManager.Instance.RegisterCheckpoints(this);
    }

    /// <summary>
    /// 현재 씬 기준으로 체크포인트 다시 찾아오기
    /// </summary>
    private void RefreshCheckpoints()
    {
        checkpoints = FindObjectsOfType<Checkpoint>(true);
    }

    /// <summary>
    /// 현재 씬 기준으로 플레이어 다시 찾아오기
    /// </summary>
    private void RefreshPlayer()
    {
        var p = FindObjectOfType<Player>();
        player = p != null ? p.transform : null;
    }

    // ─────────────────────────────────────────────
    // 저장
    // ─────────────────────────────────────────────
    public void Save()
    {
        RefreshCheckpoints();

        Dictionary<string, bool> checkpointDict = new Dictionary<string, bool>();

        foreach (var cp in checkpoints)
        {
            if (cp == null)
                continue;

            checkpointDict[cp.id] = cp.activationStatus;
        }

        ES3.Save(SaveKeys.CheckpointDict, checkpointDict, filePath);

        // 가장 가까운 활성 체크포인트 저장
        Checkpoint closest = GameManager.instance.GetClosestActiveCheckpoint();
        string closestId = closest != null ? closest.id : string.Empty;

        ES3.Save(SaveKeys.ClosestCheckpointId, closestId, filePath);
    }

    // ─────────────────────────────────────────────
    // 로드
    // ─────────────────────────────────────────────
    public void Load()
    {
        RefreshCheckpoints();
        RefreshPlayer();

        if (!ES3.FileExists(filePath))
            return;

        // 체크포인트 상태 로드
        Dictionary<string, bool> savedDict =
            ES3.Load(SaveKeys.CheckpointDict, filePath, new Dictionary<string, bool>());

        foreach (var cp in checkpoints)
        {
            if (cp == null)
                continue;

            if (savedDict.TryGetValue(cp.id, out bool isActive))
            {
                if (isActive)
                    cp.ActivateCheckpoint();
                else
                    cp.DeactivateCheckpoint();
            }
        }

        // 플레이어 위치 로드
        string closestId = ES3.Load(SaveKeys.ClosestCheckpointId, filePath, string.Empty);

        if (!string.IsNullOrEmpty(closestId))
            TeleportPlayerToCheckpoint(closestId);
    }

    // ─────────────────────────────────────────────
    // 새 게임 초기화
    // ─────────────────────────────────────────────
    public void ResetToDefault()
    {
        RefreshCheckpoints();

        foreach (var cp in checkpoints)
            if (cp != null)
                cp.DeactivateCheckpoint();

        if (ES3.FileExists(filePath))
            ES3.DeleteFile(filePath);
    }

    // ─────────────────────────────────────────────
    // 플레이어 이동
    // ─────────────────────────────────────────────
    private void TeleportPlayerToCheckpoint(string id)
    {
        if (player == null)
            RefreshPlayer();

        if (player == null)
            return;

        foreach (var cp in checkpoints)
        {
            if (cp != null && cp.id == id)
            {
                player.position = cp.transform.position;
                return;
            }
        }
    }
}
