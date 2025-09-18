using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TJ.Scripts
{
    public class ClosePanelScript : MonoBehaviour
    {
        [SerializeField] private Button closeBtn;

        void Start()
        {
            closeBtn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }


}