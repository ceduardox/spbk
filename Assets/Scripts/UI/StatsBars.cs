using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsBars : MonoBehaviour
{
    public UnityEngine.UI.Image speedDown;
    public UnityEngine.UI.Image speedUp;
    public UnityEngine.UI.Image speedReal;
    public UnityEngine.UI.Image accelerationReal;
    public UnityEngine.UI.Image accelerationUp;
    public UnityEngine.UI.Image accelerationDown;
    public UnityEngine.UI.Image turnReal;
    public UnityEngine.UI.Image turnUp;
    public UnityEngine.UI.Image turnDown;
    public TMPro.TextMeshProUGUI level;
    public TMPro.TextMeshProUGUI levelSpeedTxt;
    public TMPro.TextMeshProUGUI levelAccTxt;
    public TMPro.TextMeshProUGUI levelTurnTxt;
    float speedValueReal, accelarationValueReal, turnValueReal;
    float speedValue, accelarationValue, turnValue;
    float speedPreviewValue, accelarationPreviewValue, turnPreviewValue;
    float speedValueInc, accelarationValueInc, turnValueInc;

    bool preview;
    // Start is called before the first frame update
    void Start()
    {/*
        //TEsteo

        yield return new WaitForSeconds(2);
        setStats(0, 0, 0);// .2f, .3f, .2f);
        yield return new WaitForSeconds(2);
        setStats(.2f, .3f, .2f);
        yield return new WaitForSeconds(2);
        setPreviewStats(.4f, .7f, .8f);
        yield return new WaitForSeconds(2);
        setStats(.4f, .7f, .8f);*/
    }
    public void setRealStats(float _speed, float _acc, float _turn)
    {
        speedValueReal = _speed;
        accelarationValueReal = _acc;
        turnValueReal = _turn;
    }
    public void setStats(float _speed, float _acc, float _turn)
    {
        speedValue = _speed;
        accelarationValue = _acc;
        turnValue = _turn;
    }

    public void setNewStats(float _speed, float _acc, float _turn)
    {
        speedValueInc = _speed;
        accelarationValueInc = _acc;
        turnValueInc = _turn;
    }
    public void setPreviewStats(float _speed, float _acc, float _turn)
    {
        speedPreviewValue = _speed;
        accelarationPreviewValue = _acc;
        turnPreviewValue = _turn;
    }
    float speedUpdate = 5;
    // Update is called once per frame
    float levelSpeed, levelAcc, levelTurn;
    float lastLevel, lastLevelSpeed, lastLevelAcc, lastLevelTurn;
    float actualLevel, actualLevelSpeed, actualLevelAcc, actualLevelTurn;
    float multLevel = 1000;
    void LateUpdate()
    {

        //speedValue: Base del kart
        //speedValueReal: Agregado ya adquirido
        //speedValueInc: Incremento preview de la mejora seleccionada 

        setBar(speedReal, speedValue + speedValueReal);
        setBar(speedUp, speedValue + speedValueInc);

        levelSpeed = speedValue + speedValueInc;// speedValue + speedValueReal+ speedValueInc;

        if (speedValue + speedValueReal <= speedValue + speedValueInc)
            setBar(speedDown, speedValue + speedValueReal);
        else
        {
            setBar(speedDown, speedValue + speedValueInc);
            levelSpeed = speedValue + speedValueInc;
        }


        //CLog.Log("values: " + speedValue + " - " + speedValueReal + " - " + speedValueInc+" - "+ levelSpeed);


        setBar(accelerationReal, accelarationValue + accelarationValueReal);
        setBar(accelerationUp, accelarationValue + accelarationValueInc);
        levelAcc = accelarationValue + accelarationValueInc;// speedValue + speedValueReal+ speedValueInc;


        if (accelarationValue + accelarationValueReal <= accelarationValue + accelarationValueInc)
            setBar(accelerationDown, accelarationValue + accelarationValueReal);
        else
        {
            setBar(accelerationDown, accelarationValue + accelarationValueInc);
            levelAcc = accelarationValue + accelarationValueInc;
        }



        setBar(turnReal, turnValue + turnValueReal);
        setBar(turnUp, turnValue + turnValueInc);
        levelTurn = turnValue + turnValueInc;// speedValue + speedValueReal+ speedValueInc;

        if (turnValue + turnValueReal <= turnValue + turnValueInc)
            setBar(turnDown, turnValue + turnValueReal);
        else
        {
            setBar(turnDown, turnValue + turnValueInc);
            levelTurn = turnValue + turnValueInc;

        }

        //CLog.Log("level: " + levelSpeed + levelAcc + levelTurn);


        lastLevelSpeed = levelSpeed * multLevel;
        if (lastLevelSpeed != actualLevelSpeed)
            actualLevelSpeed = Mathf.Lerp(actualLevelSpeed, lastLevelSpeed, Time.deltaTime * speedUpdate);
        levelSpeedTxt.text = "" + (int)actualLevelSpeed;

        lastLevelAcc = levelAcc * multLevel;
        if (lastLevelAcc != actualLevelAcc)
            actualLevelAcc = Mathf.Lerp(actualLevelAcc, lastLevelAcc, Time.deltaTime * speedUpdate);
        levelAccTxt.text = "" + (int)actualLevelAcc;

        lastLevelTurn = levelTurn * multLevel;
        if (lastLevelTurn != actualLevelTurn)
            actualLevelTurn = Mathf.Lerp(actualLevelTurn, lastLevelTurn, Time.deltaTime * speedUpdate);
        levelTurnTxt.text = "" + (int)actualLevelTurn;


        lastLevel = (levelSpeed + levelAcc + levelTurn) * multLevel;
        if (lastLevel != actualLevel)
            actualLevel = Mathf.Lerp(actualLevel, lastLevel, Time.deltaTime * speedUpdate);
        level.text = "NIVEL: " + (int)actualLevel;


    }

    void setLevel( )
    {

    }


    void setBar(UnityEngine.UI.Image _bar, float _value)
    { 
    _bar.fillAmount = Mathf.Lerp(_bar.fillAmount, _value, Time.deltaTime* speedUpdate);// += .01f;
        
    }
}
