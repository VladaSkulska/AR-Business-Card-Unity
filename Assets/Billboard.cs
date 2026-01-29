using UnityEngine;

public class Billboard : MonoBehaviour
{
    // ОЦЕ створює галочку в Інспекторі
    public bool flip180 = true;

    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 1. Повертаємо до камери
        transform.LookAt(transform.position + arCamera.transform.rotation * Vector3.forward,
                         arCamera.transform.rotation * Vector3.up);

        // 2. Якщо галочка стоїть - розвертаємо ще на 180 градусів
        if (flip180)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}