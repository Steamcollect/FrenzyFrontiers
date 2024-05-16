using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawnerGestionary : MonoBehaviour
{
    private bool modDebug = false;

    private int currentWave;
    private int currentNbMonsterSpawn;

    [Header("References")]
    [SerializeField] private SCO_EnnemyWave waveSettings;
    [SerializeField] private AudioClip[] waveAnnouncementClips;
    [SerializeField] private GameObject particleLocation;
    [SerializeField] private AudioClip winWaveSound;

    [Header("Settings Particles")]
    [SerializeField] private Vector3 minSizeCloud;
    [SerializeField] private Vector3 maxSizeCloud;
    [SerializeField] private int nbEnemiesLerp;

    private int nbSpawnPoint;
    private int nbMonsterSpawn;
    private float radiusCircle;

    private GameObject goLocationGroup;
    private GameObject gameObjectGroup;

    private List<EnnemyGroup> ennemyList = new List<EnnemyGroup>();
    private Queue<Tuple<int, int, GameObject>> queueInstantiate = new Queue<Tuple<int, int, GameObject>>();

    private GameStateManager gameStateManager;
    private HexagonalGrid grid;


    #region DebugEditor
    
    public void ResetExemple()
    {
        if (gameObjectGroup) 
        { 
            if (Application.isPlaying)
                Destroy(gameObjectGroup);
            else
                DestroyImmediate(gameObjectGroup);
            gameObjectGroup = null;
        }
        ennemyList.Clear();
        queueInstantiate.Clear();
        StopAllCoroutines();
    }
    #endregion

    public int GetWave() => currentWave;

    private void OnDeathEnemy()
    {

        currentNbMonsterSpawn -= 1;

        if (currentNbMonsterSpawn > 1) return;

        if( currentNbMonsterSpawn == 1)
        {
            foreach (var group in ennemyList)
            {
                foreach (var enemy in group.ennemyGroup) if (enemy != null) return; 
            }
        }

        StartCoroutine(Tool.Delay(() =>
        {
            AudioManager.instance.PlayClipAt(winWaveSound, 0, Vector2.zero);

            if (TutorialManager.instance.launchTutorial) StartCoroutine(TutorialManager.instance.OnNightEnd());
            else gameStateManager.ChangePhaseToBuild();
        }
        , 0.5f));
        
    }

    private void Start()
    {
        transform.position = Vector3.zero; //For the case of game manager not in center world

        if (waveSettings == null) throw new ArgumentNullException("Data wave not assign");
        if (!modDebug) currentWave = 0;
        else
        {
            ResetExemple();
        }

        goLocationGroup = new GameObject("LocationParticleGroup");
        goLocationGroup.transform.parent = transform;

        gameStateManager = FindFirstObjectByType<GameStateManager>();
        grid = FindFirstObjectByType<HexagonalGrid>();
    }

    private void OnEnable()
    {
        GameStateManager.OnFight += ShowNextWave;

        //I don't know why, don't touch it!!!!!!!!!!!!!!!!!! (it work...)
        GameStateManager.OnLoose -= GameStateManagerOnPausedLoose;
        GameStateManager.OnPaused -= GameStateManagerOnPausedLoose;
        GameStateManager.OnGameplay -= GameStateManagerOnGameplay; 
    }

    private void OnDisable()
    {
        GameStateManager.OnFight -= ShowNextWave;
        GameStateManager.OnLoose -= GameStateManagerOnPausedLoose;
        GameStateManager.OnPaused -= GameStateManagerOnPausedLoose;
        GameStateManager.OnGameplay -= GameStateManagerOnGameplay;
        StopAllCoroutines();
    }

    private void GameStateManagerOnPausedLoose()
    {
        PauseRefreshComponent(false);
    }
    
    private void GameStateManagerOnGameplay()
    {
        PauseRefreshComponent(true);
    }

    private void PauseRefreshComponent(bool isPaused)
    {
        if (isPaused) this.enabled = false;
    }

    private void ShowNextWave()
    {
        float time = 0f;

        AudioManager.instance.PlayClipAt(waveAnnouncementClips.ToList().GetRandom(), 0, Vector3.zero);

        ScoreManager.instance?.NewWave();
        foreach (var group in ennemyList)
        {
            foreach (var enemy in group.ennemyGroup) 
            {
                time = UnityEngine.Random.Range(0.05f, 0.15f);
                StartCoroutine(Tool.Delay(() => enemy?.SetActive(true), time));
            }
        }
        StartCoroutine(Tool.Delay(()=> { Destroy(goLocationGroup); goLocationGroup = new GameObject("LocationParticleGroup"); }, 3f));
    }

    public void PrepareNextWave()
    {
        ResetExemple();
        currentWave++;
        Tool.ResetWavePower();
        InitWave();
        
    }


    private void InitWave()
    {
        if (gameObjectGroup == null) gameObjectGroup = new GameObject("---Ennemy Group---");
        SetSpawnPointEnnemy();
        SetGroupEnemySize();
        FillGroupEnemy();
        StartCoroutine(InstantiateEnemies());
        CreateParticleWave();
    }

    private void SetSpawnPointEnnemy()
    {
        int maxIndex = GetNbSpawnPoint();
        int index = 0;
        radiusCircle = modDebug ? radiusCircle : grid.farthestHexagoneDistance + grid.horizontalDistance*3.85f;

        float currentAngle = UnityEngine.Random.Range(0, 360);
        float incrementAngle = 2 * Mathf.PI / (maxIndex + UnityEngine.Random.Range(0, 4));

        while (index < maxIndex)
        {
            ennemyList.Add(new EnnemyGroup { centerSpawnPoint  = PointOnCircleCirconference(transform.position, radiusCircle, currentAngle) });
            float factorRd = UnityEngine.Random.Range(0.8f, 1.35f);
            currentAngle = (currentAngle + incrementAngle * factorRd >= 360) ? 360 - (currentAngle + incrementAngle * factorRd) : currentAngle + incrementAngle * factorRd;
            index++;
        }
    }

    private void SetGroupEnemySize()
    {
        nbMonsterSpawn = GetNbMonsterSpawn();

        var currentMonsterWave = 0;
        float rdValue = 0;
        for (var i = 0; i < ennemyList.Count; i++)
        {
            //Random value to add a nb random in each group
            rdValue = Mathf.Ceil(UnityEngine.Random.Range(nbMonsterSpawn / ennemyList.Count * -0.8f, nbMonsterSpawn / ennemyList.Count * 0.8f));

            if (i < ennemyList.Count - 1)
            {
                int nb = ((nbMonsterSpawn / ennemyList.Count + rdValue + currentMonsterWave) > nbMonsterSpawn)
                    ? Mathf.Clamp(nbMonsterSpawn - currentMonsterWave, 0, nbMonsterSpawn)
                    : Mathf.Clamp((int)(nbMonsterSpawn / ennemyList.Count + rdValue),0, nbMonsterSpawn);
                
                ennemyList[i].ennemyGroup = new GameObject[nb];
                currentMonsterWave += ennemyList[i].ennemyGroup.Length;
            }
            else
            {
                ennemyList[i].ennemyGroup = new GameObject[(int)Mathf.Clamp(nbMonsterSpawn - currentMonsterWave,0.0f,99999999999.0f)];
                currentMonsterWave += ennemyList[i].ennemyGroup.Length;
            }
        }

        for (int i = ennemyList.Count() -1; i >=0; i--) if (ennemyList[i].ennemyGroup.Length == 0) { ennemyList.RemoveAt(i); }
        
    }

    private void FillGroupEnemy()
    {
        EnemyWave[] enemiesWave = waveSettings.waveData.FindAll(o => currentWave >= o.waveMin).ToArray();
        EnemyWave enemy =  null;
        Tuple<int, int, GameObject> tuple;

        for(int i = 0; i < ennemyList.Count; ++i)
        {
            for (int j = 0; j < ennemyList[i].ennemyGroup.Length; ++j)
            {
                enemy = enemiesWave.GetRandom();
                if (enemy == null) { Debug.LogWarning("Error"!); continue; }
                Tool.PickPowerWave(enemy);
                tuple = Tuple.Create(i, j, enemy.prefab);
                queueInstantiate.Enqueue(tuple);
            }
        }

        currentNbMonsterSpawn = queueInstantiate.Count;
    }

    private IEnumerator InstantiateEnemies()
    {
        while(queueInstantiate.Count > 0)
        {
            var obj = queueInstantiate.Dequeue();
            ennemyList[obj.Item1].ennemyGroup[obj.Item2] = Instantiate(obj.Item3);
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].GetComponent<LifeEnnemyComponent>().onDeath += OnDeathEnemy;
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].transform.parent = gameObjectGroup.transform;
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].transform.position = ennemyList[obj.Item1].centerSpawnPoint;
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].SetActive(false);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CreateParticleWave()
    {
        GameObject particleInst;
        int nbEnemyGroup;

        for (int i = 0; i < ennemyList.Count; i++)
        {
            
            nbEnemyGroup = ennemyList[i].ennemyGroup.Count();
            if (nbEnemyGroup >= 1)
            {
                particleInst = Instantiate(particleLocation);
                particleInst.transform.localPosition = ennemyList[i].centerSpawnPoint;
                particleInst.transform.parent = goLocationGroup.transform;
                particleInst.transform.localScale = Vector3.Lerp(minSizeCloud, maxSizeCloud, Mathf.Clamp01((float)nbEnemyGroup / nbEnemiesLerp));
            }
        }
    }

    public void DestroyAllEnemy()
    {
        Destroy(gameObjectGroup);
    }

    private int GetNbSpawnPoint()
    {
        //Linear interpolation to have 3 point at 0 and 12 point at 8 or more, with random + 0/1
         if (!modDebug) return Mathf.Clamp(1 + (currentWave / 3) + UnityEngine.Random.Range(-1,2),1,6);
         return nbSpawnPoint;
    }

    private int GetNbMonsterSpawn()
    {
        //add formule to increase monsters
        if (modDebug) return nbMonsterSpawn;
        else return (int)((currentWave*1.5f + 2) + UnityEngine.Random.Range(0,2));
    }

    private static Vector3 PointOnCircleCirconference(Vector3 origin, float radius, float angle)
    {
        return new Vector3(origin.x + (float)(radius * Mathf.Cos(angle)), 0, origin.z + (float)(radius * Mathf.Sin(angle)));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radiusCircle);
        foreach (EnnemyGroup group in ennemyList)
        {
            if (group.ennemyGroup.Count() > 0)
            {
                //draw point center group
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.DrawWireCube(group.centerSpawnPoint, Vector3.one);
            }
        }
    }
#endif
}

[System.Serializable]
public class EnnemyGroup
{
    public GameObject[] ennemyGroup;
    public Vector3 centerSpawnPoint;
    public float radiusSpawnPoint;
}