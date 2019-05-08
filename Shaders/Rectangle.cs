using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperMarioBros
{
    public struct Rectangle 
    {
        public float X;
        public float Y;
        public float width;
        public float height;

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            this.width = width;
            this.height = height;
        }
        

        public Vector2 TopLeftCorner { get { return new Vector2(X, Y); } }
        public Vector2 TopRightCorner { get { return new Vector2(X + width, Y); } }
        public Vector2 BottomLeftCorner { get { return new Vector2(X, Y + height); } }
        public Vector2 BottomRightCorner { get { return new Vector2(X + width, Y + height); } }
        public Vector2 TopMid { get { return new Vector2(X + width/2, Y ); } }

        public bool IsPointOutside(Vector2 vec)
        {
            return IsPointOutside(vec.X, vec.Y);
        }

        public bool IsPointInside(Point p)
        {
            return !IsPointOutside(p.X, p.Y);
        }

        public bool IsPointOutside(float x, float y)
        {
            if (x < X || y < Y || x > X + width || y > Y + height) return true;
            return false;
        }

        public static Rectangle operator+ (Rectangle A, Rectangle B) {
            return new Rectangle(A.X - B.width/2, A.Y - B.height/2, A.width + B.width, A.height + B.height);
        }

        public static Rectangle operator+(Rectangle A, Vector2 vec)
        {
            return new Rectangle(A.X + vec.X, A.Y + vec.Y, A.width, A.height);
        }

        public Vector2 center()
        {
            return new Vector2(X + width / 2, Y + height / 2);
        }

        public float distance(Vector2 vec)
        {
            return Math.Min(Math.Min(Math.Abs(X - vec.X), Math.Abs(Y - vec.Y)), Math.Min(Math.Abs(X + width - vec.X), Math.Abs(Y + height - vec.Y)));
        }

        public Rectangle Clone()
        {
            return new Rectangle(X, Y, width, height);
        }

        public override string ToString()
        {
            string str = $"X : {X}, Y : {Y}\nWidth : {width}, Height : {height}";
            return str;
        }

        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rectangle rectangle)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)rectangle.X,(int)rectangle.Y,(int)rectangle.width,(int)rectangle.height);
        }

        public float Bot
        {
            get { return this.Y + this.height; }
            set { this.Y = value - this.height; }
        }
        public float Right
        {
            get { return this.X + this.width; }
        }
    }
}
