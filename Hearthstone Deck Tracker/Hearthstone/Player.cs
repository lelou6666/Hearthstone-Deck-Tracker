#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Utility.Extensions;

#endregion

namespace Hearthstone_Deck_Tracker.Hearthstone
{
	public class Player : INotifyPropertyChanged
	{
		public const int DeckSize = 30;
		private string _name;
		private readonly GameV2 _game;

		public Player(GameV2 game, bool isLocalPlayer)
		{
			_game = game;
			IsLocalPlayer = isLocalPlayer;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				Log(value);
			}
		}

		public string Class { get; set; }
		public int Id { get; set; }
		public bool GoingFirst { get; set; }
		public int Fatigue { get; set; }
		public bool IsLocalPlayer { get; }

		public bool HasCoin => Hand.Any(e => e.CardId == HearthDb.CardIds.NonCollectible.Neutral.TheCoin);
		public int HandCount => Hand.Count(x => x.IsControlledBy(Id));
		public int DeckCount => Deck.Count(x => x.IsControlledBy(Id));

<<<<<<< HEAD
		public IEnumerable<Entity> PlayerEntites => Core.Game.Entities.Values.Where(x => !x.Info.HasOutstandingTagChanges && x.IsControlledBy(Id));
		public IEnumerable<Entity> RevealedCards => Core.Game.Entities.Values.Where(x => !x.Info.HasOutstandingTagChanges && (x.IsControlledBy(Id) || x.Info.OriginalController == Id)).Where(x => x.HasCardId);
		public IEnumerable<Entity> Hand => PlayerEntites.Where(x => x.IsInHand);
		public IEnumerable<Entity> Board => PlayerEntites.Where(x => x.IsInPlay);
		public IEnumerable<Entity> Deck => PlayerEntites.Where(x => x.IsInDeck);
		public IEnumerable<Entity> Graveyard => PlayerEntites.Where(x => x.IsInGraveyard);
		public IEnumerable<Entity> Secrets => PlayerEntites.Where(x => x.IsInSecret);
		public IEnumerable<Entity> SetAside => PlayerEntites.Where(x => x.IsInSetAside);

		public List<JoustedCard> KnownCardsInDeck { get; } = new List<JoustedCard>();

		private Tuple<List<Card>, List<Card>> GetCardsInDeck()
		{
			var knownEntitesInDeck =
				Deck.Where(x => x.HasCardId && (x.Info.Created || x.Info.Stolen)).GroupBy(ce => new { ce.CardId, Created = (ce.Info.Created || ce.Info.Stolen), ce.Info.Discarded }).Select(g =>
				{
					var card = Database.GetCardFromId(g.Key.CardId);
					card.Count = g.Count();
					card.IsCreated = g.Key.Created;
					card.HighlightInHand = Hand.Any(ce => ce.CardId == g.Key.CardId);
					return card;
				}).ToList();
			var foo = DeckList.Instance.ActiveDeckVersion.Cards.Select(x => Enumerable.Repeat(x.Id, x.Count)).SelectMany(x => x).ToList();
			var revealed = RevealedCards.Where(x => !x.Info.Created && (x.IsSpell || x.IsWeapon || x.IsMinion) && (!x.IsInDeck || (x.Info.Stolen && x.Info.OriginalController == Id))).ToList();
			var notInDeck = new List<string>();
			foreach(var e in revealed)
			{
				foo.Remove(e.CardId);
				if(!e.Info.Stolen || e.Info.OriginalController == Id)
					notInDeck.Add(e.CardId);
			}
			return new Tuple<List<Card>, List<Card>>(knownEntitesInDeck.Concat(foo.GroupBy(x => x).Select(x =>
			{
				var card = Database.GetCardFromId(x.Key);
				card.Count = x.Count();
				if(Hand.Any(e => e.CardId == x.Key))
					card.HighlightInHand = true;
				return card;
			})).ToList(), notInDeck.GroupBy(x => x).Select(c =>
			{
				var card = Database.GetCardFromId(c.Key);
				card.Count = 0;
				if(Hand.Any(e => e.CardId == c.Key))
					card.HighlightInHand = true;
				return card;
			}).ToList());
		}

		public List<Card> DisplayCards
		{
			get
			{
				var createdInHand = Config.Instance.ShowPlayerGet ? Hand.Where(x => x.Info.Created).GroupBy(x => x.CardId).Select(x => CardSelector(x, true)).ToList() : new List<Card>();
				var revealed =
					RevealedCards.Where(x => !x.Info.Created && (x.IsMinion || x.IsSpell || x.IsWeapon) && (!x.IsInDeck || (x.Info.Stolen && x.Info.OriginalController == Id)))
								 .GroupBy(x => new {x.CardId, Stolen = x.Info.Stolen && x.Info.OriginalController != Id})
								 .Select(x =>
								 {
									 var card = Database.GetCardFromId(x.Key.CardId);
									 card.Count = x.Count();
									 card.IsCreated = x.Key.Stolen;
									 card.HighlightInHand = x.Any(c => c.IsInHand);
									 return card;
								 });
				var knownEntitesInDeck =
					Deck.Where(x => x.HasCardId).GroupBy(ce => new { ce.CardId, Created = (ce.Info.Created || ce.Info.Stolen)}).Select(g =>
=======
		public IEnumerable<Entity> PlayerEntities => _game.Entities.Values.Where(x => !x.Info.HasOutstandingTagChanges && x.IsControlledBy(Id));
		public IEnumerable<Entity> RevealedEntities => _game.Entities.Values.Where(x => !x.Info.HasOutstandingTagChanges && (x.IsControlledBy(Id) || x.Info.OriginalController == Id)).Where(x => x.HasCardId);
		public IEnumerable<Entity> Hand => PlayerEntities.Where(x => x.IsInHand);
		public IEnumerable<Entity> Board => PlayerEntities.Where(x => x.IsInPlay);
		public IEnumerable<Entity> Deck => PlayerEntities.Where(x => x.IsInDeck);
		public IEnumerable<Entity> Graveyard => PlayerEntities.Where(x => x.IsInGraveyard);
		public IEnumerable<Entity> Secrets => PlayerEntities.Where(x => x.IsInSecret);
		public IEnumerable<Entity> SetAside => PlayerEntities.Where(x => x.IsInSetAside);

		public List<PredictedCard> InDeckPrecitions { get; } = new List<PredictedCard>();

		private DeckState GetDeckState()
		{
			var createdCardsInDeck =
				Deck.Where(x => x.HasCardId && (x.Info.Created || x.Info.Stolen))
					.GroupBy(ce => new {ce.CardId, Created = (ce.Info.Created || ce.Info.Stolen), ce.Info.Discarded})
					.Select(g =>
>>>>>>> refs/remotes/Epix37/master
					{
						var card = Database.GetCardFromId(g.Key.CardId);
						card.Count = g.Count();
						card.IsCreated = g.Key.Created;
<<<<<<< HEAD
						card.Jousted = true;
						return card;
					}).ToList();
				var knownCardsInDeck = KnownCardsInDeck.Select(x =>
				{
					var card = Database.GetCardFromId(x.CardId);
					card.Jousted = true;
					return card;
				});
				if(DeckList.Instance.ActiveDeck == null)
					return revealed.Concat(createdInHand).Concat(knownEntitesInDeck).Concat(knownCardsInDeck).ToSortedCardList();
				var cards = GetCardsInDeck();
				var inDeck = cards.Item1;
				var notInDeck = cards.Item2.Where(x => inDeck.All(c => x.Id != c.Id)).ToList();
				if(Config.Instance.RemoveCardsFromDeck)
				{
					if(!Config.Instance.HighlightCardsInHand)
						return inDeck.Concat(createdInHand).ToSortedCardList();
					var inHand = DeckList.Instance.ActiveDeckVersion.Cards.Where(c => inDeck.All(c2 => c2.Id != c.Id) && Hand.Any(ce => c.Id == ce.CardId))
									.Select(c =>
									{
										var card = (Card)c.Clone();
										card.Count = 0;
										card.HighlightInHand = true;
										return card;
									});
					return inDeck.Concat(inHand).Concat(createdInHand).ToSortedCardList();
				}
				return inDeck.Concat(notInDeck).Concat(createdInHand).ToSortedCardList();
=======
						card.HighlightInHand = Hand.Any(ce => ce.CardId == g.Key.CardId);
						return card;
					});
			var originalCardsInDeck = DeckList.Instance.ActiveDeckVersion.Cards.Select(x => Enumerable.Repeat(x.Id, x.Count)).SelectMany(x => x).ToList();
			var revealedNotInDeck = RevealedEntities.Where(x => !x.Info.Created && (x.IsSpell || x.IsWeapon || x.IsMinion) && (!x.IsInDeck || (x.Info.Stolen && x.Info.OriginalController == Id))).ToList();
			var removedFromDeck = new List<string>();
			foreach(var e in revealedNotInDeck)
			{
				originalCardsInDeck.Remove(e.CardId);
				if(!e.Info.Stolen || e.Info.OriginalController == Id)
					removedFromDeck.Add(e.CardId);
>>>>>>> refs/remotes/Epix37/master
			}
			return new DeckState(createdCardsInDeck.Concat(originalCardsInDeck.GroupBy(x => x).Select(x =>
			{
				var card = Database.GetCardFromId(x.Key);
				card.Count = x.Count();
				if(Hand.Any(e => e.CardId == x.Key))
					card.HighlightInHand = true;
				return card;
			})), removedFromDeck.GroupBy(x => x).Select(c =>
			{
				var card = Database.GetCardFromId(c.Key);
				card.Count = 0;
				if(Hand.Any(e => e.CardId == c.Key))
					card.HighlightInHand = true;
				return card;
			}));
		}

<<<<<<< HEAD
		private Card CardSelector(IGrouping<string, Entity> entityGrouping, bool created)
		{
			var card = Database.GetCardFromId(entityGrouping.Key);
			card.Count = entityGrouping.Count();
			card.IsCreated = created;
			card.HighlightInHand = Hand.Any(ce => ce.CardId == card.Id);
			return card;
		}

		public List<Card> DisplayRevealedCards 
			=> RevealedCards.Where(x => 
				(x.IsMinion || x.IsSpell || x.IsWeapon || !x.HasTag(GAME_TAG.CARDTYPE)) 
			&& (x.GetTag(GAME_TAG.CREATOR) == 1 || (!x.Info.Created && x.Info.OriginalController == Id) || x.IsInHand || x.IsInDeck) 
			&& !(x.Info.Created && x.IsInSetAside))
			.GroupBy(e => new {e.CardId, Hidden = (e.IsInHand || e.IsInDeck), Created = e.Info.Created || (e.Info.Stolen && e.Info.OriginalController != Id), Discarded = e.Info.Discarded && Config.Instance.HighlightDiscarded})
				.Select(g =>
				{
					var card = Database.GetCardFromId(g.Key.CardId);
					card.Count = g.Count();
					card.Jousted = g.Key.Hidden;
					card.IsCreated = g.Key.Created;
					card.WasDiscarded = g.Key.Discarded;
					return card;
				}).Concat(KnownCardsInDeck.Select(x =>
				{
					var card = Database.GetCardFromId(x.CardId);
					card.Jousted = true;
					return card;
				})).ToSortedCardList();
=======
		public IEnumerable<Card> PredictedCardsInDeck => InDeckPrecitions.Select(x =>
		{
			var card = Database.GetCardFromId(x.CardId);
			card.Jousted = true;
			return card;
		});

		public IEnumerable<Card> KnownCardsInDeck
			=> Deck.Where(x => x.HasCardId).GroupBy(ce => new {ce.CardId, Created = (ce.Info.Created || ce.Info.Stolen)}).Select(g =>
			{
				var card = Database.GetCardFromId(g.Key.CardId);
				card.Count = g.Count();
				card.IsCreated = g.Key.Created;
				card.Jousted = true;
				return card;
			}).ToList();

		public IEnumerable<Card> RevealedCards
			=> RevealedEntities.Where(x => !x.Info.Created && (x.IsMinion || x.IsSpell || x.IsWeapon)
									   && ((!x.IsInDeck && (!x.Info.Stolen || x.Info.OriginalController == Id)) || (x.Info.Stolen && x.Info.OriginalController == Id)))
								.GroupBy(x => new {x.CardId, Stolen = x.Info.Stolen && x.Info.OriginalController != Id})
								.Select(x =>
								{
									var card = Database.GetCardFromId(x.Key.CardId);
									card.Count = x.Count();
									card.IsCreated = x.Key.Stolen;
									card.HighlightInHand = x.Any(c => c.IsInHand && c.IsControlledBy(Id));
									return card;
								});

		public IEnumerable<Card> CreatedCardsInHand => Hand.Where(x => (x.Info.Created || x.Info.Stolen)).GroupBy(x => x.CardId).Select(x =>
		{
			var card = Database.GetCardFromId(x.Key);
			card.Count = x.Count();
			card.IsCreated = true;
			card.HighlightInHand = true;
			return card;
		});

		public IEnumerable<Card> GetHighlightedCardsInHand(List<Card> cardsInDeck)
			=> DeckList.Instance.ActiveDeckVersion.Cards.Where(c => cardsInDeck.All(c2 => c2.Id != c.Id) && Hand.Any(ce => c.Id == ce.CardId))
						.Select(c =>
						{
							var card = (Card)c.Clone();
							card.Count = 0;
							card.HighlightInHand = true;
							return card;
						});

		public List<Card> PlayerCardList
		{
			get
			{
				var createdInHand = Config.Instance.ShowPlayerGet ? CreatedCardsInHand : new List<Card>();
				if(DeckList.Instance.ActiveDeck == null)
					return RevealedCards.Concat(createdInHand).Concat(KnownCardsInDeck).Concat(PredictedCardsInDeck).ToSortedCardList();
				var deckState = GetDeckState();
				var inDeck = deckState.RemainingInDeck.ToList();
				var notInDeck = deckState.RemovedFromDeck.Where(x => inDeck.All(c => x.Id != c.Id)).ToList();
				if(!Config.Instance.RemoveCardsFromDeck)
					return inDeck.Concat(notInDeck).Concat(createdInHand).ToSortedCardList();
				if(Config.Instance.HighlightCardsInHand)
					return inDeck.Concat(GetHighlightedCardsInHand(inDeck)).Concat(createdInHand).ToSortedCardList();
				return inDeck.Concat(createdInHand).ToSortedCardList();
			}
		}

		public List<Card> OpponentCardList 
			=> RevealedEntities.Where(x => (x.IsMinion || x.IsSpell || x.IsWeapon || !x.HasTag(GAME_TAG.CARDTYPE))
											&& (x.GetTag(GAME_TAG.CREATOR) == 1 || (!x.Info.Created && x.Info.OriginalController == Id) || x.IsInHand || x.IsInDeck)
											&& !(x.Info.Created && x.IsInSetAside))
								.GroupBy(e => new { e.CardId, Hidden = (e.IsInHand || e.IsInDeck) && e.IsControlledBy(Id),
													Created = e.Info.Created || (e.Info.Stolen && e.Info.OriginalController != Id),
													Discarded = e.Info.Discarded && Config.Instance.HighlightDiscarded
								}).Select(g =>
								{
									var card = Database.GetCardFromId(g.Key.CardId);
									card.Count = g.Count();
									card.Jousted = g.Key.Hidden;
									card.IsCreated = g.Key.Created;
									card.WasDiscarded = g.Key.Discarded;
									return card;
								}).Concat(InDeckPrecitions.Select(x =>
								{
									var card = Database.GetCardFromId(x.CardId);
									card.Jousted = true;
									return card;
								})).ToSortedCardList();
>>>>>>> refs/remotes/Epix37/master

		public event PropertyChangedEventHandler PropertyChanged;

		public void Reset()
		{
			Name = "";
			Class = "";
			Id = -1;
			GoingFirst = false;
			Fatigue = 0;
<<<<<<< HEAD
			KnownCardsInDeck.Clear();
=======
			InDeckPrecitions.Clear();
>>>>>>> refs/remotes/Epix37/master
		}

		public void Draw(Entity entity, int turn)
		{
			if(IsLocalPlayer)
				UpdateKnownEntitesInDeck(entity.CardId);
			if(!IsLocalPlayer)
<<<<<<< HEAD
				entity.Info.Hidden = true;
=======
			{
				if(_game.OpponentEntity?.GetTag(GAME_TAG.MULLIGAN_STATE) == (int)TAG_MULLIGAN.DEALING)
					entity.Info.Mulliganed = true;
				else
					entity.Info.Hidden = true;
			}
>>>>>>> refs/remotes/Epix37/master
			entity.Info.Turn = turn;
			Log(entity);
		}


		private void Log(Entity entity, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "") 
			=> Log(entity.ToString(), memberName, sourceFilePath);

		private void Log(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "") 
			=> Utility.Logging.Log.Info((IsLocalPlayer ? "[Player] "  : "[Opponent] ") + msg, memberName, sourceFilePath);

		public void Play(Entity entity, int turn)
		{
			if(!IsLocalPlayer)
				UpdateKnownEntitesInDeck(entity.CardId, entity.Info.Turn);
			if(entity.GetTag(GAME_TAG.CARDTYPE) == (int)TAG_CARDTYPE.TOKEN)
				entity.Info.Created = true;
<<<<<<< HEAD
=======
			entity.Info.Hidden = false;
>>>>>>> refs/remotes/Epix37/master
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void DeckToPlay(Entity entity, int turn)
		{
			UpdateKnownEntitesInDeck(entity.CardId);
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void CreateInHand(Entity entity, int turn)
		{
			entity.Info.Created = true;
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void CreateInDeck(Entity entity, int turn)
		{
			entity.Info.Created |= turn > 1;
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void CreateInPlay(Entity entity, int turn)
		{
			entity.Info.Created = true;
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void RemoveFromDeck(Entity entity, int turn)
		{
			//Do not check for KnownCardIds here, this is how jousted cards get removed from the deck
			entity.Info.Turn = turn;
			entity.Info.Discarded = true;
			Log(entity);
		}

<<<<<<< HEAD
		public void Mulligan(Entity entity)
		{
			entity.Info.Mulliganed = true;
			Log(entity);
		}
=======
		public void Mulligan(Entity entity) => Log(entity);
>>>>>>> refs/remotes/Epix37/master

		public void HandDiscard(Entity entity, int turn)
		{
			if(!IsLocalPlayer)
				UpdateKnownEntitesInDeck(entity.CardId, entity.Info.Turn);
			entity.Info.Turn = turn;
			entity.Info.Discarded = true;
			Log(entity);
		}

		public void DeckDiscard(Entity entity, int turn)
		{
			UpdateKnownEntitesInDeck(entity.CardId);
			entity.Info.Turn = turn;
			entity.Info.Discarded = true;
			Log(entity);
		}

		public void BoardToDeck(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			entity.Info.Returned = true;
			Log(entity);
		}

		public void BoardToHand(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			entity.Info.Returned = true;
			Log(entity);
		}

		public void JoustReveal(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
<<<<<<< HEAD
			var card = KnownCardsInDeck.FirstOrDefault(x => x.CardId == entity.CardId);
			if(card != null)
				card.Turn = turn;
			else
				KnownCardsInDeck.Add(new JoustedCard(entity.CardId, turn));
=======
			var card = InDeckPrecitions.FirstOrDefault(x => x.CardId == entity.CardId);
			if(card != null)
				card.Turn = turn;
			else
				InDeckPrecitions.Add(new PredictedCard(entity.CardId, turn));
>>>>>>> refs/remotes/Epix37/master
			Log(entity);
		}

		private void UpdateKnownEntitesInDeck(string cardId, int turn = int.MaxValue)
		{
<<<<<<< HEAD
			var card = KnownCardsInDeck.FirstOrDefault(x => x.CardId == cardId && turn >= x.Turn);
			if(card != null)
				KnownCardsInDeck.Remove(card);
=======
			var card = InDeckPrecitions.FirstOrDefault(x => x.CardId == cardId && turn >= x.Turn);
			if(card != null)
				InDeckPrecitions.Remove(card);
>>>>>>> refs/remotes/Epix37/master
		}

		public void SecretTriggered(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void SecretPlayedFromDeck(Entity entity, int turn)
		{
			UpdateKnownEntitesInDeck(entity.CardId);
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void SecretPlayedFromHand(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void PlayToGraveyard(Entity entity, string cardId, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void RemoveFromPlay(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void StolenByOpponent(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
		}

		public void StolenFromOpponent(Entity entity, int turn)
		{
			entity.Info.Turn = turn;
			Log(entity);
<<<<<<< HEAD
		}
	}

	public class JoustedCard
	{
		public string CardId { get; set; }
		public int Turn { get; set; }

		public JoustedCard(string cardId, int turn)
		{
			CardId = cardId;
			Turn = turn;
=======
>>>>>>> refs/remotes/Epix37/master
		}
	}
}