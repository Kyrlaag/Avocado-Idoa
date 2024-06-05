using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryChecker : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            FindObjectOfType<GameManager>().LoadLevel("Level 1");
        } 
    }
}
