using System.Collections;
using UnityEngine;


public enum StatType // 능력치 종류
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")] // 주요 능력치
    public Stat strength; // 1점 증가시 데미지 1 증가 및 치명타 파워 1% 증가
    public Stat agility;  // 1점 증가시 회피 1%, 치명타 기회 1% 증가
    public Stat intelligence; // 1점 증가시 마법 데미지 1 증가 및 마법 저항력 3 증가
    public Stat vitality; // 1점 증가시 최대 체력 5 증가

    [Header("Offensive stats")] // 공격 능력치
    public Stat damage;
    public Stat critChance;
    public Stat critPower;              // 기본 치명타 비율 150%

    [Header("Defensive stats")] // 방어 능력치
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")] // 마법 능력치
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    public bool isIgnited;   // 화염 데미지 지속
    public bool isChilled;   // 방어력 20% 감소 및 이동 속도 감소
    public bool isShocked;   // 정확도 20% 감소


    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCoodlown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth; // 현재 체력

    public System.Action onHealthChanged; // 체력 변경 시 실행될 델리게이트
    public bool isDead { get; private set; } // 사망 여부
    public bool isInvincible { get; private set; } // 무적 상태 여부
    private bool isVulnerable; // 취약 상태 여부

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); 
        currentHealth = GetMaxHealthValue(); // 초기 체력을 최대 체력으로 설정

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
    }


    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCorutine(_duration)); // 취약 상태 코루틴 시작

    private IEnumerator VulnerableCorutine(float _duartion) // 취약 상태 코루틴
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duartion);

        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify) // 능력치 증가 메서드
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify)); // 능력치 증가 코루틴 시작
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify) // 능력치 증가 코루틴
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }
    

    public virtual void DoDamage(CharacterStats _targetStats) // 기본 물리 공격 메서드
    {
        bool criticalStrike = false;


        if (_targetStats.isInvincible) // 무적 상태면 공격 무시
            return;

        if (TargetCanAvoidAttack(_targetStats)) // 회피 성공 시 공격 무시
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform); // 넉백 방향 설정

        int totalDamage = damage.GetValue() + strength.GetValue(); // 기본 데미지 계산

        if (CanCrit()) // 치명타 계산
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStats.transform,criticalStrike); // 히트 이펙트 생성

        totalDamage = CheckTargetArmor(_targetStats, totalDamage); // 대상의 방어력 계산
        _targetStats.TakeDamage(totalDamage); // 대상에게 데미지 적용


        DoMagicalDamage(_targetStats); // remove if you don't want to apply magic hit on primary attack

    }

    #region Magical damage and ailemnts // 마법 데미지 및 상태 이상

    public virtual void DoMagicalDamage(CharacterStats _targetStats) // 마법 데미지 메서드
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();



        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue(); // 마법 데미지 계산

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage); // 대상의 마법 저항력 계산
        _targetStats.TakeDamage(totalMagicalDamage); // 대상에게 마법 데미지 적용


        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) // 상태 이상 적용 조건 검사
            return;


        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage); // 상태 이상 적용 시도

    }

    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage) // 상태 이상 적용 시도 메서드
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock) // 무작위로 상태 이상 적용
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                
                return;
            }

            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;

            }
        }

        if (canApplyIgnite) // 상태 이상 적용
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock) // 상태 이상 적용
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); // 상태 이상 적용
    }


    public void ApplyAilments(bool _ignite, bool _chill, bool _shock) // 상태 이상 적용 메서드
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite) // 상태 이상 적용 조건 검사
        {
            isIgnited = _ignite; // 상태 이상 적용
            ignitedTimer = ailmentsDuration; // 지속 시간 설정

            fx.IgniteFxFor(ailmentsDuration); // 이펙트 재생
        }

        if (_chill && canApplyChill) // 상태 이상 적용 조건 검사
        {
            chilledTimer = ailmentsDuration; // 지속 시간 설정
            isChilled = _chill; // 상태 이상 적용

            float slowPercentage = .2f; // 이동 속도 감소 비율 설정

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration); // 이동 속도 감소 적용
            fx.ChillFxFor(ailmentsDuration); // 이펙트 재생
        }

        if (_shock && canApplyShock) // 상태 이상 적용 조건 검사
        {
            if (!isShocked) 
            {
                ApplyShock(_shock); // 상태 이상 적용
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike(); // 가장 가까운 적에게 충격파 공격 적용
            }
        }

    }

    public void ApplyShock(bool _shock) // 충격 상태 적용 메서드
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock; // 상태 이상 적용

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike() // 가장 가까운 적에게 충격파 공격 적용 메서드
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25); // 범위 내 모든 콜라이더 검색

        float closestDistance = Mathf.Infinity; // 가장 가까운 적과의 거리 초기화
        Transform closestEnemy = null; // 가장 가까운 적 초기화

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1) // 적 콜라이더인지 확인 및 자기 자신 제외
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position); // 적과의 거리 계산

                if (distanceToEnemy < closestDistance) // 가장 가까운 적 갱신
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform; // 가장 가까운 적 설정
                }
            }

            if (closestEnemy == null)            // delete if you don't want shocked target to be hit by shock strike
                closestEnemy = transform;
        }


        if (closestEnemy != null) // 가장 가까운 적이 존재하면 충격파 공격 생성
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity); // 충격파 공격 생성
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>()); // 충격파 공격 설정
        }
    }
    private void ApplyIgniteDamage() // 화염 지속 데미지 적용 메서드
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage); // 화염 지속 데미지 적용

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoodlown; // 쿨다운 초기화
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage; // 화염 지속 데미지 설정 메서드
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage; // 충격파 데미지 설정 메서드

    #endregion

    public virtual void TakeDamage(int _damage) // 데미지 적용 메서드
    {

        if (isInvincible) // 무적 상태면 데미지 무시
            return;

        DecreaseHealthBy(_damage); // 데미지 감소 메서드 호출



        GetComponent<Entity>().DamageImpact(); // 피격 반응 메서드 호출
        fx.StartCoroutine("FlashFX"); // 피격 플래시 이펙트 재생

        if (currentHealth < 0 && !isDead)
            Die();


    }


    public virtual void IncreaseHealthBy(int _amount) // 체력 회복 메서드
    {
        currentHealth += _amount; // 체력 증가

        if (currentHealth > GetMaxHealthValue()) // 최대 체력 초과 방지
            currentHealth = GetMaxHealthValue();

        if(onHealthChanged != null)
            onHealthChanged(); // 체력 변경 델리게이트 호출
    }


    protected virtual void DecreaseHealthBy(int _damage) // 데미지 감소 메서드
    {

        if (isVulnerable)
            _damage = Mathf.RoundToInt( _damage * 1.1f); // 취약 상태일 때 받는 데미지 10% 증가

        currentHealth -= _damage; // 체력 감소

        if (_damage > 0)
            fx.CreatePopUpText(_damage.ToString()); // 데미지 팝업 텍스트 생성

        if (onHealthChanged != null)
            onHealthChanged(); // 체력 변경 델리게이트 호출
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity() // 즉시 사망 메서드
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible; // 무적 상태 설정 메서드


    #region Stat calculations
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) // 대상의 방어력 계산 메서드
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();


        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }


    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage) // 대상의 마법 저항력 계산 메서드
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion() 
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats) // 대상의 회피 성공 여부 계산 메서드
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    protected bool CanCrit() // 치명타 성공 여부 계산 메서드
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }


        return false;
    }

    protected int CalculateCriticalDamage(int _damage) // 치명타 데미지 계산 메서드
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() // 최대 체력 계산 메서드
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion

    public Stat GetStat(StatType _statType) // 능력치 반환 메서드
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelegence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }
}
