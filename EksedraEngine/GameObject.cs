using SFML.Graphics;
using System;

/*
 * Base object for all other game objects
 * Inspired by how GameMaker 7 and 8 used objects
 * Includes:
 *  - position, direction, and motion values
 *  - collision detection with masks
 *  - drawing a sprite with some scale
 *  - slowing down/controlling animation of sprite
 *  - room location and draw height (depth)
 *  - Method for updating before others, with others, and after others
 *  - Method for instantiation
 *  - Keyboard control methods
 */
namespace EksedraEngine {
    public abstract class GameObject : Drawable, IComparable<GameObject> {
        public float X, Y, HSpeed, VSpeed, Dir;
        public float MaskX, MaskY, MaskWidth, MaskHeight;
        
        public EksedraSprite SpriteIndex;
        public float ImageSpeed, ImageIndex;
        public float ImageScaleX, ImageScaleY;

        public float[] Timers;

        public int Depth;
        public bool Persistant;
        public Engine RunningEngine;

        public string Tag;

        public abstract void Draw(RenderTarget target, RenderStates states);
        public abstract void Init();

        public abstract void OnKeyDown(bool[] keyState);
        public abstract void OnKeyHeld(bool[] keyState);
        public abstract void OnKeyUp(bool[] keyState);
        public abstract void OnKeyOff(bool[] keyState);
        
        public abstract void EarlyUpdate(float deltaTime);
        public abstract void Update(float deltaTime);
        public abstract void LateUpdate(float deltaTime);
        
        public abstract void OnCollision(GameObject other);
        public abstract void OnTimer(int timerIndex);

        public GameObject(
                float x = 0, float y = 0, float hsp = 0, float vsp = 0, float dir = 0, 
                float maskX = 0, float maskY = 0, float maskW = 0, float maskH = 0,
                EksedraSprite spriteIndex = null, int imageIndex = 0, int imageSpeed = 1,
                float imageScaleX = 1, float imageScaleY = 1,
                int room = 0, int depth = 0, Engine engine = null) {
            X = x;
            Y = y;
            HSpeed = hsp;
            VSpeed = vsp;
            Dir = dir;
            MaskX = maskX;
            MaskY = maskY;
            MaskWidth = maskW;
            MaskHeight = maskH;
            SpriteIndex = spriteIndex;
            ImageIndex = imageIndex;
            ImageSpeed = imageSpeed;
            ImageScaleX = imageScaleX;
            ImageScaleY = imageScaleY;
            
            Depth = depth;
            RunningEngine = engine;
            Persistant = false;
            Tag = "";
            Timers = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
        public virtual int CompareTo(GameObject other) => other.Depth.CompareTo(Depth);
    }
}
