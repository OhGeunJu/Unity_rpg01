using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount; // 영혼 드랍 양

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = .4f;

    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100); // 기본 드랍 양 설정
        ApplyLevelModifiers(); // 레벨에 따른 스탯 수정 적용

        base.Start(); // 기본 스타트 호출

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers() // 레벨에 따른 스탯 수정 적용
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

        Modify(soulsDropAmount);
    }

    private void Modify(Stat _stat)
    {
        for (int i = 1; i < level; i++) // 레벨에 따라 스탯 수정 적용
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage) // 적용된 데미지 받기
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        myDropSystem.GenerateDrop(); // 아이템 드랍 생성


        enemy.Die();

        PlayerManager.instance.currency += soulsDropAmount.GetValue(); // 플레이어에게 영혼 추가


        Destroy(gameObject, 5f);
    }
}
