using System;

const int WIDTH = 1280;
const int HEIGHT = 720;

using var game = new Core.Game1942(WIDTH, HEIGHT, new Core.State.Splash.SplashGameState());
game.IsFixedTimeStep = true;
game.TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60);
game.Run();

