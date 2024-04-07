using UnityEngine;
using ThrashJT.Input;

namespace Thrash.Lab5
{
    public class InputManager : MonoBehaviour
    {

        private CSE3541Inputs inputScheme;

        private void Awake()
        {
            inputScheme = new CSE3541Inputs();
        }
        private void OnEnable()
        {
            var _ = new QuitHandler(inputScheme.Player.Quit);
            var t = new ToggleLines(inputScheme.Player.Toggle);
        }
    }
}
