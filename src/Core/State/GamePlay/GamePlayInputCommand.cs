namespace Core.State.GamePlay
{
    public class GamePlayInputCommand : IBaseInputCommand
    {
        public class GameMenu: IBaseInputCommand { };
        public class GamePause: IBaseInputCommand { };
        public class PlayerMoveUp: IBaseInputCommand { };
        public class PlayerMoveDown: IBaseInputCommand { };
        public class PlayerMoveLeft: IBaseInputCommand { };
        public class PlayerMoveRight: IBaseInputCommand { };
        public class PlayerActionA: IBaseInputCommand { };
        public class PlayerActionB: IBaseInputCommand { };
        public class PlayerActionY : IBaseInputCommand { };
        public class PlayerActionX : IBaseInputCommand { };

        public class PlayerActionL1 : IBaseInputCommand { };
        public class PlayerActionR1: IBaseInputCommand { };

        public class PlayerActionL2 : IBaseInputCommand { };
        public class PlayerActionR2 : IBaseInputCommand { };
    }
}
