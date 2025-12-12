using UnityEngine;
using TMPro; // Required for TextMeshPro

public class DamageText : MonoBehaviour
{
    public float lifeTime = 1.0f;
    public float floatSpeed = 2.0f;
    public Vector3 randomIntensity = new Vector3(0.5f, 0, 0);

    private TextMeshProUGUI _textMesh;

    void Start()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();

        // Add random horizontal jitter so they don't stack perfectly
        transform.localPosition += new Vector3(
            Random.Range(-randomIntensity.x, randomIntensity.x),
            Random.Range(-randomIntensity.y, randomIntensity.y),
            0);

        // Auto-Destroy after X seconds
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 1. Float Up
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // 2. Fade Out (Optional)
        if (_textMesh != null)
        {
            Color color = _textMesh.color;
            // Calculate alpha based on remaining lifetime
            // (Simply reducing alpha over time)
            color.a -= Time.deltaTime / lifeTime;
            _textMesh.color = color;
        }
    }

    public void SetText(int damageAmount)
    {
        if (_textMesh == null) _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _textMesh.text = damageAmount.ToString();
    }
}