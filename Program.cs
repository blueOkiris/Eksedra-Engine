using System.Collections.Generic;
using SFML.System;

namespace EksedraEngine {
    class Program {
        static void Main(string[] args) {
            // Add persistant objects
            List<GameObject> gameObjects = new List<GameObject>();
            gameObjects.Add(new ControlObject(0));
            gameObjects.Add(new Player(32 + 3 * 64, 720 - 2 * 64, 0));

            // Add a mini level layout for room 0
            for(int i = 0; i < 1280 / 64; i++)
                gameObjects.Add(new Rock(32 + i * 64, 720 - 32, 0));
            gameObjects.Add(new Rock(32, 720 - 32 - 64, 0));
            gameObjects.Add(new Rock(32, 720 - 32 - 64 * 2, 0));
            gameObjects.Add(new Rock(32 + 64, 720 - 32 - 64, 0));
            for(int j = 0; j < 4; j++) {
                for(int i = j; i < (1280 / 2) / 64; i++)
                    gameObjects.Add(new Rock(32 + i * 64 + (1280 / 2), 720 / 2 - 64 * j, 0));
            }
            gameObjects.Add(new JumpThrough(32 + 5 * 64, 720 - 3 * 64, 0));
            gameObjects.Add(new JumpThrough(32 + 6 * 64, 720 - 3 * 64, 0));
            gameObjects.Add(new JumpThrough(32 + 7 * 64, 720 - 3 * 64, 0));

            // Add a mini level layout for room 1 (just floor, but longer)
            for(int i = 0; i < 2560 / 64; i++)
                gameObjects.Add(new Rock(32 + i * 64, 720 - 32, 1));

            List<Vector2f> roomSizes = new List<Vector2f>();
            roomSizes.Add(new Vector2f(1280, 720));
            roomSizes.Add(new Vector2f(2560, 720));

            Engine engine = new Engine(1280, 720, "Eksedra Engine", gameObjects, roomSizes);
            engine.Run();
        }
    }
}
