using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScrollingRawImage : MonoBehaviour
{
    private RawImage rawImage;
    public float xSpeed, ySpeed;
    private float xVal, yVal;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        xVal += Time.deltaTime * xSpeed;
        yVal += Time.deltaTime * ySpeed;
        rawImage.uvRect = new Rect(xVal, yVal, rawImage.uvRect.width, rawImage.uvRect.height);
    }
    public void setNewValue(float _yval, bool sw)
    {
        if (sw)
        {
            StartCoroutine(headerTransition(_yval));
        }
        else
        {
            xSpeed = 0;
            ySpeed = _yval;
        }
    }
    IEnumerator headerTransition(float b)
    {
        float a = 5;
        float startTime = 0;
        float current;
        while (startTime < 1)
        {
            current = Mathf.Lerp(a, b, startTime / 1);
            startTime += Time.deltaTime;
            ySpeed = current;
            yield return null;
        }
        current = b;
    }
}
