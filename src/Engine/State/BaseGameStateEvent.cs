namespace Engine.State
{
    /// <summary>
    /// BaseGameStateEvents all event of the game
    /// </summary>
    public  class BaseGameStateEvent : IBaseGameStateEvent
    {
        /// <summary>
        /// Event Game Quit
        /// </summary>
        public class GameQuit : IBaseGameStateEvent { }
        /// <summary>
        /// Event Game Pause
        /// </summary>
        public class Pause: IBaseGameStateEvent { }
        /// <summary>
        /// Event Game Over
        /// </summary>
        public class GameOver : IBaseGameStateEvent { }
        /// <summary>
        /// Event Change into menus or pause or new phase or stage
        /// </summary>
        public class ChangePhase : IBaseGameStateEvent { }
        public class Nothing : IBaseGameStateEvent { }
    }
}
