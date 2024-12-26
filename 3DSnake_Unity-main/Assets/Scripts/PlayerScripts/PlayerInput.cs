using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    
    private PlayerController playerController;
    private int Horizontal=0;
    private int Vertical=0;
    private int isPlaying;

    public Button upBtn;
    public Button downBtn;
    public Button leftBtn;
    public Button rightBtn;

    private bool isUpBtnClick;
    private bool isDownBtnClick;
    private bool isLeftBtnClick;
    private bool isRightBtnClick;



    public enum Axis{
        Horizontal,
        Vertical
    }

    // Start is called before the first frame update
    void Awake()
    {
        playerController=GetComponent<PlayerController>();
        upBtn.onClick.AddListener(UpBtnClick);
        downBtn.onClick.AddListener(DownBtnClick);
        leftBtn.onClick.AddListener(LeftBtnClick);
        rightBtn.onClick.AddListener(RightBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        Horizontal=0;
        Vertical=0;
        GetKeyboardInput();
        SetMovement();
    }

    void GetKeyboardInput(){
        
        Horizontal=GetAxisRaw(Axis.Horizontal);
        Vertical=GetAxisRaw(Axis.Vertical);

        if(Horizontal !=0){

            Vertical=0;
        }

    }

    void SetMovement(){

        if(Vertical!=0){

            playerController.SetInputDirection((Vertical==1) ?
                                                PlayerDirection.UP : PlayerDirection.DOWN);
        }

        else if(Horizontal!=0){

                playerController.SetInputDirection((Horizontal==1) ?
                                                    PlayerDirection.RIGHT : PlayerDirection.LEFT);

        }

    }

    int GetAxisRaw(Axis axis){

        if(axis==Axis.Horizontal){
            //bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            bool left = isLeftBtnClick;
            if(isLeftBtnClick==true)
            {
                isLeftBtnClick = false;
            }
            //bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            bool right = isRightBtnClick;
            if(isRightBtnClick==true)
            {
                isRightBtnClick = false;
            }
            if (left){
                return -1;
            }

            if(right){
                return 1;
            }

            return 0;

        }

        else if(axis==Axis.Vertical){

            //bool up=Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            bool up = isUpBtnClick;
            if(isUpBtnClick==true)
            {
                isUpBtnClick = false;
            }
            //bool down=Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
            bool down = isDownBtnClick;
            if (isDownBtnClick == true)
            {
                isDownBtnClick = false;
            }
                

            if(up){
                return 1;
            }

            if(down){
                return -1;
            }

            return 0;

        }

        return 0;

    }
    public void UpBtnClick()
    {
        isUpBtnClick = true;
    }
    public void DownBtnClick()
    {
        isDownBtnClick = true;
    }
    public void LeftBtnClick()
    {
        isLeftBtnClick = true;
    }
    public void RightBtnClick()
    {
        isRightBtnClick = true;
    }
}
