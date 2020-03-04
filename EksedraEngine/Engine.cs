using System.Collections.Generic;
using System;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EksedraEngine {
    public class Engine {
        private List<GameObject> GameObjects;
        public int CurrentRoom = 0;
        public List<Vector2f> RoomSizes;

        private uint WindowWidth, WindowHeight;
        private string WindowTitle;
        private RenderWindow Window;

        private bool Quit;

        public bool[] KeysDown, KeysHeld, KeysUp, KeysOff;
        public Color Background = Color.Yellow;

        public FloatRect ViewPort;

        public Engine(uint windowWidth, uint windowHeight, string windowTitle, List<GameObject> gameObjects, List<Vector2f> roomSizes) {
            GameObjects = gameObjects;
            RoomSizes = roomSizes;

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

        public RenderWindow GetWindow() => Window;
        public uint GetWindowWidth() => WindowWidth;
        public uint GetWindowHeight() => WindowHeight;
        public List<GameObject> GetGameObjects() => GameObjects;
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

                foreach(GameObject gameObject in (self as Engine).GetGameObjects()) {
                    if(gameObject.Room != (self as Engine).CurrentRoom)
                        continue;

                    if(updateDown)
                        gameObject.OnKeyDown((self as Engine).KeysDown);
                    if(updateHeld)
                        gameObject.OnKeyHeld((self as Engine).KeysHeld);
                    if(updateUp)
                        gameObject.OnKeyUp((self as Engine).KeysUp);
                    if(updateOff)
                        gameObject.OnKeyOff((self as Engine).KeysOff);
                }
            }
        }

        private static void UpdateLoop(object self) {
            Clock clock = new Clock();
            float DeltaTime = 0;

            while((self as Engine).GetWindow().IsOpen) {
                DeltaTime = clock.ElapsedTime.AsSeconds();
                clock.Restart();

                foreach(GameObject gameObject in (self as Engine).GetGameObjects()) {
                    if(gameObject.Room != (self as Engine).CurrentRoom && gameObject.Persistant)
                        gameObject.Room = (self as Engine).CurrentRoom;
                        
                    if(gameObject.Room == (self as Engine).CurrentRoom)
                        gameObject.EarlyUpdate(DeltaTime);
                }

                foreach(GameObject gameObject in (self as Engine).GetGameObjects()) {
                    if(gameObject.Room == (self as Engine).CurrentRoom)
                        gameObject.Update(DeltaTime);
                }

                foreach(GameObject gameObject in (self as Engine).GetGameObjects()) {
                    if(gameObject.Room != (self as Engine).CurrentRoom)
                        continue;

                    gameObject.LateUpdate(DeltaTime);

                    gameObject.X += gameObject.HSpeed * DeltaTime;
                    gameObject.Y += gameObject.VSpeed * DeltaTime;

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
                
                foreach(GameObject gameObject in GameObjects) {
                    if(gameObject.Room != CurrentRoom)
                        continue;

                    if(gameObject.SpriteIndex != null) {
                        gameObject.SpriteIndex.MoveTo(gameObject.X, gameObject.Y);
                        gameObject.ImageIndex += DeltaTime * gameObject.ImageSpeed;

                        gameObject.SpriteIndex.ImageSpeed = gameObject.ImageSpeed;
                        gameObject.SpriteIndex.ImageIndex = gameObject.ImageIndex;
                        gameObject.SpriteIndex.SetScale(gameObject.ImageScaleX, gameObject.ImageScaleY);
                    }
                    Window.Draw(gameObject);
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

            foreach(GameObject obj in GameObjects) {
                if(obj.Tag == tag)
                    taggedObjects.Add(obj);
            }

            return taggedObjects;
        }

        private static void CollisionLoop(object self) {
            while((self as Engine).GetWindow().IsOpen) {
                foreach(GameObject gameObject in (self as Engine).GameObjects) {
                    foreach(GameObject other in (self as Engine).GameObjects) {
                        if(!gameObject.Equals(other) && gameObject.Room == other.Room) {
                            // Mask rectangle for obj 1
                            float l1_x = gameObject.X + gameObject.MaskX;
                            float l1_y = (self as Engine).GetWindowHeight() - (gameObject.Y + gameObject.MaskY);
                            float r1_x = gameObject.X + gameObject.MaskX + gameObject.MaskWidth;
                            float r1_y = (self as Engine).GetWindowHeight() - (gameObject.Y + gameObject.MaskY + gameObject.MaskHeight);

                            // Mask rectangle for obj 2
                            float l2_x = other.X + other.MaskX;
                            float l2_y = (self as Engine).GetWindowHeight() - (other.Y + other.MaskY);
                            float r2_x = other.X + other.MaskX + other.MaskWidth;
                            float r2_y = (self as Engine).GetWindowHeight() - (other.Y + other.MaskY + other.MaskHeight);

                            if(RectsIntersect(l1_x, l1_y, r1_x, r1_y, l2_x, l2_y, r2_x, r2_y))
                                gameObject.OnCollision(other);
                        }
                    }
                }
            }
        }

        private static void TimerLoop(object self) {
            Clock clock = new Clock();
            float DeltaTime = 0;

            while((self as Engine).GetWindow().IsOpen) {
                DeltaTime = clock.ElapsedTime.AsSeconds();
                clock.Restart();

                foreach(GameObject gameObject in (self as Engine).GameObjects) {
                    for(int i = 0; i < 10; i++) {
                        if(gameObject.Timers[i] > 0)
                            gameObject.Timers[i] -= DeltaTime;
                        else if(gameObject.Timers[i] != -1) {
                            gameObject.Timers[i] = -1;
                            gameObject.OnTimer(i);
                        }
                    }
                }
            }
        }

        public void Run() {
            Window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), WindowTitle);
            Window.Closed += (sender, args) => { (sender as RenderWindow).Close(); };

            foreach(GameObject gameObject in GameObjects) {
                gameObject.RunningEngine = this;
                gameObject.Init();
            }
            GameObjects.Sort();

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