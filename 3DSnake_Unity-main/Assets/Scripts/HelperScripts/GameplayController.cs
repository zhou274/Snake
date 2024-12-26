using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameplayController : MonoBehaviour
{

    public static GameplayController instance;
    public GameObject fruitPickup, bombPickup;
    private float minX=-4.25f, maxX=4.25f, minY=-2.26f, maxY=2.26f;
    private float zpos=5.8f;
    private Text scoreText;
    private int scoreCount;
    private int hightScore;
    // Start is called before the first frame update
    void Awake()
    {
        makeInstance();
        PlayerPrefs.GetInt("BestScore");
    }
    
    void Start(){
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        Invoke("StartSpawning",0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void makeInstance(){

        if(instance==null){

            instance=this;

        }
    }

    IEnumerator SpawnPickups(){

        yield return new WaitForSeconds(Random.Range(1f,1.5f));

        if(Random.Range(0,10)>=2){

            Instantiate(fruitPickup,new Vector3(Random.Range(minX,maxX),Random.Range(minY,maxY),zpos),Quaternion.identity);

        }
        else{

            Instantiate(bombPickup,new Vector3(Random.Range(minX,maxX),Random.Range(minY,maxY),zpos),Quaternion.identity);

        }

        Invoke("StartSpawning",0f);

    }

    void StartSpawning(){

        StartCoroutine(SpawnPickups());
    }

    public void CancelSpawning(){
        CancelInvoke("StartSpawning");
    }

    public void IncreaseScore(){
        hightScore = PlayerPrefs.GetInt("BestScore");
        scoreCount++;
        if(scoreCount>hightScore)
        {
            PlayerPrefs.SetInt("BestScore",scoreCount);
        }
        scoreText.text = "得分: "+scoreCount;

    }

    public void gameOver(){

        scoreCount=0;

    }
}
