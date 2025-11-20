using System.Collections.Generic;
using UnityEngine;

public class CheckpointSave : MonoBehaviour
{
    [Header("연결된 체크포인트들 (없으면 자동 검색)")]
    [SerializeField] private Checkpoint[] checkpoints;

    private Transform player;

    private void Awake()
    {
        // 체크포인트 자동 검색
        if (checkpoints == null || checkpoints.Length == 0)
            checkpoints = FindObjectsOfType<Checkpoint>(true);

        player = PlayerManager.instance.player.transform;
    }

    // ─────────────────────────────────────────────
    // 저장
    // ─────────────────────────────────────────────
    public void Save()
    {
        Dictionary<string, bool> checkpointDict = new Dictionary<string, bool>();

        foreach (var cp in checkpoints)
            checkpointDict[cp.id] = cp.activationStatus;

        ES3.Save(SaveKeys.CheckpointDict, checkpointDict);

        // 가장 가까운 활성 체크포인트 저장
        Checkpoint closest = GameManager.instance.GetClosestActiveCheckpoint();

        if (closest != null)
            ES3.Save(SaveKeys.ClosestCheckpointId, closest.id);
        else
            ES3.Save(SaveKeys.ClosestCheckpointId, string.Empty);
    }

    // ─────────────────────────────────────────────
    // 로드
    // ─────────────────────────────────────────────
    public void Load()
    {
        // 체크포인트 상태 로드
        Dictionary<string, bool> savedDict =
            ES3.Load(SaveKeys.CheckpointDict, new Dictionary<string, bool>());

        foreach (var cp in checkpoints)
        {
            if (savedDict.TryGetValue(cp.id, out bool isActive))
            {
                if (isActive)
                    cp.ActivateCheckpoint();    // 저장된 체크포인트 활성화
                else
                    cp.DeactivateCheckpoint();  // 필요하면 구현 (없으면 무시)
            }
        }

        // 플레이어 스폰 위치 로드
        string closestId = ES3.Load<string>(SaveKeys.ClosestCheckpointId, string.Empty);

        if (!string.IsNullOrEmpty(closestId))
            TeleportPlayerToCheckpoint(closestId);
    }

    // ─────────────────────────────────────────────
    // 새 게임 초기화
    // ─────────────────────────────────────────────
    public void ResetToDefault()
    {
        // 모든 체크포인트 비활성화
        foreach (var cp in checkpoints)
            cp.DeactivateCheckpoint();

        // 저장된 정보 삭제
        ES3.DeleteKey(SaveKeys.CheckpointDict);
        ES3.DeleteKey(SaveKeys.ClosestCheckpointId);
    }

    // ─────────────────────────────────────────────
    // 플레이어 이동
    // ─────────────────────────────────────────────
    private void TeleportPlayerToCheckpoint(string id)
    {
        foreach (var cp in checkpoints)
        {
            if (cp.id == id)
            {
                player.position = cp.transform.position;
                return;
            }
        }
    }
}
