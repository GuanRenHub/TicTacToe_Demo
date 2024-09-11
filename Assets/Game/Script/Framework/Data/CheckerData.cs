#region Author & Verson
// Name : CheckerData.cs
// Author : GuanRen
// CreateTime : 2024/09/09
// Job :
#endregion

using GameFramework;
using UnityEngine;

namespace Game.Framework
{
    public class CheckerData
    {
        public enum CheckerType
        {
            NONE,
            O,
            X,
        }
        
        private Vector2Int m_Pos;
        public Vector2Int Pos => m_Pos;
        
        public BindableProperty<CheckerType> Type { get; } = new BindableProperty<CheckerType>() { Value = CheckerType.NONE };

        internal void PlacingChecker(CheckerType type)
        {
            Type.Value = type;
        }
        
        public CheckerData(int x,int y)
        {
            m_Pos = new Vector2Int(x, y);
        }

        public void Clear()
        {
            Type.Value = CheckerType.NONE;
        }
    }
}