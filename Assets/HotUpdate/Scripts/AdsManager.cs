using System;
using UnityEngine;
using WeChatWASM;

namespace HotUpdate.Scripts
{
    public class AdsManager : MonoBehaviour
    {
        private static AdsManager _instance;
        public static WXRewardedVideoAd rewardedVideoAd;
        public static AdsManager instance
        {
            get
            {
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance==null) _instance=this;
            WXCreateRewardedVideoAdParam param = new WXCreateRewardedVideoAdParam()
            {
                adUnitId="adunit-57fe4bf766d54267"
            };
            rewardedVideoAd = WX.CreateRewardedVideoAd(param);

            rewardedVideoAd.OnLoad((res) =>
            {
                Debug.Log("激励视频 广告加载成功");
            });
            // 添加广告关闭的回调
            rewardedVideoAd.OnClose((res) =>
            {
                if (res!=null && res.isEnded)
                {
                    ISManager.instance.RewardCallBacks();
                    Debug.Log("广告观看完毕，可以发放奖励");
                    // 在这里可以添加发放奖励的逻辑
                }
                else
                {
                    Debug.Log("广告未观看完毕");
                }
            });

            rewardedVideoAd.onErrorAction = (res) =>
            {
                Debug.LogError("广告拉取失败"+res.errCode);
            };
        }

        public void ShowRewardedVideoAd()
        {
            Action<WXTextResponse> success =(res) =>
            {
                Debug.Log("show success");
            };
            Action<WXTextResponse> fail = (res) =>
            {
                rewardedVideoAd.Load();
                rewardedVideoAd.Show();
            };
            rewardedVideoAd.Show(success,fail);
        }
    }
}