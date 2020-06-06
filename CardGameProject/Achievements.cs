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
    public static class Achievements
    {
        public static Texture2D Background { get; set; }
        // Доделать ачивки попозже пока просто вывод ComingSoon и возможность вернуться в меню
        
        public static void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Escape))
            {
                CardGame.currGameState = GameState.Menu_mod;
            }
        }

        public static void Draw(SpriteBatch sprt)
        {
            sprt.Draw(Background, new Rectangle(0, 0, CardGame.ScreenW, CardGame.ScreenH), Color.White);
        }
    }
}
