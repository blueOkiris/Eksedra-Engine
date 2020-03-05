using EksedraEngine;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System;

namespace test {
    struct Map {
        public List<GameObject> GameObjects;
        public Vector2f RoomSize;

        public Map(List<GameObject> gameObjects, Vector2f roomSize) {
            GameObjects = gameObjects;
            RoomSize = roomSize;
        }
    }

    class Maps {
        // Individual rooms
        public static Map Room0() {
            // Add persistant objects
            /*List<GameObject> gameObjects = new List<GameObject>();
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
            gameObjects.Add(new JumpThrough(6 * 64, 720 - 2 * 64, 0));
            gameObjects.Add(new JumpThrough(7 * 64, 720 - 2 * 64, 0));
            gameObjects.Add(new JumpThrough(7 * 64, 720 - 3 * 64, 0));
            gameObjects.Add(new JumpThrough(8 * 64, 720 - 3 * 64, 0));
            gameObjects.Add(new JumpThrough(8 * 64, 720 - 4 * 64, 0));
            gameObjects.Add(new JumpThrough(9 * 64, 720 - 4 * 64, 0));

            return new Map(gameObjects, new Vector2f(1280, 720));*/
            
            GameRoom rm0 = Engine.RoomFromFile(
                                    "rooms/room0.rm",
                                    new List<Type>() {
                                        typeof(ControlObject),
                                        typeof(Player),
                                        typeof(Rock),
                                        typeof(JumpThrough)
                                    });
            return new Map(rm0.GameObjects, rm0.RoomSize);
        }

        public static Map Room1() {
            List<GameObject> gameObjects = new List<GameObject>();

            // Add a mini level layout for room 1 (just floor, but longer)
            for(int i = 0; i < 2560 / 64; i++)
                gameObjects.Add(new Rock(32 + i * 64, 720 - 32, 1));
        
            return new Map(gameObjects, new Vector2f(2560, 720));
        }

        // Get all the map data together
        public static List<GameObject> AllGameObjects() {
            List<GameObject> gameObjects = new List<GameObject>();

            List<GameObject> room0Objects = Room0().GameObjects;
            List<GameObject> room1Objects = Room1().GameObjects;

            room0Objects.ForEach(obj => gameObjects.Add(obj));
            room0Objects.ForEach(obj => gameObjects.Add(obj));

            return gameObjects;
        }

        public static List<Vector2f> RoomSizes() {
            List<Vector2f> roomSizes = new List<Vector2f>();
            
            roomSizes.Add(Room0().RoomSize);
            roomSizes.Add(Room1().RoomSize);

            return roomSizes;
        }
    }
}