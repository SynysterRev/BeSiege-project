using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class AssetsManager : MonoBehaviour
{
    private static AssetsManager instance;
    public static AssetsManager Instance
    {
        get
        {
            return instance;
        }

    }
    private Dictionary<string, AssetBundle> assetBundles;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        assetBundles = new Dictionary<string, AssetBundle>();
    }

    private void Start()
    {
        GameManager.Instance.OnReturnMenu += Destroy;
    }

    public void LoadAssetBundle(string nameBundle)
    {
        if (assetBundles.ContainsKey(nameBundle))
        {
            Debug.Log("AssetBundle already exists!");
            return;
        }
        assetBundles.Add(nameBundle, AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, nameBundle)));
    }

    public void UnLoadAssetBundle(string nameBundle, bool unloadAllLoadedObjects = true)
    {
        AssetBundle myLoadedAssetBundle = assetBundles[nameBundle];
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("AssetBundle doesn't exist!");
        }

        myLoadedAssetBundle.Unload(unloadAllLoadedObjects);
        assetBundles.Remove(nameBundle);
    }

    public void UnloadAllBundle()
    {
        if (assetBundles != null)
        {
            foreach (KeyValuePair<string, AssetBundle> asset in assetBundles)
            {
                assetBundles[asset.Key].Unload(true);
            }
            assetBundles.Clear();
        }
    }

    /// <summary>
    /// Load asset from dictionnary assetBundles.<para/>
    /// Return null if it doesn't find a Template.<para/>
    /// </summary>
    /// <param name="nameBundle"></param>
    /// <param name="nameAsset"></param>
    /// <returns></returns>
    public T LoadingAsset<T>(string nameBundle, string nameAsset)
    {
        T asset = default(T);

        AssetBundle myLoadedAssetBundle = assetBundles[nameBundle];
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }
        else
        {
            asset = (T)(object)(myLoadedAssetBundle.LoadAsset<Object>(nameAsset));
        }
        return asset;
    }

    public List<T> LoadingAllAssets<T>(string nameBundle)
    {
        List<T> assets = new List<T>();
        List<Object> assetsobj = new List<Object>();
        AssetBundle myLoadedAssetBundle = assetBundles[nameBundle];

        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }
        else
        {
            assetsobj = myLoadedAssetBundle.LoadAllAssets<Object>().ToList();
            foreach (Object obj in assetsobj)
            {
                if (obj is T)
                {
                    assets.Add((T)(object)obj);
                }
            }
        }
        return assets;
    }

    public List<string> LoadingAllScene(string nameBundle)
    {
        List<string> assets = new List<string>();
        AssetBundle myLoadedAssetBundle = assetBundles[nameBundle];

        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }
        else
        {

            assets = myLoadedAssetBundle.GetAllScenePaths().ToList();
        }
        return assets;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnloadAllBundle();
        GameManager.Instance.OnReturnMenu -= Destroy;
    }
}
