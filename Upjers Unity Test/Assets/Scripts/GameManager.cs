
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace PlantingGame
{
    public class GameManager : MonoBehaviour
    {
        public static InputSystem_Actions inputActions;
        public bool isOverUIElement = false;
        private int money = 0;
        public UnityEvent<string> onMoneyChanged = new UnityEvent<string>();

        public void Awake()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
            Money = 100; // Initial money value for testing
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

        public int Money { 
            get => money; 
            set
            {
                if (value != money)
                {
                    money = value;
                    onMoneyChanged.Invoke(money.ToString());
                }
            }
        }
    }
}


