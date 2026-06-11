using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TherapyManager : MonoBehaviour
{
    [Header("Références UI")]
    public Slider levelSlider;
    public Slider distanceSlider;
    public Button startButton;
    public TextMeshProUGUI levelLabel;
    public TextMeshProUGUI distanceLabel;

    [Header("Paramètres de Thérapie")]
    public GameObject spiderPrefab;
    public Transform player;
    public Transform[] spawnPoints;

    [Header("Paramètres de Difficulté")]
    public float baseMoveSpeed = 1f;

    // Variables d'état
    private int currentLevel = 1;
    private float minDistance = 2f;
    private bool exposureActive = false;
    private List<GameObject> activeSpiders = new List<GameObject>();

    void Start()
    {
        levelSlider.onValueChanged.AddListener(SetLevel);
        distanceSlider.onValueChanged.AddListener(SetDistance);
        startButton.onClick.AddListener(() => { StartCoroutine(ToggleExposure_Coroutine()); });
        
        SetLevel(levelSlider.value);
        SetDistance(distanceSlider.value);
    }

    void Update()
    {
        HandleKeyboardInputs();
    }

    public void SetLevel(float level)
    {
        currentLevel = (int)level;
        if (levelLabel != null) levelLabel.text = $"Niveau : {currentLevel}";
    }

    public void SetDistance(float distance)
    {
        minDistance = distance;
        if (distanceLabel != null) distanceLabel.text = $"Distance Min : {minDistance:F1}m";
        
        foreach (var spider in activeSpiders)
        {
            if (spider != null)
            {
                spider.GetComponent<SpiderBehavior>().UpdateStopDistance(minDistance);
            }
        }
    }

    private IEnumerator ToggleExposure_Coroutine()
    {
        yield return null; 

        exposureActive = !exposureActive;
        if (exposureActive)
        {
            StartExposure();
            if (startButton != null) startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Arrêter l'Exposition";
        }
        else
        {
            StopExposure();
            if (startButton != null) startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Démarrer l'Exposition";
        }
    }

    private void StartExposure()
    {
        StopExposure();
        Debug.Log($"Démarrage - Niveau {currentLevel}");

        if (spawnPoints.Length == 0) { Debug.LogError("ERREUR : Aucun point de spawn n'est assigné !"); return; }

        for (int i = 0; i < currentLevel; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spiderInstance = Instantiate(spiderPrefab, spawnPoint.position, spawnPoint.rotation);
            
            SpiderBehavior behavior = spiderInstance.GetComponent<SpiderBehavior>();
            if (behavior != null)
            {
                float spiderSpeed = baseMoveSpeed + (currentLevel * 0.1f);
                // On donne à l'araignée sa mission : la cible, sa vitesse, la distance, et sa "place" (i) sur le nombre total (currentLevel)
                behavior.Initialize(player, spiderSpeed, minDistance, i, currentLevel);
            }

            activeSpiders.Add(spiderInstance);
        }
    }

    private void StopExposure()
    {
        foreach (var spider in activeSpiders)
        {
            if (spider != null) Destroy(spider);
        }
        activeSpiders.Clear();
        Debug.Log("Exposition arrêtée.");
    }

    private void HandleKeyboardInputs()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(ToggleExposure_Coroutine());
        }
    }
}
