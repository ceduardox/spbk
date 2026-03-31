using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateUI : MonoBehaviour
{
    public static TranslateUI instance;
    public TextAsset UI_lang;
    public TextAsset STORE_lang;
    public static string langActual;
    static Dictionary<string, Dictionary<string, string>> UI_langMaster;
    static Dictionary<string, Dictionary<string, string>> STORE_langMaster;
    static List<TranslateUI_ITEM> items;
    private void Awake()
    {
        if (instance) return;
        instance = this;
        items = new List<TranslateUI_ITEM>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        UI_langMaster = StartLangs(UI_lang);
        STORE_langMaster = StartLangs(STORE_lang);
        langActual = "es";

        CLog.Log("TEST: " + getStringUI(UI_CODE.Hielo_DM_00_Name));
        CLog.Log("TEST: " + getStringUI(UI_CODE.Hielo_DM_00_Desc));
        CLog.Log("TEST: " + getStringStore(ClassPart.MOTOR));
        CLog.Log("TEST: " + getStringStore(""));

        //
    }

    private Dictionary<string, Dictionary<string, string>> StartLangs(TextAsset text)
    {
        string[] registros = text.ToString().Split("\n");

        Dictionary<string, Dictionary<string, string>> managerLang = new Dictionary<string, Dictionary<string, string>>();

        string[] idiomas = registros[0].Split("	");//creo las key de los lenguajes

        for (int j = 1; j < registros.Length; j++)//recorro los registros planos, saltando el encabezado
        {
            string[] registro = registros[j].Split("	");

            for (int i = 1; i < idiomas.Length; i++)
            {
                // CLog.Log("ADD: " + idiomas[i]+" - "+ registro[0] + " - " + registro[i]);
                if (managerLang.ContainsKey(idiomas[i]))
                    managerLang[idiomas[i]].Add(registro[0], registro[i]);
                else
                    managerLang.Add(idiomas[i], new Dictionary<string, string>() { { registro[0], registro[i] } });

            }

        }
        return managerLang;

        //langManager.Add
        /*foreach (var t in langManager)
        {
            //var u = t;
            CLog.Log("Lang: " + t.Key);
            foreach (var t2 in t.Value)
                CLog.Log(t2.Key+" - "+t2.Value);
        }*/

    }

    public void changeLang()
    {
        updateAllUI();
    }
    public static string getStringUI(UI_CODE _code)
    {
        return getString(_code.ToString(), UI_langMaster);
    }
    public static string getStringStore(string _code)
    {
        return getString(_code, STORE_langMaster);
    }
    public static string getStringStore(ClassPart _code)
    {
        return getString(_code.ToString(), STORE_langMaster);
    }
    private static string getString(string _code, Dictionary<string, Dictionary<string, string>> _langMaster)
    {
        if (_langMaster.ContainsKey(langActual))
        {
            if (_langMaster[langActual].ContainsKey(_code))
            {
                return _langMaster[langActual][_code].Replace("\\n", "\n");
            }
            else
            {
                CLog.Log("ERROR - El codigo: " + _code + " no existe");
            }
        }
        else
        {
            CLog.LogError("El lenguaje: " + langActual + " no existe");
        }
        return "report Error " + _code;
    }

    public static void addItem(TranslateUI_ITEM _item)
    {
        items.Add(_item);
    }
    public void updateAllUI()
    {
        foreach (TranslateUI_ITEM _item in items)
            _item.setText();
    }
}
public enum UI_CODE
{
    POPUP_MSJ_BETCONFIRM,
    POPUP_MSJ_NOTEL,
    POPUP_MSJ_BUYTEL,
    POPUP_MSJ_BUYITEM,
    POPUP_MSJ_ERRORBUY,
    POPUP_MSJ_NEEDLEVEL,
    POPUP_MSJ_CONFIRMNEXTBETRACE,
    POPUP_MSJ_EXIT,
    POPUP_MSJ_09,
    POPUP_MSJ_10,
    POPUP_MSJ_11,
    POPUP_MSJ_12,
    POPUP_MSJ_13,
    POPUP_MSJ_14,
    POPUP_MSJ_15,
    POPUP_MSJ_16,
    POPUP_MSJ_17,
    POPUP_MSJ_18,
    POPUP_MSJ_19,
    POPUP_MSJ_20,
    POPUP_TITLE_WARNING,
    POPUP_TITLE_ERROR,
    POPUP_TITLE_QUESTION,
    POPUP_MSJ_ERRORNET,
    POPUP_TITLE_EXIT,
    POPUP_TITLE_05,
    POPUP_TITLE_06,
    POPUP_TITLE_07,
    POPUP_TITLE_08,
    POPUP_TITLE_09,
    POPUP_TITLE_OK,
    BTN_KART,
    BTN_CHARS,
    BTN_UPGRADE_KART,
    BTN_UPGRADE_CHAR,
    POPUP_CLN_01,
    POPUP_CLN_02,
    POPUP_CLN_03,
    POPUP_CLN_04,
    POPUP_CLN_05,
    POPUP_CLN_06,
    POPUP_FND_01,
    POPUP_FND_02,
    EDITPROFILE_BTN,
    //GENERALES
    _ACCEPT,
    _CANCEL,
    _BACK,
    _WRITE,
    _USERNAME,
    //GENERALES
    INI_TITTLEPANEL,
    INI_LOGIN,
    INI_REGISTER,
    INI_AUTOLOG,
    INI_QUIT,
    REG_TITTLEPANEL,
    REG_BACK,
    REG_CONFIRM,
    REG_QUIT,
    REG_REGION,
    MAIN_START,
    MAIN_EVENTS,
    MAIN_CLANS,
    MAIN_SKILLS,
    MAIN_KARTS,
    MAIN_CHARS,
    MAIN_OPTIONS,
    MAIN_QUIT,
    FRD_BTNFRIENDS,
    FRD_BTNMESSAGE,
    FRD_BTNREQUEST,
    FRD_BACK,
    FRD_TTLLISTF,
    FRD_TTLMSG,
    FRD_TTLREQ,
    FRD_IMPFIND,
    FRD_AVATAR,
    FRD_ADDFRD,
    PWU_BNTINV,
    PWU_BTNSTORE,
    KRT_ITEMS,
    KRT_BUY,
    KRT_STS_VEL,
    KRT_STS_ACL,
    KRT_STS_TRQ,
    GMD_MODE,
    GMD_CHLLG,
    GMD_DEATH,
    GMD_ADVT,
    GMD_CMNG,
    SRV_JOIN,
    SRV_TITTLE,
    SRV_NAMESRV,
    SRV_MODE,
    SRV_BET,
    SRV_PLAYERS,
    SRV_PING,
    SRV_BETDET,
    SRV_PLAYERSDET,
    SRV_LAPSDET,
    SRV_STATUSDET,
    LBY_ROOM,
    LBY_BTNPWR,
    LBY_BTNKART,
    LBY_BTNCHAR,
    LBY_BTNREADY,
    LBY_BTNBET,
    LBY_LPS,
    CLN_TITTLE,
    CLN_TABMYCLAN,
    CLN_TABEXPLORER,
    CLN_TABTOPCLAN,
    CLN_BTN_SENDINV,
    CLN_BTN_REQUEST,
    CLN_BTN_MESSAGE,
    CLN_BTN_LEAVE,
    CLN_PLAYER,
    CLN_RANGE,
    CLN_CUPS,
    CLN_LEVEL,
    CLN_TTLCUPS,
    CLN_MEMBERS,
    CLN_CLAN,
    CLN_SENDREQ,
    CLN_NOCLNTITTLE,
    CLN_NEWCLAN,
    CLN_INVITATIONS,
    CLN_MSJNEWCLAN,
    CLN_VIEWPROF,
    CLN_ADDFRD,
    CLN_KICK,
    SMPU_POP_01,
    SMPU_POP_02,
    SMPU_POP_03,
    SMPU_POP_04,
    OPT_TITTLE,
    OPT_BTN_CTRLS,
    OPT_BTN_SOUND,
    OPT_BTN_GRPCS,
    OPT_BTN_TERMS,
    OPT_CMOV,
    OPT_Cacel,
    OPT_Crght,
    OPT_Clft,
    OPT_Cback,
    OPT_Cdrft,
    OPT_CGAME,
    OPT_Cpp,
    OPT_Cps,
    OPT_Cppr,
    OPT_Cpaus,
    OPT_Cbackcam,
    OPT_SVOL,
    OPT_Svg,
    OPT_Svm,
    OPT_Sve,
    OPT_Svui,
    OPT_GGME,
    OPT_Gbr,
    OPT_Glang,
    OPT_Gqlty,
    OPT_Gfs,
    OPT_Gptr,
    OPT_Thelp,
    OPT_Ttrms,
    OPT_Tpolicies,
    HUD_RESUME,
    HUD_OPTIONS,
    HUD_LEAVE,
    HUD_WAIT,
    HUD_RESULTS,
    HUD_CHOINCE,
    ////////////////////////////////////////////////////////TRACKS
    Desierto_Location,
    Desierto_Race_00_Name,
    Desierto_Race_00_Desc,
    Desierto_DM_00_Name,
    Desierto_DM_00_Desc,
    Cementerio_Location,
    Cementerio_Race_00_Name,
    Cementerio_Race_00_Desc,
    Cementerio_DM_00_Name,
    Cementerio_DM_00_Desc,
    Hielo_Location,
    Hielo_Race_00_Name,
    Hielo_Race_00_Desc,
    Hielo_DM_00_Name,
    Hielo_DM_00_Desc,
    Egipto_Location,
    Egipto_Race_00_Name,
    Egipto_Race_00_Desc,
    Egipto_DM_00_Name,
    Egipto_DM_00_Desc,
    //--------------------------PROFILE-------------
    MAIL_TXT,
    ADDFRIEND_BTN,
    SENDMSJ_BTN,
    TOTAL_RACES_TXT,
    TOTAL_WIN_RACES_TXT,
    NIVEL_TXT,
    REGION_TXT,
    KRT_STS_BUY,//KART STORE UI
    KRT_STS_EQUIP,//KART STORE UI
    TNL_TITTLE,
    TNL_CHEST,
    POPUP_MSJ_TELOUT2,
    POPUP_MSJ_UPDATE,
    POPUP_MSJ_FAILSTART,
    POPUP_MSJ_MULTILOG,
    CLN_RULES,
    CLN_INFO,
    Desierto_DM_01_Name,
    Desierto_DM_01_Desc,
    CLOSE_SESSION_BTN,
    HUD_TIMER,
    TOOLTIP_01,
    TOOLTIP_02,
    TOOLTIP_03,
    TRN_MSJ_01,
    TRN_MSJ_02,
    TRN_MSJ_03,
    TRN_UI_TRNMT,
    TRN_UI_RULES,
    TRN_UI_SCBT,
    TRN_UI_RWDS,
    TRN_UI_TIME,
    TRN_UI_DAYS,
    TRN_UI_HRS,
    TRN_UI_MIN,
    TRN_UI_SEG,
    TRN_UI_END,
    TRN_UI_BEG,
    TRN_UI_ENDED,
    TRN_UI_SUBS_1,
    TRN_UI_SUBS_0,
    POPUP_MSJ_BUY,
    LDNG_MSJ_01,
    EVT_MSJ_01,
    EVT_MSJ_02,
    TRN_UI_NT,
    TRN_UI_DT,
    TRN_UI_WN,
    POPUP_TITLE_10,
    TRK_RACE_JG_01_NAME,
    TRK_RACE_JG_01_DESC,
    TRK_RACE_JG_02_NAME,
    TRK_RACE_JG_02_DESC,
    TRK_RACE_DG_01_NAME,
    TRK_RACE_DG_01_DESC,
    TRK_DTM_JG_01_NAME,
    TRK_DTM_JG_01_DESC,
    TRK_DTM_JG_02_NAME,
    TRK_DTM_JG_02_DESC,
    TRK_DTM_DG_01_NAME,
    TRK_DTM_DG_01_DESC,
    STP_TUT_00,
    STP_TUT_01,
    STP_TUT_02,
    STP_TUT_03,
    STP_TUT_04,
    STP_TUT_05,
    STP_TUT_06,
    STP_TUT_07,
    STP_TUT_F,
    TERMS,
    MSJ_CONSEJO_01,
    MSJ_CONSEJO_02,
    MSJ_CONSEJO_03,
    MSJ_CONSEJO_04,
    MSJ_CONSEJO_05,
    MSJ_CONSEJO_06,
    MSJ_CONSEJO_07,
    MSJ_CONSEJO_08,
    MSJ_CONSEJO_09,
    MSJ_CONSEJO_10,
    //--- PROMO POPUP
    POP_PROMO_title,
    POP_PROMO_SUBTITLE,
    POP_PROMO_COMERTIALS,
    POP_PROMO_REMAINNINGTIME,
    POP_GRATS_01,
    POP_GRATS_02,
    POP_GRATS_03,
    POP_GRATS_04,
    UI_EDICION_LIMITADA,
    OPT_accelerometro,
    OPT_accelerometroSensibilidad,
    OPT_accelerometroDeadZone,
    //ADDED 06/03/2023
    LG_PAG,
    LG_REG1,
    LG_REG2,
    LG_INW,
    LG_PSW,
    LG_BNB,
    RG_INPT1,
    RG_INPT2,
    RG_INPT3,
    RG_INPT4,
    RG_INPT5,
    RG_INPT6,
    RG_INPT7,
    RG_INPT8,
    MAIN_SCL,
    FRND_FF,
    CL_DESC,
    CL_LJ,
    CL_CC,
    CL_ADD,
    CL_INV,
    QUIT,
    POP_UP_SELECT_REGION,
    BUY_CHAR,
    BUY_KART
}