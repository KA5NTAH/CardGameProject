using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CardGameProject
{
    public class MenuButton
    {
        public int width; // 400 
        public int height; // 128
        public Vector2 Pos { get; set; }
        public Texture2D ActiveButton { get; set; }
        public Texture2D NotActiveButton { get; set; }

        public GameState dstState;

        public Rectangle srcRec;
        public MenuButton(int w, int h, Vector2 p, Texture2D ActButton, Texture2D NotActButton, GameState dst)
        {
            width = w;
            height = h;
            Pos = p;
            ActiveButton = ActButton;
            NotActiveButton = NotActButton;
            dstState = dst;
            srcRec = new Rectangle((int)p.X, (int)p.Y, w, h);
        }

        public bool IsMouseInside()
        {
            MouseState mouse = Mouse.GetState();
            int mX = mouse.X;
            int mY = mouse.Y;
            return mX >= Pos.X && mX <= Pos.X + width && mY >= Pos.Y && mY <= Pos.Y + height;
        }

        public virtual bool IsButtonActive()
        {
            return IsMouseInside();
        }

        public void Draw(SpriteBatch sprt)
        {
            if (IsButtonActive())
                sprt.Draw(ActiveButton, srcRec, Color.White);
            else
            {
                Color color = Color.FromNonPremultiplied(255, 255, 255, 150);
                sprt.Draw(NotActiveButton, srcRec, color);
            }
        }

        public void Update()
        {
            if (IsButtonActive())
                CardGame.currGameState = dstState;
        }
    }
}
