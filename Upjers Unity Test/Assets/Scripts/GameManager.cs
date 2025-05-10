
using UnityEngine;

namespace PlantingGame
{
    public class GameManager : MonoBehaviour
    {
        public static InputSystem_Actions inputActions;
        public bool isOverUIElement = false;

        public void Awake()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
        }

        private void Update()
        {
            PointerOverUiElementCheck(); // has to be called here because using EventSystem.current.IsPointerOverGameObject() in ActionCallbacks (InputSystem) throws a warning
        }

        /// <summary>
        /// Updates the isOverUIElement variable if the pointer is over a UI element.
        /// </summary>
        private void PointerOverUiElementCheck()
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                isOverUIElement = true;
            }
            else
            {
                isOverUIElement = false;
            }
        }
    }
}


