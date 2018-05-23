using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {

    public SeasonManager seasonManager;
    public EnemyManager enemyManager;

    [HideInInspector]
    public float playTime;

    public float tileSize;
    public float loadDiameter = 6;

    public Transform cameraRig;

    bool isMakingChunks = true;
    bool needMakingChunks = true;

    public Vector3 startingPosition;
    Vector3 playerPosition;
    Vector3 viewPosition;

    Dictionary<Vector3, ChunkRequest> chunkDictionary = new Dictionary<Vector3, ChunkRequest>();

    List<GameObject> tileList = new List<GameObject>();
    List<GameObject> restingList = new List<GameObject>();

    void Awake() {
        GetComponent<WolfManager>().startingPosition = startingPosition;
        cameraRig.position = startingPosition;
    }

    void Start () {
        Application.targetFrameRate = 300;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GameObject[] go = Resources.LoadAll<GameObject>("Generation/Tiles");
        foreach(GameObject g in go) {
            if (g.name.ToLower().EndsWith("r")) {
                restingList.Add(g);
            } else {
                tileList.Add(g);
            }
        }

        playerPosition = new Vector3(Mathf.FloorToInt(startingPosition.x / tileSize), 0, Mathf.FloorToInt(startingPosition.z / tileSize)); ;
        
        LoadArray();
        UpdateView();
        StartCoroutine(MakeChunks());
    }

	void Update () {
        playTime = Time.time;

        Vector3 newPlayerPosition = new Vector3(Mathf.FloorToInt(cameraRig.position.x / tileSize), 0, Mathf.FloorToInt(cameraRig.position.z / tileSize));// cameraRig.position / (int)tileSize;
        Vector3 newViewPosition = new Vector3(Mathf.FloorToInt(cameraRig.position.x), 0, Mathf.FloorToInt(cameraRig.position.z));

        if (newPlayerPosition != playerPosition) {
            LoadArray();
        }
        if (newViewPosition != viewPosition) {
            UpdateView();
        }

        if (isMakingChunks == false && isMakingChunks != needMakingChunks) {
            isMakingChunks = true;
            StartCoroutine(MakeChunks());
        }

        playerPosition = newPlayerPosition;
        viewPosition = newViewPosition;
    }

    void LoadArray() {
        Vector3 yOffset = new Vector3(0, 0, 1);
        Vector3 pos = (playerPosition + yOffset) * tileSize;

        List<Vector3> list = new List<Vector3>();

        for (int y = -(int)loadDiameter; y <= (int)loadDiameter; y++) {
            for (int x = -(int)loadDiameter; x <= (int)loadDiameter + 1; x++) {

                Vector3 key = pos + new Vector3(x * tileSize, 0, y * tileSize);

                if (!chunkDictionary.ContainsKey(key)) {
                    chunkDictionary.Add(key, AddRequest(key));
                    needMakingChunks = true;
                }

                enemyManager.UpdateChunk(pos, x, y, tileSize);

                list.Add(key);
            }
        }
        
        List<Vector3> remove = new List<Vector3>();

        foreach (KeyValuePair<Vector3, ChunkRequest> chunk in chunkDictionary) {
            if (!list.Contains(chunk.Key)) {
                remove.Add(chunk.Key);
            }
        }

        while (remove.Count > 0) {
            Destroy(chunkDictionary[remove[0]].gameObject);
            chunkDictionary.Remove(remove[0]);
            remove.RemoveAt(0);
        }
    }

    IEnumerator MakeChunks() {
        while (isMakingChunks) {
            ChunkRequest currentRequest = null;
            foreach (ChunkRequest re in chunkDictionary.Values) {
                if (re.gameObject == null) {
                    currentRequest = re;
                    break;
                }
            }
            
            if(currentRequest == null) {
                isMakingChunks = false;
                needMakingChunks = false;
                UpdateView();
                break;
            }

            MakeTileChunk(currentRequest);

            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    void MakeTileChunk(ChunkRequest request) {
        GameObject go = null;
        if (request.isResting) {
            go = Instantiate(restingList[Random.Range(0, restingList.Count)]);
        } else {
            go = Instantiate(tileList[Random.Range(0, tileList.Count)]);
        }

        Transform springItem = go.transform.Find("spring");
        Transform summerItem = go.transform.Find("summer");
        Transform autumnItem = go.transform.Find("autumn");
        Transform winterItem = go.transform.Find("winter");

        if (springItem != null && seasonManager.season != Season.Spring) Destroy(springItem.gameObject);
        if (summerItem != null && seasonManager.season != Season.Summer) Destroy(summerItem.gameObject);
        if (autumnItem != null && seasonManager.season != Season.Autumn) Destroy(autumnItem.gameObject);
        if (winterItem != null && seasonManager.season != Season.Winter) Destroy(winterItem.gameObject);

        go.GetComponent<Renderer>().material = seasonManager.seasonMaterial;
        foreach(Transform child in go.transform) {
            child.GetComponent<Renderer>().material = seasonManager.seasonMaterial;
        }

        go.name = request.key.ToString();
        go.transform.position = request.key;
        go.transform.parent = transform;
        go.SetActive(false);
        request.gameObject = go;
    }

    ChunkRequest AddRequest(Vector3 pos) {
        ChunkRequest request = new ChunkRequest();
        request.key = pos;

        return request;
    }

    void UpdateView() {
        foreach(KeyValuePair<Vector3, ChunkRequest> chunk in chunkDictionary) {
            if (chunk.Value.gameObject == null)
                continue;

            if (IsPossibleChunkInView(chunk.Key)) {
                chunk.Value.gameObject.SetActive(true);
            }else if(chunk.Value.gameObject.activeSelf) {
                chunk.Value.gameObject.SetActive(false);
            }
        }
    }

    bool IsPossibleChunkInView(Vector3 origin) {
        bool b1 = IsVectorInView(origin + new Vector3((tileSize * 0.5f), 0, (tileSize * 0.5f)));
        if (b1)
            return true;

        bool b2 = IsVectorInView(origin + new Vector3(-(tileSize * 0.5f), 0, (tileSize * 0.5f)));
        if (b2)
            return true;

        bool b3 = IsVectorInView(origin + new Vector3((tileSize * 0.5f), 0, -(tileSize * 0.5f)));
        if (b3)
            return true;

        bool b4 = IsVectorInView(origin + new Vector3(-(tileSize * 0.5f), 0, -(tileSize * 0.5f)));
        return b4;
    }

    bool IsVectorInView(Vector3 origin) {
        Vector3 v3 = Camera.main.WorldToViewportPoint(origin);
        float t1 = -0.1f, t2 = 1.1f;
        return v3.x > t1 && v3.x < t2 && v3.y > t1 - 0.3f && v3.y < t2 && v3.z > 0;
    }

    void OnDrawGizmosSelected() {
        foreach (KeyValuePair<Vector3, ChunkRequest> chunk in chunkDictionary) {
            if (chunk.Value.gameObject == null) {
                Gizmos.color = Color.white;
            }
            else if(!chunk.Value.gameObject.activeSelf) {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireCube(chunk.Key, new Vector3(tileSize, 1, tileSize));
        }
    }

}


public class ChunkRequest {
    //position in the world
    public Vector3 key;

    //in need of resting area
    public bool isResting;

    //The object it needs to keep track off
    //When != null it already has data, so it doesn't need to be generated again
    public GameObject gameObject;

}