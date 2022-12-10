namespace Engine.State
{
    public  class BaseGameStateEvent : IBaseGameStateEvent
    {
        public class GameQuit : IBaseGameStateEvent { }
        public class Pause: IBaseGameStateEvent { }
        public class GameOver : IBaseGameStateEvent { }
        public class ChangePhase : IBaseGameStateEvent { }
    }
}
