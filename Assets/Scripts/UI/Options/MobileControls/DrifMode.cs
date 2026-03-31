using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenButton component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A button that is visually represented on-screen and triggered by touch or other pointer
    /// input.
    /// </summary>
    //[AddComponentMenu("Input/On-Screen Button")]

    //[HelpURL(InputSystem.kDocUrl + "/manual/OnScreen.html#on-screen-buttons")]
    public class DrifMode : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject LarrowBtn;
        public GameObject RarrowBtn;
        public static GameObject Lbtn;
        public static GameObject Rbtn;
        //public bool isDrift;
        [Header("Mode double Tap Button")]
        public static bool modetap=true;
        private int tap;
        public static float interval = 0.5f;
        public bool readyForTap = true;
        [Space]
        [Header("Mode Helded Button")]
        public static bool modeheld;
        public static float timeMouseDown = 0.5f;
        /// </summary>
        private void Awake()
        {
            Lbtn = LarrowBtn;
            Rbtn = RarrowBtn;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
            SendValueToControl(0.0f);
            readyForTap = true;
            //isDrift = false;
        }
        //public void _modeHeld()
        //{
        //    modetap = false;
        //    modeheld = true;
        //}
        public void OnPointerDown(PointerEventData eventData)
        {
            if (modetap)
            {
                tap++;
                if (tap == 1)
                {
                    StartCoroutine(doubleTapInterval());
                }
                else if (tap > 1 && readyForTap)
                {
                    //isDrift = true;
                    tap = 0;
                    SendValueToControl(1.0f);
                    readyForTap = false;
                }
            }
            if (modeheld)
            {
                //readyForTap = false;
                StartCoroutine(heldedDelay());
            }

        }
        IEnumerator doubleTapInterval()
        {
            yield return new WaitForSeconds(interval);
            tap = 0;
        }
        IEnumerator heldedDelay()
        {
            yield return new WaitForSeconds(timeMouseDown);
            
            SendValueToControl(1.0f);
        }
        ////TODO: pressure support
        /*
        /// <summary>
        /// If true, the button's value is driven from the pressure value of touch or pen input.
        /// </summary>
        /// <remarks>
        /// This essentially allows having trigger-like buttons as on-screen controls.
        /// </remarks>
        [SerializeField] private bool m_UsePressure;
        */

        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}