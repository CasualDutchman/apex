using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)
using UnityEditor;

[CustomEditor(typeof(EnemyManager))]
public class EditorPerlinSettings : Editor {

    EnemyManager manager;

    Texture2D debugTex;

    void OnEnable() {
        manager = (EnemyManager)target;

        UpdateDebugInfo();
    }

    void OnDisable() {
        debugTex = null;
    }

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) {
            UpdateDebugInfo();
        }

        if (debugTex != null) {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(debugTex);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }

    void UpdateDebugInfo() {
        int size = 48;
        int scale = 4;

        if (debugTex == null) {
            debugTex = new Texture2D(size * scale, size * scale) {
                filterMode = FilterMode.Point
            };
        }

        Vector2[,] iArr = GetArray(0, 0, size);

        Color[] colorss = new Color[(size * scale) * (size * scale)];

        for (int y = 0; y < size * scale; y++) {
            for (int x = 0; x < size * scale; x++) {

                Vector2 id = iArr[Mathf.FloorToInt(x / (float)scale), Mathf.FloorToInt(y / (float)scale)];

                //Debug.Log(id);
                 
                Color col = Color.black;
                if ((int)id.x == 1) {
                    col = new Color(1 - ((1 / (float)manager.predators.Count) * id.y), 0, 0);
                    //Debug.Log((1 / (float)manager.predators.Count) * id.y);
                }
                else if ((int)id.x == 2) {
                    col = new Color(0, 1 - ((1 / (float)manager.preys.Count) * id.y), 0);
                }

                colorss[(y * (size * scale)) + x] = col;
            }
        }

        debugTex.SetPixels(colorss);
        debugTex.Apply();
    }

    Vector2[,] GetArray(int posX, int posY, int size) {
        Vector2[,] arr = new Vector2[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                int i = 0;
                SpawnType t = manager.GetSpawnType(posX + (x * 12), posY + (y * 12), out i);
                //Debug.Log(t.ToString());
                arr[x, y] = new Vector2(t == SpawnType.None ? 0 : (t == SpawnType.Predator ? 1 : 2), i);
            }
        }
        return arr;
    }
}

#endif

public enum SpawnType { None, Prey, Predator }

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    public Transform cameraRig;

    public float enemyOutOfCamera;

    public int radius;
    public float scale;
    public float despawnRadius;

    public List<GameObject> preys = new List<GameObject>();
    public List<GameObject> predators = new List<GameObject>();

    Dictionary<Vector3, GameObject> enemyDictionary = new Dictionary<Vector3, GameObject>();

    public float perlin1, perlin2;
    public float bias;
    public AnimationCurve curve;

    //public void UpdateEnemyList(int posX, int posY) {
    //    StartCoroutine(UpdateList(posX, posY));
    //}

    void Awake() {
        instance = this;
    }

    public bool OutOfReach(Vector3 pos) {
        return Vector3.Distance(pos, cameraRig.position) > enemyOutOfCamera;
    }

    public void UpdateChunk(Vector3 pos) {
        Vector3 key = pos;

        if (!enemyDictionary.ContainsKey(key)) {
            int animalIndex = 0;
            SpawnType type = GetSpawnType((int)key.x, (int)key.z, out animalIndex);

            if (type != SpawnType.None) {
                GameObject go = Instantiate(type == SpawnType.Prey ? preys[animalIndex] : predators[animalIndex]);
                go.transform.position = key;
                go.transform.eulerAngles = new Vector3(0, Random.Range(0, 359), 0);
                Enemy enemy = go.GetComponent<Enemy>();
                enemy.homePos = key;
                enemy.manager = this;

                enemyDictionary.Add(key, go);
            }
        }
    }

    public void RemoveEnemy(Vector3 homePos) {
        if (enemyDictionary.ContainsKey(homePos)) {
            Destroy(enemyDictionary[homePos]);
            enemyDictionary.Remove(homePos);
        }
    }

    public SpawnType GetSpawnType(int x, int y, out int i) {
        i = 0;
        SpawnType type;

        float f1 = Mathf.PerlinNoise(x * perlin1, y * perlin1);
        f1 = curve.Evaluate(f1);

        if (f1 < bias) { type = SpawnType.Prey; } 
        else if (f1 > 1f - bias) { type = SpawnType.Predator; } 
        else { type = SpawnType.None; }

        if (type != SpawnType.None) {
            float f2 = Mathf.PerlinNoise(x * perlin2, y * perlin2);
            i = Mathf.FloorToInt(f2 * (float)(type == SpawnType.Prey ? preys.Count : predators.Count));
        }
        return type;
    }
}
