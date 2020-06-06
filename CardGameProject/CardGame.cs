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
    public enum GameState
    {
        Menu_mod,
        Play_mod,
        Exit_mod,
        Achievements_mod,
    }


    public class CardGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public const int ScreenW = 1600;
        public const int ScreenH = 1000;
        public static GameState currGameState = GameState.Menu_mod;

        public CardGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

  

        protected override void Initialize()
        {            
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = ScreenW;
            graphics.PreferredBackBufferHeight = ScreenH;
            graphics.ApplyChanges();
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //кнопки для меню
            Menu.MenuArt = Content.Load<Texture2D>("MenuArt");
            Menu.PlBArt = Content.Load<Texture2D>("Buttons/playB");
            Menu.AcBArt = Content.Load<Texture2D>("Buttons/AchBut");
            Menu.ExBArt = Content.Load<Texture2D>("Buttons/ExBut");
            Menu.PlBArt_on = Content.Load<Texture2D>("Buttons/playB_on");
            Menu.AcBArt_on = Content.Load<Texture2D>("Buttons/AchBut_on");
            Menu.ExBArt_on = Content.Load<Texture2D>("Buttons/ExBut_on");
            Menu.Init();

            // достижения
            Achievements.Background = Content.Load<Texture2D>("Achievements/Achievements");


            // ЗАГРУЗКА КАРТ
            Card.CardBack = Content.Load<Texture2D>("Backs/Card-Back-04");  // рубашка 
            // Загрузим карты
            string Cardpath = "Cards/";
            int i = 1;            
            foreach (Suits s in Enum.GetValues(typeof(Suits)))
            {
                foreach (Ranks r in Enum.GetValues(typeof(Ranks)))
                {
                    string curr_name = "c";
                    if (i <= 9)
                        curr_name += ("0" + i.ToString());
                    else
                        curr_name += i.ToString();
                    Texture2D currArt = Content.Load<Texture2D>(Cardpath + curr_name);
                    Play.AllCards.Add(new Card(r, s, currArt));
                    i++;
                }
            }

            // Данные для стола
            Board.CombatPic = Content.Load<Texture2D>("Board/combat");
            Board.EndButton = Content.Load<Texture2D>("Board/end_button");
            Board.ActiveEndButton = Content.Load<Texture2D>("Board/active_end_button");
            Board.PokerCloth = Content.Load<Texture2D>("Board/poker_cloth");
            Board.PAStage = Content.Load<Texture2D>("Board/PAStage");
            Board.PDStage = Content.Load<Texture2D>("Board/PDStage");
            Board.OAStage = Content.Load<Texture2D>("Board/OAStage");
            Board.ODStage = Content.Load<Texture2D>("Board/ODStage");
            Board.TrumpDiam = Content.Load<Texture2D>("Board/trumpDiam");
            Board.TrumpHearts = Content.Load<Texture2D>("Board/trumpHearts");
            Board.TrumpSpades = Content.Load<Texture2D>("Board/trumpSpades");
            Board.TrumpClubs = Content.Load<Texture2D>("Board/trumpClubs");
            Board.GoldenPlace = Content.Load<Texture2D>("Board/Gold");

            Board.DeckBar = Content.Load<Texture2D>("Board/deckBar");
            Board.DeckFrame = Content.Load<Texture2D>("Board/DeckFrame");

            Play.win = Content.Load<Texture2D>("EndGameArts/win");
            Play.loss = Content.Load<Texture2D>("EndGameArts/loss");
            Play.tie = Content.Load<Texture2D>("EndGameArts/tie");

        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            switch (currGameState)
            {
                case GameState.Menu_mod:
                    Menu.Update();
                    break;
                case GameState.Achievements_mod:
                    Achievements.Update();
                    break;
                case GameState.Play_mod:
                    Play.Update();
                    break;
                case GameState.Exit_mod:
                    Exit();
                    break;
            }

            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            switch (currGameState)
            {
                case GameState.Menu_mod:
                    Menu.Draw(spriteBatch);
                    break;
                case GameState.Achievements_mod:
                    Achievements.Draw(spriteBatch);
                    break;
                case GameState.Play_mod:
                    Play.Draw(spriteBatch);
                    break;                
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
