using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

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


            int randomSound = Random.Range(34, 35); // 데미지 사운드 재생
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
}
