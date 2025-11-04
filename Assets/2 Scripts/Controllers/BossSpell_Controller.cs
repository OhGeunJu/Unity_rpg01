using UnityEngine;

public class BossSpell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private BossStats myStats;

    public void SetupSpell(BossStats _stats)
    {
        myStats = _stats;
    }

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, 0, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoBossDamage(hit.GetComponent<PlayerStats>(), 0.3f, 0);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(check.position, boxSize);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
