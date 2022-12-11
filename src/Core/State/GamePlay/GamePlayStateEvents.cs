namespace Core.State.GamePlay
{
    public class GamePlayStateEvents : IBaseGameStateEvent
    {
        public class PlayerShootsBullets : GamePlayStateEvents { }
        public class PlayerShootsMissile : GamePlayStateEvents { }
        public class PlayerDies : GamePlayStateEvents { }

        public class ObjectHitBy : GamePlayStateEvents
        {
            public IGameObjectWithDamage HitBy { get; private set; }
            public ObjectHitBy(IGameObjectWithDamage gameObject)
            {
                HitBy = gameObject;
            }
        }

        public class ObjectLostLife : GamePlayStateEvents
        {
            public int CurrentLife { get; private set; }
            public ObjectLostLife(int currentLife)
            {
                CurrentLife = currentLife;
            }
        }

        public class TurretShoots : GamePlayStateEvents
        {
            public Vector2 Direction { get; private set; }
            public Vector2 Bullet1Position { get; private set; }
            public Vector2 Bullet2Position { get; private set; }
            public float Angle { get; private set; }

            public TurretShoots(Vector2 bullet1Pos, Vector2 bullet2Pos, float angle, Vector2 direction)
            {
                Direction = direction;
                Bullet1Position = bullet1Pos;
                Bullet2Position = bullet2Pos;
                Angle = angle;
            }
        }
    }

}
