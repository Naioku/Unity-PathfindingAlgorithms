using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DynamicImage : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void Initialize(Enums.Icon sprite, Vector3 position)
        {
            image.sprite = AllManagers.Instance.UIManager.GetSprite(sprite);
            image.transform.position = position;
        }
    }
}