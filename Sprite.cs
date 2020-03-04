using SFML.Graphics;
using SFML.System;
using System;

namespace EksedraEngine {
    class EksedraSprite : Drawable {
        public float ImageSpeed;
        public float ImageIndex;

        private Texture Source;
        private bool Smooth;
        private IntRect[] SourceRects;
        private float X, Y, XScale, YScale;
        
        public uint GetWidth() => Source.Size.X;
        public uint GetHeight() => Source.Size.Y;
        public IntRect GetCurrentRect() => SourceRects[(int) Math.Floor(ImageIndex) % SourceRects.Length];

        public EksedraSprite(string fileName, IntRect[] sourceRects) {
            Source = new Texture(fileName);
            Source.Smooth = true;

            X = 0;
            Y = 0;
            XScale = 1;
            YScale = 1;

            SourceRects = sourceRects;
            Smooth = true;
        }

        public void MoveTo(float x, float y) {
            X = x;
            Y = y;
        }

        public void SetScale(float xScale, float yScale) {
            XScale = xScale;
            YScale = yScale;
        }

        public void Draw(RenderTarget target, RenderStates states) {
            Source.Smooth = Smooth;
            Sprite drawSprite = new Sprite(Source, SourceRects[(int) Math.Floor(ImageIndex) % SourceRects.Length]);
            drawSprite.Position = new Vector2f(X + -XScale * GetCurrentRect().Width / 2, Y + -YScale * GetCurrentRect().Height / 2);
            drawSprite.Scale = new Vector2f(XScale, YScale);
            target.Draw(drawSprite);

            /*CircleShape center = new CircleShape(4);
            center.FillColor = Color.Green;
            center.Position = new Vector2f(X, Y);
            CircleShape topLeft = new CircleShape(4);
            topLeft.FillColor = Color.Green;
            topLeft.Position = new Vector2f(X - GetCurrentRect().Width / 2, Y - GetCurrentRect().Height / 2);
            CircleShape bottomRight = new CircleShape(4);
            bottomRight.FillColor = Color.Green;
            bottomRight.Position = new Vector2f(X + GetCurrentRect().Width / 2, Y + GetCurrentRect().Height / 2);
            target.Draw(center);
            target.Draw(topLeft);
            target.Draw(bottomRight);*/
        }
    }
}