#region Author & Verson
// Name : CheckerBoardSystem.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job : 棋盘系统
#endregion

using GameFramework;
using UnityEngine;

namespace Game.Framework
{
    public interface  ICheckerBoardSystem : ISystem
    {
        //属性
        public BindableProperty<ECurRoundType> CurRoundType { get; }
        
        //方法
        public void PlacingChecker(Vector2Int pos);
        public void ReStart();
        public void Stop();
        public Vector2Int[] GetCurMarkPoses();
        
        //事件
        public BindableAction<ECurRoundType> OnGameWin { get; }
        public BindableAction OnGameDraw { get; } // 和局

    }
    
    public class CheckerBoardSystem : AbstractSystem , ICheckerBoardSystem
    {
        public BindableProperty<ECurRoundType> CurRoundType { get; } = new BindableProperty<ECurRoundType>() {Value = ECurRoundType.O};
        
        public BindableProperty<int> WinNum { get; } = new BindableProperty<int>() {Value = 3} ;

        public BindableAction<ECurRoundType> OnGameWin { get; } = new BindableAction< ECurRoundType>();
        public BindableAction OnGameDraw { get; } = new BindableAction();

        private ICheckerboardModel m_CheckerboardModel;
        private Vector2Int[] m_CurMarkPoses;
        private int m_CurGridCount;
        
        public Vector2Int[] GetCurMarkPoses()
        {
            return m_CurMarkPoses;
        }

        protected override void OnInit()
        {
            m_CheckerboardModel = this.GetModel<ICheckerboardModel>();
            m_CurMarkPoses = new Vector2Int[WinNum.Value];
            m_CurGridCount = m_CheckerboardModel.GridCount;
        }

        public void ReStart()
        {
            Stop();
            CurRoundType.Value = ECurRoundType.O;
            m_CheckerboardModel.IsEnd.Value = false;
            m_CurGridCount = m_CheckerboardModel.GridCount;
        }
        
        public void Stop()
        {
            m_CheckerboardModel.Clear();
        }
        
        public void PlacingChecker(Vector2Int pos) 
        {
            if(m_CheckerboardModel.IsEnd)
                return;
            m_CurGridCount--;
            
            CheckerData data = m_CheckerboardModel.GetCheckerAt(pos);
            if (data == null)
            {
                GLog.LogError("坐标越界");
                return;
            }
            
            if (data.Type.Value != CheckerData.CheckerType.NONE)
            {
                GLog.Log("已有棋子，不可操作");
                return;
            }

            CheckerData.CheckerType cType = CurRoundType.Value == ECurRoundType.O
                ? CheckerData.CheckerType.O
                : CheckerData.CheckerType.X;
            
            // 放置棋子
            data.PlacingChecker(cType);
            
            // 检查棋子结果
            bool hasResult = CheckPlayChessResult(pos);

            if (hasResult)
            {
                m_CheckerboardModel.IsEnd.Value = true;
                OnGameWin.Invoke(CurRoundType.Value);
            }
            else if (m_CurGridCount <= 0)
            {
                m_CheckerboardModel.IsEnd.Value = true;
                OnGameDraw.Invoke();
            }
            else
            {
                // 回合切换
                SwitchRound();
            }
        }

        private bool CheckPlayChessResult(Vector2Int pos)
        {
            // 横向
            int count = 0;//统计棋子的数量
            for (int i = pos.x; CheckDataIsCurRound(i, pos.y); i++)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, pos.y);
            }
            for (int i = pos.x - 1; CheckDataIsCurRound(i, pos.y); i--)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, pos.y);
            }
            if (count >= WinNum.Value)
            {
                // Debug.Log("横向连棋");
                return true;
            }

            // 纵向
            count = 0;
            for (int i = pos.y; CheckDataIsCurRound(pos.x, i); i++)
            {
                m_CurMarkPoses[count++] = new Vector2Int(pos.x, i);
            }
            for (int i = pos.y - 1; CheckDataIsCurRound(pos.x, i); i--)
            {
                m_CurMarkPoses[count++] = new Vector2Int(pos.x, i);
            }
            if (count >= WinNum.Value)
            {
                // Debug.Log("纵向连棋");
                return true;
            }
            
            // 右斜线
            count = 0;
            for (int i = pos.x, j = pos.y; CheckDataIsCurRound(i, j); i--, j--)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, j);
            }
            for (int i = pos.x + 1, j = pos.y + 1; CheckDataIsCurRound(i, j); i++, j++)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, j);
            }
            if (count >= WinNum.Value)
            {
                // Debug.Log("右斜线连棋");
                return true;
            }

            // 左斜线
            count = 0;
            for (int i = pos.x, j = pos.y; CheckDataIsCurRound(i, j); i++, j--)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, j);
            }
            for (int i = pos.x - 1, j = pos.y + 1; CheckDataIsCurRound(i, j); i--, j++)
            {
                m_CurMarkPoses[count++] = new Vector2Int(i, j);
            }
            if (count >= WinNum.Value)
            {
                // Debug.Log("左斜线连棋");
                return true;
            }

            return false;
        }

        // 定点是检查当前回合的棋子
        private bool CheckDataIsCurRound(int x, int y)
        {
            var data = m_CheckerboardModel.GetCheckerAt(x, y);
            if (data == null)
            {
                return false;
            }
            if (data.Type.Value == CheckerData.CheckerType.NONE)
            {
                return false;
            }
            
            return data.Type.Value == GetRoundCheckerType();
        }

        private CheckerData.CheckerType GetRoundCheckerType()
        {
            return CurRoundType.Value == ECurRoundType.O ? CheckerData.CheckerType.O : CheckerData.CheckerType.X;
        }

        private void SwitchRound()
        {
            CurRoundType.Value = CurRoundType.Value == ECurRoundType.O ? ECurRoundType.X : ECurRoundType.O;
        }

    }
}