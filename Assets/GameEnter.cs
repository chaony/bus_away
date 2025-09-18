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
    private string CDN = "https://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl/StreamingAssets/yoo/bus/";
    //https://636c-cloud1-2gklr93t1302d7ef-1362080318.tcb.qcloud.la/webgl/index.html?sign=ee5e7b1e50cfee8bf1d5b4cc996e6cda&t=1753188540
    //private string CDN = "http://49.232.32.95/abfiles/StreamingAssets/yoo/bus/";
    public TextMeshProUGUI tip;

    public Button clickBtn;

    public  Text result_text;

    public EPlayMode _PlayMode;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        InitResource().Forget();
    }


    private IEnumerator InitPackage()
    {
        package = YooAssets.CreatePackage("bus");
        YooAssets.SetDefaultPackage(package);
        var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        var initParameters = new OfflinePlayModeParameters();
        initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
        var initOperation = package.InitializeAsync(initParameters);
        yield return initOperation;

        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }



    IEnumerator testWebRequestForWeGame()
    {
        string _requestURL=CDN + "bus.version";
        UnityWebRequest _webRequest = new UnityWebRequest(_requestURL, UnityWebRequest.kHttpVerbGET);
        DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
        _webRequest.downloadHandler = handler;
        _webRequest.disposeDownloadHandlerOnDispose = true;
        UnityWebRequestAsyncOperation _requestOperation = _webRequest.SendWebRequest();

        while(!_requestOperation.isDone)
        {
            yield return null;
        }

        string result = _webRequest.downloadHandler.text;
        result_text.text = result;
        Debug.Log(result);
    }

    private ResourcePackage package;

    async UniTask InitResource()
    {

        YooAssets.Initialize(null);

        YooAssets.SetOperationSystemMaxTimeSlice(1000);

       // DefaultPackage = YooAssets.CreatePackage("DefaultPackage");
        package = YooAssets.CreatePackage("bus");
        YooAssets.SetDefaultPackage(package);

        result_text.text =CDN;


#if UNITY_WEBGL
        // var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        // var packageRoot = buildResult.PackageRootDirectory;
        //
        // initializeParameters = new EditorSimulateModeParameters()
        // {
        //     EditorFileSystemParameters =
        //         FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot)
        // };

#if WEIXINMINIGAME

        // initializeParameters = new OfflinePlayModeParameters()
        // {
        //     BuildinFileSystemParameters =
        //         FileSystemParameters.CreateDefaultBuildinFileSystemParameters()
        // };

        // IRemoteServices remoteServices = new RemoveServer(CDN);
        // var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
        // var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载
        //
        // var initializeParameters = new WebPlayModeParameters();
        // initializeParameters.WebServerFileSystemParameters = webServerFileSystemParams;
        // initializeParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;

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
        result_text.text = version.PackageVersion;
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

        //更新字体 事先隐藏的就不需要
        // tip.UpdateFontAsset();
        // tip.SetAllDirty();

        // tip.text = "1、修改转换工具cdn地址 、appid、设置导出路径\n2、修改GameEnter CDN\n3、添加TMP_SDF-Mobile着色器到内置shaders清单";
        // tip.gameObject.SetActive(true);
        // var download = DefaultPackage.CreateResourceDownloader(3, 10);
        //
        // var a = 0;
        // clickBtn.gameObject.SetActive(true);
        // clickBtn.onClick.AddListener(() =>
        // {
        //     a++;
        //     tip.text = a.ToString();
        // });
        //
        // download.BeginDownload();
        //
        // await download;
        //
        // var loadHandle = DefaultPackage.LoadAssetAsync<GameObject>("GameObject");
        //
        // await loadHandle;
        //
        // var instant = loadHandle.InstantiateAsync();
        //
        // await instant;
        //
        // var _wxFileSystemMgr = WeChatWASM.WX.GetFileSystemManager();
        //
        // //测试读取 sa 资源
        // _wxFileSystemMgr.ReadFile(new WeChatWASM.ReadFileParam()
        // {
        //     filePath = WeChatWASM.WX.env.USER_DATA_PATH + "/StreamingAssets/aa.png",
        //     success = (success) =>
        //     {
        //         Debug.Log("load success");
        //     },
        //     fail = (fail) =>
        //     {
        //
        //     }
        // });


#else
        tip.text = "这是编辑器模式";
        tip.gameObject.SetActive(true);
#endif
    }
#if WEIXINMINIGAME


    public void ClearVersionFile(string packageRoot,string packageName)
    {
        string versionPath = YooAsset.PathUtility.Combine("StreamingAssets", YooAssetSettingsData.GetDefaultYooFolderName(), packageName,".version");

        versionPath=YooAsset.PathUtility.Combine(packageRoot,versionPath);
        if (CheckCacheFileExist(versionPath))
        {
            WX.RemoveFile(versionPath);
        }
    }

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

    async UniTask InitTmpAsset()
    {
        var fallbackFont = CDN + "AlibabaPuHuiTi-2-65-Medium.ttf";

        UniTaskCompletionSource<bool> source = new UniTaskCompletionSource<bool>();

        WeChatWASM.WX.GetWXFont(fallbackFont, (font) =>
        {
            //Debug.Log("get font: code:" + code + " font:" + font);

            if (font != null)
            {
                //注意：需要将shader: TMP_SDF-Mobile 添加到editor Graphics included Shaders内置着色器列表
                var tmp_font = TMP_FontAsset.CreateFontAsset(font);

                TMP_Text.OnFontAssetRequest += (hashcode, asset) =>
                {
                    return tmp_font;
                };

                //TMP_Settings.defaultFontAsset = tmp_font;

                Debug.Log("load font success");
            }
            source.TrySetResult(font != null);
        });


        await source.Task;
    }
#else
    async UniTask InitTmpAsset()
    {
        //出小游戏包，可以不打包字体，使用微信字体，减少包体大小
        var handle = DefaultPackage.LoadAssetAsync<TMP_FontAsset>("alibaba");

        await handle;
        var tmp_font = handle.AssetObject as TMP_FontAsset;
        TMP_Text.OnFontAssetRequest += (hashcode, asset) =>
        {
            return tmp_font;
        };

        //TMP_Settings.defaultFontAsset = tmp_font;
    }
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
