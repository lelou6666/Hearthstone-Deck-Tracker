using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
<<<<<<< HEAD
=======
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
>>>>>>> refs/remotes/Epix37/master
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Windows;

namespace Hearthstone_Deck_Tracker
{
	public class DeckManager
	{
		private static bool _waitingForClass;
		private static bool _waitingForUserInput;
		private static int _waitingForDraws;
		public static Guid IgnoredDeckId;
		public static List<Card> NotFoundCards { get; set; } = new List<Card>(); 

		public static async Task DetectCurrentDeck()
		{
<<<<<<< HEAD
			var deck = DeckList.Instance.ActiveDeckVersion;
			if(deck == null || !Config.Instance.AutoDeckDetection || deck.DeckId == IgnoredDeckId || _waitingForClass || _waitingForUserInput)
=======
			var deck = DeckList.Instance.ActiveDeck;
			if(deck == null || deck.DeckId == IgnoredDeckId || _waitingForClass || _waitingForUserInput)
>>>>>>> refs/remotes/Epix37/master
				return;
			if(string.IsNullOrEmpty(Core.Game.Player.Class))
			{
				_waitingForClass = true;
				while(string.IsNullOrEmpty(Core.Game.Player.Class))
					await Task.Delay(100);
				_waitingForClass = false;
			}
<<<<<<< HEAD
			var cardEntites = Core.Game.Player.AllCardEntities.Where(x => x.Entity != null && (x.Entity.IsMinion || x.Entity.IsSpell || x.Entity.IsWeapon) && !x.Created && !x.Stolen).GroupBy(x => x.CardId).ToList();
			var notFound = cardEntites.Where(x => !deck.Cards.Any(c => c.Id == x.Key && c.Count >= x.Count())).ToList();
			if(notFound.Any())
			{
				NotFoundCards = notFound.SelectMany(x => x).Select(x => x.Entity?.Card).Distinct().ToList();
				await AutoSelectDeck(Core.Game.Player.Class, Core.Game.CurrentGameMode, cardEntites);
=======
			var cardEntites = Core.Game.Player.RevealedEntities.Where(x => (x.IsMinion || x.IsSpell || x.IsWeapon) && !x.Info.Created && !x.Info.Stolen).GroupBy(x => x.CardId).ToList();
			var notFound = cardEntites.Where(x => !deck.GetSelectedDeckVersion().Cards.Any(c => c.Id == x.Key && c.Count >= x.Count())).ToList();
			if(notFound.Any())
			{
				NotFoundCards = notFound.SelectMany(x => x).Select(x => x.Card).Distinct().ToList();
				Log.Warn("Cards not found in deck: " + string.Join(", ", NotFoundCards.Select(x => $"{x.Name} ({x.Id})")));
				if(Config.Instance.AutoDeckDetection)
					await AutoSelectDeck(Core.Game.Player.Class, Core.Game.CurrentGameMode, cardEntites);
>>>>>>> refs/remotes/Epix37/master
			}
			else
				NotFoundCards.Clear();
		}
<<<<<<< HEAD
		private static async Task AutoSelectDeck(string heroClass, GameMode mode, List<IGrouping<string, CardEntity>> cardEntites = null)
=======
		private static async Task AutoSelectDeck(string heroClass, GameMode mode, List<IGrouping<string, Entity>> cardEntites = null)
>>>>>>> refs/remotes/Epix37/master
		{
			_waitingForDraws++;
			await Task.Delay(500);
			_waitingForDraws--;
			if(_waitingForDraws > 0)
				return;
<<<<<<< HEAD
			var validDecks = DeckList.Instance.Decks.Where(x => x.Class == heroClass && !x.Archived).Select(x => x.GetSelectedDeckVersion()).ToList();
=======
			var validDecks = DeckList.Instance.Decks.Where(x => x.Class == heroClass && !x.Archived).ToList();
>>>>>>> refs/remotes/Epix37/master
			if(mode == GameMode.Arena)
				validDecks = validDecks.Where(x => x.IsArenaDeck && x.IsArenaRunCompleted != true).ToList();
			else if(mode != GameMode.None)
				validDecks = validDecks.Where(x => !x.IsArenaDeck).ToList();
			if(validDecks.Count > 1 && cardEntites != null)
<<<<<<< HEAD
				validDecks = validDecks.Where(x => cardEntites.All(ce => x.Cards.Any(c => c.Id == ce.Key && c.Count >= ce.Count()))).ToList();
=======
				validDecks = validDecks.Where(x => cardEntites.All(ce => x.GetSelectedDeckVersion().Cards.Any(c => c.Id == ce.Key && c.Count >= ce.Count()))).ToList();
>>>>>>> refs/remotes/Epix37/master
			if(validDecks.Count == 0)
			{
				Log.Info("Could not find matching deck.");
				ShowDeckSelectionDialog(validDecks);
				return;
			}
			if(validDecks.Count == 1)
			{
<<<<<<< HEAD
				Log.Info("Found one matching deck!");
				Core.MainWindow.SelectDeck(validDecks.Single(), true);
=======
				var deck = validDecks.Single();
				Log.Info("Found one matching deck: " + deck);
				Core.MainWindow.SelectDeck(deck, true);
>>>>>>> refs/remotes/Epix37/master
				return;
			}
			var lastUsed = DeckList.Instance.LastDeckClass.FirstOrDefault(x => x.Class == heroClass);
			if(lastUsed != null)
			{
				var deck = validDecks.FirstOrDefault(x => x.DeckId == lastUsed.Id);
				if(deck != null)
				{
					Log.Info($"Last used {heroClass} deck matches!");
					Core.MainWindow.SelectDeck(deck, true);
					return;
				}
			}
			ShowDeckSelectionDialog(validDecks);
		}

		private static void ShowDeckSelectionDialog(List<Deck> decks)
		{
			decks.Add(new Deck("Use no deck", "", new List<Card>(), new List<string>(), "", "", DateTime.Now, false, new List<Card>(),
								   SerializableVersion.Default, new List<Deck>(), false, "", Guid.Empty, ""));
<<<<<<< HEAD
			if(decks.Count == 1 && DeckList.Instance.ActiveDeckVersion != null)
=======
			if(decks.Count == 1 && DeckList.Instance.ActiveDeck != null)
>>>>>>> refs/remotes/Epix37/master
			{
				decks.Add(new Deck("No match - Keep using active deck", "", new List<Card>(), new List<string>(), "", "", DateTime.Now, false,
								   new List<Card>(), SerializableVersion.Default, new List<Deck>(), false, "", Guid.Empty, ""));
			}
			_waitingForUserInput = true;
<<<<<<< HEAD
=======
			Log.Info("Waiting for user input...");
>>>>>>> refs/remotes/Epix37/master
			var dsDialog = new DeckSelectionDialog(decks);
			dsDialog.ShowDialog();

			var selectedDeck = dsDialog.SelectedDeck;
			if(selectedDeck != null)
			{
				if(selectedDeck.Name == "Use no deck")
				{
<<<<<<< HEAD
=======
					Log.Info("Auto deck detection disabled.");
>>>>>>> refs/remotes/Epix37/master
					Core.MainWindow.SelectDeck(null, true);
					NotFoundCards.Clear();
				}
				else if(selectedDeck.Name == "No match - Keep using active deck")
				{
<<<<<<< HEAD
					IgnoredDeckId = DeckList.Instance.ActiveDeckVersion?.DeckId ?? Guid.Empty;
					Log.Info($"Now ignoring {DeckList.Instance.ActiveDeckVersion?.Name}");
=======
					IgnoredDeckId = DeckList.Instance.ActiveDeck?.DeckId ?? Guid.Empty;
					Log.Info($"Now ignoring {DeckList.Instance.ActiveDeck?.Name}");
>>>>>>> refs/remotes/Epix37/master
					NotFoundCards.Clear();
				}
				else
				{
					Log.Info("Selected deck: " + selectedDeck.Name);
					Core.MainWindow.SelectDeck(selectedDeck, true);
				}
			}
			else
			{
<<<<<<< HEAD
				Core.MainWindow.ShowMessage("Auto deck selection disabled.", "This can be re-enabled by selecting \"AUTO\" in the bottom right of the deck picker.").Forget();
				Core.MainWindow.DeckPickerList.UpdateAutoSelectToggleButton();
				Config.Save();
			}
			_waitingForUserInput = false;
		}
=======
				Log.Info("Auto deck detection disabled.");
				Core.MainWindow.ShowMessage("Auto deck selection disabled.", "This can be re-enabled by selecting \"AUTO\" in the bottom right of the deck picker.").Forget();
				Config.Instance.AutoDeckDetection = false;
				Config.Save();
				Core.MainWindow.DeckPickerList.UpdateAutoSelectToggleButton();
			}
			_waitingForUserInput = false;
		}

		public static void ResetIgnoredDeckId() => IgnoredDeckId = Guid.Empty;
>>>>>>> refs/remotes/Epix37/master
	}
}
