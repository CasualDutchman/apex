using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationMethod { Procedural, Tiles }

public class WorldGeneration : MonoBehaviour {

    public GenerationMethod generationMethod;

    public float tileSize;
    public float loadDiameter = 6;

    public Transform cameraRig;

    public Material testMaterial;

    bool isMakingChunks = true;
    public bool needMakingChunks = true;

    public Vector3 startingPosition;
    Vector3 playerPosition;

    Dictionary<Vector3, ChunkRequest> chunkDictionary = new Dictionary<Vector3, ChunkRequest>();

    List<Vector3> arr = new List<Vector3>();

    void Start () {
        cameraRig.position = startingPosition;
        playerPosition = new Vector3(Mathf.FloorToInt(startingPosition.x / tileSize), 0, Mathf.FloorToInt(startingPosition.z / tileSize)); ;

        LoadArray();
        UpdateView();
        StartCoroutine(MakeChunks());
    }
	
	void Update () {
        Vector3 newPlayerPosition = new Vector3(Mathf.FloorToInt(cameraRig.position.x / tileSize), 0, Mathf.FloorToInt(cameraRig.position.z / tileSize));// cameraRig.position / (int)tileSize;

        if (newPlayerPosition != playerPosition) {
            LoadArray();
        }
        UpdateView();

        playerPosition = newPlayerPosition;
    }

    void LoadArray() {
        int diameter = (int)(tileSize * loadDiameter);
        Vector3 yOffset = new Vector3(0, 0, 1);
        Vector3 pos = (playerPosition + yOffset) * tileSize;

        List<Vector3> list = new List<Vector3>();

        for (int y = -diameter; y <= diameter; y++) {
            for (int x = -diameter; x <= diameter; x++) {
                Vector3 key = pos + new Vector3(x * tileSize, 0, y * tileSize);
                float dis = Vector3.Distance(pos, key);
                if (dis < diameter) {
                    if (!chunkDictionary.ContainsKey(key)) {
                        chunkDictionary.Add(key, AddRequest(key, dis));
                        needMakingChunks = true;
                    }
                    list.Add(key);
                }
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

        if (isMakingChunks == false && isMakingChunks != needMakingChunks) {
            isMakingChunks = true;
            StartCoroutine(MakeChunks());
        }
        Debug.Log(chunkDictionary.Count);
    }

    IEnumerator MakeChunks() {
        while (isMakingChunks) {
            List<ChunkRequest> chunkRequests = new List<ChunkRequest>(chunkDictionary.Values);
            chunkRequests.Sort((p1, p2) => p1.activation.CompareTo(p2.activation));

            ChunkRequest currentRequest = null;
            foreach (ChunkRequest re in chunkRequests) {
                //Debug.Log(re.activation);
                if (re.gameObject == null) {
                    currentRequest = re;
                    break;
                }
            }
            
            if(currentRequest == null) {
                isMakingChunks = false;
                needMakingChunks = false;
                break;
            }

            if (generationMethod == GenerationMethod.Procedural) {
                MakeProceduralChunk(currentRequest);
            } else {
                MakeTileChunk(currentRequest);
            }

            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    void MakeProceduralChunk(ChunkRequest request) {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = new Vector3(tileSize, 1, tileSize);
        go.transform.position = request.key;
        go.transform.parent = transform;
        request.gameObject = go;
    }

    void MakeTileChunk(ChunkRequest request) {
        GameObject tile = Resources.Load<GameObject>("Generation/Tiles/Tile");

        MeshFilter[] meshFilters = tile.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length + 1) {
            if(i == 0) {
                combine[i].mesh = tile.GetComponent<MeshFilter>().sharedMesh;
                combine[i].transform = tile.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                i++;
            } else {
                combine[i - 1].mesh = meshFilters[i - 1].sharedMesh;
                combine[i - 1].transform = Matrix4x4.TRS(request.key + meshFilters[i - 1].transform.localPosition, meshFilters[i - 1].transform.localRotation, meshFilters[i - 1].transform.localScale);
                i++;
            }
        }

        GameObject go = new GameObject();
        go.name = "woo";
        go.transform.parent = transform;

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine);

        MeshRenderer render = go.AddComponent<MeshRenderer>();
        render.material = testMaterial;

        request.gameObject = go;
    }

    ChunkRequest AddRequest(Vector3 pos, float distance) {
        ChunkRequest request = new ChunkRequest();
        request.key = pos;
        request.activation = distance / (tileSize * loadDiameter);

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
        return v3.x > t1 && v3.x < t2 && v3.y > t1 && v3.y < t2 && v3.z > 0;
    }

    void OnDrawGizmos() {
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

    //[0, 1] 1 needs data, 0 doesnt really need data
    //when the player doesn't move, the world will generate the object with the highest activation first and than the one below that, untill all chunks are filled
    //there is a possibility for a request to not be executed, because it never got activated
    public float activation;

    //The object it needs to keep track off
    //When != null it already has data, so it doesn't need to be generated again
    public GameObject gameObject;

}