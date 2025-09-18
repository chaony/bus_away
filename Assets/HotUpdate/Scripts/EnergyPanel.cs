using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WeChatWASM;


namespace TJ.Scripts
{
    public class EnergyPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI energyText; // 添加体力显示文本

        public static EnergyPanel instance;
        //[SerializeField] private GameObject energyLackingGo;
        [SerializeField] private Animator energyLackingAnimator;
        [SerializeField] private GameObject shareGo;
        [SerializeField] private Button plusBtn;
        private void Awake()
        {
            instance = this;
#if WEIXINMINIGAME
            WX.InitSDK((code)=>
            {
                Debug.Log($"微信SDK初始化成功{code}");
            });
#endif
            plusBtn.onClick.AddListener(() =>
            {
                OnPlusBtnClick();
                // 添加点击事件处理逻辑
            });
        }

        public void ShowEnergyLacking()
        {
            //energyLackingGo.SetActive(true);
           //energyLackingAnimator.SetTrigger("show");
            energyLackingAnimator.Play("EnergyLackingAni");
        }



        // private void Start()
        // {
        //     // 在开始时更新体力显示文本
        //     UpdateEnergyText(EnergyManager.Instance.GetCurrentEnergy());
        // }
        // 更新体力显示文本的方法
        public void UpdateEnergyText(int currentEnergy)
        {
            if (energyText != null)
            {
                energyText.text = currentEnergy.ToString();
            }
        }

        //显示接口
        public  void ShowShareMenu()
        {
            // 使用新的分享管理器
            WeChatShareManager.Instance.ShowShareMenu(() => {
                Debug.Log("分享菜单已显示");
            });
            Debug.Log("分享菜单已显示");
            Debug.LogError("分享菜单已显示");
        }

        //关闭接口
        public  void HideShareMenu()
        {
            WeChatShareManager.Instance.HideShareMenu(() => {
                Debug.Log("分享菜单已隐藏");
            });
        }

        // 主动分享接口
        public void ShareAppMessage()
        {
#if WEIXINMINIGAME
            WeChatShareManager.Instance.ShareAndAddEnergy();
#endif
        }

        public void OnPlusBtnClick()
        {
            if (shareGo!=null)
            {
                shareGo.SetActive(true);
            }
        }

    }
}