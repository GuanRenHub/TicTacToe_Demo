#region Author & Verson
// Name : UIChecker.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job : 棋盘格子与棋子
#endregion

using System;
using Game.Framework;
using Game.GamePlay;
using GameFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIChecker : UIController , IPointerClickHandler
    {
        [SerializeField] private CanvasGroup m_Group;
        [SerializeField] private Image m_CheckO;
        [SerializeField] private Image m_CheckX;
        
        private ICheckerBoardSystem m_CheckerBoardSystem;
        private CheckerData m_Data;
        public CheckerData Data => m_Data;

        private void Awake()
        {
            m_CheckerBoardSystem = this.GetSystem<ICheckerBoardSystem>();
        }

        public void Init(CheckerData data)
        {
            m_Data = data;
            if (m_Data != null)
            {
                m_Data.Type.Register(OnCheckerTypeChange);
            }
        }

        private void OnDestroy()
        {
            if (m_Data != null)
            {
                m_Data.Type.UnRegister(OnCheckerTypeChange);
            }
        }

        public void SetAlpha(float alpha)
        {
            m_Group.alpha = alpha;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!TTTManager.Instance.CanPlacingChecker)
                return;
            
            if (m_Data != null)
            {
                m_CheckerBoardSystem.PlacingChecker(m_Data.Pos);
            }
        }
        
        private void OnCheckerTypeChange(CheckerData.CheckerType obj)
        {
            m_CheckO.gameObject.SetActive(false);
            m_CheckX.gameObject.SetActive(false);
            
            switch (obj)
            {
                case CheckerData.CheckerType.O:
                    m_CheckO.gameObject.SetActive(true);
                    break;
                case CheckerData.CheckerType.X:
                    m_CheckX.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}