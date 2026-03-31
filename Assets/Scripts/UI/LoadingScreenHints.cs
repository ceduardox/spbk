using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingScreenHints : MonoBehaviour
{
    string[] hints=new string[10];
    public Text printedHint;

    private void OnEnable()
    {
        //
        hints[0] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_01);
        hints[1] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_02);
        hints[2] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_03);
        hints[3] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_04);
        hints[4] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_05);
        hints[5] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_06);
        hints[6] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_07);
        hints[7] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_08);
        hints[8] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_09);
        hints[9] = TranslateUI.getStringUI(UI_CODE.MSJ_CONSEJO_10);
        PrintRandomHint();
    }

	private void PrintRandomHint()
    {
        printedHint.text = hints[Random.Range(0, hints.Length)];
    }
}
