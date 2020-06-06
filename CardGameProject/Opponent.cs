using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CardGameProject
{
    static class Opponent
    {
        public static List<Card> hand = new List<Card>();

        public static int drawStartX = 5;
        public static int drawStepX = Play.defaultDrawStepX;  // TODO уменьшать шаг если карт в руке слишком много 
        public static int drawStartY = -100;
        public static int lateGameCond = 10;  // Если в колоде еще много карт бережем козыри
        private static bool cardTooValuable;
        // private static int OpTurnTime = 3; // время хода оппонента в секнудах FIXME

        public static void Update()
        {           
            //Для искуственного интеллекта были выбраны несколько достаточно простых принципов            
            if (Play.curr_state == PlayState.OpAttack)
            {
                // Для выбора первой атакующей карты присвоим каждой карте показатель хорошести
                // он будет определяться формулой (10 - r + 2 * p - 3 * tr)
                // где r ранг карты; p наличие пары(0 или 1); tr являетя ли карта козырем
                // То есть мы ходим картами с меньшим рангом при этом желательно чтобы они не являлись козырями 
                // Так же наличие пары является плюсом так как открывает возможность для подбрасывания
                if (Play.turnBeatenCards.Count == 0)
                {
                    // битых карт нет значит первая атака
                    int cardToPlay = 0;
                    int maxQuality = 0;
                    for (int i = 0; i < hand.Count; i++)
                    {
                        int currQuality = DefineCardQuality(i);
                        if (currQuality > maxQuality)
                        {
                            maxQuality = currQuality;
                            cardToPlay = i;
                        }
                    }
                    Play.PlayCardAsAttacker(hand[cardToPlay], hand);                    
                }
                else
                {
                    // подбрасываем карту
                    // почти идентичный алгоритм только нужна дополнительная проверка на возможность подбрасывания
                    int ThrowIndex = -1;
                    int maxQuality = 0;
                    for (int i = 0; i < hand.Count; i++)
                    {
                        int currQuality = DefineCardQuality(i);
                        if (currQuality > maxQuality && Play.CanThrowCard(hand[i]))
                        {
                            maxQuality = currQuality;
                            ThrowIndex = i;
                        }
                    }

                    // Нельзя подкинуть карту если нечего подбрасывать или карта слишком ценная для текущей стадии
                    if (ThrowIndex != -1)
                        cardTooValuable = Play.deck.Count > lateGameCond && hand[ThrowIndex].Suit == Play.trumpSuit;
                    else
                        cardTooValuable = false;

                    if (ThrowIndex == -1 || cardTooValuable)
                        Play.NormalTurnEnding();
                    else
                        Play.PlayCardAsAttacker(hand[ThrowIndex], hand);                    
                }

            }

            if (Play.curr_state == PlayState.OpDef)
            {
                // В защите придерживаемся простой стратегии - отбиваться любой ценой 
                // Но если есть несколько возможностей отбиться то опять же ориентируемся на хорошесть карты
                // В начале игры все же придержим козыри
                int defenceIndex = -1;
                int maxQuality = 0;
                for (int i = 0; i < hand.Count; i++)
                {
                    int currQuality = DefineCardQuality(i);
                    if (Play.TrialByCombat(hand[i], Play.combatCard) && currQuality > maxQuality)
                    {
                        maxQuality = currQuality;
                        defenceIndex = i;
                    }
                }

                if (defenceIndex != -1)
                {
                    cardTooValuable = Play.deck.Count > lateGameCond && hand[defenceIndex].Suit == Play.trumpSuit &&
                        (int)hand[defenceIndex].Rank > (int)Ranks.Jack;
                }
                else
                    cardTooValuable = false;

                if (defenceIndex == -1 || cardTooValuable)                
                    Play.OpponentTakesAllCards();                
                
                else
                    Play.PlayCardAsDefender(hand[defenceIndex], hand);                    
            }
        }


        public static int DefineCardQuality(int numberInHand)
        {
            Card givenCard = hand[numberInHand];
            int rank = (int) givenCard.Rank;
            int trumpSuit = givenCard.Suit == Play.trumpSuit ? 1 : 0;
            int pairInHand = 0;
            for (int i = 0; i < hand.Count; i++)
            {
                if (i == numberInHand)
                    continue;
                if (hand[i].Rank == givenCard.Rank)
                    pairInHand = 1;
            }
            return (15 - rank + pairInHand * 2 - trumpSuit * 3);
        }


        public static void Draw(SpriteBatch sprt)
        {
            // В случае большого количества карт понизить порог
            if (hand.Count * Play.defaultDrawStepX + Card.width > CardGame.ScreenW)
                drawStepX = (int)Math.Floor((CardGame.ScreenW - Card.width) / hand.Count);
            else
                drawStepX = Play.defaultDrawStepX;
            for (int i = 0; i < hand.Count(); i++)                           
                hand[i].Draw(sprt, new Vector2(drawStartX + drawStepX * i, drawStartY), (float)0);            
        }
        
    }
}
