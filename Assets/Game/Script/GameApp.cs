#region Author & Verson
// Name : GameApp.cs
// Author : GuanRen
// CreateTime : 2024/09/09
#endregion

using Game.Framework;
using GameFramework;

namespace Game
{
    public class GameApp: Architecture<GameApp>
    {
        protected override void Init()
        {
            //model
            this.RegisterModel<ICheckerboardModel>(new CheckerboardModel());
            
            //system
            this.RegisterSystem<ICheckerBoardSystem>(new CheckerBoardSystem());
            this.RegisterSystem<ITicTacToeSystem>(new TicTacToeSystem());
        }
    }
}