using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    
    public class SegmentEvent : UnityEvent<Segment> {}
    public class ShipEvent : UnityEvent<Ship> {}

    public Camera MainCamera;
    public LineRenderer MainLine;
    public GameObject ShipPrefab;
    public GameObject SegmentPrefab;
    public TextMeshPro SegmentPriceText;

    public TextMeshProUGUI MoneyText;

    public int Money = 1000;
    public int SegmentPrice = 500;
    public float SegmentPriceMultiplier = 1.1f;
    public int ShipLoadingPrice = 50;
    
    public float FreezeRate = 5;
    public float ShipRate = 5;
    public float ShipSpeed = 3;
    public float ShipRateMultiplier = 1.1f;
    public float ShipSpeedMultiplier = 1.1f;
    public int OpenSegments = 5;
    
    private Vector3[] pathPositions;
    
    public List<Segment> segments = new List<Segment>();
    public List<Base> bases;
    
    public MeshFilter PathMesh;
    
    public SegmentEvent SegmentClickEvent = new SegmentEvent();
    public ShipEvent ShipDoneEvent = new ShipEvent();
    public ShipEvent ShipFailedEvent = new ShipEvent();
    public UnityEvent MoneyIsOver = new UnityEvent();
    public UnityEvent PathIsDone = new UnityEvent();
    
    
    void Start(){

        

        pathPositions = new Vector3[MainLine.positionCount];

        MainLine.GetPositions(pathPositions);
        
        GenerateMesh();
        MainLine.enabled = false;
        
        SegmentClickEvent.AddListener(OnSegmentEvent);
        ShipDoneEvent.AddListener(OnShipDone);
        ShipFailedEvent.AddListener(onShipFailed);
        MoneyIsOver.AddListener(OnMoneyIsOver);
        PathIsDone.AddListener(OnPathIsDone);
        
        ShowTutorial();

        StartCoroutine(FreezeSegment());
        StartCoroutine(SpawnShip());

    }

    void ShowTutorial(){
        
        PlayerPrefs.DeleteAll();

        int showTutorial = PlayerPrefs.GetInt("showTutorial");
        
        if (showTutorial == 0) {
            Time.timeScale = 0;
            SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
            PlayerPrefs.SetInt("showTutorial", 1);
        }

    }
    
    public void OnSegmentEvent(Segment target){

        if (Money >= SegmentPrice) {
            int index = segments.IndexOf(target);
            Segment nextCracked = segments.ElementAtOrDefault(index + 1);
            
            target.setState(Segment.StateType.Water);

            foreach (var bBase in bases) {
                if (bBase.index == index && !bBase.isOpen) {
                    bBase.Sprite.color = Color.white;
                    bBase.isOpen = true;
                    bBase.OpenEvent.Invoke();
                }
            }

            SegmentPrice = Mathf.FloorToInt(SegmentPrice * SegmentPriceMultiplier);
            ShipSpeed = ShipSpeed * ShipSpeedMultiplier;
            ShipRate = ShipRate * ShipRateMultiplier;

            if (nextCracked) {
                nextCracked.setState(Segment.StateType.Cracked);
                SegmentPriceText.transform.position = nextCracked.GetComponent<PolygonCollider2D>().bounds.center;
                SegmentPriceText.text = SegmentPrice.ToString();

            }
            
            
            Money -= SegmentPrice;
            OpenSegments++;

            if (OpenSegments == pathPositions.Length - 1) {
                PathIsDone.Invoke();
            }
        }
    }

    public void OnShipDone(Ship ship){
        Money += ship.Reward;
        SoundManager.PlaySound(1);
    }

    public void onShipFailed(Ship ship){
        Money -= Mathf.FloorToInt(ship.Reward / 2);
        SoundManager.PlaySound(2);
    }

    // Update is called once per frame
    void LateUpdate(){
        MoneyText.text = Money.ToString();
    }

    void Update(){
        
        if (Money <= 0) MoneyIsOver.Invoke();
        
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject.CompareTag("Segment")) {
                Segment script = hit.collider.GetComponent<Segment>();
                script.ClickEvent.Invoke();
            }
        }
        
    }

    IEnumerator SpawnShip(){
        yield return new WaitForSeconds(ShipRate);

        List<Base> openedBases = bases.Where(b => b.isOpen).ToList();

        int baseStartIndex = Random.Range(0, openedBases.Count);
        int baseFinishIndex = Random.Range(0, openedBases.Count);
        
        while (baseFinishIndex == baseStartIndex)
            baseFinishIndex = Random.Range(0, openedBases.Count);

        Base baseStart = openedBases[baseStartIndex];
        Base baseFinish = openedBases[baseFinishIndex];
      
        GameObject newShip = Instantiate(ShipPrefab);
        
        Ship shipScript = newShip.GetComponent<Ship>();
        shipScript.path = pathPositions;
        shipScript.manager = this;
        shipScript.startPoint = baseStart.index;
        shipScript.finishPoint = baseFinish.index;
        shipScript.TimeToSegment = ShipSpeed;

        StartCoroutine(SpawnShip());
    }

    void GenerateMesh(){
        Mesh mesh = new Mesh();
        MainLine.BakeMesh(mesh, MainCamera, true);
        PathMesh.mesh = mesh;

        List<Vector3> pathVertices = new List<Vector3>(mesh.vertexCount);
        List<Vector2> pathUVs = new List<Vector2>(mesh.vertexCount);
        mesh.GetVertices(pathVertices);
        mesh.GetUVs(0,pathUVs);
       
        int segmentsCount = pathPositions.Length - 1;
        
        for (int i = 0; i < segmentsCount; i++) {
            GameObject newSegment = Instantiate(SegmentPrefab);
            Mesh segmentMesh = GenerateSegment(pathVertices, pathUVs, i * 2);
            Segment script = newSegment.GetComponent<Segment>();
            newSegment.GetComponent<MeshFilter>().mesh = segmentMesh;
            
            script.UpdateCollider();
            
            script.manager = this;

            if (i < OpenSegments) {
                script.setState(Segment.StateType.Water);
            } else if (i == OpenSegments) {
                script.setState(Segment.StateType.Cracked);
                SegmentPriceText.transform.position = script.GetComponent<PolygonCollider2D>().bounds.center;
            } else {
                script.setState(Segment.StateType.Ice);
            }
            
            segments.Add(script);
        }
        
    }

    Mesh GenerateSegment(List<Vector3> vertices, List<Vector2> uvs, int startTriangle){
        Mesh segmentMesh = new Mesh();
        segmentMesh.vertices = vertices.ToArray();
        segmentMesh.uv = uvs.ToArray();
        segmentMesh.triangles = new [] {
            startTriangle, 
            startTriangle + 1,
            startTriangle + 2,
            startTriangle + 2,
            startTriangle + 1,
            startTriangle + 3
        };

        return segmentMesh;
    }


    IEnumerator FreezeSegment(){
        yield return new WaitForSeconds(FreezeRate);

        int randomIndex = Random.Range(3, OpenSegments - 1);

        Segment randomSegment = segments[randomIndex];
        
        if (!randomSegment.isShipOn)
            randomSegment.setState(Segment.StateType.Frozen);
        
        
        StartCoroutine(FreezeSegment());
    }

    void OnMoneyIsOver(){
        SceneManager.LoadSceneAsync("Gameover");
    }

    void OnPathIsDone(){
        SceneManager.LoadSceneAsync("Won");
    }

    public void Pause(){
        Time.timeScale = 1 - Time.timeScale;
        SceneManager.LoadSceneAsync("Pause", LoadSceneMode.Additive);
    }

    public void Unpause(){
        Time.timeScale = 1;
    }
}
