namespace Core.State.GamePlay
{
    public  class GamePlayStateEvents : IBaseGameStateEvent
    {
        public class PlayerShoots: GamePlayStateEvents { }
        public class PlayerShootsMissiles : GamePlayStateEvents { }
        public class PlayerDies : GamePlayStateEvents { }

        public class ChopperHitBy : GamePlayStateEvents 
        { 
            public IGameObjectWithDamage HitBy { get; private set; }
            public ChopperHitBy(IGameObjectWithDamage damage) 
            { 
                HitBy = damage;
            }
        }

        public class EnemyLostLife : GamePlayStateEvents
        {
            public int CurrentLife { get; private set; }
            public EnemyLostLife(int currentLife)
            {
                CurrentLife = currentLife;
            }
        }


    }
}
