using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [System.Serializable]
    public struct TrackablePrefab
    {
        public string name;
        public GameObject prefab;
    }

    [SerializeField]
    private List<TrackablePrefab> trackablePrefabs;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();
    private string activeMarkerName = null;

    // Налаштування "Мертвої зони"
    // Якщо зміщення менше 2 міліметрів - ігноруємо його (прибирає тремтіння)
    private float positionThreshold = 0.002f;
    // Якщо поворот менше 1 градуса - ігноруємо
    private float rotationThreshold = 1.0f;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.removed)
        {
            string name = trackedImage.referenceImage.name;
            if (instantiatedObjects.ContainsKey(name))
            {
                instantiatedObjects[name].SetActive(false);
                if (activeMarkerName == name) activeMarkerName = null;
            }
        }

        List<ARTrackedImage> allImages = new List<ARTrackedImage>();
        allImages.AddRange(eventArgs.added);
        allImages.AddRange(eventArgs.updated);

        foreach (var trackedImage in allImages)
        {
            UpdateObject(trackedImage);
        }
    }

    private void UpdateObject(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

        // Створення
        if (!instantiatedObjects.ContainsKey(name))
        {
            foreach (var trackable in trackablePrefabs)
            {
                if (trackable.name == name)
                {
                    var newObj = Instantiate(trackable.prefab, trackedImage.transform.position, trackedImage.transform.rotation);
                    newObj.SetActive(false);
                    instantiatedObjects[name] = newObj;
                    break;
                }
            }
        }

        if (instantiatedObjects.TryGetValue(name, out GameObject instance))
        {
            if (isTracking)
            {
                // ВИПАДОК 1: Новий маркер (тільки з'явився)
                if (activeMarkerName != name)
                {
                    if (activeMarkerName != null && instantiatedObjects.ContainsKey(activeMarkerName))
                    {
                        instantiatedObjects[activeMarkerName].SetActive(false);
                    }

                    activeMarkerName = name;
                    instance.SetActive(true);

                    // Ставимо миттєво
                    instance.transform.position = trackedImage.transform.position;
                    instance.transform.rotation = trackedImage.transform.rotation;

                    var interaction = instance.GetComponentInChildren<ARInteraction>();
                    if (interaction != null) interaction.ResetModel();
                }
                // ВИПАДОК 2: Маркер вже на екрані
                else
                {
                    // Рахуємо різницю між тим де модель ЗАРАЗ і де хоче бути МАРКЕР
                    float dist = Vector3.Distance(instance.transform.position, trackedImage.transform.position);
                    float angle = Quaternion.Angle(instance.transform.rotation, trackedImage.transform.rotation);

                    // РУХАЄМО ТІЛЬКИ ЯКЩО ЗМІНИ СУТТЄВІ
                    // Це вбиває дрібне тремтіння, бо ми просто не оновлюємо позицію для мікро-зсувів
                    if (dist > positionThreshold || angle > rotationThreshold)
                    {
                        instance.transform.position = trackedImage.transform.position;
                        instance.transform.rotation = trackedImage.transform.rotation;
                    }
                }

                if (!instance.activeSelf) instance.SetActive(true);
            }
            else
            {
                instance.SetActive(false);
                if (activeMarkerName == name) activeMarkerName = null;
            }
        }
    }
}