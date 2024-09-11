#region Author & Verson
// Name : GameSceneMain.cs
// Author : GuanRen
// CreateTime : 2024/09/10
// Job :
#endregion

using System;
using Game.Framework;
using Game.UI;
using GameFramework;
using UnityEngine;

namespace Game.GamePlay
{
    public class GameSceneMain : MonoBehaviour , IController
    {
        private void Awake()
        {
    
        }

        private void Start()
        {
            UIManager.Instance.Open<UIStartView>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}