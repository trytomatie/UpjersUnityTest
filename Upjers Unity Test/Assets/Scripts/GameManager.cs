
using UnityEngine;

namespace PlantingGame
{
    public class GameManager : MonoBehaviour
    {
        public static InputSystem_Actions inputActions;

        public void Start()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
        }
    }
}


