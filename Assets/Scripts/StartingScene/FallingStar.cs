using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FallingStar : MonoBehaviour
{
    [Header("Fall Parameters")]
    public float angle = 45f;              // Derece yön
    public float fallSpeed = 5f;           // Hareket hýzý
    public float fallDuration = 2f;        // Düþüþ süresi 

    private Vector3 direction;
    private float elapsedTime = 0f;
    private Vector3 initialScale;

    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    void Start()
    {
        float rad = angle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(rad), -Mathf.Sin(rad), 0).normalized;

        initialScale = transform.localScale;

        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < fallDuration)
        {
            transform.position += direction * fallSpeed * Time.deltaTime;

            // Ölçek küçültme
            float t = elapsedTime / fallDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            // Þeffaflýk azaltma
            Color newColor = initialColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = newColor;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
