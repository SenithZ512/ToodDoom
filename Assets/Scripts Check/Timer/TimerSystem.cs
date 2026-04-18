using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Timer System — นับถอยหลัง พร้อม Max/Min clamp
/// เวลาหมด: Restart ด่านเดิม หรือ โหลด Game Over Scene
/// </summary>
public class TimerSystem : MonoBehaviour
{
    public static TimerSystem Instance { get; private set; }

    [Header("Timer Settings")]
    public float maxTime = 60f;
    public float minTime = 0f;
    public float startTime = 30f;

    [Header("Kill Reward")]
    public float timePerKill = 3f;

    [Header("Warning")]
    public float warningThreshold = 10f;

    [Header("Game Over Settings")]
    public GameOverMode gameOverMode = GameOverMode.RestartScene;
    public int gameOverSceneIndex = 1;             // Build index ของ Scene แพ้ (ดูจาก Build Settings)
    public float gameOverDelay = 1.5f;             // หน่วงเวลาก่อน load (วินาที)

    public enum GameOverMode
    {
        RestartScene,       // Restart ด่านเดิม
        LoadGameOverScene,  // โหลด Scene แพ้
        Both                // Restart ก่อน ถ้าหมด retry แล้วค่อยไป Game Over
    }

    [Header("Retry Settings (ใช้เมื่อ mode = Both)")]
    public int maxRetries = 2;           // จำนวนครั้งที่ restart ได้ก่อนไป Game Over
    private int _retryCount = 0;

    private float _currentTime;
    private bool _isRunning;
    private bool _warningFired;
    private bool _endFired;

    public float CurrentTime => _currentTime;
    public float NormalizedTime => Mathf.InverseLerp(minTime, maxTime, _currentTime);
    public bool IsRunning => _isRunning;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ข้ามScene ได้
    }

    void Start()
    {
        _currentTime = Mathf.Clamp(startTime, minTime, maxTime);
        _isRunning = true;
    }

    void Update()
    {
        if (!_isRunning) return;

        _currentTime -= Time.deltaTime;
        _currentTime = Mathf.Clamp(_currentTime, minTime, maxTime);

        if (!_warningFired && _currentTime <= warningThreshold)
        {
            _warningFired = true;
            Debug.Log($"[TimerSystem] Warning! เหลือ {_currentTime:F1}s");
        }

        if (!_endFired && _currentTime <= minTime)
        {
            _endFired = true;
            _isRunning = false;
            OnTimeUp();
        }
    }

    void OnTimeUp()
    {
        Debug.Log("[TimerSystem] หมดเวลา!");

        switch (gameOverMode)
        {
            case GameOverMode.RestartScene:
                Invoke(nameof(RestartScene), gameOverDelay);
                break;

            case GameOverMode.LoadGameOverScene:
                Invoke(nameof(LoadGameOverScene), gameOverDelay);
                break;

            case GameOverMode.Both:
                if (_retryCount < maxRetries)
                {
                    _retryCount++;
                    Debug.Log($"[TimerSystem] Retry {_retryCount}/{maxRetries}");
                    Invoke(nameof(RestartScene), gameOverDelay);
                }
                else
                {
                    Debug.Log("[TimerSystem] หมด retry → Game Over");
                    Invoke(nameof(LoadGameOverScene), gameOverDelay);
                }
                break;
        }
    }

    void RestartScene()
    {
        // Reset ก่อน restart
        _retryCount = gameOverMode == GameOverMode.Both ? _retryCount : 0;
        _endFired = false;
        _warningFired = false;
        _currentTime = Mathf.Clamp(startTime, minTime, maxTime);
        _isRunning = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneIndex);
    }

    // ── Public API ───────────────────────────────────────────────

    public void AddTime(float seconds)
    {
        _currentTime = Mathf.Clamp(_currentTime + seconds, minTime, maxTime);
        if (_currentTime > warningThreshold)
            _warningFired = false;
    }

    public void OnEnemyKilled()
    {
        AddTime(timePerKill);
        Debug.Log($"[TimerSystem] +{timePerKill}s → {_currentTime:F1}s");
    }

    public void OnPowerupCollected(float bonusTime)
    {
        AddTime(bonusTime);
        Debug.Log($"[TimerSystem] Powerup +{bonusTime}s → {_currentTime:F1}s");
    }

    public void ResetRetries() => _retryCount = 0;
    public void PauseTimer() => _isRunning = false;
    public void ResumeTimer() => _isRunning = true;
}