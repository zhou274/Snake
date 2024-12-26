using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public AudioClip pickupSound, deadSound;
    // Start is called before the first frame update
    void Awake()
    {
        makeInstance();
    }

    void makeInstance(){
        
        if(instance==null){
            instance=this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPickupSound(){

        
        AudioSource.PlayClipAtPoint(pickupSound,transform.position);

    }

    public void PlayDeadSound(){

        AudioSource.PlayClipAtPoint(deadSound,transform.position);
        
    }
}
