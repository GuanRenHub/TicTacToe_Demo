#region Author & Verson
// Name : TicTacToeSystem.cs
// Author : GuanRen
// CreateTime : 2024/09/11
// Job :
#endregion

using GameFramework;

namespace Game.Framework
{
    public interface ITicTacToeSystem : ISystem
    {
        public BindableProperty<ETicTacToeType> GameType { get; }

        public void StartGame(ETicTacToeType type);
    }
    
    public class TicTacToeSystem : AbstractSystem, ITicTacToeSystem
    {
        public BindableProperty<ETicTacToeType> GameType { get; } = new BindableProperty<ETicTacToeType>()
            {Value = ETicTacToeType.NONE};

        protected override void OnInit()
        {
            
        }

        public void StartGame(ETicTacToeType type)
        {
            GameType.Value = type;
        }
    }
}