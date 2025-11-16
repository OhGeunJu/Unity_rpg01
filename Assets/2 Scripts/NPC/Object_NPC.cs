using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_NPC : MonoBehaviour
{
    protected Transform player;
    protected UI ui;

    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactToolTip;
    //private bool facingRight = true;

    [Header("Floaty Tooltip")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    protected bool isPlayerInRange = false;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactToolTip.transform.position;
        interactToolTip.SetActive(false);
    }
    
    protected virtual void Update()
    {
        //handlenpcflip();
        HandleToolTipFloat();
    }

    private void HandleToolTipFloat()
    {
        if (interactToolTip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactToolTip.transform.position = startPosition + new Vector3(0, yOffset);
        }
    }

    //private void HandleNpcFlip()
    //{
    //    if (player == null || npc == null)
    //        return;

    //    if (npc.position.x > player.position.x && facingRight)
    //    {
    //        npc.transform.Rotate(0,180,0);
    //        facingRight = false;
    //    }
    //    else if (npc.position.x < player.position.x && facingRight == false)
    //    {
    //        npc.transform.Rotate(0, 180, 0);
    //        facingRight = true;
    //    }
    //}

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        player = collision.transform;
        isPlayerInRange = true;
        interactToolTip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

        isPlayerInRange = false;
        interactToolTip.SetActive(false);
    }

    public virtual void OnTalk() // 대화 시작 시(트리거/이벤트에서 호출)
    {
        //ui.OpenCraftUI();
    }

    public virtual void OnEndTalk() // 대화 종료 시(원하면)
    {
        ui.CloseToInGameUI();
    }
}
