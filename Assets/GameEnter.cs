using System.Collections;
using System.Reflection;
using Cysharp.Threading.Tasks;
//using TJ.Scripts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using UnityEngine.Networking;
using WeChatWASM;
using System;

public class GameEnter : MonoBehaviour
{
    //private strin7g CDN = "http://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl/StreamingAssets/yoo/bus/";
    //private string CDN = "https://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl/StreamingAssets/yoo/bus/";
    private string CDN = "https://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl2/StreamingAssets/yoo/bus/";
    //https://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl/index.html?sign=ee5e7b1e50cfee8bf1d5b4cc996e6cda&t=1753188540
    //private string CDN = "http://49.232.32.95/abfiles/StreamingAssets/yoo/bus/";
    public TextMeshProUGUI tip;

    public Button clickBtn;

    public  Text result_text;

    public EPlayMode _PlayMode;
    private bool isInit = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        yield return null;
        InitResource().Forget();
    }


    private ResourcePackage package;

    async UniTask InitResource()
    {

        YooAssets.Initialize(null);

        YooAssets.SetOperationSystemMaxTimeSlice(1000);

       // DefaultPackage = YooAssets.CreatePackage("DefaultPackage");
        package = YooAssets.CreatePackage("bus");
        YooAssets.SetDefaultPackage(package);


#if UNITY_WEBGL


#if WEIXINMINIGAME


        string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE";

        InitializeParameters initializeParameters = null;
        initializeParameters = new WebPlayModeParameters()
        {
            WebRemoteFileSystemParameters =
                WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, new RemoveServer(CDN)),
        };
#endif

#endif

        var init = package.InitializeAsync(initializeParameters);
        await init;

        //ClearVersionFile(packageRoot,package.PackageName);

        var version = package.RequestPackageVersionAsync();

        await version;

        var update = package.UpdatePackageManifestAsync(version.PackageVersion);

        await update;
        //can load assets
       // await InitTmpAsset();
       //LevelManager.EnterGame();
       //var loadHandle = package.LoadAssetAsync<GameObject>("GameObject");

       var laodHotUpdateDll = package.LoadAssetAsync<TextAsset>("hot_update.dll");
       await laodHotUpdateDll;
       TextAsset dllData = laodHotUpdateDll.AssetObject as TextAsset;
       Debug.LogError("dllDataLength"+dllData.bytes.Length);
       Assembly hotUpdateAss = Assembly.Load(dllData.bytes);
       Type levelManager= hotUpdateAss.GetType("TJ.Scripts.LevelManager");
       levelManager.GetMethod("EnterMainScene").Invoke(null, null);
      // LevelManager.EnterMainScene();
#if WEIXINMINIGAME



#else
        tip.text = "这是编辑器模式";
        tip.gameObject.SetActive(true);
#endif
    }
#if WEIXINMINIGAME




    public bool CheckCacheFileExist(string filePath)
    {
        string result = WX.GetCachePath(filePath);
        if (string.IsNullOrEmpty(result))
            return false;
        else
            return true;
    }
    public static void ShowURL(string url)
    {
        //result_text.text =url;
    }


#else

#endif

    class RemoveServer : IRemoteServices
    {
        //注意微信CDN地址与Yoo远端加载地址需一致，才会触发缓存
        //https://wechat-miniprogram.github.io/minigame-unity-webgl-transform/Design/FileCache.html

        string CDN;
        public RemoveServer(string cdn)
        {
            CDN = cdn;
        }

        //远端目录结构为：
        //CDN:
        //    StreamingAssets
        //    xxwebgl.wasm.code.unityweb.wasm.br

        //    xxx.version
        //    xxx.hash
        //    xx/bundle

        //    xx.ttf 备用字体
        public string GetRemoteFallbackURL(string fileName)
        {
            return CDN + fileName;
        }

        public string GetRemoteMainURL(string fileName)
        {
            return CDN + fileName;
        }
    }
}
