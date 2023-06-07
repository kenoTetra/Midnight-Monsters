using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveMinigame : MonoBehaviour, IDamagable
{
    [Header("Minigame Stuff")]
    [SerializeField] private bool startMinigame;
    [SerializeField] private Vector3 center;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float timeBetweenWaves = 10f;
    [Space(5)]

    // Wave timer + cur wave
    float waveTimer = 0f;
    [SerializeField] private int curWave = 1;

    [Header("Wave Setup")]
    [SerializeField] private List<GameObject> wave1 = new List<GameObject>();
    [SerializeField] private List<GameObject> wave2 = new List<GameObject>();
    [SerializeField] private List<GameObject> wave3 = new List<GameObject>();
    [Space(5)]

    [Header("Visuals")]
    [SerializeField] private TMP_Text visualTimer;
    MeshRenderer meshRenderer;
    BoxCollider boxCollider;

    // Start shooting thing
    float health = 1f;

    // Start is called before the first frame update
    void Start()
    {
        waveTimer = timeBetweenWaves;
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(startMinigame)
            SpawnWave();

        else
        {
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
            curWave = 1;
            waveTimer = timeBetweenWaves;
            visualTimer.text = "Shoot to start wave shooter.";
        }
    }

    public void Damage(float damage, bool crit, float critHitMult, string gunName)
    {
        health -= (int)damage;
        
        if(health <= 0)
        {
            startMinigame = true;
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
        }

        // Hit numbers
        IDamagable.spawnHitNumber(damage, crit, critHitMult, transform.position);
    }

    void SpawnWave()
    {
        if(waveTimer < 0)
        {
            switch(curWave)
            {
                case 1:
                    curWave++;
                    foreach (GameObject enemy in wave1)
                        SpawnEnemy(enemy);
                    waveTimer = timeBetweenWaves;
                    break;
                    
                case 2:
                    curWave++;
                    foreach (GameObject enemy in wave2)
                        SpawnEnemy(enemy);
                    waveTimer = timeBetweenWaves;
                    break;
                    
                case 3:
                    curWave++;
                    foreach (GameObject enemy in wave3)
                        SpawnEnemy(enemy);
                    waveTimer = timeBetweenWaves;
                    break;

                default:
                    Debug.Log("No more waves!");
                    startMinigame = false;
                    waveTimer = timeBetweenWaves;
                    break;
            }
        }

        else
        {
            waveTimer -= Time.deltaTime;
            visualTimer.text = "Next wave in: " + waveTimer.ToString("0.00");
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 spawnPosition = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit;
        var foundPos = NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas);
        
        Instantiate(enemy, hit.position, Quaternion.identity);
    }
}
