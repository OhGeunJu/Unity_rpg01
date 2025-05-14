using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;
    
    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;
    
    private bool canDuplicateClone;
    private float chanceToDuplicate;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
        }

        if(sr.color.a <= 0)
            Destroy(gameObject);
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate, Player _player)
    {
        if(_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
            {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream:Assets/2 Scripts/Skill/Skill_Controllers/Clone_Skill_Controller.cs
                hit.GetComponent<Enemy>().Damage();
=======
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());
>>>>>>> Stashed changes:Assets/2 Scripts/Controller/Skill_Controllers/Clone_Skill_Controller.cs
=======
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());
>>>>>>> Stashed changes
=======
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());
>>>>>>> Stashed changes

                if(canDuplicateClone)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget() //clone skill target
    {
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}