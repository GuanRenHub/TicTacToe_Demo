#region Author & Verson
// Name : UIController.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job : ui基类
#endregion

using GameFramework;
using UnityEngine;

namespace Game.UI
{
    public abstract class UIController : MonoBehaviour, IController
    {
        public bool IsShowing => gameObject.activeSelf;
        
        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}