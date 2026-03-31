using FusionExamples.Utility;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    public GameUI hudPrefab;
    public NicknameUI nicknameCanvasPrefab;
    public System.Collections.Generic.List<KartDefinition> kartDefinitions;
    public System.Collections.Generic.List<DriverDefinition> driverDefinitions;
    public System.Collections.Generic.List<TrackDefinition> tracksDefinitions;
    public GameType[] gameTypes;
    public Powerup[] powerups;
    public Powerup noPowerup;
    public System.Collections.Generic.List<Sprite> partsIcons;
    public RuntimeAnimatorController animator;
    //public IconsSubcategory[] CatalogCategory;
    [SerializeField] public Sprite[] ImgAvatar;

    public static ResourceManager Instance => Singleton<ResourceManager>.Instance;

    private void Awake()
    {
            if (instance == null)
        {
            instance = this;
            StartCoroutine(loadDefinitions());
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

    }
    
    System.Collections.IEnumerator loadDefinitions()
        {
        loadImageProfiles();

        kartDefinitions.Clear();
        driverDefinitions.Clear();
        tracksDefinitions.Clear();

        KartDefinition[] kartDefs = Resources.LoadAll<KartDefinition>("Scriptable Objects/Kart Definitions");
        foreach (KartDefinition kd in kartDefs.OrderBy(x => x.Id))
        {
            if (!kd) continue;
            if (kd.prefab) kd.nameKart = kd.prefab.name;
            kartDefinitions.Add(kd);
            CLog.Log("CARGANDO: Kart ID " + kd.Id);
            yield return null;
        }

        DriverDefinition[] driverDefs = Resources.LoadAll<DriverDefinition>("Scriptable Objects/Drivers");
        foreach (DriverDefinition dd in driverDefs.OrderBy(x => x.Id))
        {
            if (!dd) continue;
            if (dd.prefab) dd.nameDriver = dd.prefab.name;
            driverDefinitions.Add(dd);
            yield return null;
        }

        TrackDefinition[] trackDefs = Resources.LoadAll<TrackDefinition>("Scriptable Objects/Track Definitions");
        foreach (TrackDefinition td in trackDefs)
        {
            if (!td || string.IsNullOrEmpty(td.trackSceneName))
            {
                yield return null;
                continue;
            }

            // Keep only tracks that are actually present in Build Settings.
            if (!Application.CanStreamedLevelBeLoaded(td.trackSceneName))
            {
                CLog.LogWarning("TRACK DESCARTADA (no esta en Build Settings): " + td.name + " -> " + td.trackSceneName);
                yield return null;
                continue;
            }

            tracksDefinitions.Add(td);
            yield return null;
        }

        tracksDefinitions.Sort((p1, p2) => p1.index.CompareTo(p2.index));
    }

    /*private async void LoadSomethingAsync()
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>("path");
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip clip = handle.Result;
            // use clip here
        }
        else
        {
            // handle loading error
        }
    }*/
    public void loadImageProfiles()
    {
        ImgAvatar = Resources.LoadAll("Images/avatar/", typeof(Sprite)).Cast<Sprite>().ToArray();
        //Sprite[] currentArray = Resources.LoadAll("Images/avatar/", typeof(Sprite)).Cast<Sprite>().ToArray();
        //var textures = Resources.LoadAll("Images/avatar/", typeof(Texture2D)).Cast<Texture2D>().ToArray();
    }
    public KartDefinition getKart(int _id)
    {
        foreach (KartDefinition kd in kartDefinitions) 
        {
            if (kd.Id == _id)
                return kd;
        }
        CLog.LogError("ERROR ID NO ENCONTRADO: " + _id);
        //return kartDefinitions[0];
        return null;
    }

    public DriverDefinition getChar(int _id)
    {
        foreach (DriverDefinition dd in driverDefinitions)
        {
            if (dd.Id == _id)
                return dd;
        }
        CLog.LogError("ERROR ID NO ENCONTRADO: " + _id);

        //return driverDefinitions[0];//
        return null;
    }


    public Powerup getPowerup(ClassPart _powerUpType)
    {
        foreach (Powerup pu in powerups)
        {
            if (pu._class == _powerUpType)
                return pu;
        }
        CLog.LogError("ERROR ID NO ENCONTRADO: " + _powerUpType);
        return null;
    }

    public int getPowerupIndex(ClassPart _powerUpClass)
    {
        for (int i = 0; i < powerups.Length; i++)
        {
            if (powerups[i]._class == _powerUpClass)
                return i;
        }
        CLog.LogError("ERROR CLASS NO ENCONTRADO: " + _powerUpClass);
        return -2;
    }

    public Sprite getIconPowerUps(ClassPart _class)
    {
        foreach (Powerup pu in powerups)
        {
            if (_class.Equals(pu._class))
                return pu.itemIcon;
        }
        Debug.Log("ERROR SPRITE NO ENCONTRADO: " + _class);
        return null;
    }
    public Sprite getBackIconPowerUps(ClassPart _class)
    {
        foreach (Powerup pu in powerups)
        {
            if (_class.Equals(pu._class))
                return pu.backIconStore;
        }
        Debug.Log("ERROR SPRITE NO ENCONTRADO: " + _class);
        return null;
    }

    public Sprite getIcon(ClassPart _class)
    {
        foreach (Sprite sp in partsIcons)
        {
            if (_class.ToString().Equals(sp.name)) 
                return sp;
        }
        Debug.Log("ERROR SPRITE NO ENCONTRADO: " + _class);
        return null;
    }
    
    private int indexLastrack=0;
    public int getTrackID(GameModes _mode)
    {
        while (true)
        {
            for (int i = indexLastrack; i < tracksDefinitions.Count; i++)
            {
                if (tracksDefinitions[i].mode == _mode)
                {
                    indexLastrack = i + 1;
                    return i;
                }
            }
            indexLastrack = 0;
        }
        return 0;
    }
}


[System.Serializable]
public class IconsSubcategory
{
    [Header("Items por categoria")]
    public string nameCategory;
    public Sprite iconCategory;
    public List<Sprite> cItems;
}
