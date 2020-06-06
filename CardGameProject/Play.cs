using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CardGameProject
{
    internal enum PlayState
    {
        Initialiazation,
        PlayerAttack,
        PlayerDef,
        OpAttack,
        OpDef,
        PlayerWon,
        PLayerLost,
        PlayerTied
    }

    static class Play
    {
        // Заполнить колоду 
        // перемешать карты
        // Определить кто ходит первым
        // раздать карты + определить козырь
        // Цикл (пока 1 не победит)
        //      Игрок1 ходит на Игрок2 В зависимости от исхода определить кто будет атакующим в следующей итерациии
        //      Добор карт

        // Карты вместе с артами загружаются в Content.Load 
        public static List<Card> AllCards = new List<Card>();
        public static PlayState curr_state = PlayState.Initialiazation;
        public static Stack<Card> deck = new Stack<Card>();
        public static int defaultDrawStepX = 50;

        // жребий
        public static bool humanIsFirstToDraw;

        // козырь
        public static Card trumpCard;
        public static Suits trumpSuit;
        public static bool trumpCardDefined;
        public static bool trumpCardTaken;

        const int HandSize = 6;
        const int ThrowLimit = 6;
        const int FirstThrowLimit = 5;
        public static bool firstAttackPassed = false;



        public static Card combatCard;  // карта которую должен покрыть 1 из игроков
        public static bool combatCardPlayed;       
        public static List<Card> wastedCards = new List<Card>();  // Выбывшие из игры карты
        public static List<Card> turnBeatenCards = new List<Card>(); // Карты побитые на этом ходу   
        //
        public static Texture2D endGameArt;
        public static Texture2D win;
        public static Texture2D loss;
        public static Texture2D tie;

        public static void Update()
        {
            Player.Update();
            switch (curr_state)
            {
                case PlayState.Initialiazation:
                    Initialization();                    
                    break;                
                case PlayState.PlayerAttack:
                    Player.Update();
                    break;
                case PlayState.PlayerDef:
                    Player.Update();
                    break;
                case PlayState.OpAttack:
                    Opponent.Update();
                    break;
                case PlayState.OpDef:
                    Opponent.Update();
                    break;
                case PlayState.PlayerTied:
                    EndGameUpdate();
                    break;
                case PlayState.PlayerWon:
                    EndGameUpdate();
                    break;
                case PlayState.PLayerLost:
                    EndGameUpdate();
                    break;
            }
        } 


        public static void EndGameUpdate()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Escape))
                CardGame.currGameState = GameState.Menu_mod;
        }


        public static void Draw(SpriteBatch sprt)
        {   if (curr_state == PlayState.PLayerLost || curr_state == PlayState.PlayerWon || curr_state == PlayState.PlayerTied)
            {
                if (curr_state == PlayState.PLayerLost)
                    endGameArt = loss;
                else if (curr_state == PlayState.PlayerWon)
                    endGameArt = win;
                else
                    endGameArt = tie;
                sprt.Draw(endGameArt, new Rectangle(0, 0, CardGame.ScreenW, CardGame.ScreenH), Color.White);
            }
            else
            {
                Board.Draw(sprt);
                Opponent.Draw(sprt);
                Player.Draw(sprt);                
            }
        }


        public static void Shuffle()
        {
            // перетасуем колоду
            // Просто берем 2 карты и меняем их местами так повторяем какое то количество раз
            Random rnd = new Random();
            int CardsCount = Play.AllCards.Count;
            for (int j = 0; j <= 100; j++)
            {
                int ind1 = rnd.Next() % CardsCount;
                int ind2 = rnd.Next() % CardsCount;
                var tmp = Play.AllCards[ind1];
                AllCards[ind1] = AllCards[ind2];
                AllCards[ind2] = tmp;
            }
        }


        public static void Initialization()
        {
            Shuffle();
            wastedCards.Clear();
            deck.Clear();
            // Добавляем карты в колоду
            foreach (Card c in AllCards)
            {
                Card curr = c;
                deck.Push(curr);
            }

            Player.hand.Clear();
            Opponent.hand.Clear();
            turnBeatenCards.Clear();

            combatCardPlayed = false;            
            trumpCardDefined = false;
            trumpCardTaken = false;
            firstAttackPassed = false;
            DealCards();

            // жребий
            Random rnd = new Random();
            int roll = rnd.Next();
            humanIsFirstToDraw = roll % 2 == 0;
            if (humanIsFirstToDraw)
                curr_state = PlayState.PlayerAttack;
            else
                curr_state = PlayState.OpAttack;            
        }


        public static void DealCards()
        {
            bool playerLast = true;
            // Сначала игроки по очереди берут карты в порядке соотвутствующем игровой логике
            // Карта в нашей руке лицом вверх
            if (humanIsFirstToDraw)
            {
                while (deck.Count > 0 && Player.hand.Count < HandSize && Opponent.hand.Count < HandSize)
                {
                    Player.hand.Add(deck.Pop());
                    playerLast = true;
                    if (deck.Count > 0)
                    {
                        Opponent.hand.Add(deck.Pop());
                        playerLast = false;
                    }
                }
            }
            else
            {
                while (deck.Count > 0 && Player.hand.Count < HandSize && Opponent.hand.Count < HandSize)
                {
                    Opponent.hand.Add(deck.Pop());
                    playerLast = false;
                    if (deck.Count > 0)
                    {
                        Player.hand.Add(deck.Pop());
                        playerLast = true;
                    }
                }
            }


            // Если у 1 игрока уже достаточно карт а второй еще не добрал
            // то этот игрок добирает карты до нужного количества
            while (deck.Count > 0 && Player.hand.Count < HandSize && Opponent.hand.Count >= HandSize)
                Player.hand.Add(deck.Pop());            
            while (deck.Count > 0 && Opponent.hand.Count < HandSize && Player.hand.Count >= HandSize)
                Opponent.hand.Add(deck.Pop());            


            // После первой раздачи нужно определить козырь
            if (!trumpCardDefined && deck.Count > 0)
            {
                trumpCardDefined = true;
                trumpCard = deck.Pop();
                trumpCard.FaceUp = true;
                trumpSuit = trumpCard.Suit;
            }


            // Если колода уже закончилась но у одного из игроков не хватает карт кто то должен забрать козырь
            if (deck.Count == 0 && (Player.hand.Count < HandSize || Opponent.hand.Count < HandSize) && !trumpCardTaken && trumpCardDefined)
            {             
                if (Player.hand.Count < HandSize && Opponent.hand.Count < HandSize)
                {
                    if (playerLast)
                        Opponent.hand.Add(trumpCard);
                    else
                        Player.hand.Add(trumpCard);
                }
                else if (Player.hand.Count < HandSize)
                {
                    Player.hand.Add(trumpCard);
                }
                else
                    Opponent.hand.Add(trumpCard);

                trumpCardTaken = true;
            }

            SetAllCardsFaceUp(Player.hand);
            SetAllCardsFaceDown(Opponent.hand);
        }


        public static bool TrialByCombat(Card hero, Card enemy)
        {       
            // Расммотрим 2 случая       
            if (hero.Suit == enemy.Suit)
            {
                // Масти карт равны: тогда нужно только проверить что наша карта старше
                if ((int)hero.Rank > (int)enemy.Rank)
                    return true;
            }
            else
            {
                // Если наша козырь то из условия вторая карта козырем не является значит победа 
                if (hero.Suit == Play.trumpSuit)
                    return true;
            }
            return false;
        }


        public static bool CanThrowCard(Card card)
        {
            // определяет можно ли подбросить карту 
            CheckWinner(); // Для начала убедимся что игра еще идет
            if (Opponent.hand.Count == 0 || Player.hand.Count == 0)
                return false;

            bool beatenCardsCond = false;
            // есть ли в побитых за этот ход картах карта с таким же рангом
            foreach (Card curr in turnBeatenCards)
            {
                if (card.Rank == curr.Rank)
                    beatenCardsCond = true;
            }

            bool limitCond = false;
            if (!firstAttackPassed)
                limitCond = Play.turnBeatenCards.Count < FirstThrowLimit * 2;
            else
                limitCond = Play.turnBeatenCards.Count < ThrowLimit * 2;
                
            return beatenCardsCond && limitCond;
        }

    
        public static int NumberOfSelectedCard(int drawStartX, int y_min, int y_max, int drawStepX, List<Card> cards)
        {
            // Есть список карт которые рисуются подряд с определенным шагом Функция определяет на какую из них наведена мыщь

            int number = -1;
            MouseState ms = Mouse.GetState();
            // Индекс карты левая граница которой наиболее близка к курсору
            double leftInd = Math.Floor(((double)ms.X - drawStartX) / drawStepX);
            // Если индекс соотвествует любой карте кроме последней тогда это и есть ответ
            if (leftInd < cards.Count && ms.Y > y_min && ms.Y < y_max)
                number = (int)leftInd;
            if (leftInd > cards.Count - 1 && ms.Y > y_min && ms.Y < y_max)
            {
                // С последней картой все немного посложнее потому что она не перекрывается следующей
                // Поэтому если ответ не был найден среди первых карт нужно проверить последнюю
                double left = drawStartX + drawStepX * (cards.Count - 1);
                double right = left + Card.width;
                if (ms.X <= right && ms.X >= left)
                    number = (int)cards.Count - 1;
            }

            // Определяем позицию выбранной пользователем на данный момент карты
            return number;
        }


        public static void PlayCardAsAttacker(Card card, List<Card> hand)
        {
            combatCard = card;
            combatCardPlayed = true;
            hand.Remove(card);
            combatCard.FaceUp = true;

            if (curr_state == PlayState.PlayerAttack)
                curr_state = PlayState.OpDef;
            else if (curr_state == PlayState.OpAttack)
                curr_state = PlayState.PlayerDef;
        }


        public static void PlayCardAsDefender(Card card, List<Card> hand)
        {
            combatCardPlayed = false;
            hand.Remove(card);           
            turnBeatenCards.Add(combatCard);
            turnBeatenCards.Add(card);

            if (curr_state == PlayState.PlayerDef)
                curr_state = PlayState.OpAttack;
            else if (curr_state == PlayState.OpDef)
                curr_state = PlayState.PlayerAttack;
        }


        public static void ClearTurnBeatenCards(List<Card> destination)
        {                    
            while (turnBeatenCards.Count > 0)
            {                
                destination.Add(turnBeatenCards[0]);
                turnBeatenCards.RemoveAt(0);
            }            
        }


        public static void SetAllCardsFaceUp(List<Card> cards)
        {
            foreach (Card c in cards)
                c.FaceUp = true;
        }


        public static void SetAllCardsFaceDown(List<Card> cards)
        {
            foreach (Card c in cards)
                c.FaceUp = false;
        }
        

        public static void PlayerTakesAllCards()
        {
            combatCardPlayed = false;
            ClearTurnBeatenCards(Player.hand);
            Player.hand.Add(combatCard);
            SetAllCardsFaceUp(Player.hand);
            Play.curr_state = PlayState.OpAttack;

            humanIsFirstToDraw = false;
            firstAttackPassed = true;
            DealCards();
            CheckWinner();
        }


        public static void OpponentTakesAllCards()
        {
            combatCardPlayed = false;
            ClearTurnBeatenCards(Opponent.hand);
            Opponent.hand.Add(combatCard);
            SetAllCardsFaceDown(Opponent.hand);
            Play.curr_state = PlayState.PlayerAttack;

            humanIsFirstToDraw = true;
            firstAttackPassed = true;
            DealCards();
            CheckWinner();
        }           


        public static void NormalTurnEnding()
        {
            ClearTurnBeatenCards(wastedCards);
            combatCardPlayed = false;
            if (curr_state == PlayState.PlayerAttack)
                curr_state = PlayState.OpAttack;
            else if (curr_state == PlayState.OpAttack)
                curr_state = PlayState.PlayerAttack;

            humanIsFirstToDraw = !humanIsFirstToDraw;
            firstAttackPassed = true;
            DealCards();
            CheckWinner();
        }


        public static void CheckWinner()
        {
            if (deck.Count == 0 && trumpCardTaken)
            {
                if (Opponent.hand.Count == 0 && Player.hand.Count == 0)
                    curr_state = PlayState.PlayerTied;

                else if (Opponent.hand.Count == 0)
                    curr_state = PlayState.PLayerLost;

                else if (Player.hand.Count == 0)
                    curr_state = PlayState.PlayerWon;
            }
        }
    }
}
