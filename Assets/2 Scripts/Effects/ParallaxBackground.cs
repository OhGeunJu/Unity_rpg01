using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [Header("ÆÐ·²·°½º °­µµ")]
    [SerializeField] private float parallaxEffect = 1;
    [SerializeField] private float parallaxEffectY = 1f;

    private float xPosition;
    private float yPosition;
    private float length;

    private Vector3 targetPosition;
    [SerializeField] private float lerpSpeed = 10;
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        xPosition = transform.position.x;
        yPosition = transform.position.y;
    }

    void Update()
    {
        Vector3 camPos = cam.transform.position;

        // X ÆÐ·²·°½º
        float distanceMoved = camPos.x * (1 - parallaxEffect);
        float distanceToMove = camPos.x * parallaxEffect;

        // Y ÆÐ·²·°½º
        float distanceToMoveY = camPos.y * parallaxEffectY;

        targetPosition = new Vector3(xPosition + distanceToMove, yPosition + distanceToMoveY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

        if (distanceMoved > xPosition + length)
            xPosition = xPosition + length;
        else if (distanceMoved < xPosition - length)
            xPosition = xPosition - length;

    }

}
