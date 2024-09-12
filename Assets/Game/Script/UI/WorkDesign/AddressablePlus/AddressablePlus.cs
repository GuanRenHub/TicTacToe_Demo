#region Author & Version
// Name : AddressablePlus.cs
// Author : GuanRen
// CreateTime : 2024/01/02
// Job : AddressablePlus AA包封装
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GameTool.AddressablePlus
{
    
    public static class AddressablePlus
    {
        //正在加载的资源 用资源的path做索引
        private static readonly Dictionary<string, AsyncOperationHandle> s_LoadingAssets = new Dictionary<string, AsyncOperationHandle>();
        //已经加载的资源 用资源的path做索引
        private static readonly Dictionary<string, AsyncOperationHandle> s_LoadedAssets = new Dictionary<string, AsyncOperationHandle>();
        
        public static TObjectType GetAsset<TObjectType>(string path) where TObjectType : Object
        {
            //判断路径是否为空
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"资源加载失败；{nameof(path)} is not valid for 'empty'.");
                return null;
            }
            
            //资源已经加载过了
            if (s_LoadedAssets.ContainsKey(path))
            {
                try
                {
                    AsyncOperationHandle<TObjectType>  handle = s_LoadedAssets[path].Convert<TObjectType>();

                    return handle.Result;
                }
                catch
                {
                    Debug.LogError("m_LoadedAssets 有 key 但获取失败！");
                }
            }
            
            //创建aRefKey
            var aRefKey = new AssetReference(path);
            if (CheckRuntimeKey(aRefKey.RuntimeKey))
            {
                Debug.LogError($"资源加载失败；<color=#FF0000>{nameof(aRefKey)}</color> is not valid for '{aRefKey}'.");
                return null;
            }
            //新资源流程
            var newHandle = Addressables.LoadAssetAsync<TObjectType>(aRefKey);
            s_LoadingAssets.Add(path, newHandle);
            
            newHandle.Completed += op =>
            {
                s_LoadedAssets.Add(path, op);
                s_LoadingAssets.Remove(path);
            };
            return newHandle.WaitForCompletion();
        }
        
        public static AsyncOperationHandle<TObjectType> GetAssetAsync<TObjectType>(string path, Action<TObjectType> complete) 
        {
            //判断路径是否为空
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidKeyException($"资源加载失败；{nameof(path)} is not valid for 'empty'.");
            }
            
            //创建aRefKey
            var aRefKey = new AssetReference(path);
            
            //aRefKey 合法性判断
            if (CheckRuntimeKey(aRefKey.RuntimeKey))
            {
                //不传空出去，要不很麻烦。
                throw new InvalidKeyException($"资源加载失败；{nameof(aRefKey)} is not valid for '{aRefKey}'.");
            }
            
            AsyncOperationHandle<TObjectType> handle;
            
            //资源是否在loading, 认为已经注册过回调，不用重复注册。
            if (s_LoadingAssets.ContainsKey(path))
            {
                try
                {
                    handle = s_LoadingAssets[path].Convert<TObjectType>();
                }
                catch(Exception ex)
                {
                    Debug.LogError("LoadingAssets 有 key 但获取失败！");
                    Debug.LogError("捕获异常：" + ex.Message);
                    Debug.LogError("异常类型：" + ex.GetType());
                    Debug.LogError("堆栈跟踪：" + ex.StackTrace);
                    
                    handle = Addressables.ResourceManager.CreateChainOperation(s_LoadingAssets[path], chainOp =>
                        Addressables.ResourceManager.CreateCompletedOperation(chainOp.Result is TObjectType ? (TObjectType) chainOp.Result : default, string.Empty));
                }
                
                handle.Completed += op =>
                {
                    complete?.Invoke(op.Result);
                };
                
                return handle;
            }
            
            //资源已经加载过了
            if (s_LoadedAssets.ContainsKey(path))
            {
                try
                {
                    handle = s_LoadedAssets[path].Convert<TObjectType>();
                }
                catch(Exception ex)
                {
                    Debug.LogError("m_LoadedAssets 有 key，但有异常！");
                    Debug.LogError("捕获异常：" + ex.Message);
                    Debug.LogError("异常类型：" + ex.GetType());
                    Debug.LogError("堆栈跟踪：" + ex.StackTrace);
                    
                    //已经被加载过但是handle丢失了
                    handle = Addressables.ResourceManager.CreateCompletedOperation(s_LoadedAssets[path].Result is TObjectType ? (TObjectType) s_LoadedAssets[path].Result : default, string.Empty);
                }
                
                //完成时，触发回调
                handle.Completed += op =>
                {
                    complete?.Invoke(op.Result);
                };
                
                return handle;
            }
            
            //新资源流程
            handle = Addressables.LoadAssetAsync<TObjectType>(aRefKey);

            s_LoadingAssets.Add(path, handle);
            
            handle.Completed += op =>
            {
                s_LoadedAssets.Add(path, op);
                s_LoadingAssets.Remove(path);

                complete?.Invoke(op.Result);
            };
            
            return handle;
        }
        
        private static bool CheckRuntimeKey(object key)
        {
            return Guid.TryParse(key.ToString(), out var result);
        }
    }
}