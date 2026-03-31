using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlide : MonoBehaviour
{
    [SerializeField] private Animator anm;
    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    public void MenuSlideActivated()
    {
        anm.enabled = true;
    }
}
