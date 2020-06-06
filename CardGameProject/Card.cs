using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGameProject
{
    public enum Ranks
    {
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
    public enum Suits
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    sealed class Card
    {
        public Ranks Rank { get; private set; }
        public Suits Suit { get; private set; }
        public bool FaceUp { get; set; }
        public Texture2D CardArt { get; private set; }
        public static Texture2D CardBack { get; set; }
        public static double width = 230;  // FIXME
        //public static int width = Convert.ToInt32(CardBack.Width * scale);

        public static float scale = 0.33f;

        public Card(Ranks rank, Suits suit, Texture2D art)
        {
            FaceUp = false;
            Rank = rank;
            Suit = suit;
            CardArt = art;
        }

        public void Draw(SpriteBatch sprt, Vector2 location, float angle)
        {
            // Карта рисует себя на указанной позиции с поворотом на указанный угол
            // С масштабом 0.33
            Rectangle src = new Rectangle(0, 0, CardArt.Width, CardArt.Height);
            Vector2 origin = new Vector2(0, 0);      
            if (FaceUp)
                sprt.Draw(CardArt, location, src, Color.White, angle, origin, scale, SpriteEffects.None, 1);
            else
                sprt.Draw(CardBack, location, src, Color.White, angle, origin, scale, SpriteEffects.None, 1);
        }
    }
}
