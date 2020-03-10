/*
 * This file is used to make custom game objects (children of base GameObject)
 * These are instantiated in room files at the start of the application
 */
using SFML.Window;
using SFML.Graphics;
using System;
using System.Linq;
using SFML.System;
using System.IO;
using EksedraEngine;

namespace test {
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
        public ObjectName() { }
    }*/

    public class ControlObject : GameObject {
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

            RunningEngine.Audio["overworld"].Loop = true;
            RunningEngine.Audio["overworld"].Play();
        }

        public override void OnKeyDown(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Escape])
                RunningEngine.SetQuit(true);
        }
        
        public ControlObject() {
            Persistant = true;
            Cull = false;
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            Text text = new Text("Eksedra Engine Test Demo", RunningEngine.Fonts["JosefinSans"], 20);
            text.Position = new Vector2f(RunningEngine.ViewPort.Left + 32, RunningEngine.ViewPort.Top + 32);
            text.FillColor = Color.Black;

            target.Draw(text);
        }
    }

    public class JumpThrough : GameObject {
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

        public JumpThrough(int x, int y) {
            X = x;
            Y = y;
        }
        
        public override void Init() {
            Tag = "JumpThrough";
            Depth = 1;

            SpriteIndex = new EksedraSprite(RunningEngine.Images["jumpthrough"], new IntRect[] { new IntRect(0, 0, 64, 64) });
            ImageSpeed = 0;
            ImageIndex = 0;

            MaskX = -SpriteIndex.GetWidth() / 2;
            MaskY = -SpriteIndex.GetHeight() / 2;
            MaskWidth = SpriteIndex.GetWidth();
            MaskHeight = 9;
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            target.Draw(SpriteIndex);
        }
    }

    public class Rock : GameObject {
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

        public Rock(int x, int y) {
            X = x;
            Y = y;
        }
        
        public override void Init() {
            Tag = "Rock";
            Depth = 1;

            SpriteIndex = new EksedraSprite(RunningEngine.Images["rock"], new IntRect[] { new IntRect(0, 0, 64, 64) });
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

    public class Player : GameObject {
        private float MoveSpeed = 300;
        private float Gravity = 3500;
        private float MaxVSpeed = 5000;
        private float JumpSpeed = 900;
        private bool IsGrounded = false;

        public override void EarlyUpdate(float deltaTime) {}
        public override void LateUpdate(float deltaTime) {}
        public override void OnKeyUp(bool[]  keyState) {}
        public override void OnTimer(int timerIndex) {}
        public override void OnCollision(GameObject other) {}

        private EksedraSprite PlayerStand, PlayerFall, PlayerRun;

        public Player(int x, int y) {
            X = x;
            Y = y;
            Persistant = true;
        }

        public override void Init() {
            Tag = "Player";
            Depth = 0;

            PlayerStand = new EksedraSprite(RunningEngine.Images["link-minish"], new IntRect[] {
                                                new IntRect(0, 404, 120, 116)
                                            });
            PlayerFall = new EksedraSprite(RunningEngine.Images["link-minish"], new IntRect[] {
                                                new IntRect(120 * 7, 924, 120, 116)
                                            });
            PlayerRun = new EksedraSprite(RunningEngine.Images["link-minish"], new IntRect[] {
                                                new IntRect(0, 924, 120, 116),
                                                new IntRect(120, 924, 120, 116),
                                                new IntRect(120 * 2, 924, 120, 116),
                                                new IntRect(120 * 3, 924, 120, 116),
                                                new IntRect(120 * 4, 924, 120, 116),
                                                new IntRect(120 * 5, 924, 120, 116),
                                                new IntRect(120 * 6, 924, 120, 116),
                                                new IntRect(120 * 7, 924, 120, 116),
                                                new IntRect(120 * 8, 924, 120, 116),
                                                new IntRect(120 * 9, 924, 120, 116),
                                            });
            ImageScaleX = 0.5333f;
            ImageScaleY = 0.5333f;

            SpriteIndex = PlayerStand;
            ImageSpeed = 10;
            MaskX = -8;
            MaskY = -30;
            MaskWidth = 16;
            MaskHeight = 60;
        }

        public override void Draw(RenderTarget target, RenderStates states) {
            target.Draw(SpriteIndex);

            /*RectangleShape mask = new RectangleShape();
            mask.FillColor = Color.Black;
            mask.Position = new Vector2f(X + MaskX, Y + MaskY);
            mask.Size = new Vector2f(MaskWidth, MaskHeight);
            target.Draw(mask);*/
        }

        public override void Update(float deltaTime) {
            //Console.WriteLine(RunningEngine.GetWindowWidth() + ", " + RunningEngine.GetWindowHeight());
            if(X + MaskX + MaskWidth > RunningEngine.GetRoomSize().X) {
                X = RunningEngine.GetRoomSize().X - MaskX - MaskWidth;
                HSpeed = 0;
            } else if(X + MaskX < 0) {
                X = -MaskX;
                HSpeed = 0;
            } else if(Y + MaskY + MaskHeight > RunningEngine.GetRoomSize().Y) {
                Y = RunningEngine.GetRoomSize().Y - MaskY - MaskHeight;
                VSpeed = 0;
            } else if(Y + MaskY < 0) {
                Y = -MaskY;
                VSpeed = 0;
            }
            
            if(VSpeed < MaxVSpeed && !IsGrounded)
                VSpeed += Gravity * deltaTime;

            // Horizontal collision
            GameObject other = null;
            if(HSpeed > 0 && RunningEngine.CheckCollision(X + HSpeed * deltaTime, Y - 0.1f, this, typeof(Rock), (self, otra) => true, ref other)) {
                X = other.X + other.MaskX - (MaskX + MaskWidth);
                HSpeed = 0;
            }
            
            if(HSpeed < 0 && RunningEngine.CheckCollision(X + HSpeed * deltaTime, Y - 0.1f, this, typeof(Rock), (self, otra) => true, ref other)) {
                X = other.X + other.MaskX + other.MaskWidth - MaskX;
                HSpeed = 0;
            }

            // Vertical Collision
            if(VSpeed > 0 && RunningEngine.CheckCollision(X, Y + VSpeed * deltaTime, this, typeof(Rock), 
                    (self, otra) => self.Y + self.MaskY + self.MaskHeight <= otra.Y + otra.MaskY, ref other)) {
                Y = other.Y + other.MaskY - (MaskY + MaskHeight);
                VSpeed = 0;
                IsGrounded = true;
            } else if(VSpeed > 0 && RunningEngine.CheckCollision(X, Y + VSpeed * deltaTime, this, typeof(JumpThrough), 
                    (self, otra) => self.Y + self.MaskY + self.MaskHeight <= otra.Y + otra.MaskY, ref other)) {
                Y = other.Y + other.MaskY - (MaskY + MaskHeight);
                VSpeed = 0;
                IsGrounded = true;
            } else if(!RunningEngine.CheckCollision(X, Y + 1, this, typeof(Rock), 
                        (self, otra) => self.Y + self.MaskY + self.MaskHeight <= otra.Y + otra.MaskY, ref other)
                    && !RunningEngine.CheckCollision(X, Y + 1, this, typeof(JumpThrough), 
                        (self, otra) => self.Y + self.MaskY + self.MaskHeight <= otra.Y + otra.MaskY, ref other))
                IsGrounded = false;
            
            if(VSpeed < 0 && RunningEngine.CheckCollision(X, Y + VSpeed * deltaTime, this, typeof(Rock), 
                    (self, otra) => self.Y + self.MaskY >= otra.Y + otra.MaskY + otra.MaskHeight, ref other)) {
                Y = other.Y + other.MaskY + other.MaskHeight - MaskY;
                VSpeed = 0;
            }

            // Animate
            if(IsGrounded)
                SpriteIndex = Math.Abs(HSpeed) > 0 ? PlayerRun : PlayerStand;
            else
                SpriteIndex = PlayerFall;
            
            if(HSpeed > 0)
                ImageScaleX = Math.Abs(ImageScaleX);
            else if(HSpeed < 0)
                ImageScaleX = -Math.Abs(ImageScaleX);

            // Move the view
            if(X - RunningEngine.ViewPort.Width / 2 < 0)
                RunningEngine.ViewPort.Left = 0;
            else if(X + RunningEngine.ViewPort.Width / 2 > RunningEngine.GetRoomSize().X)
                RunningEngine.ViewPort.Left = RunningEngine.GetRoomSize().X - RunningEngine.ViewPort.Width;
            else
                RunningEngine.ViewPort.Left = X - RunningEngine.ViewPort.Width / 2;

            if(Y - RunningEngine.ViewPort.Height / 2 < 0)
                RunningEngine.ViewPort.Top = 0;
            else if(Y + RunningEngine.ViewPort.Height / 2 > RunningEngine.GetRoomSize().Y)
                RunningEngine.ViewPort.Top = RunningEngine.GetRoomSize().Y - RunningEngine.ViewPort.Height;
            else
                RunningEngine.ViewPort.Top = Y - RunningEngine.ViewPort.Height / 2;
        }

        public override void OnKeyDown(bool[] keyState) {
            if(keyState[(int) Keyboard.Key.Up] && IsGrounded) {
                VSpeed = -JumpSpeed;
                IsGrounded = false;

                RunningEngine.Audio["jump"].Play();
            }
            
            if(keyState[(int) Keyboard.Key.LShift] && RunningEngine.CurrentRoom == "test")
                RunningEngine.CurrentRoom = "wide_floor";
            else if(keyState[(int) Keyboard.Key.LShift] && RunningEngine.CurrentRoom == "wide_floor")
                RunningEngine.CurrentRoom = "test";
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