using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;

    [Header("Checkpoint Data")]
    public string id;
    public bool activationStatus;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    /// <summary>
    /// 체크포인트 활성화
    /// </summary>
    public void ActivateCheckpoint()
    {
        if (!activationStatus)
            AudioManager.instance.PlaySFX(4, transform);

        activationStatus = true;
        anim.SetBool("active", true);
    }

    /// <summary>
    /// 저장/로드/새 게임 초기화를 위해 필요한 비활성화 함수
    /// </summary>
    public void DeactivateCheckpoint()
    {
        activationStatus = false;
        anim.SetBool("active", false);
    }
}
