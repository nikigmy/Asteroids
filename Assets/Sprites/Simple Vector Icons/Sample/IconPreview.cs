using UnityEngine;
using UnityEngine.UI;

namespace Sprites.Simple_Vector_Icons.Sample
{
    public class IconPreview : MonoBehaviour
    {
        private GameObject icon;
        public Sprite[] icons;


        // Use this for initialization
        private void Awake()
        {
            for (var i = 0; i < icons.Length; i++)
            {
                icon = new GameObject("icon" + i);
                icon.transform.SetParent(gameObject.transform);
                icon.AddComponent<RectTransform>();
                icon.AddComponent<Image>();
                icon.GetComponent<Image>().sprite = icons[i];
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}