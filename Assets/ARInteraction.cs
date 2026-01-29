using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARInteraction : MonoBehaviour
{
    [Header("Øâèäê³ñòü")]
    public float rotationSpeed = 0.3f;

    // ß òðîõè çìåíøèâ öå çíà÷åííÿ, áî íîâà ôîðìóëà ïîòóæí³øà
    public float scaleSpeed = 0.001f;

    public float moveSpeed = 0.0003f;

    [Header("Ë³ì³òè Çá³ëüøåííÿ (ó Ðàçàõ)")]
    public float minScaleMultiplier = 0.5f;
    public float maxScaleMultiplier = 1.5f;

    private Vector3 defaultPos;
    private Quaternion defaultRot;
    private Vector3 defaultScale;

    private float initialDistance;
    private bool isSelected = false;
    private Camera arCamera;

    void Awake()
    {
        defaultPos = transform.localPosition;
        defaultRot = transform.localRotation;
        defaultScale = transform.localScale;
        arCamera = Camera.main;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        isSelected = false;
    }

    public void ResetModel()
    {
        transform.localPosition = defaultPos;
        transform.localRotation = defaultRot;
        transform.localScale = defaultScale;
        isSelected = false;
    }

    void Update()
    {
        if (Touch.activeTouches.Count == 0)
        {
            isSelected = false;
            return;
        }

        // --- ÂÈÁ²Ð ÌÎÄÅË² ---
        if (Touch.activeTouches.Count == 1 &&
            Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Touch.activeTouches[0].screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform) isSelected = true;
            }
        }

        if (!isSelected && Touch.activeTouches[0].phase != UnityEngine.InputSystem.TouchPhase.Began) return;


        // --- 1 ÏÀËÅÖÜ: ÎÁÅÐÒÀÍÍß ---
        if (Touch.activeTouches.Count == 1)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                transform.Rotate(Vector3.up, -touch.delta.x * rotationSpeed, Space.World);
                transform.Rotate(arCamera.transform.right, touch.delta.y * rotationSpeed, Space.World);
            }
        }

        // --- 2 ÏÀËÜÖ²: ÇÓÌ + ÏÅÐÅÌ²ÙÅÍÍß ---
        else if (Touch.activeTouches.Count == 2)
        {
            Touch touch1 = Touch.activeTouches[0];
            Touch touch2 = Touch.activeTouches[1];

            if (touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                touch2.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                // 1. ÇÓÌ (ÂÈÏÐÀÂËÅÍÎ ÄËß ÌÀËÅÍÜÊÈÕ ÎÁ'ªÊÒ²Â)
                float currentDist = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);
                float prevDist = Vector2.Distance(touch1.screenPosition - touch1.delta, touch2.screenPosition - touch2.delta);
                float touchDelta = currentDist - prevDist;

                // ÇÌ²ÍÀ: Ìíîæèìî íà transform.localScale.
                // ßêùî îá'ºêò ìàëèé (0.1), ïðèð³ñò áóäå ìàëèì. ßêùî âåëèêèé (1.0) - âåëèêèì.
                Vector3 scaleChange = transform.localScale * (touchDelta * scaleSpeed);

                Vector3 targetScale = transform.localScale + scaleChange;

                // Ë³ì³òè
                float minLimit = defaultScale.x * minScaleMultiplier;
                float maxLimit = defaultScale.x * maxScaleMultiplier;

                targetScale.x = Mathf.Clamp(targetScale.x, minLimit, maxLimit);
                targetScale.y = Mathf.Clamp(targetScale.y, minLimit, maxLimit);
                targetScale.z = Mathf.Clamp(targetScale.z, minLimit, maxLimit);

                transform.localScale = targetScale;


                // 2. ÏÅÐÅÌ²ÙÅÍÍß
                Vector2 avgDelta = (touch1.delta + touch2.delta) / 2;
                Vector3 move = new Vector3(avgDelta.x, avgDelta.y, 0) * moveSpeed;

                Vector3 proposedPosition = transform.position + (arCamera.transform.right * move.x + arCamera.transform.up * move.y);
                Vector3 viewportPoint = arCamera.WorldToViewportPoint(proposedPosition);

                if (viewportPoint.x > 0.1f && viewportPoint.x < 0.9f &&
                    viewportPoint.y > 0.1f && viewportPoint.y < 0.9f &&
                    viewportPoint.z > 0)
                {
                    transform.position = proposedPosition;
                }
            }
        }
    }
}