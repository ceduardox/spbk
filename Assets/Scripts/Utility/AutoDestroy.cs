using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    
    ParticleSystem fx;
    public ParticleSystem fx_duracion;
    Transform target;
    void Start()
    {
        fx = GetComponent<ParticleSystem>();
        fx.Play();
        if(fx_duracion)
            fx = fx_duracion;
    }

    public void setTraget(Transform _target)
    {
        target = _target;
        //CLog.Log("TARGET: " + target);
    }
    // Update is called once per frame
    void Update()
    {
        if (!fx.isPlaying)
        { 
            Destroy(gameObject);
            return;
        }
        if(target)
        {
            transform.position = target.position;
        }
    }
}
