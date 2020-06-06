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
    static class Board
    {
        /*         
         Стол занимается отображением себя на экран
         и содержит всю визуальную информацию
         */
        //public static int LeftinDeck { get; set; }  // FIXME
        //public static int WastedCount { get; set; } // FIXME
        public static Texture2D PokerCloth { get; set; }


        // границы боевой зоны
        public static Texture2D CombatPic { get; set; }
        public static int combatWidth = 400;
        public static int combatHeight = 400;
        public static Vector2 combatLeftUpCorner = new Vector2(950, 250);
        public static Rectangle combatRecSrc = new Rectangle(950, 250, combatWidth, combatHeight);

        // границы кнопки выхода
        public static Texture2D EndButton { get; set; }
        public static Texture2D ActiveEndButton { get; set; }
        public static int endButtonW = 100;
        public static int endButtonH = 100;
        public static Vector2 endButtonLUCorner = new Vector2(1450, 400);
        public static Rectangle endButtonSrc = new Rectangle((int)endButtonLUCorner.X, (int)endButtonLUCorner.Y, endButtonW, endButtonH);

        // определим как будут нарисованы битые на этом ходу карты
        private static int selectedOffsetY = -80;
        private static int drawMinY = 300;
        private static int drawMaxY = drawMinY + Convert.ToInt32(Card.CardBack.Height * Card.scale);
        private static int drawFinishX = 300;
        private static int drawStartX = 300;
        private static int drawStepX = Play.defaultDrawStepX;

        // Также для удобства будет отображаться текущая стадия игры
        public static int PlayStageWidth = 400;
        public static int PlayStageHeight = 80;
        public static Rectangle stageSrc = new Rectangle(950, 630, PlayStageWidth, PlayStageHeight);
        public static Texture2D PAStage { get; set; }
        public static Texture2D PDStage { get; set; }
        public static Texture2D OAStage { get; set; }
        public static Texture2D ODStage { get; set; }

        // Все что связано с козырем
        public static Texture2D GoldenPlace { get; set; }
        public static Texture2D TrumpDiam { get; set; }
        public static Texture2D TrumpHearts { get; set; }
        public static Texture2D TrumpClubs { get; set; }
        public static Texture2D TrumpSpades { get; set; }
        public static Texture2D currTrumpArt { get; set; }


        public static Rectangle trumpSpotRectangle = new Rectangle(20, 300, 270, 370);
        public static Vector2 trumpCardSpot = new Vector2(28, 308);

        public static Rectangle currTrumpSpot = new Rectangle(1450, 550, 100, 100);


        // Данные остатка колоды
        public static Texture2D DeckFrame {get; set;}
        public static Texture2D DeckBar { get; set; }
        public static Rectangle frameSrc = new Rectangle(1450, 150, 100, 198);
        public static int barWidth = 93;
        public static int bottom = 340;
        public static int cardHeigt = 5;

        public static void DrawCombat(SpriteBatch sprt)
        {
            sprt.Draw(CombatPic, combatRecSrc, Color.White);
            if (Play.combatCardPlayed)
                Play.combatCard.Draw(sprt, new Vector2(combatLeftUpCorner.X + 10, combatLeftUpCorner.Y + 10), 0);        
        }


        public static void DrawBeatenCards(SpriteBatch sprt)
        {            
            if (Play.turnBeatenCards.Count > 0)
            {
                int seqLength = Convert.ToInt32((Play.turnBeatenCards.Count - 1) * drawStepX) + Convert.ToInt32(Card.scale * Card.CardBack.Width);  
                int currStepX = drawStepX;
                if (Play.turnBeatenCards.Count * Play.defaultDrawStepX + Card.width + drawStartX > drawFinishX)
                    currStepX = (int)Math.Floor((drawFinishX - Card.width) / Play.turnBeatenCards.Count);
                else
                    drawStepX = Play.defaultDrawStepX;
                int selectedBeatenCard = Play.NumberOfSelectedCard(drawStartX, drawMinY, drawMaxY, currStepX, Play.turnBeatenCards);

                Play.SetAllCardsFaceUp(Play.turnBeatenCards);
                for (int i = 0; i < Play.turnBeatenCards.Count; i++)
                {
                    if (i == selectedBeatenCard)
                        Play.turnBeatenCards[i].Draw(sprt, new Vector2(drawStartX + drawStepX * i, drawMinY + selectedOffsetY), 0);
                    else
                        Play.turnBeatenCards[i].Draw(sprt, new Vector2(drawStartX + drawStepX * i, drawMinY), 0);
                }
            }                     
        }


        public static void DrawPlayStage(SpriteBatch sprt)
        {
            switch (Play.curr_state)
            {
                case PlayState.OpAttack:
                    sprt.Draw(OAStage, stageSrc, Color.White);
                    break;
                case PlayState.OpDef:
                    sprt.Draw(ODStage, stageSrc, Color.White);
                    break;
                case PlayState.PlayerAttack:
                    sprt.Draw(PAStage, stageSrc, Color.White);
                    break;
                case PlayState.PlayerDef:
                    sprt.Draw(PDStage, stageSrc, Color.White);
                    break;
            }
        }


        public static void DrawEndButton(SpriteBatch sprt)
        {
            if (Player.MouseInsideEndButton() && Player.CanEndTurnWithButton())
                sprt.Draw(ActiveEndButton, endButtonSrc, Color.White);
            else
                sprt.Draw(EndButton, endButtonSrc, Color.White);            
        }


        public static void DrawTrump(SpriteBatch sprt)
        {
            sprt.Draw(GoldenPlace, trumpSpotRectangle, Color.White);
            if (!Play.trumpCardTaken && Play.trumpCardDefined)
                Play.trumpCard.Draw(sprt, trumpCardSpot, 0);
            
            switch (Play.trumpSuit)
            {
                case Suits.Clubs:
                    sprt.Draw(TrumpClubs, currTrumpSpot, Color.White);
                    break;
                case Suits.Spades:
                    sprt.Draw(TrumpSpades, currTrumpSpot, Color.White);
                    break;
                case Suits.Hearts:
                    sprt.Draw(TrumpHearts, currTrumpSpot, Color.White);
                    break;
                case Suits.Diamonds:
                    sprt.Draw(TrumpDiam, currTrumpSpot, Color.White);
                    break;
            }

        }


        public static void DrawDeck(SpriteBatch sprt)
        {
            int barHeight = Play.deck.Count * cardHeigt;
            int startDraw = bottom - barHeight;
            Rectangle barSrc = new Rectangle(1452, startDraw, barWidth, barHeight);
            sprt.Draw(DeckBar, barSrc, Color.White);
            sprt.Draw(DeckFrame, frameSrc, Color.White);
        }


        public static void Draw(SpriteBatch sprt)
        {
            sprt.Draw(PokerCloth, new Rectangle(0, 0, CardGame.ScreenW, CardGame.ScreenH), Color.White);          
            DrawBeatenCards(sprt);
            DrawCombat(sprt);
            DrawPlayStage(sprt);
            DrawTrump(sprt);
            DrawEndButton(sprt);
            DrawDeck(sprt);
        }
        
    }
}
