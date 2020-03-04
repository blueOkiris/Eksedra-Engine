using System.Collections.Generic;
using SFML.System;
using EksedraEngine;

namespace test {
    class Program {
        static void Main(string[] args) {
            List<GameObject> gameObjects = Maps.AllGameObjects();
            List<Vector2f> roomSizes = Maps.RoomSizes();
            Engine engine = new Engine(1280, 720, "Eksedra Engine", gameObjects, roomSizes);
            engine.Run();
        }
    }
}
