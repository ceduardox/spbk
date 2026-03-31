using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Fusion;



public enum CustomDataItem
{
    Acceleration, Speed, Turn, Xp,Level,Look, Remaining
}

public enum Currencys
{
    TE, NL
}
public enum ClassPart//NO MODIFICAR
{
    KART,
    SPOILER_M, //alerones Delanteros
    SPOILER_F,
    SPOILER_B,
    FRONT_F,
    MOTOR,
    TIRES,
    EXHAUST,
    LIGHT_F,
    LIGHT_B,
    STEERT_WHEEL,
    GEAR,
    ACCESORIES_KART,
    CHAIR,
    CHASIS,
    ALL_KARTS,

    CHAR,
    GLOVES,
    HELMET,
    HAT,
    FACE,
    GLASSES,
    HAND,
    BODY,
    ACCESORIES_CHAR,

    ALL_CHAR,

    ANTENNA,
    LEGS, 
    DRIVER,
    PAINT,
    NEW2,
    NEW3,
    NEW4,

    //Power Ups
    POWERUPS,
    SLOT1,
    SLOT2,
    SLOT3,
    SLOT4,
    SLOT5,
    TURBO,
    BANANA,
    MISSILE,
    MISSILE_GUIDED,
    MINA,
    THUNDER,
    GHOST,
    TWISTED,
    BOMB,
    FAKEITEM,
    WALL,
    ACEITE,
    TRONCO,
    IMAN,
    SKULL,//Para nuevos items usa estos slots, si incertas uno mas arriba desplazas las asignaciones y quedan mal
    OIL,
    BOXEXPLOSIVE,
    SIZEBOX,
    BUBBLEGUM,
    SOAP,
    SMOKE,
    BALL,
    ALL,
    NONE,
    MUROESPINAS,
    REMOTEKART,
    PARTY,
    SPINS,
    SPINSVIP,
    SPINSV2,
    SPINSV2VIP,
    MINAVIP,
    SUPERBOOST,
    BUBBLEDF,
    CANON,
    CANONGRAVEDAD,
    BOMBGRAVEDAD,
    SMOKEADHESIVE,
    BOMBADHESIVE,
    BALLOONGRAVITY,
    BALLOONSPINAS,
    FREEZE,
    BOMBTELEPORT,
    BOMBENEMY,
    SIZE,
    BOMBSIZE,
    BOMBCAMBIO,
    SIZEALL,
    SMOKEALL,
    BOMBPARTY,
    DRON,
    MELEE_SABLELASER,
    MELEE_CHIPOTE,
    MELEE_BATE,
    MELEE_KATANA,
    MELEE_ANTORCHA,
    MELEE_HELADO,
    MELEE_PALETA,
    MELEE_PARCA,
    MELEE_SLOT1,
    MELEE_SLOT2,
    MELEE_SLOT3,
    MELEE_SLOT4,
    MELEE_SLOT5,
    MELEE_SLOT6,
    MELEE_SLOT7,
    MELEE_SLOT8,
    MELEE_SLOT9,
    MELEE_SLOT10,
    SUPERBOMB,
    BOMBVIP,
    BANANAVIP,
    MINASMOKEVIP,
    BOMBGOLD,
    IMANGOLD,
    MINAGOLD,
    MISILVIP,
    IMANVIP//ADD KARLOS
}




public class Char_Store : MonoBehaviour 
{
    public Transform CharModel;
    public int ID;
    public Material material;
    public int model;
    public List<GameObject> skins;
    public GameObject faceLow;
    public GameObject faceHight;
    public GameObject bodyLow;
    public GameObject bodyHight;
    public Transform hat;
    public Transform helmet;
    public Transform face;
    public Transform glasses;

    public Transform gloves;
    public Transform hand;

    public Transform spine;
    public KartEntity kart;
    public Animator animator;
    public AudioSource sfx;
    public Melee meleGun;
    public List<GameObject> accesorios;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if(animator==null)
            animator = GetComponentInChildren<Animator>();
        

        sfx = gameObject.AddComponent<AudioSource>();
        sfx.clip = Resources.Load("Sfx/melee") as AudioClip;
        sfx.outputAudioMixerGroup = AudioManager.Instance.GetMixerGroup(AudioManager.MixerTarget.SFX);
        sfx.spatialBlend = 1;
        sfx.maxDistance = 10;
        if(Track.Current)
        {
            foreach(GameObject t in accesorios)
            {
                t.SetActive(false);
            }
        }

        // rotation = chasis.localRotation;
    }

    public void playMele()
    {
        animator.Play("melee", 0, 0);
        if (sfx.isPlaying)
            sfx.Stop();
        sfx.Play();
        Invoke("attackON", .3f);
       // AudioManager.PlayAndFollow("meleeSFX", transform, AudioManager.MixerTarget.SFX);

    }
    public void attackON()
    {
        if (meleGun)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("melee") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .2f)
            {
                meleGun.enabledCollider(true);
                Invoke("attackOFF", .3f);

            }
        }
    }

    public void attackOFF()
    {

       // if (animator.GetCurrentAnimatorStateInfo(0).IsName("melee") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .2f)
        {
            meleGun.enabledCollider(false);
        }
    }
    public GameObject loadPart(DriverDefinition _kd, PlayerD _partID)
    {
        GameObject partChar=Resources.Load("Prefabs/Drivers/Accesories/"+_partID.ClassPart+"/" +_partID.Id.Split("-")[1]) as GameObject;

        //CLog.Log("Prefabs/Drivers/Accesories/" + _partID.Split("-")[1]);

        if (partChar)
        {
            partChar = Instantiate(partChar, Vector3.zero, Quaternion.identity, transform);

        }
        else
        {
            CLog.Log("NO ENCONTRADO: " + "Prefabs/Drivers/Accesories" + _partID.ClassPart + "/" + _partID.Id.Split("-")[1]);
            return null;
        }

        return partChar;

    }
    public void findMelee()
    {
        //CLog.Log("SOY: " + driver.GetChild(0).GetComponent<Char_Store>().hand);
        //CLog.Log("BUSCO: " + driver.GetChild(0).GetComponent<Char_Store>().hand.childCount);
        // CLog.Log("BUSCO: " + driver.GetChild(0).GetComponent<Char_Store>().hand.GetComponentInChildren<Melee>());
        if (hand.childCount > 0)
        {
            if(meleGun = hand.GetComponentInChildren<Melee>())
                meleGun.kartParent = kart;
        }
    }

    Quaternion rotation;
    public void changeCharPart(DriverDefinition _kd, PlayerD _playerD, bool _remove)
    {



        if(_playerD.ClassPart==ClassPart.BODY)
        {
            //CLog.Log("NAME: "+name+" - "+transform.parent+" - "+"Prefabs/Drivers/" + _kd.nameDriver + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/text"+" - "+ "Prefabs/Drivers/" + _kd.name + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/normal");
            material.EnableKeyword("_NORMALMAP");
//            material.EnableKeyword("_METALLICGLOSSMAP");
            material.SetTexture("_MainTex", Resources.Load<Texture2D>("Prefabs/Drivers/" + _kd.nameDriver + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/text"));
            material.SetTexture("_BumpMap", Resources.Load<Texture2D>("Prefabs/Drivers/" + _kd.nameDriver + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/normal"));
            
            if(skins!=null)
            {
                foreach (GameObject t in skins)
                {
                    if(t)t.SetActive(false);                                        
                }
                GameObject _skin=null;
                if (skins.Exists(x => x.name == _playerD.Id.Replace(_kd.Id + "-", "")))
                {
                    (_skin= skins.Find(x => x.name == _playerD.Id.Replace(_kd.Id + "-", ""))).SetActive(true);

                          
                    

                    if (_skin)
                    {
                        Material material2 = Instantiate(material);
                        material2.EnableKeyword("_NORMALMAP");
                        material2.SetTexture("_MainTex", Resources.Load<Texture2D>("Prefabs/Drivers/" + _kd.nameDriver + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/Skin/text"));
                        material2.SetTexture("_BumpMap", Resources.Load<Texture2D>("Prefabs/Drivers/" + _kd.nameDriver + "_Body/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/Skin/normal"));

                        _skin.GetComponent<Renderer>().material = material2;
                    }
                }
            }

            return;
        }
        GameObject test = null;
        Transform _part = null;
        if (!_remove)
        {
            test = loadPart(_kd, _playerD);
            _part = null;
            if (test)
            {
                _part = test.transform;
            }
        }
        else
        {

        }
     //   rotation = chasis.localRotation;

        switch (_playerD.ClassPart)
        {
            case ClassPart.HAT:
                if (!test||_remove)
                {
                    // if (spoilerF) spoilerF.gameObject.SetActive(false);
                  
                    //break;
                }

                removeAll(hat);
                if (!_part) break;
                removeAll(helmet);
                //removeAll(hat);
                _part.parent = hat;
                _part.localPosition = Vector3.zero;
                _part.localRotation = Quaternion.Euler(0,0,0);

                foreach (Transform t in _part)
                    t.gameObject.SetActive(false);
                _part.GetChild(model).gameObject.SetActive(true);

                //if (spoilerF) spoilerF.gameObject.SetActive(false);
                //spoilerF = _part;
                break;
            case ClassPart.HELMET:
                removeAll(helmet);
                if (!_part) break;
                removeAll(face);
                removeAll(hat);
                removeAll(glasses);
                _part.parent = helmet;
                _part.localPosition = Vector3.zero;
                _part.localRotation = Quaternion.Euler(0, 0, 0);
                //if(!model_A)
                {

                    foreach (Transform t in _part)
                        t.gameObject.SetActive(false);
                    switch (model)
                    {
                        case 0:
                        case 1:
                            _part.GetChild(0).gameObject.SetActive(true);
                            break;
                        case 2:
                            _part.GetChild(1).gameObject.SetActive(true);

                            break;
                        case 3:
                            _part.GetChild(2).gameObject.SetActive(true);
                            break;
                        case 4:
                            _part.GetChild(3).gameObject.SetActive(true);
                            break;
                        case 5:
                            _part.GetChild(4).gameObject.SetActive(true);
                            break;
                    }

                }
                //if (spoilerF) spoilerF.gameObject.SetActive(false);
                //spoilerF = _part;
                break;
            case ClassPart.BODY:
                //material.mainTexture=
                //material.norma=


                break;
            case ClassPart.FACE:
                removeAll(face);
                if (!_part) break;
                removeAll(helmet);

                if (_part.childCount > 1)
                {
                    foreach (Transform t in _part)
                        t.gameObject.SetActive(false);
                    _part.GetChild(model).gameObject.SetActive(true);
                }

                _part.parent = face;
                _part.localPosition = Vector3.zero;
                _part.localRotation = Quaternion.Euler(0, 0, 0);

                break;
            case ClassPart.GLASSES:
                

                removeAll(face);
                if (!_part) break;
                removeAll(helmet);

                if (_part.childCount > 1)
                {
                    foreach (Transform t in _part)
                        t.gameObject.SetActive(false);
                    if (model == 5) 
                    {
                        _part.GetChild(0).gameObject.SetActive(true);
                        _part.transform.localScale = new Vector3(.8f, .8f, .8f);
                    }
                    else if(_part.childCount > 2)
                        _part.GetChild((model == 2 || model == 3 || model == 4) ? 2 : 1).gameObject.SetActive(true);//_part.GetChild(model).gameObject.SetActive(true);
                    else
                        _part.GetChild((model == 0 || model == 1) ? 0 : 1).gameObject.SetActive(true);











                    // _part.GetChild(model).gameObject.SetActive(true);
                }

                _part.parent = face;
                _part.localPosition = Vector3.zero;
                _part.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case ClassPart.GLOVES:
                break;
            case ClassPart.HAND:
                if (!test || _remove)
                {
                    // if (spoilerF) spoilerF.gameObject.SetActive(false);
                    if (hand.childCount > 0) Destroy(hand.GetChild(0).gameObject);

                    break;
                }
                removeAll(hand);
                if (hand.childCount > 0)
                {
                    hand.GetChild(0).gameObject.SetActive(false);
                    Destroy(hand.GetChild(0).gameObject);
                }
                _part.parent = hand;
                _part.localPosition = Vector3.zero;
                _part.localRotation = Quaternion.Euler(0, 0, 0);
                findMelee();
                break;
        }

        if (_part)
        {
            foreach(Transform t in _part)
            t.gameObject.layer = 3;
        }
        //CLog.Log("SOY YO: " + _part + " " + _part.gameObject.layer);
    }



    public void removeAll(Transform Node)
    {
        if (Node)
        {
            if (Node.childCount > 0)
            {
                foreach (Transform t in Node)
                    Destroy(t.gameObject);
            }
        }

    }

    public void PruebaDeAnimacion()
    {
        animator.SetTrigger("Festejar");
    }


    public void activePart(Transform _part, int model)
    {
        foreach (Transform t in _part)
            t.gameObject.SetActive(false);
        switch (model)
        {
            case 0:
            case 1:
                _part.GetChild(0).gameObject.SetActive(true);
                break;
            case 2:
                _part.GetChild(1).gameObject.SetActive(true);

                break;
            case 3:
                _part.GetChild(2).gameObject.SetActive(true);
                break;
            case 4:
                _part.GetChild(3).gameObject.SetActive(true);
                break;
            case 5:
                _part.GetChild(4).gameObject.SetActive(true);
                break;
        }
    }

}
