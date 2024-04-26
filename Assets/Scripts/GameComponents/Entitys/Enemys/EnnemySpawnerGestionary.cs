using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnnemySpawnerGestionary : MonoBehaviour
{
    [SerializeField] private bool modDebug = false;

    [SerializeField] private int currentWave;
    [SerializeField] private SCO_EnnemyWave waveSettings;

    [SerializeField] private int nbSpawnPoint;
    [SerializeField] private int nbMonsterSpawn;
    [SerializeField] private float radiusCircle;

    [SerializeField] private List<EnnemyGroup> ennemyList = new List<EnnemyGroup>();
    
    private GameObject gameObjectGroup;

    private GameStateManager gameStateManager;
    private HexagonalGrid grid;

    private int currentNbMonsterSpawn;

    private Queue<Tuple<int, int, GameObject>> queueInstantiate = new Queue<Tuple<int, int, GameObject>>();

    #region DebugEditor
    public void ShowExemple()
    {
        ResetExemple();
        InitWave();
    }
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

    private void OnEnnemyDeath()
    {
        currentNbMonsterSpawn -= 1;
        if (currentNbMonsterSpawn <= 0) gameStateManager.ChangePhaseToBuild();
    }

    private void Start()
    {
        if (waveSettings == null) throw new ArgumentNullException("Data wave not assign");
        if (!modDebug) currentWave = 1;
        else
        {
            ResetExemple();
        }
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
        ScoreManager.instance?.NewWave();
        foreach (var group in ennemyList)
        {
            foreach(var enemy in group.ennemyGroup) enemy?.SetActive(true);
        }
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
    }

    private void SetSpawnPointEnnemy()
    {
        int maxIndex = GetNbSpawnPoint();
        int index = 0;
        radiusCircle = (modDebug) ? radiusCircle : grid.farthestHexagoneDistance + grid.horizontalDistance * 4.5f;

        float currentAngle = UnityEngine.Random.Range(0, 360);
        float incrementAngle = 2 * Mathf.PI / (maxIndex + UnityEngine.Random.Range(0, 4));

        while (index < maxIndex)
        {
            ennemyList.Add(new EnnemyGroup { area = new AreaData { center = PointOnCircleCirconference(transform.position, radiusCircle, currentAngle) } });
            float factorRd = UnityEngine.Random.Range(0.8f, 1.35f);
            currentAngle = (currentAngle + incrementAngle * factorRd >= 360) ? 360 - (currentAngle + incrementAngle * factorRd) : currentAngle + incrementAngle * factorRd;
            index++;
        }
    }

    private void SetGroupEnemySize()
    {
        nbMonsterSpawn = GetNbMonsterSpawn();
        //Debug.Log(nbMonsterSpawn);
        //Debug.Log(ennemyList.Count);

        var currentMonsterWave = 0;
        float rdValue = 0;
        for (var i = 0; i < ennemyList.Count; i++)
        {
            Vector3 targetForward = new Vector3(transform.position.x - ennemyList[i].area.center.x, 0,transform.position.z - ennemyList[i].area.center.z).normalized;

            //Random value to add a nb random in each group
            rdValue = Mathf.Ceil(UnityEngine.Random.Range(nbMonsterSpawn / ennemyList.Count * -0.8f, nbMonsterSpawn / ennemyList.Count * 0.8f));

            if (i < ennemyList.Count - 1)
            {
                int nb = ((nbMonsterSpawn / ennemyList.Count + rdValue + currentMonsterWave) > nbMonsterSpawn)
                    ? Mathf.Clamp(nbMonsterSpawn - currentMonsterWave, 0, nbMonsterSpawn)
                    : Mathf.Clamp((int)(nbMonsterSpawn / ennemyList.Count + rdValue),0, nbMonsterSpawn);
                //Debug.Log(nb);
                ennemyList[i].ennemyGroup = new GameObject[nb];
                currentMonsterWave += ennemyList[i].ennemyGroup.Length;
            }
            else
            {
                ennemyList[i].ennemyGroup = new GameObject[nbMonsterSpawn - currentMonsterWave];
                currentMonsterWave += ennemyList[i].ennemyGroup.Length;
            }

            ennemyList[i].area.AlignCenterInCenterArea(ref targetForward, ennemyList[i].ennemyGroup.Length);
            ennemyList[i].area.GenerateArea(ref targetForward, ennemyList[i].ennemyGroup.Length/2, ennemyList[i].ennemyGroup.Length/2 , ennemyList[i].ennemyGroup.Length/2);
        }
        currentNbMonsterSpawn = ennemyList.Sum(o => o.ennemyGroup.Length);
        
    }

    private void FillGroupEnemy()
    {
        EnemyWave[] enemiesWave = waveSettings.waveData.FindAll(o => o.waveMin <= currentWave).ToArray();
        EnemyWave enemy =  null;
        Tuple<int, int, GameObject> tuple;

        for(int i = 0; i < ennemyList.Count; ++i)
        {
            for (int j = 0; j < ennemyList[i].ennemyGroup.Length; ++j)
            {
                enemy = waveSettings.waveData.GetRandom();
                Tool.PickPowerWave(enemy);
                tuple = Tuple.Create(i, j, enemy.prefab);
                queueInstantiate.Enqueue(tuple);
            }
        }
    }

    private IEnumerator InstantiateEnemies()
    {
        while(queueInstantiate.Count > 0)
        {
            var obj = queueInstantiate.Dequeue();
            ennemyList[obj.Item1].ennemyGroup[obj.Item2] = Instantiate(obj.Item3);
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].GetComponent<LifeEnnemyComponent>().onDeath += OnEnnemyDeath;
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].transform.parent = gameObjectGroup.transform;
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].transform.position = ennemyList[obj.Item1].area.GetRandomPointInside();
            ennemyList[obj.Item1].ennemyGroup[obj.Item2].SetActive(false);

            yield return new WaitForSeconds(0.08f);
        }
    }

    public void DestroyAllEnemy()
    {
        Destroy(gameObjectGroup);
    }

    private int GetNbSpawnPoint()
    {
        //Linear interpolation to have 3 point at 0 and 12 point at 8 or more, with random + 0/1
         if (!modDebug) return 3 + (3 * (Mathf.Clamp(currentWave, 0, 8) / 8)) + (int)(UnityEngine.Random.Range(0, 2) * Mathf.Clamp01(currentWave));
         return nbSpawnPoint;
    }

    private int GetNbMonsterSpawn()
    {
        //add formule to increase monsters
        if (modDebug) return nbMonsterSpawn;
        else return (int)((currentWave*1.5f) + UnityEngine.Random.Range(0,2));
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
            //draw point center group
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireCube(group.area.center, Vector3.one *10f);

            //draw target forward group vector
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawLine(group.area.center,new Vector3(group.area.center.x - transform.position.x, 0, group.area.center.z - transform.position.z).normalized);

            //draw area property
            group.area.DrawGizmoArea();

        }
    }
#endif
}

[System.Serializable]
public class EnnemyGroup
{
    public GameObject[] ennemyGroup;
    public Vector3 spawnPoint;
    public AreaData area;
}

[System.Serializable]
public class AreaData
{
    [SerializeField] private Vector3 vertexUpR;
    [SerializeField] private Vector3 vertexUpL;
    [SerializeField] private Vector3 vertexDownR;
    [SerializeField] private Vector3 vertexDownL;
    [SerializeField] private Vector3 currentForward;

    public Vector3 center;

    public Vector3 GetRandomPointInside()
    {
        float sizeX = Vector3.Distance(vertexUpR, vertexUpL)/2f;
        float sizeZ = Vector3.Distance(vertexUpL, vertexDownL)/2f;
        return new Vector3(UnityEngine.Random.Range(-sizeX,sizeX),0, UnityEngine.Random.Range(-sizeZ,sizeZ)) + center;
    }

    public void GenerateArea(ref Vector3 targetForward, float distance, float sizeUp,float sizeDown)
    {
        vertexUpR = new Vector3(sizeUp, 0, Vector3.forward.z * distance);
        vertexUpL = new Vector3(-sizeUp, 0, Vector3.forward.z * distance);
        vertexDownR = new Vector3(sizeUp, 0, -Vector3.forward.z * distance);
        vertexDownL = new Vector3(- sizeUp, 0, -Vector3.forward.z * distance);


        //RotateArea(ref targetForward);

        CalculForwardVector();
        ApplyCoordinateToCenterArea();
    }

    public void RotateArea(ref Vector3 targetForward)
    {
        float angle = Mathf.Atan2(targetForward.z - currentForward.z, targetForward.x - currentForward.x);

        RotatePointArea(ref vertexDownL, ref angle);
        RotatePointArea(ref vertexDownR, ref angle);
        RotatePointArea(ref vertexUpL, ref angle);
        RotatePointArea(ref vertexUpR, ref angle);
    }

    public void ApplyCoordinateToCenterArea()
    {
        vertexDownL += center;
        vertexDownR += center;
        vertexUpL += center;
        vertexUpR += center;
    }

    public void CalculForwardVector()
    {
        currentForward = ((vertexUpL + vertexUpR) / 2f - (vertexDownL + vertexDownR) / 2f).normalized;
    }

    public void RotatePointArea(ref Vector3 point, ref float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        //point.x = cos * (point.x - center.x) + sin * (point.z - center.z) + center.x;
        //point.z = sin * -(point.x - center.x) + cos * (point.z - center.z) + center.z;

        point.x = point.x * cos - point.z * sin;
        point.z = point.x * sin + point.z * cos;
    }

    public void AlignCenterInCenterArea(ref Vector3 moveVector, float distance)
    {
        this.center += moveVector* -2 * distance/2;
    }

    public void DebugRotate(float angle)
    {
        
        angle = Mathf.Deg2Rad * angle;

        RotatePointArea(ref vertexDownL, ref angle);
        RotatePointArea(ref vertexDownR, ref angle);
        RotatePointArea(ref vertexUpL, ref angle);
        RotatePointArea(ref vertexUpR, ref angle);

        CalculForwardVector();
    }

#if UNITY_EDITOR
    public void DrawGizmoArea()
    {
        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.DrawLine(vertexUpR, vertexUpL);
        UnityEditor.Handles.DrawLine(vertexDownR, vertexDownL);
        UnityEditor.Handles.DrawLine(vertexUpL, vertexDownL);
        UnityEditor.Handles.DrawLine(vertexUpR, vertexDownR);

        //draw current forward vector area
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawLine(center, center + currentForward * 20f);
    }
#endif
}