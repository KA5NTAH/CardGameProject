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

    static class Menu
    {
        /*
         Класс Menu отвечает за прорисовку и обновления начальной заставки 
         Его методы вызываются когда состояние игры ровно меню при 
         Метод Update следит за нажатием на мыши и если была нажата 1 из кнопок меню то переключает состояние игры

         Метод Draw Отрисовывает меню
        */
        // Арты для кнопок и фона
        public static Texture2D MenuArt { get; set; }
        public static Texture2D PlBArt { get; set; }
        public static Texture2D PlBArt_on { get; set; }
        public static Texture2D AcBArt { get; set; }
        public static Texture2D AcBArt_on { get; set; }
        public static Texture2D ExBArt { get; set; }
        public static Texture2D ExBArt_on { get; set; }
        private static MouseState old_mouse = Mouse.GetState();

        public const int buttonW = CardGame.ScreenW / 4;
        public const int buttonH = CardGame.ScreenH / 8;

        // Определим стартовые позиции для кнопок и сдвиг по оси Y
        static int start_x = (CardGame.ScreenW - buttonW) / 2;
        static int start_y = CardGame.ScreenH / 16;
        static int step_y = CardGame.ScreenH / 16 + buttonH;      
        public static MenuButton[] menuButtons;

        public static void Init()
        {
            MenuButton playButton = new MenuButton(buttonW, buttonH, new Vector2(start_x, start_y), PlBArt_on, PlBArt, GameState.Play_mod);
            MenuButton achButton = new MenuButton(buttonW, buttonH, new Vector2(start_x, start_y + step_y), AcBArt_on, AcBArt, GameState.Achievements_mod);
            MenuButton exitButton = new MenuButton(buttonW, buttonH, new Vector2(start_x, start_y + step_y * 2), ExBArt_on, ExBArt, GameState.Exit_mod);
            menuButtons = new [] { playButton, achButton, exitButton };
        }
        

        public static void Update()
        {
            MouseState new_mouse = Mouse.GetState();

            // Считаем что кнопка нажата если она было разжата на месте нашей кнопки
            if (new_mouse.LeftButton == ButtonState.Released && old_mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (MenuButton b in menuButtons)
                {
                    b.Update();
                    if (b.dstState == GameState.Play_mod)
                        Play.curr_state = PlayState.Initialiazation;
                }
            }
            old_mouse = new_mouse;
        }


        public static void Draw(SpriteBatch sprt)
        {         
            sprt.Draw(MenuArt, new Rectangle(0, 0, CardGame.ScreenW, CardGame.ScreenH), Color.White);
            foreach (MenuButton b in menuButtons)
                b.Draw(sprt);
        }

    }
      
}

