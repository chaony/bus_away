using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TJ.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        public GameObject gameOverPanle, winPanel,backPanel;

        [SerializeField] private Button restartButtonForTest;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnNext;
        [SerializeField] private Button skipButton;
        [SerializeField] private TextMeshProUGUI levelText;

        [SerializeField] private Button back_ConfirmButton;
        [SerializeField] private Button back_CancelButton;
        [SerializeField] private Button stopButton;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Restart For Testing
        restartButtonForTest.onClick.AddListener(()=>
        {
            Vibration.Vibrate(30);
            SoundController.Instance.PlayOneShot(SoundController.Instance.buttonSound);
            DOVirtual.DelayedCall(0.3f, LevelManager.ReloadLevel);
        });
        //gameover panel restart Button
        btnRestart.onClick.AddListener(()=>
        {
            Vibration.Vibrate(30);
            SoundController.Instance.PlayOneShot(SoundController.Instance.buttonSound);
            DOVirtual.DelayedCall(0.3f,LevelManager.ReloadLevel);
        });
        //next button for WinPanel
        btnNext.onClick.AddListener(()=>
        {
            Vibration.Vibrate(30);
            SoundController.Instance.PlayOneShot(SoundController.Instance.buttonSound);
           LevelManager.isLoaded=false;
            DOVirtual.DelayedCall(0.3f,LevelManager.LoadScene);
        });
        skipButton.onClick.AddListener(() =>
        {
            LevelManager.LevelProgressed();
            SoundController.Instance.PlayOneShot(SoundController.Instance.buttonSound);
            DOVirtual.DelayedCall(0.3f, LevelManager.LoadScene);
        });

        levelText.text = "关卡 "+LevelManager.GetCurrentLeveLNumber();

       //暂停返回功能
        stopButton.onClick.AddListener(()=>
        {
            backPanel.SetActive(true);
        });
        back_ConfirmButton.onClick.AddListener(()=>
        {
            backPanel.SetActive( false);
            LevelManager.EnterMainScene();
        });
        back_CancelButton.onClick.AddListener(()=>
        {
            backPanel.SetActive(false);
        });
    }

    public void TogglePanel(GameObject panel, bool value)
    {
        panel.SetActive(value);
    }
    }

}
