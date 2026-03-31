using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;





public class Kart_Store : KartComponent
{
    public Transform KartModel;
    public int ID;


    public Transform driver;
    /// <summary>
    /// Exhaust escape General
    /// </summary>
    public Transform exhaust;
    public Transform exhaust_L;
    public Transform exhaust_R;

    public Transform spoilerF;
    public Transform spoilerM;
    public Transform spoilerB;

    public Transform motor;

    public Transform lightF;
    public Transform lightB;

    public Transform steertingWheel;
    public Transform gear;

    public Transform chair;

    public Transform frontF;
    public Transform chasis;
    public Transform antenna;

    public Transform tireF_L;
    public Transform tireF_R;
    public Transform tireB_L;
    public Transform tireB_R;

    public Light lightlight;

    public List<PlayerD> listUpgrade = new List<PlayerD>();
    public List<Kart_Parts> parts = new List<Kart_Parts>();

    private void Awake()
    {
        lightlight.gameObject.SetActive(false);



    }
    // Start is called before the first frame update
    void Start()
    {
        //CLog.Log("SOY EL KART:::"+name);
        rotation = chasis.localRotation;
        lightlight.transform.localPosition = new Vector3(0, 0.45f, 0.4f); 
        StartCoroutine(setLight());

    }

    IEnumerator setLight()
    {
        while (true)
        {
            if (Track.Current == null)
            {
                yield return new WaitForSeconds(.1f);
                continue;
            }

            try
            {
                if (Track.Current.Object != null &&
                    Track.Current.Object.IsValid &&
                    Track.Current.lightIndex != -1)
                {
                    lightlight.gameObject.SetActive(Track.Current.lightIndex == 2);
                    yield break;
                }
            }
            catch (System.InvalidOperationException)
            {
                // Track state can be not spawned yet for a few frames.
            }

            yield return new WaitForSeconds(.1f);
        }
    }
    public GameObject loadPart(KartDefinition _kd, PlayerD _partID)
    {
        GameObject partKart;





        if (_partID.ClassPart == ClassPart.DRIVER)
        {
            //CLog.Log("SPAWN DRIVER: " + _partID.Id);
            
            partKart = Instantiate(ResourceManager.Instance.getChar(int.Parse(_partID.Id)).prefab.gameObject);// Resources.Load("Prefabs/Drivers/" + ResourceManager.Instance.getChar(int.Parse(_partID.Id)).name + "_Parts/" + _partID.Id.Replace(_kd.Id + "-", "")) as GameObject;

            Transform tmp = partKart.transform.Find("Joints.002");
            if (tmp) tmp.name = "Joints";
            else
            {
                tmp = partKart.transform.Find("Joints.002");
                if (tmp) tmp.name = "Joints";
            }
            Animator anim = partKart.GetComponentInChildren<Animator>();
            anim.runtimeAnimatorController = ResourceManager.instance.animator;
            anim.avatar = null;

            CLog.Log("Estoy cargando el driver " + ResourceManager.Instance.getChar(int.Parse(_partID.Id)).prefab.gameObject);
        }
        else
        {
            partKart =_partID.ClassPart == ClassPart.ANTENNA?
                Resources.Load("Prefabs/KartPrefabs/Accesorios/"+ _partID.ClassPart+"/"+ _partID.Id.Replace(ClassPart.ALL_KARTS + "-", "")) as GameObject:
                Resources.Load("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + _partID.Id.Replace(_kd.Id + "-", "")) as GameObject;
           // changeSfx(_partID.Id);
            //CLog.Log("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + _partID.Replace(_kd.Id + "-", ""));

            if (partKart)
            {
                
                partKart = Instantiate(partKart, Vector3.zero, Quaternion.identity, transform);

            }
            else
            {
                if (_partID.ClassPart == ClassPart.ANTENNA)//no existe, debe ser una bandera
                {
                    GameObject flag = Resources.Load("Prefabs/KartPrefabs/Accesorios/FLAGS/Flag_00"/* + _partID.ClassPart + "/" + _partID.Id.Replace(ClassPart.ALL_KARTS + "-", "")*/) as GameObject;
                    flag= Instantiate(flag, Vector3.zero, Quaternion.identity, transform);
    
                    if (flag.GetComponent<FlagLoad>().loadFlag(_partID.Id))
                        return flag;
                    
                    
                }
                
                {
                    CLog.Log("NO ENCONTRADO " + _kd.nameKart + "_Parts/" + _partID.Id.Replace(_kd.Id + "-", ""));
                    CLog.Log("NO ENCONTRADO " + "Prefabs/KartPrefabs/Accesorios/" + _partID.ClassPart + "/" + _partID.Id.Replace(_partID.ClassPart + "-", ""));
                    return null;
                }
            }
        }
        return partKart;

    }
    bool firsTime = true;
    Quaternion rotation;
    public Material material;
    public Char_Store changePart(KartDefinition _kd, PlayerD _playerD, bool _remove)
    {


        if (_playerD.ClassPart == ClassPart.PAINT)
        {
            //CLog.Log("PATHS: NAME: " + "Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/normal" + " ----- "+ "Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/mat");
            CLog.Log("+ " + "Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/mat");
            material = Resources.Load<Material>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/mat");
            if (material)
            {
                setPartsMaterial();
            }
            else
            {
                material = Resources.Load<Material>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/mat");
                material.EnableKeyword("_NORMALMAP");
                material.EnableKeyword("_METALLICGLOSSMAP");
                Texture2D textTMP;
                material.SetTexture("_MainTex", Resources.Load<Texture2D>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/text"));

                textTMP = textTMP = Resources.Load<Texture2D>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/normal");
                if (textTMP == null)
                    textTMP = textTMP = Resources.Load<Texture2D>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/normal");
                material.SetTexture("_BumpMap", textTMP);
                textTMP = Resources.Load<Texture2D>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/" + _playerD.Id.Replace(_kd.Id + "-", "") + "/metallic");
                if (textTMP == null)
                    textTMP = Resources.Load<Texture2D>("Prefabs/KartPrefabs/" + _kd.nameKart + "_Parts/" + ClassPart.PAINT + "/metallic");
                material.SetTexture("_MetallicGlossMap", textTMP);
                setPartsMaterial();
            }
            return null;
        }


        if (checkParts(_playerD))
        {
            GameObject test = loadPart(_kd, _playerD);
            Transform oldPart = null;
            Transform _part = null;
            if (test)
            {
                _part = test.transform;
            }
            rotation = chasis.localRotation;

            //CLog.Log("ESTO ES: " + _part.name);

            switch (_playerD.ClassPart)
            {
                case ClassPart.DRIVER:
                    if(driver.childCount>0)
                    {
                        Destroy(driver.GetChild(0).gameObject);
                    }
                    _part.parent = driver;
                    _part.transform.localPosition = Vector3.zero;
                    _part.transform.localScale= Vector3.one*.9f;
                    _part.localRotation = Quaternion.identity;
                    _part.SetAsFirstSibling();
                    GetComponent<KartController>().spine = _part.GetComponent<Char_Store>().spine;
                    _part.GetComponent<Char_Store>().kart=GetComponent<KartEntity>();

                    break;
                case ClassPart.ANTENNA:

                    if (!test)
                    {
                        if (antenna.GetChild(0).gameObject) antenna.GetChild(0).gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = antenna;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = Quaternion.identity;
                    if (antenna.GetChild(0).gameObject) antenna.GetChild(0).gameObject.gameObject.SetActive(false);
                    oldPart = antenna.GetChild(0);
                    //spoilerB = _part;
                    break;




                    if (_remove)
                    {
                        Destroy(antenna.GetChild(0).gameObject);
                    }
                    else
                    {
                        if (antenna.childCount > 0)
                        {
                            if (!test)
                            {
                                antenna.GetChild(0).gameObject.SetActive(false);
                                break;
                            }
                            Destroy(antenna.GetChild(0).gameObject);
                            _part.parent = antenna;
                            _part.localRotation = Quaternion.Euler(0, 0, 0);
                            _part.localPosition = Vector3.zero;
                        }
                    }
                    break;
                case ClassPart.SPOILER_F:
                    if (!test)
                    {
                        if (spoilerF) spoilerF.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (spoilerF) spoilerF.gameObject.SetActive(false);
                    oldPart = spoilerF;
                    spoilerF = _part;

                    break;
                case ClassPart.SPOILER_M:
                    if (!test)
                    {
                        if (spoilerM) spoilerM.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (spoilerM) spoilerM.gameObject.SetActive(false);
                    oldPart = spoilerM;
                    spoilerM = _part;

                    break;
                case ClassPart.SPOILER_B:
                    if (!test)
                    {
                        if (spoilerB) spoilerB.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (spoilerB) spoilerB.gameObject.SetActive(false);
                    oldPart = spoilerB;
                    spoilerB = _part;

                    break;
                case ClassPart.MOTOR:
                    if (!test)
                    {
                        if (motor) motor.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (motor) motor.gameObject.SetActive(false);
                    oldPart = motor;
                    motor = _part;
                    break;
                case ClassPart.EXHAUST:
                    if (!test)
                    {
                        if (exhaust) exhaust.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = exhaust.parent;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = exhaust.localRotation;
                    exhaust.gameObject.SetActive(false);
                    oldPart = exhaust;
                    exhaust = _part;
                    exhaust.GetComponent<Kart_Parts>().StartExhaust();
                    exhaust_L.parent = exhaust.GetComponent<Kart_Parts>().exhaust_L;
                    exhaust_L.localRotation = Quaternion.identity;
                    exhaust_R.parent = exhaust.GetComponent<Kart_Parts>().exhaust_R; 
                    exhaust_R.localRotation = Quaternion.identity;
                    exhaust_L.localPosition = exhaust_R.localPosition = Vector3.zero;
                    exhaust_R.GetChild(1).Find("Boost Particle (2)").localScale = exhaust_R.GetChild(1).Find("Boost Particle (3)").localScale =
                    exhaust_L.GetChild(1).Find("Boost Particle (2)").localScale = exhaust_L.GetChild(1).Find("Boost Particle (3)").localScale = Vector3.one*2;

                    //return;
                    break;
                case ClassPart.LIGHT_F:
                    if (!test)
                    {
                        if (lightF) lightF.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (lightF) lightF.gameObject.SetActive(false);
                    oldPart = lightF;
                    lightF = _part;
                    break;
                case ClassPart.TIRES:
                    for (int i = 0; i < 4; i++)
                    {
                        Transform _partTMP = _part.GetChild(0);
                        Transform partTMP = getSetTire(_partTMP, null);

                        //CLog.Log("VERIFICO: " + _partTMP.name+" - POR: "+partTMP);
                        _partTMP.parent = partTMP.parent;
                        _partTMP.localPosition = partTMP.localPosition;
                        _partTMP.localRotation = partTMP.localRotation;
                        partTMP.gameObject.SetActive(false);
                        oldPart = partTMP;
                        getSetTire(_partTMP, _partTMP);
                        _partTMP.gameObject.layer = 3;
                    }
                    break;
                case ClassPart.STEERT_WHEEL:
                    Destroy(_part.gameObject);
                    break;
                case ClassPart.GEAR:
                    Destroy(_part.gameObject);
                    break;
                case ClassPart.CHASIS:
                    if (!test)
                    {
                        if (chasis) chasis.gameObject.SetActive(false);
                        break;
                    }
                    _part.parent = KartModel;
                    _part.localPosition = Vector3.zero;
                    _part.localRotation = rotation;
                    if (chasis) chasis.gameObject.SetActive(false);
                    oldPart = chasis;
                    chasis = _part;
                    //Destroy(_part.gameObject);
                    break;
                case ClassPart.CHAIR:
                    Destroy(_part.gameObject);
                    break;
                case ClassPart.FRONT_F:
                    Destroy(_part.gameObject);
                    break;
            }
            if (_part) _part.gameObject.layer = 3;

            if (oldPart)
                Destroy(oldPart.gameObject);

            
            setPartsMaterial();
            /*parts = new List<Kart_Parts>();
            parts.Add(spoilerB.GetComponent<Kart_Parts>()); 
            parts.Add(spoilerM.GetComponent<Kart_Parts>());
            parts.Add(spoilerF.GetComponent<Kart_Parts>());
            parts.Add( antenna.GetChild(antenna.childCount>1?1:0).GetComponent<Kart_Parts>());
            parts.Add(chair.GetComponent<Kart_Parts>());
            if (lightF)
                parts.Add(lightF.GetComponent<Kart_Parts>());
            if (lightB)
                parts.Add(lightB.GetComponent<Kart_Parts>()); 
            */

            if (_playerD.ClassPart == ClassPart.DRIVER)  
                return _part.GetComponent<Char_Store>();
            else return null;  

        }
        else return null;

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

    void setLayer(GameObject _gameObject, int _layer)
    {
        _gameObject.layer = _layer;
        foreach (Transform child in _gameObject.transform)
        {

            setLayer(child.gameObject, _layer);
        }

    }

    void setPartsMaterial()
    {
    setMaterial(exhaust); 
    //setMaterial(exhaust_L);
    //setMaterial(exhaust_R);

    setMaterial(spoilerF);
    setMaterial(spoilerM);
    setMaterial(spoilerB);

    setMaterial(motor);

    setMaterial(lightF);
    setMaterial(lightB);

    setMaterial(steertingWheel);
    setMaterial(gear);

    setMaterial(chair);

    setMaterial(frontF);
    setMaterial(chasis);
}
    void setMaterial(Transform _part)
    {
        if (_part == null || material == null) return;
        Renderer renderPart;
        if (renderPart = _part.GetComponent<Renderer>())
            renderPart.material = material;

        if (_part.childCount > 0)
        {
            if (renderPart = _part.GetChild(0).GetComponent<Renderer>())
                renderPart.material = material;
        }
    }
    public void changeSfx(string _sfx)
    {
        KartAudio ka = GetBehaviour<KartEntity>().Audio;
        AudioClip ac = Resources.Load<AudioClip>("Prefabs/KartPrefabs/Accesorios/Exhaust_Sfx/" + _sfx + "/Start");
        if (ac)
        {
            ka.StartSound.clip = ac;
            ka.StartSound.Play();
        }
        ac = Resources.Load<AudioClip>("Prefabs/KartPrefabs/Accesorios/Exhaust_Sfx/" + _sfx + "/Idle");
        if (ac)
        {
            ka.IdleSound.clip = ac;
            ka.IdleSound.Play();
        }
        ac = Resources.Load<AudioClip>("Prefabs/KartPrefabs/Accesorios/Exhaust_Sfx/" + _sfx + "/Running");
        if (ac)
        {
            ka.RunningSound.clip = ac;
            ka.RunningSound.Play();
            //ka.RunningSound.volume * 1.1f;
        }
        ac = Resources.Load<AudioClip>("Prefabs/KartPrefabs/Accesorios/Exhaust_Sfx/" + _sfx + "/Reverse");
        if (ac)
        {
            ka.ReverseSound.clip = ac;
            ka.ReverseSound.Play();
        }

    }


    public void setDriver(Transform _part)
    {
        if (firsTime)
        {
            driver.parent = driver.parent.parent.parent;
            firsTime = false;
        }
        removeAll(driver);
        _part.parent = driver;

        driver.localPosition = new Vector3(0, 0, 0);// Vector3.zero;
        _part.localRotation = Quaternion.Euler(0, 10, 0);
        _part.localPosition = new Vector3(.8f, 0, 0);// Vector3.zero;



        setLayer(driver.gameObject, 3);
    }
    public Transform getSetTire(Transform _partIn, Transform _partSet)
    {
        if (_partIn.name.Contains("B.L"))
        {
            if (_partSet) tireB_L = _partSet;
            return tireB_L;
        }
        else if (_partIn.name.Contains("B.R"))
        {
            if (_partSet) tireB_R = _partSet;
            return tireB_R;
        }
        else if (_partIn.name.Contains("F.L"))
        {
            if (_partSet) tireF_L = _partSet;
            return tireF_L;
        }
        else 
        {
            if (_partSet) tireF_R = _partSet;
            return tireF_R;
        }

    }
    // Update is called once per framess
private bool checkParts(PlayerD _playerD)
    {
        foreach (PlayerD part in listUpgrade)
        {
            if (part.Id.Equals(_playerD.Id))
                return false;
        }

        foreach (PlayerD part in listUpgrade)
        {
            if (part.ClassPart.Equals(_playerD.ClassPart))
            {
                listUpgrade.Remove(part);
                break;
            }
        }

        listUpgrade.Add(_playerD);
        return true;
    }
    
}


