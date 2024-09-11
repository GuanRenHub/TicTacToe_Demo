#region Author & Verson
// Name : TTTManager.cs
// Author : GuanRen
// CreateTime : 2024/09/11
// Job :
#endregion

using System.Collections.Generic;
using Game.Framework;
using Game.UI;
using GameFramework;
using UnityEngine;

namespace Game.GamePlay
{
    public class TTTManager : MonoSingleton<TTTManager>, IController
    {
        private TTTManager(){}
        
        private ICheckerboardModel m_CheckerboardModel;
        private ITicTacToeSystem m_TicTacToeSystem;
        private ICheckerBoardSystem m_CheckerBoardSystem;

        public bool CanPlacingChecker =>
            m_TicTacToeSystem.GameType == ETicTacToeType.PVP ||
            m_CheckerBoardSystem.CurRoundType.Value == ECurRoundType.O;

        public bool IsAiRound =>
            m_TicTacToeSystem.GameType == ETicTacToeType.PVE &&
            m_CheckerBoardSystem.CurRoundType.Value == ECurRoundType.X;
            
        public override void OnSingletonInit()
        {
            m_CheckerboardModel = this.GetModel<ICheckerboardModel>();
            m_TicTacToeSystem = this.GetSystem<ITicTacToeSystem>();
            m_CheckerBoardSystem = this.GetSystem<ICheckerBoardSystem>();
        }
        
        public void StartGame(ETicTacToeType type)
        {
            m_TicTacToeSystem.StartGame(type);
            m_CheckerBoardSystem.ReStart();
            UIManager.Instance.Open<UICheckerBoard>();

            if (type == ETicTacToeType.PVE)
            {
                // 监听当前回合
                m_CheckerBoardSystem.CurRoundType.Register(OnRoundChange);
            }
        }
        
        public void Close()
        {
            UIManager.Instance.Close<UICheckerBoard>();
            UIManager.Instance.Open<UIStartView>();
            m_CheckerBoardSystem.Stop();
            
            // 取消监听当前回合
            m_CheckerBoardSystem.CurRoundType.UnRegister(OnRoundChange);
        }

        private void OnRoundChange(ECurRoundType e)
        {
            if (IsAiRound)
            {
                AIPlacingChecker(CheckerData.CheckerType.X);
            }
        }
        
        // 简单处理AI
        private void AIPlacingChecker(CheckerData.CheckerType aiType)
        {
            // 尝试赢得游戏
            var move = FindWinningMove(aiType);
            if (move.HasValue)
            {
                var pos = move.Value;
                if (m_CheckerboardModel.GetCheckerAt(pos).Type.Value == CheckerData.CheckerType.NONE)
                {
                    m_CheckerBoardSystem.PlacingChecker(pos);
                    return;
                }
            }

            // 尝试阻止对手赢得游戏
            var opponentType = aiType == CheckerData.CheckerType.X ? CheckerData.CheckerType.O : CheckerData.CheckerType.X;
            move = FindWinningMove(opponentType);
            if (move.HasValue)
            {
                var pos = move.Value;
                if (m_CheckerboardModel.GetCheckerAt(pos).Type.Value == CheckerData.CheckerType.NONE)
                {
                    m_CheckerBoardSystem.PlacingChecker(pos);
                    return;
                }
            }

            // 如果没有胜利或阻止胜利的机会，则随机落子
            var voidCheckerDataList = GetEmptyPositions();
            if (voidCheckerDataList.Count > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, voidCheckerDataList.Count);
                var randomCheckerData = voidCheckerDataList[randomIndex];
                m_CheckerBoardSystem.PlacingChecker(randomCheckerData.Pos);
            }
        }

        
        private Vector2Int? FindWinningMove(CheckerData.CheckerType type)
        {
            // 检查所有行、列和对角线是否有两处相同且有一处为空的位置
            // 这里简化处理，具体实现取决于CheckerboardModel如何表示棋盘状态
            for (int i = 0; i < 3; i++)
            {
                if (IsLineWinning(i, 0, 1, 0, 2, 0, type)) return new Vector2Int(i, 0);
                if (IsLineWinning(0, i, 1, i, 2, i, type)) return new Vector2Int(0, i);
            }
            if (IsLineWinning(0, 0, 1, 1, 2, 2, type)) return new Vector2Int(0, 0);
            if (IsLineWinning(0, 2, 1, 1, 2, 0, type)) return new Vector2Int(0, 2);

            return null;
        }
        
        private bool IsLineWinning(int x1, int y1, int x2, int y2, int x3, int y3, CheckerData.CheckerType type)
        {
            var checker1 = m_CheckerboardModel.GetCheckerAt(x1, y1);
            var checker2 = m_CheckerboardModel.GetCheckerAt(x2, y2);
            var checker3 = m_CheckerboardModel.GetCheckerAt(x3, y3);

            if (checker1.Type.Value == type && checker2.Type.Value == type && checker3.Type.Value == CheckerData.CheckerType.NONE)
            {
                return true;
            }
            if (checker1.Type.Value == type && checker2.Type.Value == CheckerData.CheckerType.NONE && checker3.Type.Value == type)
            {
                return true;
            }
            if (checker1.Type.Value == CheckerData.CheckerType.NONE && checker2.Type.Value == type && checker3.Type.Value == type)
            {
                return true;
            }
            return false;
        }
        
        private List<CheckerData> GetEmptyPositions()
        {
            var emptyPositions = new List<CheckerData>();
            foreach (var checkerData in m_CheckerboardModel.GetCheckers())
            {
                if (checkerData.Type.Value == CheckerData.CheckerType.NONE)
                {
                    emptyPositions.Add(checkerData);
                }
            }
            return emptyPositions;
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}