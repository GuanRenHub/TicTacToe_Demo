#region Author & Verson
// Name : UIManager.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job :
#endregion

using System;
using System.Collections.Generic;
using GameFramework;
using GameTool.AddressablePlus;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas m_Canvas;
        private Dictionary<Type, UIController> m_OpenedUIDict = new Dictionary<Type, UIController>();
        
        private UIManager(){}
        
        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = Object.FindObjectOfType<Canvas>();
                    if (m_Canvas == null)
                    {
                        GameObject go = new GameObject("Canvas");
                        m_Canvas = go.AddComponent<Canvas>();
                    }
                }
                return m_Canvas;
            }
        }
        
        public T Create<T>(Transform parent = null) where T : UIController
        {
            if (parent == null)
                parent = Instance.Canvas.transform;
            if (UIRes.UIPath.TryGetValue(typeof(T),out string path))
            {
                GameObject goAsset = AddressablePlus.GetAsset<GameObject>(path);
                if (goAsset)
                {
                    var go = Object.Instantiate(goAsset, parent, false);
                    return go.GetComponent<T>();
                }
            }
            else
            {
                Debug.LogError($"{typeof(T)}对应的UI在res中不存在");
            }
        
            return null;
        }
        
        public T Open<T>() where T : UIController
        {
            m_OpenedUIDict.TryGetValue(typeof(T), out UIController uiController);
                
            if (uiController != null)
            {
                uiController.Show();
            }
            else
            {
                uiController = Create<T>();
            }
            
            m_OpenedUIDict[typeof(T)] = uiController;
            
            return uiController as T;
        }
        
        public void Close(UIController uiController)
        {
            Object.Destroy(uiController.gameObject);
            m_OpenedUIDict[uiController.GetType()] = null;
        }

        public void Close<T>() where T : UIController
        {
            if (m_OpenedUIDict.TryGetValue(typeof(T), out UIController uiController))
            {
                if (uiController != null)
                    Object.Destroy(uiController.gameObject);
            }

            m_OpenedUIDict[typeof(T)] = null;
        }
    }
}