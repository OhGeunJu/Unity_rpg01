using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerStats : CharacterStats
{
    private Player player;

    [Header("Level")]
    public int level = 1;           // 현재 레벨
    public int Exp = 0;      // 현재 경험치
    public int expToNextLevel = 100; // 다음 레벨까지 필요한 경험치
    public int statPoints = 0;      // 아직 안 찍은 스탯 포인트

    public event Action<int> onLevelChanged;
    public event Action<int, int> onExpChanged; // (currentExp, expToNextLevel)
    public event Action<int> onStatPointChanged;
    public event Action onStatsChanged;

    protected override void Start()
    {
        base.Start();

        player= GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die() // 사망 처리
    {
        base.Die(); // 기본 사망 처리
        player.Die(); // 플레이어 사망 처리

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency; // 잃은 골드 저장
        PlayerManager.instance.currency = 0; // 플레이어 골드 0으로 초기화

        GetComponent<PlayerItemDrop>()?.GenerateDrop(); // 아이템 드롭 생성
    }

    protected override void DecreaseHealthBy(int _damage) // 체력 감소
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * .3f ) // 데미지를 입었을 때
        {
            player.SetupKnockbackPower(new Vector2(10,6)); // 넉백 설정
            player.fx.ScreenShake(player.fx.shakeHighDamage); // 화면 흔들림 효과


            int randomSound = UnityEngine.Random.Range(34, 35); // 데미지 사운드 재생
            AudioManager.instance.PlaySFX(randomSound, null); // 플레이어 데미지 사운드 재생

        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor); // 장착된 갑옷의 효과 실행

        if (currentArmor != null) // 장착된 갑옷이 있을 때
            currentArmor.Effect(player.transform); // 갑옷 효과 실행
    }

    public override void OnEvasion() // 회피 시
    {
        player.skill.dodge.CreateMirageOnDodge(); // 회피 시 신기루 생성
    }

    public void CloneDoDamage(CharacterStats _targetStats,float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);


        DoMagicalDamage(_targetStats); // remove if you don't want to apply magic hit on primary attack
    }

    public void GainExp(int amount)
    {
        Exp += amount;
        onExpChanged?.Invoke(Exp, expToNextLevel);

        // 여러 레벨이 한 번에 오를 수도 있으니 while
        while (Exp >= expToNextLevel)
        {
            Exp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        // 레벨업할 때 줄 스탯 포인트 양 (원하는 대로 조정 가능)
        statPoints += 3;

        // 다음 레벨까지 필요한 경험치 증가 (계단식 성장)
        expToNextLevel = CalculateNextExpRequirement();

        // 이벤트 호출해서 UI 갱신할 수 있게
        onLevelChanged?.Invoke(level);
        onExpChanged?.Invoke(Exp, expToNextLevel);
        onStatPointChanged?.Invoke(statPoints);
    }

    private int CalculateNextExpRequirement()
    {
        // 1.2배씩 증가
        return Mathf.RoundToInt(expToNextLevel * 1.2f);
    }

    public void AllocateStatPoint(StatType statType)
    {
        if (statPoints <= 0)
            return;

        Stat targetStat = GetStat(statType);
        if (targetStat == null)
            return;

        // 체력 관련 스탯이면 최대 체력 재계산 및 현재 체력 조정
        if (statType == StatType.vitality)
        {
            int beforeMax = GetMaxHealthValue();

            
            targetStat.baseValue += 1;
            statPoints--;

            // 체력 올리기
            int afterMax = GetMaxHealthValue();
            int diff = afterMax - beforeMax;

            currentHealth += diff;

            if (currentHealth > afterMax)
                currentHealth = afterMax;

            onHealthChanged?.Invoke();
            onStatsChanged?.Invoke();
            onStatPointChanged?.Invoke(statPoints);
            return;
        }

        //Stat에 +1
        targetStat.baseValue += 1;

        statPoints--;
        onStatsChanged?.Invoke();
        onStatPointChanged?.Invoke(statPoints);
    }

    public void ResetPoint(int level)
    {
        List<StatType> statTypes = new List<StatType>()
        {
            StatType.strength,
            StatType.agility,
            StatType.intelligence,
            StatType.vitality
        };

        foreach (StatType statType in statTypes)
        {
            Stat stat = GetStat(statType);
            statPoints += stat.baseValue;
            stat.SetDefaultValue(0);
        }

        onHealthChanged?.Invoke();
        onStatsChanged?.Invoke();
        onStatPointChanged?.Invoke(statPoints);
    }

    public void ResetPoint()
    {
        ResetPoint(level);
    }
}
