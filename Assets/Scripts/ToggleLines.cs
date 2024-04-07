using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Thrash.Lab5
{
    public class ToggleLines
    {

        
        public ToggleLines(InputAction toggleAction)
        {
            toggleAction.performed += ToggleAction_performed;
            toggleAction.Enable();
        }
        private void ToggleAction_performed(InputAction.CallbackContext obj)
        {
            //disable all line renderers in scene

           List<LineRenderer> lines = Object.FindObjectsOfType<LineRenderer>().ToList();

            foreach(LineRenderer line in lines)
            {
                line.enabled = !line.enabled;
            }
        }
    }
}