#region Author & Verson
// Name : CheckerboardModel.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job :
#endregion

using GameFramework;
using UnityEngine;

namespace Game.Framework
{
    public interface ICheckerboardModel : IModel
    {
        public int RowCount { get; }
        public int ColCount { get; }
        public int GridCount { get; }
        public BindableProperty<bool> IsEnd { get; }

        public CheckerData[] GetCheckers();
        public CheckerData GetCheckerAt(Vector2Int pos);
        public CheckerData GetCheckerAt(int x, int y);
        public void Clear();
    }

    public class CheckerboardModel : AbstractModel, ICheckerboardModel
    {
        private CheckerData[,] m_Checkers = new CheckerData[3,3];
        public BindableProperty<bool> IsEnd { get; } = new BindableProperty<bool>() { Value = false };
        public int RowCount => m_Checkers.GetLength(0);
        public int ColCount => m_Checkers.GetLength(1);
        public int GridCount => RowCount * ColCount;


        protected override void OnInit()
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColCount; x++)
                {
                    m_Checkers[y, x] = new CheckerData(x, y);
                }
            }
        }

        public void Clear()
        {
            for (var y = 0; y < RowCount; y++)
            {
                for (var x = 0; x < ColCount; x++)
                {
                    m_Checkers[y, x].Clear();
                }
            }
        }
        
        public CheckerData GetCheckerAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= ColCount || pos.y < 0 || pos.y >= RowCount)
            {
                return null;
            }
            return m_Checkers[pos.y, pos.x];
        }

        public CheckerData GetCheckerAt(int x, int y)
        {
            if (x < 0 || x >= ColCount || y < 0 || y >= RowCount)
            {
                return null;
            }

            return GetCheckerAt(new Vector2Int(x, y));
        }

        public CheckerData[] GetCheckers()
        {
            int row = m_Checkers.GetLength(0);
            int col = m_Checkers.GetLength(1);
            CheckerData[] checkers = new CheckerData[row * col];
            for (var y = 0; y < row; y++)
            {
                for (var x = 0; x < col; x++)
                {
                    checkers[y * col + x] = m_Checkers[y, x];
                }
            }
            return checkers;
        }
    }
}