using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;
using Proto.Promises;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace XX {
    public class ResourceComponent : Singleton<ResourceComponent> {
        public void LoadAsset<T>(string assetFullName, Action<T> onComplete) {
            Addressables.LoadAssetAsync<T>(assetFullName).Completed += (AsyncOperationHandle<T> asset) => {
                if (onComplete != null)
                    onComplete(asset.Result);
            };
        }

        public Promise<T> LoadAssetPro<T>(string assetFullName) {
            return Promise.New<T>(deferred => {
                LoadAsset<T>(assetFullName, (t) => {
                    deferred.Resolve(t);
                });
            });
        }
    }
}