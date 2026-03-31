using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSize : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    [Header("Mobile Size")]
    [SerializeField] private int widthMobile;
    [SerializeField] private int heightMobile;
    [Header("Desktop Size")]
    [SerializeField] private int widthDesktop;
    [SerializeField] private int heightDesktop;

    private void Awake()
    {
        Camera cam = this.GetComponent<Camera>();
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            renderTexture.Release();
            renderTexture.width = widthMobile;
            renderTexture.height = heightMobile;
            renderTexture.Create();
        }
        else
        {
            renderTexture.Release();
            renderTexture.width = widthDesktop;
            renderTexture.height = heightDesktop;
            renderTexture.Create();
        }
    }
}
