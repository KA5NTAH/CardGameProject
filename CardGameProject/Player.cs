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
    static class Player
    {
        public static List<Card> hand = new List<Card>();

        public static int drawStartX = 5;         
        public static int drawStartY = 750;
        public static int drawStepX = Play.defaultDrawStepX;
        public static int higherY = 650;   
        
        public static MouseState old_ms = Mouse.GetState();

        // Векторы позволяющие пользователю двигать карту
        public static int selectedNumberCard = hand.Count + 1;
        public static Vector2 selectedFinishPos;
        public static Vector2 selectedCurrPos;
        public static Vector2 selectedOffset; // Переменная которая определяет отклонение между положением курсора и краем карты
        // Нужна для того чтобы при движение карты положении курсора внутри нее не изменялось

        public static bool playerIsHoldingCard = false;    

        public static void Update()
        {
            // Выход в меню
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Escape))
                CardGame.currGameState = GameState.Menu_mod;

            // определим карту на которую наведена мышь
            if (!playerIsHoldingCard)
            {
                selectedNumberCard = Play.NumberOfSelectedCard(drawStartX, drawStartY, CardGame.ScreenH, drawStepX, hand);
                selectedCurrPos = new Vector2(drawStartX + drawStepX * selectedNumberCard, higherY);
            }


            // Если мышь нажата то должна быть возможность перенести карту
            MouseState ms = Mouse.GetState();

            // Первое нажатие
            if (ms.LeftButton == ButtonState.Pressed && old_ms.LeftButton == ButtonState.Released && selectedNumberCard != -1)
            {
                // Если произошло нажатие на карту значит сейчас мы ее держим (Если конечно курсор на карте)
                playerIsHoldingCard = true;
                // Нажатие на карту произошло первый раз посчитаем отклонение
                Vector2 cardEdge = new Vector2(drawStartX + drawStepX * selectedNumberCard, drawStartY);
                selectedFinishPos = cardEdge; // Если мы отпустим карту не доведя до места назначения она вернется на свое место
                Vector2 mp = new Vector2(ms.X, ms.Y);
                selectedOffset = Vector2.Subtract(cardEdge, mp);
            }

            // Продолжительное нажатие двигаем карту
            if (ms.LeftButton == ButtonState.Pressed && old_ms.LeftButton == ButtonState.Pressed)          
                selectedCurrPos = Vector2.Add(new Vector2 (ms.X, ms.Y), selectedOffset);             


            // Произошло разжатие Значит возможно несколько вариантов:
            // Отпустили карту (варианты с атакой и защитой)
            // Нажали на кнопку конца хода
            // Ничего
            if (ms.LeftButton == ButtonState.Released && old_ms.LeftButton == ButtonState.Pressed)
            {
                if (playerIsHoldingCard)
                {                                                          
                    // Атака игрока               
                    if (Play.curr_state == PlayState.PlayerAttack && MouseInsideCombat())
                    {
                        // Если битых карт нет значит это первая атака и можно ходить чем угодно
                        // Если битые карты есть то нужно проверить можно ли подкидывать
                        if (Play.turnBeatenCards.Count == 0 || (Play.turnBeatenCards.Count > 0 && Play.CanThrowCard(hand[selectedNumberCard])))
                            Play.PlayCardAsAttacker(hand[selectedNumberCard], hand);                        
                    }

                    // Если игрок защищается то игра проверяет может ли его карта покрыть карту противника
                    else if (Play.curr_state == PlayState.PlayerDef && MouseInsideCombat())
                    {
                        if (Play.TrialByCombat(hand[selectedNumberCard], Play.combatCard))
                            Play.PlayCardAsDefender(hand[selectedNumberCard], hand);
                    }

                    // сбрасываем карту из руки
                    playerIsHoldingCard = false;
                    selectedNumberCard = -1;
                }

                else if (MouseInsideEndButton())
                {
                    // Досрочное завершение хода при защите 
                    if (Play.curr_state == PlayState.PlayerDef)
                        Play.PlayerTakesAllCards();

                    // Досрочное завершение хода при нападении возможно только если подбрасываем карты
                    if (Play.curr_state == PlayState.PlayerAttack && Play.turnBeatenCards.Count > 0)
                        Play.NormalTurnEnding();
                }                
            }

            old_ms = ms;
        }


        public static bool MouseInsideCombat()
        {
            MouseState ms = Mouse.GetState();
            int lx = (int)Board.combatLeftUpCorner.X;
            int ly = (int)Board.combatLeftUpCorner.Y;
            int rx = lx + Board.combatWidth;
            int ry = ly + Board.combatHeight;        
            return (ms.X >= lx && ms.X <= rx && ms.Y >= ly && ms.Y <= ry);
        }


        public static bool CanEndTurnWithButton()
        {
            return Play.curr_state == PlayState.PlayerDef || (Play.curr_state == PlayState.PlayerAttack && Play.turnBeatenCards.Count > 0);
        }


        public static bool MouseInsideEndButton()
        {
            MouseState ms = Mouse.GetState();
            int lx = (int)Board.endButtonLUCorner.X;
            int ly = (int)Board.endButtonLUCorner.Y;
            int rx = lx + Board.endButtonW;
            int ry = ly + Board.endButtonH;
            return (ms.X >= lx && ms.X <= rx && ms.Y >= ly && ms.Y <= ry);
        }


        public static void Draw(SpriteBatch sprt)
        {
            // Если карт в руке слишком много рисуем их пополотнее
            if (hand.Count * Play.defaultDrawStepX + Card.width > CardGame.ScreenW)
                drawStepX = (int)Math.Floor((CardGame.ScreenW - Card.width) / hand.Count);
            else
                drawStepX = Play.defaultDrawStepX;

            for (int i = 0; i < hand.Count(); i++) {
                if (i == selectedNumberCard)
                    hand[i].Draw(sprt, selectedCurrPos, (float)0);
                else
                    hand[i].Draw(sprt, new Vector2(drawStartX + drawStepX * i, drawStartY), (float)0);
            }            
        }
    }
}
