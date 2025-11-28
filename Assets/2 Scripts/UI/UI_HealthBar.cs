using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>(); // Entity 컴포넌트를 가져오는 속성
    private CharacterStats myStats => GetComponentInParent<CharacterStats>(); // 체력 정보를 가져오는 속성
    private RectTransform myTransform => GetComponent<RectTransform>(); // UI의 RectTransform 컴포넌트를 가져오는 속성
    private Slider slider;


    private void Start()
    {
        //myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        UpdateHealthUI(); // 초기 체력 UI 업데이트
    }

    private void UpdateHealthUI() // 체력 UI 업데이트
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void OnEnable() // 이벤트 구독
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }

    private void OnDisable() // 이벤트 구독 해제
    {
        if(entity !=null)
            entity.onFlipped -= FlipUI;

        if(myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }
    private void FlipUI() => myTransform.Rotate(0, 180, 0); // UI 뒤집기

    public void SetVisible(bool value)
    {
        gameObject.SetActive(value);
    }
}
