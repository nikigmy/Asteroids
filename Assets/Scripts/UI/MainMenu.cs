using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas mainMenuCanvas;
        [SerializeField] private Dropdown levelsDropdown;
        [SerializeField] private Camera mainMenuCamera;

        public void Init(LevelData[] levels)
        {
            levelsDropdown.options = levels.Select(x => new Dropdown.OptionData(x.name)).ToList();
            mainMenuCanvas.gameObject.SetActive(true);
            mainMenuCamera.gameObject.SetActive(true);
        }
    
        public void LoadSelected()
        {
            GameManager.instance.StartGame();
        }

        public void Disable()
        {
            mainMenuCanvas.gameObject.SetActive(false);
            mainMenuCamera.gameObject.SetActive(false);
        }
    }
}
