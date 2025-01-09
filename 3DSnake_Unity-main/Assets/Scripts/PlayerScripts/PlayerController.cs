using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerDirection direction;
    [HideInInspector] public float stepLength=0.2f;
    [HideInInspector] public float movementFrequency=0.1f;
    private float counter;
    private bool move;
    [SerializeField] private GameObject tailPrefab;
    private List<Vector3> deltaPosition;
    private List<Rigidbody> nodes;
    private Rigidbody mainBody;
    private Rigidbody headBody;
    private Transform tr;
    private bool createNodeAtTail;
    private bool isGameOver=false;

    public GameObject gameOverPanel;

    public string clickid;
    private StarkAdManager starkAdManager;


    // Start is called before the first frame update
    void Awake()
    {
        tr=transform;
        mainBody=GetComponent<Rigidbody>();

        InitSnakeNodes();
        InitPlayer();     

        deltaPosition=new List<Vector3>(){
            new Vector3(-stepLength,0f), //Left direction
            new Vector3(0f,stepLength), // Upward direction
            new Vector3(stepLength,0f), // Right direction 
            new Vector3(0f,-stepLength) // Downward direction

        };

    }

    // Update is called once per frame
    void Update()
    {
        checkMovementFrequency();
    }

    void FixedUpdate(){
        if(move){
            move=false;

            Move();
        }

    }
    
    
    void InitSnakeNodes(){
        nodes=new List<Rigidbody>();

        //Get rigidbody components of the initial snake
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>());

        //Set headBody reference to point to the head game object rigid body 
        headBody=nodes[0];
    }

    void InitPlayer(){
        
        SetRandomDirection();

        switch(direction) {

            case PlayerDirection.RIGHT:
                nodes[1].position=nodes[0].position- new Vector3(Metrics.NODE,0f,0f);
                nodes[2].position=nodes[0].position- new Vector3(Metrics.NODE*2f,0f,0f);
                break;

            case PlayerDirection.LEFT:
                nodes[1].position=nodes[0].position+ new Vector3(Metrics.NODE,0f,0f);
                nodes[2].position=nodes[0].position+ new Vector3(Metrics.NODE*2f,0f,0f);
                Debug.Log("LEFT");
                break;

            case PlayerDirection.UP:
                nodes[1].position=nodes[0].position- new Vector3(0f,Metrics.NODE,0f);
                nodes[2].position=nodes[0].position- new Vector3(0f,Metrics.NODE*2f,0f);
                break;

            case PlayerDirection.DOWN:
                nodes[1].position=nodes[0].position+ new Vector3(0f,Metrics.NODE,0f);
                nodes[2].position=nodes[0].position+ new Vector3(0f,Metrics.NODE*2f,0f);
                break;

        }
    }

    void SetRandomDirection(){
        int dirRandom=Random.Range(0, (int)PlayerDirection.COUNT);
        direction=(PlayerDirection)dirRandom;
    }

    void checkMovementFrequency(){

        counter+=Time.deltaTime;

        if(counter>=movementFrequency){
            counter=0;
            move=true;
        }

    }

    void Move(){
        Vector3 dPosition=deltaPosition[(int)direction];
        Vector3 parentPosition=headBody.position;
        Vector3 previousPosition;

        mainBody.position+=dPosition;
        headBody.position+=dPosition;

        for(int i=1;i<nodes.Count;i++){
            previousPosition=nodes[i].position;
            nodes[i].position=parentPosition;
            parentPosition=previousPosition;
        }

        if(createNodeAtTail){

            createNodeAtTail=false;
            
            GameObject newNode= Instantiate(tailPrefab, nodes[nodes.Count-1].position, Quaternion.identity);
            newNode.transform.SetParent(transform, true);
            nodes.Add(newNode.GetComponent<Rigidbody>());
        }
    }

    public void SetInputDirection(PlayerDirection dir){

        if(dir==PlayerDirection.UP && direction==PlayerDirection.DOWN ||
        dir==PlayerDirection.DOWN && direction==PlayerDirection.UP||
        dir==PlayerDirection.LEFT && direction==PlayerDirection.RIGHT||
        dir==PlayerDirection.RIGHT && direction==PlayerDirection.LEFT){

            return; //Exit the function if opposite directions encountered.
        }

        direction=dir;

        ForceMove();
    }
    
    void ForceMove() {

        counter=0;
        move=false;
        Move();

    }

    void OnTriggerEnter(Collider target){

        if(target.tag == Tags.FRUIT){

            target.gameObject.SetActive(false);
            createNodeAtTail=true;
            GameplayController.instance.IncreaseScore();
            AudioManager.instance.PlayPickupSound();

        }

        if(target.tag==Tags.WALL || target.tag == Tags.BOMB  || target.tag==Tags.TAIL){

            AudioManager.instance.PlayDeadSound();
            Time.timeScale=0f;
            ShowInterstitialAd("af36h9l6kgmfea3m83",
           () => {
           },
           (it, str) => {
               Debug.LogError("Error->" + str);
           });
            gameOverPanel.SetActive(true);

        }

    }


    public void Reload()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
    public void PlayerReset()
    {
        ShowVideoAd("l57jo48s04211ohnc7",
            (bol) => {
                if (bol)
                {
                    transform.position = new Vector3(0, 0, 5.8f);
                    gameOverPanel.SetActive(false);
                    Time.timeScale = 1;
                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);
                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });

    }

    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
