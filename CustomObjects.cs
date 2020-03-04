using SFML.Window;
using SFML.Graphics;
using System;
using System.Linq;
using SFML.System;
using System.IO;

namespace EksedraEngine {
    /*class ObjectName : GameObject {
        public override void Draw(RenderTarget target, RenderStates states) {}
        public override void Init() {}
        public override void EarlyUpdate(float deltaTime) {}
        public override void Update(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[] keyState) {}
        public override void OnKeyDown(bool[] keyState) {}
        public override void OnKeyOff(bool[]  keyState) {}
        public override void OnKeyHeld(bool[] keyState) {}
        public override void OnCollision(GameObject other) {}
        public override void OnTimer(int timerIndex) {}

        public ObjectName(int room) {
            Room = room;
        }
    }*/

    class ControlObject : GameObject {
        public override void Draw(RenderTarget target, RenderStates states) {}
        public override void EarlyUpdate(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[] keyState) {}
        public override void OnKeyOff(bool[]  keyState) {}
        public override void OnKeyHeld(bool[] keyState) {}
        public override void OnCollision(GameObject other) {}
        public override void Update(float deltaTime) {}
        public override void OnTimer(int timerIndex) {}
        
        public override void Init() {
            Tag = "Control";
        }

        public override void OnKeyDown(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Escape])
                RunningEngine.SetQuit(true);
        }
        public ControlObject(int room) {
            Room = room;
            Persistant = true;
        }
    }

    class JumpThrough : GameObject {
        public override void EarlyUpdate(float deltaTime) {}
        public override void Update(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[] keyState) {}
        public override void OnKeyDown(bool[] keyState) {}
        public override void OnKeyOff(bool[]  keyState) {}
        public override void OnKeyHeld(bool[] keyState) {}
        public override void OnTimer(int timerIndex) {}
        
        public override void OnCollision(GameObject other) {
            //Console.WriteLine("Rock collision with: " + other.Tag);
        }

        public JumpThrough(int x, int y, int room) {
            X = x;
            Y = y;
            Room = room;
        }
        
        public override void Init() {
            Tag = "JumpThrough";
            Depth = 1;

            SpriteIndex = new EksedraSprite("images/jumpthrough.png", new IntRect[] { new IntRect(0, 0, 64, 64) });
            ImageSpeed = 0;
            ImageIndex = 0;

            MaskX = -SpriteIndex.GetWidth() / 2;
            MaskY = -SpriteIndex.GetHeight() / 2;
            MaskWidth = SpriteIndex.GetWidth();
            MaskHeight = SpriteIndex.GetHeight();
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            target.Draw(SpriteIndex);
        }
    }

    class Rock : GameObject {
        public override void EarlyUpdate(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[] keyState) {}
        public override void OnKeyDown(bool[] keyState) {}
        public override void OnKeyOff(bool[]  keyState) {}
        public override void OnKeyHeld(bool[] keyState) {}
        public override void OnTimer(int timerIndex) {}
        
        public override void OnCollision(GameObject other) {
            //Console.WriteLine("Rock collision with: " + other.Tag);
        }

        public Rock(int x, int y, int room) {
            X = x;
            Y = y;
            Room = room;
        }
        
        public override void Init() {
            Tag = "Rock";
            Depth = 1;

            SpriteIndex = new EksedraSprite("images/rock.png", new IntRect[] { new IntRect(0, 0, 64, 64) });
            ImageSpeed = 0;
            ImageIndex = 0;

            MaskX = -SpriteIndex.GetWidth() / 2;
            MaskY = -SpriteIndex.GetHeight() / 2;
            MaskWidth = SpriteIndex.GetWidth();
            MaskHeight = SpriteIndex.GetHeight();
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            target.Draw(SpriteIndex);
        }

        public override void Update(float deltaTime) {
            //Console.WriteLine("Rock: { X: " + X + ", Y: " + Y + ", HSpeed: " + HSpeed + ", VSpeed: " + VSpeed + " }");
        }
    }

    class Player : GameObject {
        private float MoveSpeed = 512;
        private float Gravity = 4096;
        private float MaxVSpeed = 8192;
        private float JumpSpeed = 1300;
        private bool IsGrounded = false;

        public override void EarlyUpdate(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[]  keyState) {}
        public override void OnTimer(int timerIndex) {}

        private EksedraSprite PlayerStand, PlayerFall, PlayerRun;

        public Player(int x, int y, int room) {
            Room = room;

            X = x;
            Y = y;
        }

        public override void Init() {
            Tag = "Player";
            Room = 0;
            Depth = 0;

            PlayerStand = new EksedraSprite("images/player.png", new IntRect[] {
                                                new IntRect(1, 1, 64, 64),
                                                new IntRect(131, 1, 64, 64)
                                            });
            PlayerFall = new EksedraSprite("images/player.png", new IntRect[] { new IntRect(66, 1, 64, 64) });
            PlayerRun = new EksedraSprite("images/player.png", new IntRect[] {
                                                new IntRect(1, 66, 64, 64),
                                                new IntRect(66, 66, 64, 64)
                                            });

            SpriteIndex = PlayerStand;
            ImageSpeed = 10;
            MaskX = -16;
            MaskY = -32;
            MaskWidth = 32;
            MaskHeight = 64;

            Persistant = true;
        }

        public override void OnCollision(GameObject other) {
            //Console.WriteLine("Player collision with: " + other.Tag);
            //File.AppendAllText("log.txt", "Rock: " + (other.Y - other.SpriteIndex.GetCurrentRect().Height / 2) + "\n" + "Plyr: " + (Y + SpriteIndex.GetCurrentRect().Height / 2) + "\n");
            if(other.Tag == "Rock") {
                if(other.Y - other.SpriteIndex.GetCurrentRect().Height / 2 > Y + 31 && VSpeed > 0) {
                    VSpeed = 0;
                    Y = other.Y - other.SpriteIndex.GetCurrentRect().Height / 2 - 33;
                    IsGrounded = true;
                }
                
                if(other.Y + other.SpriteIndex.GetCurrentRect().Height / 2 < Y - 31 && VSpeed < 0) {
                    VSpeed = 0;
                    Y = other.Y + other.SpriteIndex.GetCurrentRect().Height / 2 + 32;
                }
                
                if(other.X - other.SpriteIndex.GetCurrentRect().Width / 2 > X + 15 && HSpeed > 0) {
                    HSpeed = 0;
                    X = other.X - other.SpriteIndex.GetCurrentRect().Width / 2 - 16;
                }
                
                if(other.X + other.SpriteIndex.GetCurrentRect().Width / 2 < X - 15 && HSpeed < 0) {
                    HSpeed = 0;
                    X = other.X + other.SpriteIndex.GetCurrentRect().Width / 2 + 16;
                }
            } else if(other.Tag == "JumpThrough") {
                if(other.Y - other.SpriteIndex.GetCurrentRect().Height / 2 > Y + 31 && VSpeed > 0) {
                    VSpeed = 0;
                    Y = other.Y - other.SpriteIndex.GetCurrentRect().Height / 2 - 33;
                    IsGrounded = true;
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            target.Draw(SpriteIndex);
        }

        public override void Update(float deltaTime) {
            //Console.WriteLine(RunningEngine.GetWindowWidth() + ", " + RunningEngine.GetWindowHeight());
            if(X + SpriteIndex.GetCurrentRect().Width / 2 > RunningEngine.RoomSizes[RunningEngine.CurrentRoom].X) {
                X = RunningEngine.RoomSizes[RunningEngine.CurrentRoom].X - SpriteIndex.GetCurrentRect().Width / 2;
                HSpeed = 0;
            } else if(X - SpriteIndex.GetCurrentRect().Width / 2 < 0) {
                X = SpriteIndex.GetCurrentRect().Width / 2;
                HSpeed = 0;
            } else if(Y + SpriteIndex.GetCurrentRect().Height / 2 > RunningEngine.RoomSizes[RunningEngine.CurrentRoom].Y) {
                Y = RunningEngine.RoomSizes[RunningEngine.CurrentRoom].Y - SpriteIndex.GetCurrentRect().Height / 2;
                VSpeed = 0;
            } else if(Y - SpriteIndex.GetCurrentRect().Height / 2 < 0) {
                Y = SpriteIndex.GetCurrentRect().Height / 2;
                VSpeed = 0;
            }

            if(VSpeed < MaxVSpeed)
                VSpeed += Gravity * deltaTime;

            // Animate
            if(IsGrounded)
                SpriteIndex = Math.Abs(HSpeed) > 0 ? PlayerRun : PlayerStand;
            else
                SpriteIndex = PlayerFall;
            
            if(HSpeed > 0)
                ImageScaleX = 1;
            else if(HSpeed < 0)
                ImageScaleX = -1;

            // Move the view
            if(X - RunningEngine.ViewPort.Width / 2 < 0)
                RunningEngine.ViewPort.Left = 0;
            else if(X + RunningEngine.ViewPort.Width / 2 > RunningEngine.RoomSizes[RunningEngine.CurrentRoom].X)
                RunningEngine.ViewPort.Left = RunningEngine.RoomSizes[RunningEngine.CurrentRoom].X - RunningEngine.ViewPort.Width;
            else
                RunningEngine.ViewPort.Left = X - RunningEngine.ViewPort.Width / 2;

            if(Y - RunningEngine.ViewPort.Height / 2 < 0)
                RunningEngine.ViewPort.Top = 0;
            else if(Y + RunningEngine.ViewPort.Height / 2 > RunningEngine.RoomSizes[RunningEngine.CurrentRoom].Y)
                RunningEngine.ViewPort.Top = RunningEngine.RoomSizes[RunningEngine.CurrentRoom].Y - RunningEngine.ViewPort.Height;
            else
                RunningEngine.ViewPort.Top = Y - RunningEngine.ViewPort.Height / 2;
        }

        public override void OnKeyDown(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Up] && IsGrounded) {
                VSpeed = -JumpSpeed;
                IsGrounded = false;
            } else if(keyState[(int) Keyboard.Key.N] && RunningEngine.CurrentRoom < RunningEngine.RoomSizes.Count - 1)
                RunningEngine.CurrentRoom++;
            else if(keyState[(int) Keyboard.Key.P] && RunningEngine.CurrentRoom > 0)
                RunningEngine.CurrentRoom--;
        }

        public override void OnKeyHeld(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Left])
                HSpeed = -MoveSpeed;
            else if(keyState[(int) Keyboard.Key.Right])
                HSpeed = MoveSpeed;
        }
        
        public override void OnKeyOff(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Left] && HSpeed < 0)
                HSpeed = 0;
            else if(keyState[(int) Keyboard.Key.Right] && HSpeed > 0)
                HSpeed = 0;
        }
    }
}