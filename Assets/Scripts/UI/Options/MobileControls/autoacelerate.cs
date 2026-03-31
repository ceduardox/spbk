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
    
    //[HelpURL(InputSystem.kDocUrl + "/manual/OnScreen.html#on-screen-buttons")]
    public class autoacelerate : OnScreenControl, IPointerDownHandler, IPointerUpHandler 
    {
        public bool autoacelerar;
        
      public void OnPointerUp(PointerEventData eventData) 
        {
           /* if(autoacelerar)
                SendValueToControl(0.0f); 
            else
                SendValueToControl(1.0f); */
        }

        public void OnPointerDown(PointerEventData eventData) 
        {
            if (autoacelerar)
                SendValueToControl(0.0f);
            else
                SendValueToControl(1.0f);

            autoacelerar = !autoacelerar; 
        }
        public void reverseMode()
        {
            if (autoacelerar)
            {
                SendValueToControl(0.0f);
                autoacelerar = !autoacelerar; 
            }
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