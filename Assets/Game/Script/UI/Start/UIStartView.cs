#region Author & Verson
// Name : UIStartView.cs
// Author : GuanRen
// CreateTime : 2024/09/11
// Job :
#endregion

using Game.Framework;
using Game.GamePlay;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIStartView : UIController
    {
        [SerializeField] private Button m_PveBtn;
        [SerializeField] private Button m_PvpBtn;
        
        private void Awake()
        {
            m_PveBtn.onClick.AddListener(OnPveBtnClicked);
            m_PvpBtn.onClick.AddListener(OnPvpBtnClicked);
        }

        private void OnPveBtnClicked()
        {
            TTTManager.Instance.StartGame(ETicTacToeType.PVE);
            UIManager.Instance.Close(this);
        }
        
        private void OnPvpBtnClicked()
        {
            TTTManager.Instance.StartGame(ETicTacToeType.PVP);
            UIManager.Instance.Close(this);
        }
    }
}