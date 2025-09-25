
using UnityEngine;
using WeChatWASM;
using SystemInfo = UnityEngine.SystemInfo;

using System;
using System.Linq;

using UnityEngine.UI;
using TMPro;

namespace TJ.Scripts
{
  public class WeChatShareManager : MonoBehaviour
    {
        private static WeChatShareManager _instance;

        public static WeChatShareManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("WeChatShareManager");
                    _instance = go.AddComponent<WeChatShareManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }

            InitShare();
        }

        public static string RemoveDuplicates(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return new string(input.Distinct().ToArray());
        }

        void Start()
        {

            InitPageManager();
            UpdateRecommendUI();
        }



        private WXPageManager pageManagerCircle;
        private WXPageManager pageManagerRecommend;

        void InitPageManager()
        {
            pageManagerCircle= WX.CreatePageManager();
            LoadOption option=new LoadOption();
            // option.openlink = "TWFRCqV5WeM2AkMXhKwJ03MhfPOieJfAsvXKUbWvQFQtLyyA5etMPabBehga950uzfZcH3Vi3QeEh41xRGEVFw";
            // pageManager.Load(option);
            option.openlink = //"TWFRCqV5WeM2AkMXhKwJ03MhfPOieJfAsvXKUbWvQFQtLyyA5etMPabBehga950uzfZcH3Vi3QeEh41xRGEVFw";
            "-SSEykJvFV3pORt5kTNpS4Xmaj7C3hxPaJXf_f-HFfs6qg1TqfBSpB8lCo2hlyacvq1YViuHSPDE0F8dRYv5yzcIeG5qDRAsVrsxJkX2eGbkHn1dMi_89NJ23MtALWElIb7TZqWUtt1dHG0MFYi_HrG7-orsySPMIVNVP-q0AuBI65VjWooQXlXsUbw6wXxu1enyzDKYiN7BGbaaEa-BaIzs4ID93QHE9DfgY8PitCthEK4nLbxDWiP2z2GfZLti8ViJ0_IoA6AFL45iT0rux3ZOb2_BEDGNPcYXpzGUcME-WdhIzqResHUaIsTwE558Pjsorw9S-Erb-ZymkDCrAg";
            pageManagerCircle.Load(option);

            // pageManagerRecommend= WX.CreatePageManager();
            // LoadOption option2=new LoadOption();
            // option2.openlink = "TWFRCqV5WeM2AkMXhKwJ03MhfPOieJfAsvXKUbWvQFQtLyyA5etMPabBehga950uzfZcH3Vi3QeEh41xRGEVFw";
            // pageManagerRecommend.Load(option2);

            LoadRecommend();
        }

        public void OpenGameCircle()
        {
            Debug.LogError("OpenGameCircle");
           ShowOption option=new ShowOption();
           option.openlink =
           //"https://game.weixin.qq.com/cgi-bin/h5/static/circle/detail.html?liteapp=liteapp%3A%2F%2Fwxalite842f9e8076010458697522e7db33761b%3Fpath%3Dpages%252Fdetail%252Findex&wechat_pkgid=circle_detail&tid=zYbOW7HdsRPRDPr4ogfcgQ#wechat_redirect";
           //"https://game.weixin.qq.com/cgi-bin/h5/lite/circlecenter/index.html?wechat_pkgid=lite_circlecenter&liteapp=liteapp%3A%2F%2Fwxalited17d79803d8c228a7eac78129f40484c%3Fpath%3Dpages%252Findex%252Findex&appid=wxb8b881a33f6f81ff#wechat_redirect";
           "-SSEykJvFV3pORt5kTNpS4Xmaj7C3hxPaJXf_f-HFfs6qg1TqfBSpB8lCo2hlyacvq1YViuHSPDE0F8dRYv5yzcIeG5qDRAsVrsxJkX2eGbkHn1dMi_89NJ23MtALWElIb7TZqWUtt1dHG0MFYi_HrG7-orsySPMIVNVP-q0AuBI65VjWooQXlXsUbw6wXxu1enyzDKYiN7BGbaaEa-BaIzs4ID93QHE9DfgY8PitCthEK4nLbxDWiP2z2GfZLti8ViJ0_IoA6AFL45iT0rux3ZOb2_BEDGNPcYXpzGUcME-WdhIzqResHUaIsTwE558Pjsorw9S-Erb-ZymkDCrAg";

           pageManagerCircle.Show(option);

        }


        public TMP_Text recommendText;
        public void Recommend()
        {

            int recommended=PlayerPrefs.GetInt(PlayerPrefsManager.recommended,0);
            if (recommended==0)
            {
                ShowRecommend();
                PlayerPrefs.SetInt(PlayerPrefsManager.recommended,1);
                Invoke("UpdateRecommendUI",3f);
                EnergyManager.Instance.AddEnergy();
            }
        }

        void UpdateRecommendUI()
        {
            int recommended=PlayerPrefs.GetInt(PlayerPrefsManager.recommended,0);
            if (recommended==1)
            {
                if (recommendText!=null)
                {
                    recommendText.text = "已推荐";
                    Transform parent=recommendText.transform.parent;
                    parent.GetComponent<Image>().color=new Color(0.7f, 0.7f, 0.7f, 1f);
                }
            }
        }

        private WXPageManager recommendPageManager;
        private const string OPENLINK = "TWFRCqV5WeM2AkMXhKwJ03MhfPOieJfAsvXKUbWvQFQtLyyA5etMPabBehga950uzfZcH3Vi3QeEh41xRGEVFw";

        private void LoadRecommend()
        {
            if (recommendPageManager == null)
            {
                // 创建页面管理器实例
                recommendPageManager = WX.CreatePageManager();

                // 监听组件加载完毕事件
                recommendPageManager.On("ready", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听ready",
                        content = "ready",
                        success = (res) =>
                        {
                            Debug.Log("Ready success" + res);
                        },
                    });
                    Debug.Log("组件加载完毕触发");
                });

                // 监听用户展示组件时触发
                recommendPageManager.On("show", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听show",
                        content = "show",
                        success = (res) =>
                        {
                            Debug.Log("Show success" + res);
                        },
                    });
                    Debug.Log("用户展示组件时触发");
                });

                // 监听用户关闭组件时触发
                recommendPageManager.On("destroy", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听destroy",
                        content = "destroy",
                        success = (res) =>
                        {
                            Debug.Log("Destroy success" + res);
                        },
                    });
                    Debug.Log($"用户关闭组件时触发，是否是相关推荐:");
                    recommendPageManager.Destroy();
                });

                // 加载推荐页面
                recommendPageManager.Load(new LoadOption
                {
                    openlink = OPENLINK,
                });
                Debug.Log("LoadRecommend success");
            }
        }

        private void ShowRecommend()
        {
            if (recommendPageManager == null)
            {
                // 如果还没有创建页面管理器，直接创建并show（会自动执行一次load）
                recommendPageManager = WX.CreatePageManager();

                // 监听用户展示组件时触发
                recommendPageManager.On("show", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听show",
                        content = "show",
                        success = (res) =>
                        {
                            Debug.Log("Show success" + res);
                        },
                    });
                    Debug.Log("用户展示组件时触发");
                });

                // 监听用户关闭组件时触发
                recommendPageManager.On("destroy", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听destroy",
                        content = "destroy",
                        success = (res) =>
                        {
                            Debug.Log("Destroy success" + res);
                        },
                    });
                    Debug.Log($"用户关闭组件时触发，是否是相关推荐:");
                    recommendPageManager.Destroy();
                });

                // 监听组件发生错误时触发
                recommendPageManager.On("error", (res) =>
                {
                    WX.ShowModal(new ShowModalOption
                    {
                        title = "监听error",
                        content = "error",
                        success = (res) =>
                        {
                            Debug.Log("Error success" + res);
                        },
                    });
                    Debug.LogError($"组件发生错误时触发:");
                });


                recommendPageManager.Show(new ShowOption
                {
                    openlink = OPENLINK,
                });
            }
            else
            {
                Debug.Log("Show recommend success");
                // 已经执行过load，直接show即可
                recommendPageManager.Show(new ShowOption
                {
                    openlink = OPENLINK,
                });
            }
        }


        private void OnDestroy()
        {
            DestroyPageManagers();
        }

        private void DestroyPageManagers()
        {
            if (recommendPageManager != null)
            {
                // 销毁页面管理器
                recommendPageManager.Destroy();
                recommendPageManager = null;
            }

            if (pageManagerCircle!= null)
            {
                // 销毁页面管理器
                pageManagerCircle.Destroy();
                pageManagerCircle = null;
            }
        }



        // public AudioSource audioSource;
        // private float[] samples = new float[128]; // 频谱数据数组
        // public Transform gameClubButtonTrans;

        // void Update()
        // {
        //     if (audioSource != null)
        //     {
        //         audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        //
        //         // // 例如，我们可以使用第一个频率带的数据来改变物体的大小
        //         // float sizeChange = samples[0] * 100; // 这里的系数可以根据需要调整
        //         //
        //         // // 改变物体的scale，保持原始比例的同时让其大小根据频谱数据变化
        //         // Vector3 newScale = new Vector3(
        //         //     sizeChange,
        //         //     sizeChange,
        //         //     sizeChange);
        //
        //         // // 应用新的缩放
        //         // gameClubButtonTrans.localScale = newScale;
        //
        //         // 寻找频谱中最大值的位置
        //         int maxIndex = 0;
        //         float maxValue = 0;
        //         for (int i = 0; i < samples.Length; i++)
        //         {
        //             if (samples[i] > maxValue)
        //             {
        //                 maxValue = samples[i];
        //                 maxIndex = i;
        //             }
        //         }
        //
        //         // 计算对应频率
        //         float sampleRate = AudioSettings.outputSampleRate;
        //         float frequency = maxIndex * (sampleRate / 2) / samples.Length;
        //
        //         // 改变物体的scale，保持原始比例的同时让其大小根据频谱数据变化
        //         Vector3 newScale = new Vector3(
        //             frequency,
        //             frequency,
        //             frequency);
        //
        //         // 应用新的缩放
        //         gameClubButtonTrans.localScale = newScale;
        //     }
        // }


        //创建游戏圈按钮
        private void CreateGameClub()
        {
            // WeChatWASM.WXCreateGameClubButtonParam param = new WeChatWASM.WXCreateGameClubButtonParam();
            // param.type = WeChatWASM.GameClubButtonType.text;
            //
            // WeChatWASM.GameClubButtonStyle gameClubButtonStyle = new WeChatWASM.GameClubButtonStyle();
            //
            // //设置位置和大小
            // Vector2 topLeftPos = UtilManager.Instance.GetUITopLeftCorner(gameClubButtonRectTransform);
            // float hRadion = (float)WeChatWASM.WX.GetSystemInfoSync().screenHeight / Screen.height;
            // float wRadion = (float)WeChatWASM.WX.GetSystemInfoSync().screenWidth / Screen.width;
            // gameClubButtonStyle.left = (int)(topLeftPos.x * wRadion);
            // gameClubButtonStyle.top = (int)(WeChatWASM.WX.GetSystemInfoSync().screenHeight - topLeftPos.y * hRadion);
            // gameClubButtonStyle.width = (int)(gameClubButtonRectTransform.sizeDelta.x * wRadion);
            // gameClubButtonStyle.height = (int)(gameClubButtonRectTransform.sizeDelta.y * hRadion);
            //
            // param.style = gameClubButtonStyle;
            //
            // //创建游戏圈按钮
            // wXGameClubButton = WeChatWASM.WX.CreateGameClubButton(param);
            // //显示游戏圈按钮
            // wXGameClubButton.Show();
        }


        /// <summary>
        /// 显示转发按钮
        /// </summary>
        /// <param name="callback">分享回调</param>
        public void ShowShareMenu(ShowShareMenuCallback callback = null)
        {
            var option = new ShowShareMenuOption();

            // 设置分享参数
            option.withShareTicket = true;
            option.menus = new string[] { "shareAppMessage", "shareTimeline" };


            // var shareOption = new WXShareAppMessageParam();
            // shareOption.title = "一起来玩这款有趣的游戏吧！";
            // shareOption.imageUrl = "https://your-game-url.com/share-image.jpg"; // 替换为实际分享图片URL
            // shareOption.query = "from=share&userid=" + GetUserId(); // 可以添加参数用于统计来源
            // // 注册转发事件
            // WX.OnShareAppMessage(shareOption);

            // 调用微信接口显示转发按钮
            WX.ShowShareMenu(option);

            // 如果有自定义回调
            if (callback != null)
            {
                callback();
            }
        }

        /// <summary>
        /// 隐藏转发按钮
        /// </summary>
        /// <param name="callback">隐藏回调</param>
        public void HideShareMenu(HideShareMenuCallback callback = null)
        {
            var option = new HideShareMenuOption();
            WX.HideShareMenu(option);

            // 如果有自定义回调
            if (callback != null)
            {
                callback();
            }
        }

        /// <summary>
        /// 主动调起转发
        /// </summary>
        /// <param name="title">分享标题</param>
        /// <param name="imageUrl">分享图片</param>
        /// <param name="query">查询参数</param>
        public void ShareAndAddEnergy(string title = null, string imageUrl = null, string query = null)
        {
            var option = new  ShareAppMessageOption();
            option.title = title ?? "一起来玩这款有趣的游戏吧！";
            //option.imageUrl = imageUrl ?? "images/unity_logo.png"; // 替换为实际分享图片URL
            option.query = query ?? "from=share&userid=" + GetUserId();

            // 注册分享回调，在分享成功后增加体力值
            // 注册分享事件
            // WX.OnShareAppMessage(option, (action) => {
            //     // 用户点击分享按钮时的处理
            //     // 可以在这里添加分享前的逻辑
            //
            //     // 如果需要动态修改分享内容，可以创建新的参数对象
            //     var dynamicParam = new WXShareAppMessageParam();
            //     dynamicParam.title = option.title;
            //     dynamicParam.imageUrl = option.imageUrl;
            //     dynamicParam.query = option.query + "&time=" + DateTime.Now.Ticks;
            //
            //     action(dynamicParam);
            //
            //     // 注意：由于微信限制，我们无法准确知道分享是否完成
            //     // 所以在调用分享时就给予奖励
            //     GiveShareReward();
            // });

            shared = true;
            WX.ShareAppMessage(option);
            // 调用分享菜单
            // var showMenuOption = new ShowShareMenuOption();
            // showMenuOption.withShareTicket = true;
            // WX.ShowShareMenu(showMenuOption);
        }

        private bool shared = false;
        private void InitShare(string title = null, string imageUrl = null, string query = null)
        {
            var option = new  WXShareAppMessageParam();
            option.title = title ?? "一起来玩这款有趣的游戏吧！";
            option.imageUrl = imageUrl ?? "images/unity_logo.png"; // 替换为实际分享图片URL
            option.query = query ?? "from=share&userid=" + GetUserId();

            // 注册分享回调，在分享成功后增加体力值
            // 注册分享事件
            WX.OnShareAppMessage(option, (action) => {
                // 用户点击分享按钮时的处理
                // 可以在这里添加分享前的逻辑

                // 如果需要动态修改分享内容，可以创建新的参数对象
                var dynamicParam = new WXShareAppMessageParam();
                dynamicParam.title = option.title;
                //dynamicParam.imageUrl = option.imageUrl;
                dynamicParam.query = option.query + "&time=" + DateTime.Now.Ticks;
                shared = true;

                action(dynamicParam);

                // 注意：由于微信限制，我们无法准确知道分享是否完成
                // 所以在调用分享时就给予奖励
                //GiveShareReward();
            });
            WX.OnShow((action) => {GiveShareReward();});
        }

        private void GiveShareReward()
        {
            // 给予分享奖励（增加5点体力值）
            if (EnergyManager.Instance != null&&shared)
            {
                EnergyManager.Instance.AddEnergy(5);
                shared = false;
                Debug.LogError("分享成功，体力值增加5点！");
            }
        }

        /// <summary>
        /// 更新转发属性
        /// </summary>
        /// <param name="withShareTicket">是否带shareTicket</param>
        public void UpdateShareMenu(bool withShareTicket = true)
        {
            var option = new UpdateShareMenuOption();
            option.withShareTicket = withShareTicket;
            WX.UpdateShareMenu(option);
        }

        /// <summary>
        /// 获取用户ID（示例实现）
        /// </summary>
        /// <returns></returns>
        private string GetUserId()
        {
            // 这里应该返回实际的用户ID
            // 可以从PlayerPrefs或其他地方获取
            return PlayerPrefs.GetString("UserId", SystemInfo.deviceUniqueIdentifier);
        }

        /// <summary>
        /// 设置用户ID
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserId(string userId)
        {
            PlayerPrefs.SetString("UserId", userId);
            PlayerPrefs.Save();
        }
    }

    // 委托定义
    public delegate void ShowShareMenuCallback();
    public delegate void HideShareMenuCallback();
}