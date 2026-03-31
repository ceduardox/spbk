using UnityEngine;
using UnityEngine.UI;

public class flowEffect : MonoBehaviour
{
    Material mat;
    public float Crecimiento = 0;

    private void Start()
    {
        mat = GetComponent<Image>().material;
        mat.SetFloat("_ShineLocation", 0);
        InvokeRepeating("Corrutina", 0f, 0.02f);
    }
    void Corrutina()
    {
        Crecimiento = Crecimiento + 0.02f;
        mat.SetFloat("_ShineLocation", Crecimiento);

        if (Crecimiento >= 1)
        {
            Crecimiento = 0;
            CancelInvoke("Corrutina");
            InvokeRepeating("Corrutina", 3f, 0.02f);
        }
    }


}
