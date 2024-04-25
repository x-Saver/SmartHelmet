using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] Text SizeText;
    private GameObject tmp = null;
    [SerializeField] private string tmpAddress = string.Empty;
    public void _ClickSpawn()
    {
        if (!ReferenceEquals(tmp, null)) ReleaseObj();

        Spawn();
    }
    public void Spawn()
    {
        Addressables.InstantiateAsync(tmpAddress, new Vector3(0, 0, 0), Quaternion.identity).Completed +=
            (AsyncOperationHandle<GameObject> obj) =>
            {
                tmp = obj.Result;
            };

    }
    public void ReleaseObj()
    {
        Addressables.ReleaseInstance(tmp);
    }

    [Space]
    [SerializeField] string LabelForBundleDown = string.Empty;

    public void _Click_BundleDown()
    {
        Addressables.DownloadDependenciesAsync(LabelForBundleDown).Completed +=
            (AsyncOperationHandle Handle) =>
            {
                
                Debug.Log("download complete");
                Addressables.Release(Handle);
            };
    }
    public void _Click_CheckTheDownloadFileSize()
    {
        Addressables.GetDownloadSizeAsync(LabelForBundleDown).Completed +=
            (AsyncOperationHandle<long> SizeHandle) =>
            {
                string sizeText = string.Concat(SizeHandle.Result, " byte");
                SizeText.text = sizeText;
                Addressables.Release(SizeHandle);
            };
    }
}
