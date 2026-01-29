using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class URLButton : MonoBehaviour
{
    [Header(" уди веде кнопка")]
    public string url = "https://www.google.com";

    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        // 1. √ќЋќ¬Ќ≈ ¬»ѕ–ј¬Ћ≈ЌЌя: якщо немаЇ дотик≥в - виходимо
        if (Touch.activeTouches.Count == 0) return;

        // –еагуЇмо т≥льки на початок дотику
        if (Touch.activeTouches.Count == 1 &&
            Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Touch.activeTouches[0].screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // якщо влучили в цю кнопку
                if (hit.transform == transform)
                {
                    Debug.Log("¬≥дкриваю URL: " + url);
                    Application.OpenURL(url);

                    // ¬≥брац≥€
                    if (SystemInfo.supportsVibration) Handheld.Vibrate();
                }
            }
        }
    }
}