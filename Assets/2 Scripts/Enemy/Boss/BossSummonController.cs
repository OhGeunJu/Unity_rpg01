using UnityEngine;

public class BossSummonController : MonoBehaviour
{
    [Header("Spawn Layout")]
    [SerializeField] private Transform[] summonPoints; // 있으면 여기서 스폰
    [SerializeField] private float radius = 2.5f;      // 없으면 원형 배치
    [SerializeField] private float heightOffset = 0f;

    [Header("Per Cast")]
    public int SummonCountPerCast = 2;

    [Header("Servant Prefab")]
    [SerializeField] private Enemy_Servant[] servantPrefabs;
    [SerializeField][Range(0f, 1f)] private float probA = 0.5f;

    private Enemy_Boss owner;

    public void Init(Enemy_Boss boss) => owner = boss;

    // 애니메이션 이벤트에서 호출: 조건 만족 시 즉시 스폰
    public void TrySummonNow()
    {
        if (owner == null) return;
        if (!owner.CanSummon()) return;

        int want = SummonCountPerCast;
        int slotsLeft = Mathf.Max(0, owner.maxMinionsOnField - owner.CurrentMinionCount);
        int budgetLeft = (owner.totalSummonBudget > 0)
            ? Mathf.Max(0, owner.totalSummonBudget - owner.totalSummoned)
            : int.MaxValue;

        int can = Mathf.Min(want, slotsLeft, budgetLeft);
        if (can < 2) return; // 최소 2마리 이상 소환해야 함
        DoSummon(can);
    }

    // 실제 생성(풀에서 꺼내기) + 등록
    public void DoSummon(int count)
    {
        if (servantPrefabs == null || servantPrefabs.Length == 0)
        {
            Debug.LogError("보스소환컨트롤러에 프리팹이 비어있음");
            return;
        }

        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetSummonPos(i, count);

            var prefab = ChooseSimple();

            var servant = Instantiate(prefab, pos, Quaternion.identity);
            if (servant != null)
            {
                servant.Init(owner, this);
                spawned++;
            }
        }
        if (spawned > 0)
        {
            owner.OnMinionSpawned(spawned);
            // 소환 직후 30초(= baseSummonCooldown) 쿨타임
            owner.nextSummonTime = Time.time + owner.baseSummonCooldown;
        }
    }

    private Enemy_Servant ChooseSimple()
    {
        // A/B 두 개일 때 간단 확률
        if (servantPrefabs.Length == 1) return servantPrefabs[0];
        if (servantPrefabs.Length >= 2)
        {
            // index 0 = A, index 1 = B 라고 가정
            return (Random.value < probA) ? servantPrefabs[0] : servantPrefabs[1];
        }
        // 방어
        return servantPrefabs[Random.Range(0, servantPrefabs.Length)];
    }

    private Vector3 GetSummonPos(int idx, int count)
    {
        if (summonPoints != null && summonPoints.Length > 0)
        {
            var p = summonPoints[idx % summonPoints.Length].position;
            return new Vector3(p.x, p.y + heightOffset, p.z);
        }
        float angle = (360f / Mathf.Max(1, count)) * idx * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
        var basePos = transform.position + offset;
        basePos.y += heightOffset;
        return basePos;
    }
}
