#region Author & Verson
// Name : UICheckerBroard.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job :
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Game.Framework;
using Game.GamePlay;
using GameFramework;
using UnityEngine;

namespace Game.UI
{
    public class UICheckerBoard : UIController
    {
        [SerializeField] 
        private RectTransform m_Container;
        
        private List<UIChecker> m_Checkers = new List<UIChecker>();
        
        // model
        private ICheckerboardModel m_CheckerboardModel;
        private ICheckerBoardSystem m_CheckerBoardSystem;

        private void Awake()
        {
            m_CheckerboardModel = this.GetModel<ICheckerboardModel>();
            m_CheckerBoardSystem = GameApp.Interface.GetSystem<ICheckerBoardSystem>();
        }

        public void Start()
        {
            m_CheckerBoardSystem.OnGameWin.AddListener(OnGameWin);
            m_CheckerBoardSystem.OnGameDraw.AddListener(OnGameDraw);
            
            foreach (var checkerData in m_CheckerboardModel.GetCheckers())
            {
                var uiChecker = UIManager.Instance.Create<UIChecker>(m_Container);
                uiChecker.Init(checkerData);
                m_Checkers.Add(uiChecker);
            }
        }

        private void OnDestroy()
        {
            m_CheckerBoardSystem.OnGameWin.RemoveListener(OnGameWin);
            m_CheckerBoardSystem.OnGameDraw.RemoveListener(OnGameDraw);
        }

        public void PingResult()
        {
            ClearPing();
            var poses = m_CheckerBoardSystem.GetCurMarkPoses();
            foreach (var uiChecker in m_Checkers)
            {
                if (!((IList) poses).Contains(uiChecker.Data.Pos))
                {
                    uiChecker.SetAlpha(0.5f);
                }
            }
        }

        private void ClearPing()
        {
            foreach (var uiChecker in m_Checkers)
            {
                uiChecker.SetAlpha(1);
            }
        }
        
        private void OnGameWin(ECurRoundType obj)
        {
            PingResult();
            StartCoroutine(ShowGameOverTip());
        }

        private void OnGameDraw()
        {
            ShowGameOverTipInternal("和局");
        }
        
        private IEnumerator ShowGameOverTip()
        {
            yield return  new WaitForSeconds(2);
            ShowGameOverTipInternal($"游戏结束，{m_CheckerBoardSystem.CurRoundType.Value} 赢了");
        }

        private void ShowGameOverTipInternal(string content)
        {
            var tip = UIManager.Instance.Open<UITip>();
            tip.Init(content, "再来一局", "关闭游戏", 
                () =>
                {
                    ClearPing();
                    m_CheckerBoardSystem.ReStart();
                }, 
                () =>
                {
                    ClearPing();
                    TTTManager.Instance.Close();
                });
        }
    }
}