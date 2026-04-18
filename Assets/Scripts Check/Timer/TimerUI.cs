using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Timer UI — หลอดแนวนอน Flat style
/// Setup:
///   - Background Image (หลอดเปล่า — สีเข้ม)
///   - Fill Image      (ของเหลวข้างใน — fillAmount)
///   - Shine Image     (แถบขาวบนสุด — optional highlight)
///   - TextMeshPro     (ตัวเลข)
/// </summary>
public class TimerUI : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundBar;     // หลอดด้านหลัง (สีเข้ม)
    public Image fillBar;           // ของเหลวข้างใน (Filled Horizontal)
    public Image shineBar;          // แถบ highlight บนสุด (ขาวโปร่งใส)
    public TextMeshProUGUI timerText;

    [Header("Colors")]
    public Color colorFull = new Color(0.2f, 0.85f, 0.4f);   // เขียว
    public Color colorWarn = new Color(1f, 0.75f, 0f);      // เหลือง
    public Color colorEmpty = new Color(0.9f, 0.2f, 0.2f);   // แดง
    public Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);

    [Header("Thresholds")]
    public float warnThreshold = 0.5f;   // % เริ่มเหลือง
    public float emptyThreshold = 0.25f;  // % เริ่มแดง

    [Header("Pulse (ตอนใกล้หมด)")]
    public bool enablePulse = true;
    public float pulseSpeed = 4f;

    private TimerSystem _timer;

    void Start()
    {
        _timer = TimerSystem.Instance;
        if (_timer == null)
            Debug.LogWarning("[TimerUI] ไม่พบ TimerSystem ใน Scene!");

        if (backgroundBar != null)
            backgroundBar.color = bgColor;

        if (shineBar != null)
        {
            // แถบขาวบนสุด ความสูง ~20% ของหลอด โปร่งใสนิดหน่อย
            var c = Color.white;
            c.a = 0.15f;
            shineBar.color = c;
        }
    }

    void Update()
    {
        if (_timer == null) return;

        float t = _timer.NormalizedTime; // 0-1

        UpdateFill(t);
        UpdateText(t);
    }

    void UpdateFill(float t)
    {
        if (fillBar == null) return;

        fillBar.fillAmount = t;

        // Color: เขียว → เหลือง → แดง
        Color target;
        if (t > warnThreshold)
            target = Color.Lerp(colorWarn, colorFull,
                        (t - warnThreshold) / (1f - warnThreshold));
        else if (t > emptyThreshold)
            target = Color.Lerp(colorEmpty, colorWarn,
                        (t - emptyThreshold) / (warnThreshold - emptyThreshold));
        else
            target = colorEmpty;

        // Pulse alpha ตอนใกล้หมด
        if (enablePulse && t <= emptyThreshold)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            target.a = Mathf.Lerp(0.45f, 1f, pulse);
        }
        else
        {
            target.a = 1f;
        }

        fillBar.color = target;

        // shine ตามความยาว fill (ขยับให้อยู่บนของเหลวเสมอ)
        if (shineBar != null)
            shineBar.fillAmount = t;
    }

    void UpdateText(float t)
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(_timer.CurrentTime);
        timerText.text = seconds.ToString();
        timerText.color = t <= emptyThreshold ? colorEmpty
                        : t <= warnThreshold ? colorWarn
                                              : colorFull;
    }
}