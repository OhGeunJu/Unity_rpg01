using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Data")]
    public string id;
    public bool activationStatus;

    [Header("Animator")]
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // 씬 로드 후 최초 상태 반영
        UpdateVisual();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
            return;

        ActivateCheckpoint();
    }

    /// <summary>
    /// 체크포인트 활성화
    /// </summary>
    public void ActivateCheckpoint()
    {
        //if (!activationStatus)
            //AudioManager.instance.PlaySFX(4, transform);

        activationStatus = true;
        UpdateVisual();
    }

    /// <summary>
    /// 체크포인트 비활성화 (로드/새 게임 시 사용)
    /// </summary>
    public void DeactivateCheckpoint()
    {
        activationStatus = false;
        UpdateVisual();
    }

    /// <summary>
    /// Animator와 비주얼을 activationStatus에 맞게 갱신
    /// </summary>
    private void UpdateVisual()
    {
        if (anim != null && this != null)
            anim.SetBool("active", activationStatus);
    }
}
