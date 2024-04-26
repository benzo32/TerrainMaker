using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainMaker : EditorWindow
{
    private const string TreePrefabKeyPrefix = "TreePrefab_";
    private const string RockPrefabKeyPrefix = "RockPrefab_";
    private const string GrassPrefabKeyPrefix = "GrassPrefab_";
    private float terrainWidth = 2000f;
    private float terrainLength = 2000f;
    private int totalTreeCount = 2000;
    private int totalRockCount = 6000;
    private int totalGrassCount = 30000;
    private int treePrefabCount = 2;
    private int rockPrefabCount = 2;
    private int grassPrefabCount = 4;
    private List<GameObject> rockPrefabs = new List<GameObject>();
    private List<GameObject> treePrefabs = new List<GameObject>();
    private List<GameObject> grassPrefabs = new List<GameObject>();
    Vector2 scrollPos;
    private GameObject environment;

    [MenuItem("Tools/Terrain Maker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TerrainMaker));
    }

    private void OnEnable()
    {
        LoadPrefabs();
    }

    private void LoadPrefabs()
    {
        treePrefabs.Clear();
        rockPrefabs.Clear();
        grassPrefabs.Clear();

        for (int i = 0; i < treePrefabCount; i++)
        {
            treePrefabs.Add(LoadPrefab(TreePrefabKeyPrefix + i));
        }

        for (int i = 0; i < rockPrefabCount; i++)
        {
            rockPrefabs.Add(LoadPrefab(RockPrefabKeyPrefix + i));
        }

        for (int i = 0; i < grassPrefabCount; i++)
        {
            grassPrefabs.Add(LoadPrefab(GrassPrefabKeyPrefix + i));
        }
    }

    private GameObject LoadPrefab(string key)
    {
        string prefabPath = EditorPrefs.GetString(key);
        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    private void SavePrefabs()
    {
        for (int i = 0; i < treePrefabs.Count; i++)
        {
            string key = TreePrefabKeyPrefix + i;
            string prefabPath = AssetDatabase.GetAssetPath(treePrefabs[i]);
            EditorPrefs.SetString(key, prefabPath);
        }

        for (int i = 0; i < rockPrefabs.Count; i++)
        {
            string key = RockPrefabKeyPrefix + i;
            string prefabPath = AssetDatabase.GetAssetPath(rockPrefabs[i]);
            EditorPrefs.SetString(key, prefabPath);
        }

        for (int i = 0; i < grassPrefabs.Count; i++)
        {
            string key = GrassPrefabKeyPrefix + i;
            string prefabPath = AssetDatabase.GetAssetPath(grassPrefabs[i]);
            EditorPrefs.SetString(key, prefabPath);
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(480), GUILayout.Height(800));

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Terrain Settings", EditorStyles.boldLabel);
        terrainWidth = EditorGUILayout.FloatField("Terrain Width", terrainWidth);
        terrainLength = EditorGUILayout.FloatField("Terrain Lenght", terrainLength);

        EditorGUILayout.Space();
        GUILayout.Label("Object Creation Settings", EditorStyles.boldLabel);
        totalTreeCount = EditorGUILayout.IntField("Total Rock Count", totalTreeCount);
        totalRockCount = EditorGUILayout.IntField("Total Tree Count", totalRockCount);
        totalGrassCount = EditorGUILayout.IntField("Total Grass Count", totalGrassCount);
        treePrefabCount = EditorGUILayout.IntSlider("Tree Prefab Count", treePrefabCount, 0, 10);
        rockPrefabCount = EditorGUILayout.IntSlider("Rock Prefab Count", rockPrefabCount, 0, 10);
        grassPrefabCount = EditorGUILayout.IntSlider("Grass Prefab Count", grassPrefabCount, 0, 10);

        EditorGUILayout.Space();
        GUILayout.Label("Prefab Selection", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tree Prefabs", EditorStyles.boldLabel);
        for (int i = 0; i < rockPrefabCount; i++)
        {
            if (i >= rockPrefabs.Count)
            {
                rockPrefabs.Add(null);
            }
            rockPrefabs[i] = EditorGUILayout.ObjectField("Tree Prefab" + i, rockPrefabs[i], typeof(GameObject), false) as GameObject;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rock Prefabs", EditorStyles.boldLabel);
        for (int i = 0; i < treePrefabCount; i++)
        {
            if (i >= treePrefabs.Count)
            {
                treePrefabs.Add(null);
            }
            treePrefabs[i] = EditorGUILayout.ObjectField("Rock Prefab" + i, treePrefabs[i], typeof(GameObject), false) as GameObject;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grass Prefabs", EditorStyles.boldLabel);
        for (int i = 0; i < grassPrefabCount; i++)
        {
            if (i >= grassPrefabs.Count)
            {
                grassPrefabs.Add(null);
            }
            grassPrefabs[i] = EditorGUILayout.ObjectField("Grass Prefab" + i, grassPrefabs[i], typeof(GameObject), false) as GameObject;
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate"))
        {
            GenerateTerrains();
            SavePrefabs();
        }

        if (GUILayout.Button("Delete Created"))
        {
            ClearEnvironment();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void GenerateTerrains()
    {
        ClearEnvironment();

        environment = new GameObject("Ortam");
        environment.tag = "BenzoTerrainMakerOrtam";
        Terrain terrain = GenerateTerrain();
        GenerateTrees(terrain, totalTreeCount);
        GenerateRocks(terrain, totalRockCount);
        GenerateGrass(terrain, totalGrassCount);
    }

    private Terrain GenerateTerrain()
    {
        GameObject terrainObject = Terrain.CreateTerrainGameObject(new TerrainData());
        terrainObject.transform.parent = environment.transform;
        terrainObject.name = "Terrain_";
        terrainObject.transform.position = new Vector3(terrainWidth, 0f, 0f);
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(terrainWidth, 0f, terrainLength);

        return terrain;
    }

    private void GenerateTrees(Terrain terrain, int totalTreeCount)
    {
        for (int i = 0; i < totalTreeCount; i++)
        {
            GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabCount)];
            Vector3 position = GenerateValidPosition(terrain);
            float height = terrain.SampleHeight(position);
            position.y = height;
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject tree = Instantiate(treePrefab, position, rotation);
            tree.transform.parent = environment.transform;
        }
    }

    private void GenerateRocks(Terrain terrain, int totalRockCount)
    {
        for (int i = 0; i < totalRockCount; i++)
        {
            GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabCount)];
            Vector3 position = GenerateValidPosition(terrain);
            float height = terrain.SampleHeight(position);
            position.y = height;
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject rock = Instantiate(rockPrefab, position, rotation);
            rock.transform.parent = environment.transform;
        }
    }

    private Vector3 GenerateValidPosition(Terrain terrain)
    {
        float minDistance = 10f;
        float maxAttempts = 1000;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector3 position = new Vector3(Random.Range(0f, terrainWidth), 0f, Random.Range(0f, terrainLength));
            float height = terrain.SampleHeight(position);
            position.y = height;

            bool isValidPosition = CheckCollision(position, minDistance);
            if (isValidPosition)
            {
                return position;
            }

            attempts++;
        }

        Debug.LogWarning("A valid location for the object could not be found.");
        return Vector3.zero;
    }

    private bool CheckCollision(Vector3 position, float minDistance)
    {
        Collider[] colliders = Physics.OverlapSphere(position, minDistance);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Tree") || collider.CompareTag("Rock"))
            {
                return false;
            }
        }

        return true;
    }


    private void GenerateGrass(Terrain terrain, int totalGrassCount)
    {
        for (int i = 0; i < totalGrassCount; i++)
        {
            GameObject grassPrefab = grassPrefabs[Random.Range(0, grassPrefabCount)];
            Vector3 position = new Vector3(Random.Range(0f, terrainWidth), 0f, Random.Range(0f, terrainLength));
            float height = terrain.SampleHeight(position);
            position.y = height;

            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f); // Rastgele yön oluşturulması
            GameObject grass = Instantiate(grassPrefab, position, rotation);
            grass.transform.parent = environment.transform;
        }
    }

    private void ClearEnvironment()
    {
        GameObject[] ortamObjeleri = GameObject.FindGameObjectsWithTag("BenzoTerrainMakerOrtam");
        
        foreach (GameObject ortamObjesi in ortamObjeleri)
        {
            DestroyImmediate(ortamObjesi);
        }
    }


    private void OnDestroy()
    {
        SavePrefabs();
    }
}
