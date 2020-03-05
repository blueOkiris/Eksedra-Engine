using System.Collections.Generic;
using System;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.Linq;

namespace EksedraEngine {
    public class GameRoom {
        public List<GameObject> GameObjects;
        public Vector2f RoomSize;

        public GameRoom(List<GameObject> gameObjects, Vector2f roomSize) {
            GameObjects = gameObjects;
            RoomSize = roomSize;
        }
    }

    public class Engine {
        private Dictionary<string, List<GameObject>> GameObjects;
        private List<GameObject> PersistantObjects;
        private Dictionary<string, Vector2f> RoomSizes;
        public string CurrentRoom;

        private uint WindowWidth, WindowHeight;
        private string WindowTitle;
        private RenderWindow Window;

        private bool Quit;

        public bool[] KeysDown, KeysHeld, KeysUp, KeysOff;
        public Color Background = Color.Yellow;

        public FloatRect ViewPort;

        public Engine(uint windowWidth, uint windowHeight, string windowTitle, string startRoom, List<Type> customGameObjectTypes) {
            CurrentRoom = startRoom;
            LoadAllRooms(customGameObjectTypes);

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            WindowTitle = windowTitle;

            Quit = false;

            KeysDown = new bool[(int) Keyboard.Key.KeyCount];
            KeysUp= new bool[(int) Keyboard.Key.KeyCount];
            KeysHeld = new bool[(int) Keyboard.Key.KeyCount];
            KeysOff = new bool[(int) Keyboard.Key.KeyCount];

            for(int i = 0; i < (int) Keyboard.Key.KeyCount; i++) {
                KeysDown[i] = false;
                KeysUp[i] = false;
                KeysHeld[i] = false;
                KeysOff[i] = true;
            }

            ViewPort = new FloatRect(0, 0, WindowWidth / 2, WindowHeight / 2);
        }

        private static GameRoom RoomFromFile(string fileName, List<Type> gameObjectTypes) {
            string roomCode = File.ReadAllText(fileName);
            var asm = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "EksedraEngine");
            ScriptOptions options = ScriptOptions.Default.WithImports(new List<string>() {
                                        "System.Collections.Generic",
                                        "SFML.System",
                                        "EksedraEngine",
                                        "test"
                                    }).AddReferences(asm);
            foreach(Type t in gameObjectTypes)
                options = options.AddReferences(t.Assembly);

            Script<GameRoom> script = CSharpScript.Create<GameRoom>(roomCode, options);
            script.Compile();

            return script.RunAsync().Result.ReturnValue;
        }

        private void LoadAllRooms(List<Type> gameObjectTypes) {
            DirectoryInfo levelFolder = new DirectoryInfo("rooms/");
            Dictionary<string, List<GameObject>> gameObjects = new Dictionary<string, List<GameObject>>();
            List<GameObject> persistantObjects = new List<GameObject>();
            Dictionary<string, Vector2f> roomSizes = new Dictionary<string, Vector2f>();

            foreach(FileInfo file in levelFolder.GetFiles()) {
                GameRoom fileRoom = RoomFromFile("rooms/" + file.Name, gameObjectTypes);

                for(int i = 0; i < fileRoom.GameObjects.Count; i++) {
                    if(fileRoom.GameObjects[i].Persistant) {
                        persistantObjects.Add(fileRoom.GameObjects[i]);
                        fileRoom.GameObjects.RemoveAt(i);
                        i--;
                    }
                }
                gameObjects.Add(file.Name, fileRoom.GameObjects);
                roomSizes.Add(file.Name, fileRoom.RoomSize);
            }

            GameObjects = gameObjects;
            PersistantObjects = persistantObjects;
            RoomSizes = roomSizes;
        }

        public RenderWindow GetWindow() => Window;
        public uint GetWindowWidth() => WindowWidth;
        public uint GetWindowHeight() => WindowHeight;
        public List<GameObject> GetGameObjects() {
            List<GameObject> gameObjects = new List<GameObject>();

            foreach(GameObject gameObject in GameObjects[CurrentRoom])
                gameObjects.Add(gameObject);
            
            foreach(GameObject gameObject in PersistantObjects)
                gameObjects.Add(gameObject);

            return gameObjects;
        }
        public Vector2f GetRoomSize() => RoomSizes[CurrentRoom];
        public void SetQuit(bool quit) { Quit = quit; }
        
        private static void KeyLoop(object self) {
            while((self as Engine).GetWindow().IsOpen) {
                bool updateDown = false, updateHeld = false, updateUp = false, updateOff = false;
                //Console.WriteLine("Left pressed: " + Keyboard.IsKeyPressed(Keyboard.Key.Left));

                for(Keyboard.Key key = (Keyboard.Key) 0; (int) key < (int) Keyboard.Key.KeyCount; key = (Keyboard.Key) ((int) key + 1)) {
                    if(Keyboard.IsKeyPressed(key)) {
                        // Key was pressed. Check if not pressed before, had just been pressed before, or has been pressed for a while
                        (self as Engine).KeysUp[(int) key] = false;
                        (self as Engine).KeysOff[(int) key] = false;

                        if(!(self as Engine).KeysDown[(int) key] && !(self as Engine).KeysHeld[(int) key]) {
                            (self as Engine).KeysDown[(int) key] = true;

                            updateDown = true;
                        } else if((self as Engine).KeysDown[(int) key]) {
                            (self as Engine).KeysDown[(int) key] = false;
                            (self as Engine).KeysHeld[(int) key] = true;

                            updateHeld = true;
                        } else
                            updateHeld = true;
                    } else {
                        // Key not pressed. Check if just let go, or been not pressed for a while
                        (self as Engine).KeysDown[(int) key] = false;
                        (self as Engine).KeysHeld[(int) key] = false;

                        if(!(self as Engine).KeysUp[(int) key] && !(self as Engine).KeysOff[(int) key]) {
                            (self as Engine).KeysUp[(int) key] = true;

                            updateUp = true;
                        } else if((self as Engine).KeysUp[(int) key]) {
                            (self as Engine).KeysUp[(int) key] = false;
                            (self as Engine).KeysOff[(int) key] = true;

                            updateOff = true;
                        } else
                            updateOff = true;
                    }
                }

                // Get game objects will only give for current room!
                // DO NOT USE FOREACH HERE IT WILL NOT WORK
                List<GameObject> gameObjects = (self as Engine).GetGameObjects();
                for(int i = 0; i < gameObjects.Count; i++) {
                    if(updateDown)
                        gameObjects[i].OnKeyDown((self as Engine).KeysDown);
                    if(updateHeld)
                        gameObjects[i].OnKeyHeld((self as Engine).KeysHeld);
                    if(updateUp)
                        gameObjects[i].OnKeyUp((self as Engine).KeysUp);
                    if(updateOff)
                        gameObjects[i].OnKeyOff((self as Engine).KeysOff);
                }
            }
        }

        private static void UpdateLoop(object self) {
            Clock clock = new Clock();
            float DeltaTime = 0;

            while((self as Engine).GetWindow().IsOpen) {
                DeltaTime = clock.ElapsedTime.AsSeconds();
                clock.Restart();

                // DO NOT USE FOREACH IN ANY OF THESE IT WON'T WORK
                List<GameObject> gameObjects = (self as Engine).GetGameObjects();
                for(int i = 0; i < gameObjects.Count; i++)
                    gameObjects[i].EarlyUpdate(DeltaTime);

                for(int i = 0; i < gameObjects.Count; i++)
                        gameObjects[i].Update(DeltaTime);

                for(int i = 0; i < gameObjects.Count; i++) {
                    gameObjects[i].LateUpdate(DeltaTime);

                    gameObjects[i].X += gameObjects[i].HSpeed * DeltaTime;
                    gameObjects[i].Y += gameObjects[i].VSpeed * DeltaTime;

                    //Console.WriteLine("HSpeed: " + gameObject.HSpeed + ", VSpeed: " + gameObject.VSpeed);
                }
            }
        }

        private void DrawLoop() {
            Clock clock = new Clock();
            float DeltaTime = 0;
            
            while(Window.IsOpen) {
                do {
                    DeltaTime = clock.ElapsedTime.AsSeconds();
                } while(DeltaTime < (float) 1 / 30);
                clock.Restart();

                Window.DispatchEvents();
                if(Quit)
                    Window.Close();

                Window.SetView(new View(ViewPort));
                Window.Clear();

                // Back rect
                RectangleShape backShape = new RectangleShape(new Vector2f(RoomSizes[CurrentRoom].X, RoomSizes[CurrentRoom].Y));
                backShape.Position = new Vector2f(0, 0);
                backShape.FillColor = Background;
                Window.Draw(backShape);
                
                // DONT USE FOREACH
                List<GameObject> gameObjects = GetGameObjects();
                for(int i = 0; i < gameObjects.Count; i++) {
                    if(gameObjects[i].SpriteIndex != null) {
                        gameObjects[i].SpriteIndex.MoveTo(gameObjects[i].X, gameObjects[i].Y);
                        gameObjects[i].ImageIndex += DeltaTime * gameObjects[i].ImageSpeed;

                        gameObjects[i].SpriteIndex.ImageSpeed = gameObjects[i].ImageSpeed;
                        gameObjects[i].SpriteIndex.ImageIndex = gameObjects[i].ImageIndex;
                        gameObjects[i].SpriteIndex.SetScale(gameObjects[i].ImageScaleX, gameObjects[i].ImageScaleY);
                    }
                    Window.Draw(gameObjects[i]);
                }

                Window.Display();
            }
        }

        private static bool RectsIntersect(float l1_x, float l1_y, float r1_x, float r1_y, float l2_x, float l2_y, float r2_x, float r2_y) {
            if(l1_x > r2_x || l2_x > r1_x)
                return false;
            
            if(l1_y < r2_y || l2_y < r1_y)
                return false;

            return true;
        }

        public List<GameObject> FindGameObjectsWithTag(string tag) {
            List<GameObject> taggedObjects = new List<GameObject>();

            foreach(GameObject obj in GetGameObjects()) {
                if(obj.Tag == tag)
                    taggedObjects.Add(obj);
            }

            return taggedObjects;
        }

        private static void CollisionLoop(object self) {
            while((self as Engine).GetWindow().IsOpen) {
                List<GameObject> gameObjects = (self as Engine).GetGameObjects();
                // DON'T USE FOREACH HERE
                for(int i = 0; i < gameObjects.Count; i++) {
                    for(int j = 0; j < gameObjects.Count; j++) {
                        if(!gameObjects[i].Equals(gameObjects[j])) {
                            // Mask rectangle for obj 1
                            float l1_x = gameObjects[i].X + gameObjects[i].MaskX;
                            float l1_y = (self as Engine).GetWindowHeight() - (gameObjects[i].Y + gameObjects[i].MaskY);
                            float r1_x = gameObjects[i].X + gameObjects[i].MaskX + gameObjects[i].MaskWidth;
                            float r1_y = (self as Engine).GetWindowHeight() - (gameObjects[i].Y + gameObjects[i].MaskY + gameObjects[i].MaskHeight);

                            // Mask rectangle for obj 2
                            float l2_x = gameObjects[j].X + gameObjects[j].MaskX;
                            float l2_y = (self as Engine).GetWindowHeight() - (gameObjects[j].Y + gameObjects[j].MaskY);
                            float r2_x = gameObjects[j].X + gameObjects[j].MaskX + gameObjects[j].MaskWidth;
                            float r2_y = (self as Engine).GetWindowHeight() - (gameObjects[j].Y + gameObjects[j].MaskY + gameObjects[j].MaskHeight);

                            if(RectsIntersect(l1_x, l1_y, r1_x, r1_y, l2_x, l2_y, r2_x, r2_y))
                                gameObjects[i].OnCollision(gameObjects[j]);
                        }
                    }
                }
            }
        }

        public bool CheckCollision(float x, float y, GameObject self, Type otherGameObjectType, Func<GameObject, GameObject, bool> test, ref GameObject other) {
            // If my object moves to (x, y) will there be a collision?
            List<GameObject> gameObjects = GetGameObjects();
            for(int j = 0; j < gameObjects.Count; j++) {
                if(gameObjects[j].GetType() == otherGameObjectType && !gameObjects[j].Equals(self)) {
                    // Mask rectangle for obj 1
                    float l1_x = x + self.MaskX;
                    float l1_y = WindowHeight - (y + self.MaskY);
                    float r1_x = x + self.MaskX + self.MaskWidth;
                    float r1_y = WindowHeight - (y + self.MaskY + self.MaskHeight);

                    // Mask rectangle for obj 2
                    float l2_x = gameObjects[j].X + gameObjects[j].MaskX;
                    float l2_y = WindowHeight - (gameObjects[j].Y + gameObjects[j].MaskY);
                    float r2_x = gameObjects[j].X + gameObjects[j].MaskX + gameObjects[j].MaskWidth;
                    float r2_y = WindowHeight - (gameObjects[j].Y + gameObjects[j].MaskY + gameObjects[j].MaskHeight);
                    
                    if(RectsIntersect(l1_x, l1_y, r1_x, r1_y, l2_x, l2_y, r2_x, r2_y) && test(self, gameObjects[j])) {
                        other = gameObjects[j];
                        return true;
                    }
                }
            }

            return false;
        }

        private static void TimerLoop(object self) {
            Clock clock = new Clock();
            float DeltaTime = 0;

            while((self as Engine).GetWindow().IsOpen) {
                DeltaTime = clock.ElapsedTime.AsSeconds();
                clock.Restart();

                // DO NOT USE FOREACH LOOP HERE
                List<GameObject> gameObjects = (self as Engine).GetGameObjects();
                for(int j = 0; j < gameObjects.Count; j++) {
                    for(int i = 0; i < 10; i++) {
                        if(gameObjects[j].Timers[i] > 0)
                            gameObjects[j].Timers[i] -= DeltaTime;
                        else if(gameObjects[j].Timers[i] != -1) {
                            gameObjects[j].Timers[i] = -1;
                            gameObjects[j].OnTimer(i);
                        }
                    }
                }
            }
        }

        public void Run() {
            Window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), WindowTitle);
            Window.Closed += (sender, args) => { (sender as RenderWindow).Close(); };

            foreach(string room in GameObjects.Keys) {
                foreach(GameObject gameObject in GameObjects[room]) {
                    gameObject.RunningEngine = this;
                    gameObject.Init();
                }

                GameObjects[room].Sort();
            }

            foreach(GameObject gameObject in PersistantObjects) {
                gameObject.RunningEngine = this;
                gameObject.Init();
            }
            
            PersistantObjects.Sort();

            Thread updateThread = new Thread(new ParameterizedThreadStart(UpdateLoop));
            Thread keyThread = new Thread(new ParameterizedThreadStart(KeyLoop));
            Thread collisionThread = new Thread(new ParameterizedThreadStart(CollisionLoop));
            Thread timerThread = new Thread(new ParameterizedThreadStart(TimerLoop));
            updateThread.Start(this);
            keyThread.Start(this);
            collisionThread.Start(this);
            timerThread.Start(this);

            DrawLoop();

            updateThread.Join();
            keyThread.Join();
            collisionThread.Join();
            timerThread.Join();
        }
    }
}