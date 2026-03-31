
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenButton component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A button that is visually represented on-screen and triggered by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu("Input/On-Screen Button")]
   // [HelpURL(InputSystem.kDocUrl + "/manual/OnScreen.html#on-screen-buttons")]
    public class OnScreenButtonTurn : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        
        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(0.0f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SendValueToControl(1.0f);
        }


        public bool isLeft;

        
        private void LateUpdate()
        {
            
            CLog.Log(Input.acceleration+" "+ Accelerometro.deadZone+" "+ Accelerometro.sensibilidad+" "+ Accelerometro.deadZone);
            if (!Accelerometro.isAcelerometro) return;
            if (Input.acceleration.x<-Accelerometro.deadZone*.25f && isLeft)
            {
                SendValueToControl(-Input.acceleration.x* Accelerometro.sensibilidad*5);

            }
            else if (Input.acceleration.x > Accelerometro.deadZone * .25f && !isLeft)
            {
                SendValueToControl(Input.acceleration.x* Accelerometro.sensibilidad*5);
            }
            else SendValueToControl(0.0f);

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

