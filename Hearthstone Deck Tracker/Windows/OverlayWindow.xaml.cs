#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Controls;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
<<<<<<< HEAD
using Hearthstone_Deck_Tracker.Overlay;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.BoardDamage;
=======
using Hearthstone_Deck_Tracker.Utility.Logging;
using static System.Windows.Visibility;
>>>>>>> refs/remotes/Epix37/master
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;

#endregion

namespace Hearthstone_Deck_Tracker.Windows
{
	/// <summary>
	///     Interaction logic for OverlayWindow.xaml
	/// </summary>
	// ReSharper disable once RedundantExtendsListEntry
	public partial class OverlayWindow : Window, INotifyPropertyChanged
	{
		private const double RankCoveredMaxLeft = 0.1;
		private const double PlayerRankCoveredMaxHeight = 0.8;
		private const double OpponentRankCoveredMaxTop = 0.12;
		private const int ChancePanelsMargins = 8;
		private readonly Point[][] _cardMarkPos = new Point[MaxHandSize][];
		private readonly List<CardMarker> _cardMarks = new List<CardMarker>();
		private readonly int _customHeight;
		private readonly int _customWidth;
		private readonly List<UIElement> _debugBoardObjects = new List<UIElement>();
		private readonly GameV2 _game;
		private readonly Dictionary<UIElement, ResizeGrip> _movableElements = new Dictionary<UIElement, ResizeGrip>();
		private readonly int _offsetX;
		private readonly int _offsetY;
		private readonly List<Ellipse> _oppBoard = new List<Ellipse>();
		private readonly List<Ellipse> _playerBoard = new List<Ellipse>();
		private readonly List<Rectangle> _playerHand = new List<Rectangle>();
		private bool? _isFriendsListOpen;
		private int _updateRequestsPlayer;
		private int _updateRequestsOpponent;
		private string _lastToolTipCardId;
		private bool _lmbDown;
		private User32.MouseInput _mouseInput;
		private Point _mousePos;
		private bool _opponentCardsHidden;
		private bool _playerCardsHidden;
		private bool _resizeElement;
		private bool _secretsTempVisible;
		private UIElement _selectedUiElement;
		private bool _uiMovable;

		public OverlayWindow(GameV2 game)
		{
			_game = game;
			InitializeComponent();

			if(Config.Instance.ExtraFeatures && Config.Instance.ForceMouseHook)
				HookMouse();
			ShowInTaskbar = Config.Instance.ShowInTaskbar;
			if(Config.Instance.VisibleOverlay)
				Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#4C0000FF");
			_offsetX = Config.Instance.OffsetX;
			_offsetY = Config.Instance.OffsetY;
			_customWidth = Config.Instance.CustomWidth;
			_customHeight = Config.Instance.CustomHeight;
			if(Config.Instance.ShowBatteryLife)
				EnableBatteryMonitor();
			InitializeCollections();
			GridMain.Visibility = Hidden;
			if(User32.GetHearthstoneWindow() != IntPtr.Zero)
				UpdatePosition();
			Update(true);
			UpdateScaling();
			UpdatePlayerLayout();
			UpdateOpponentLayout();
			GridMain.Visibility = Visible;
		}

		private double ScreenRatio => (4.0 / 3.0) / (Width / Height);
		public bool ForceHidden { get; set; }
		public Visibility WarningVisibility { get; set; }
		public List<Card> PlayerDeck => _game.Player.PlayerCardList;
		public List<Card> OpponentDeck => _game.Opponent.OpponentCardList;
		public event PropertyChangedEventHandler PropertyChanged;

		public double PlayerStackHeight => (Config.Instance.PlayerDeckHeight / 100 * Height) / (Config.Instance.OverlayPlayerScaling / 100);
		public double PlayerListHeight => PlayerStackHeight - PlayerLabelsHeight;
		public double PlayerLabelsHeight => CanvasPlayerChance.ActualHeight + CanvasPlayerCount.ActualHeight
			+ LblPlayerFatigue.ActualHeight + LblDeckTitle.ActualHeight + LblWins.ActualHeight + ChancePanelsMargins;

		public VerticalAlignment PlayerStackPanelAlignment
			=> Config.Instance.OverlayCenterPlayerStackPanel ? VerticalAlignment.Center : VerticalAlignment.Top;

		public double OpponentStackHeight => (Config.Instance.OpponentDeckHeight / 100 * Height) / (Config.Instance.OverlayOpponentScaling / 100);
		public double OpponentListHeight => OpponentStackHeight - OpponentLabelsHeight;

		public double OpponentLabelsHeight => CanvasOpponentChance.ActualHeight + CanvasOpponentCount.ActualHeight
											+ LblOpponentFatigue.ActualHeight + LblWinRateAgainst.ActualHeight + ChancePanelsMargins;

		public VerticalAlignment OpponentStackPanelAlignment
			=> Config.Instance.OverlayCenterOpponentStackPanel ? VerticalAlignment.Center : VerticalAlignment.Top;

		public void ShowOverlay(bool enable)
		{
			if(enable)
			{
				Show();
				if(User32.GetForegroundWindow() == new WindowInteropHelper(this).Handle)
					User32.BringHsToForeground();
			}
			else
				Hide();
		}

		public void ForceHide(bool hide)
		{
			ForceHidden = hide;
			UpdatePosition();
<<<<<<< HEAD
	    }

        private void SetRect(int top, int left, int width, int height)
        {
            Top = top + _offsetY;
            Left = left + _offsetX;
            Width = (_customWidth == -1) ? width : _customWidth;
            Height = (_customHeight == -1) ? height : _customHeight;
            CanvasInfo.Width = (_customWidth == -1) ? width : _customWidth;
            CanvasInfo.Height = (_customHeight == -1) ? height : _customHeight;
            StackPanelAdditionalTooltips.MaxHeight = Height;
        }

        private void ReSizePosLists()
        {
            var totalPlayerLabelsHeight = CanvasPlayerChance.ActualHeight + CanvasPlayerCount.ActualHeight + LblPlayerFatigue.ActualHeight
                                          + LblDeckTitle.ActualHeight + LblWins.ActualHeight;
	        if(!double.IsNaN(Height))
	        {
		        ListViewPlayer.MaxHeight = Height * Config.Instance.PlayerDeckHeight / (Config.Instance.OverlayPlayerScaling / 100) / 100 - totalPlayerLabelsHeight;
				ListViewPlayer.UpdateSize();
	        }

            Canvas.SetTop(StackPanelPlayer, Height * Config.Instance.PlayerDeckTop / 100);
            Canvas.SetLeft(StackPanelPlayer,  Width * Config.Instance.PlayerDeckLeft / 100 - StackPanelPlayer.ActualWidth * Config.Instance.OverlayPlayerScaling / 100);



            var totalOpponentLabelsHeight = CanvasOpponentChance.ActualHeight + CanvasOpponentCount.ActualHeight + LblOpponentFatigue.ActualHeight
                                          + LblWinRateAgainst.ActualHeight;
	        if(!double.IsNaN(Height))
	        {
		        ListViewOpponent.MaxHeight = Height * Config.Instance.OpponentDeckHeight / (Config.Instance.OverlayOpponentScaling / 100) / 100 - totalOpponentLabelsHeight;
				ListViewOpponent.UpdateSize();
	        }

			//secrets
			//StackPanelSecrets.RenderTransform = new ScaleTransform(Config.Instance.SecretsPanelScaling, Config.Instance.SecretsPanelScaling);

			Canvas.SetTop(StackPanelOpponent, Height * Config.Instance.OpponentDeckTop / 100);
            Canvas.SetLeft(StackPanelOpponent, Width * Config.Instance.OpponentDeckLeft / 100);

            //Secrets
            Canvas.SetTop(StackPanelSecrets, Height * Config.Instance.SecretsTop / 100);
            Canvas.SetLeft(StackPanelSecrets, Width * Config.Instance.SecretsLeft / 100);

            // Timers
            Canvas.SetTop(LblTurnTime, Height * Config.Instance.TimersVerticalPosition / 100 - 5);
            Canvas.SetLeft(LblTurnTime, Width * Config.Instance.TimersHorizontalPosition / 100);

            Canvas.SetTop(LblOpponentTurnTime, Height * Config.Instance.TimersVerticalPosition / 100 - Config.Instance.TimersVerticalSpacing);
            Canvas.SetLeft(LblOpponentTurnTime,
                           (Width * Config.Instance.TimersHorizontalPosition / 100) + Config.Instance.TimersHorizontalSpacing);

            Canvas.SetTop(LblPlayerTurnTime, Height * Config.Instance.TimersVerticalPosition / 100 + Config.Instance.TimersVerticalSpacing);
            Canvas.SetLeft(LblPlayerTurnTime, Width * Config.Instance.TimersHorizontalPosition / 100 + Config.Instance.TimersHorizontalSpacing);

            //Canvas.SetTop(LblGrid, Height * 0.03);
            var handCount = _game.Opponent.HandCount;
            if (handCount < 0)
                handCount = 0;
            if (handCount > 10)
                handCount = 10;

            var ratio = (4.0 / 3.0) / (Width / Height);
            for (int i = 0; i < handCount; i++)
            {
                Canvas.SetLeft(_cardMarks[i],
                Helper.GetScaledXPos(_cardMarkPos[handCount - 1][i].X, (int)Width, ratio) - _cardMarks[i].ActualWidth / 2);
                Canvas.SetTop(_cardMarks[i], Math.Max(_cardMarkPos[handCount - 1][i].Y * Height - _cardMarks[i].ActualHeight / 3, 5));
            }

            //Gold progress
            var goldFrameHeight = Height * 25 / 768;
            var goldFrameWidth = 6 * goldFrameHeight;
            var goldFrameOffset = 85 / 25 * goldFrameHeight;
            RectGoldDisplay.Height = goldFrameHeight;
            RectGoldDisplay.Width = goldFrameWidth;
            var left = Width - RectGoldDisplay.ActualWidth - goldFrameOffset;
            var top = Height - RectGoldDisplay.ActualHeight;// - 2;
            Canvas.SetTop(RectGoldDisplay, top);
            Canvas.SetLeft(RectGoldDisplay, left);

            GoldProgressGrid.Height = goldFrameHeight;
            GPLeftCol.Width = new GridLength(goldFrameHeight);
            GPRightCol.Width = new GridLength(goldFrameHeight);
            LblGoldProgress.Margin = new Thickness(goldFrameHeight * 1.2, 0, goldFrameHeight * 0.8, 0);
            LblGoldProgress.FontSize = Height * 0.017;
            Canvas.SetTop(GoldProgressGrid, top + (goldFrameHeight - GoldProgressGrid.ActualHeight) / 2);// - 2);
            Canvas.SetLeft(GoldProgressGrid, left - GoldProgressGrid.ActualWidth - 10);

			//Attack icons
			Canvas.SetTop(IconBoardAttackPlayer, Height * Config.Instance.AttackIconPlayerVerticalPosition / 100);
			Canvas.SetLeft(IconBoardAttackPlayer, Helper.GetScaledXPos(Config.Instance.AttackIconPlayerHorizontalPosition / 100, (int)Width, ratio));
			Canvas.SetTop(IconBoardAttackOpponent, Height * Config.Instance.AttackIconOpponentVerticalPosition / 100);
			Canvas.SetLeft(IconBoardAttackOpponent, Helper.GetScaledXPos(Config.Instance.AttackIconOpponentHorizontalPosition / 100, (int)Width, ratio));
			//Scale attack icons, with height
			var atkWidth = (int)Math.Round(Height * 0.0695, 0);
			var atkFont = (int)Math.Round(Height * 0.0223, 0);
			IconBoardAttackPlayer.Width = atkWidth;
			IconBoardAttackPlayer.Height = atkWidth;
			TextBlockPlayerAttack.FontSize = atkFont;
			IconBoardAttackOpponent.Width = atkWidth;
			IconBoardAttackOpponent.Height = atkWidth;
			TextBlockOpponentAttack.FontSize = atkFont;
		}

        private void Window_SourceInitialized_1(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            User32.SetWindowExStyle(hwnd, User32.WsExTransparent | User32.WsExToolWindow);
        }

        public void Update(bool refresh)
        {
            if (refresh)
            {
               // ListViewPlayer.Items.Refresh();
                //ListViewOpponent.Items.Refresh();
                Topmost = false;
                Topmost = true;
                Logger.WriteLine("Refreshed overlay topmost status", "UpdateOverlay");
            }


            var handCount = _game.Opponent.HandCount;
            if (handCount < 0)
                handCount = 0;
            if (handCount > 10)
                handCount = 10;


            for (var i = 0; i < handCount; i++)
            {
                if (!Config.Instance.HideOpponentCardAge)
                {
                    _cardMarks[i].Text = _game.Opponent.Hand[i].Turn.ToString();
                }
                else
                {
                    _cardMarks[i].Text = "";
                }

                if (!Config.Instance.HideOpponentCardMarks)
                {
                    _cardMarks[i].Mark = _game.Opponent.Hand[i].CardMark;
				}
                else
                {
                    _cardMarks[i].Mark = CardMark.None;
                }

                _cardMarks[i].Visibility = (_game.IsInMenu || (Config.Instance.HideOpponentCardAge && Config.Instance.HideOpponentCardMarks))? Visibility.Hidden : Visibility.Visible;
            }
            //Hide unneeded card marks.
            for (var i = handCount; i < 10; i++)
            {
                _cardMarks[i].Visibility = Visibility.Collapsed;
            }

            StackPanelPlayer.Opacity = Config.Instance.PlayerOpacity / 100;
            StackPanelOpponent.Opacity = Config.Instance.OpponentOpacity / 100;
	        StackPanelSecrets.Opacity = Config.Instance.SecretsOpacity / 100;
            Opacity = Config.Instance.OverlayOpacity / 100;

            if (!_playerCardsHidden)
            {
                StackPanelPlayer.Visibility = (Config.Instance.HideDecksInOverlay || (Config.Instance.HideInMenu && _game.IsInMenu)) && !_uiMovable
                                                  ? Visibility.Collapsed : Visibility.Visible;
            }

            if (!_opponentCardsHidden)
            {
                StackPanelOpponent.Visibility = (Config.Instance.HideDecksInOverlay || (Config.Instance.HideInMenu && _game.IsInMenu))
                                                && !_uiMovable ? Visibility.Collapsed : Visibility.Visible;
            }

            CanvasPlayerChance.Visibility = Config.Instance.HideDrawChances ? Visibility.Collapsed : Visibility.Visible;
            LblPlayerFatigue.Visibility = Config.Instance.HidePlayerFatigueCount ? Visibility.Collapsed : Visibility.Visible;
            CanvasPlayerCount.Visibility = Config.Instance.HidePlayerCardCount ? Visibility.Collapsed : Visibility.Visible;

            CanvasOpponentChance.Visibility = Config.Instance.HideOpponentDrawChances ? Visibility.Collapsed : Visibility.Visible;
            LblOpponentFatigue.Visibility = Config.Instance.HideOpponentFatigueCount ? Visibility.Collapsed : Visibility.Visible;
            CanvasOpponentCount.Visibility = Config.Instance.HideOpponentCardCount ? Visibility.Collapsed : Visibility.Visible;

            if (_game.IsInMenu && !_uiMovable)
                HideTimers();

            ListViewOpponent.Visibility = Config.Instance.HideOpponentCards ? Visibility.Collapsed : Visibility.Visible;
            ListViewPlayer.Visibility = Config.Instance.HidePlayerCards ? Visibility.Collapsed : Visibility.Visible;

            DebugViewer.Visibility = Config.Instance.Debug ? Visibility.Visible : Visibility.Hidden;
            DebugViewer.Width = (Width * Config.Instance.TimerLeft / 100);

            SetCardCount(_game.Player.HandCount, _game.Player.DeckCount);

            SetOpponentCardCount(_game.Opponent.HandCount, _game.Opponent.DeckCount);


            LblWins.Visibility = Config.Instance.ShowDeckWins && _game.IsUsingPremade ? Visibility.Visible : Visibility.Collapsed;
            LblDeckTitle.Visibility = Config.Instance.ShowDeckTitle && _game.IsUsingPremade ? Visibility.Visible : Visibility.Collapsed;
            LblWinRateAgainst.Visibility = Config.Instance.ShowWinRateAgainst && _game.IsUsingPremade ? Visibility.Visible : Visibility.Collapsed;

            var showWarning = !_game.IsInMenu && !_game.Player.DrawnCardsMatchDeck;
            StackPanelWarning.Visibility = showWarning ? Visibility.Visible : Visibility.Collapsed;
            if (showWarning)
            {
				
                var drawn = new Deck { Cards = new ObservableCollection<Card>(_game.Player.DrawnCards.Where(c => !c.IsCreated)) };
                var diff = (drawn - DeckList.Instance.ActiveDeckVersion).Where(c => c.Count > 0).ToList();
                if (diff.Count > 0)
                {
                    var count = diff.Count > 3 ? 3 : diff.Count;
                    LblWarningCards.Text = diff.Take(count).Select(c => c.LocalizedName).Aggregate((c, n) => c + ", " + n);
                    if (diff.Count > 3)
                        LblWarningCards.Text += ", ...";
                }
            }

            if (_game.IsInMenu)
            {
                if (Config.Instance.AlwaysShowGoldProgress)
                {
                    UpdateGoldProgress();
                    GoldProgressGrid.Visibility = Visibility.Visible;
                }
            }
            else
                GoldProgressGrid.Visibility = Visibility.Collapsed;

	        UpdateAttackValues();

            SetDeckTitle();
            SetWinRates();

            ReSizePosLists();


            if (Core.Windows.PlayerWindow.Visibility == Visibility.Visible)
                Core.Windows.PlayerWindow.Update();
            if (Core.Windows.OpponentWindow.Visibility == Visibility.Visible)
                Core.Windows.OpponentWindow.Update();
        }

	    private void UpdateAttackValues()
=======
		}

		private void SetRect(int top, int left, int width, int height)
>>>>>>> refs/remotes/Epix37/master
		{
			Top = top + _offsetY;
			Left = left + _offsetX;
			Width = (_customWidth == -1) ? width : _customWidth;
			Height = (_customHeight == -1) ? height : _customHeight;
			CanvasInfo.Width = (_customWidth == -1) ? width : _customWidth;
			CanvasInfo.Height = (_customHeight == -1) ? height : _customHeight;
			StackPanelAdditionalTooltips.MaxHeight = Height;
		}

<<<<<<< HEAD
	    private void UpdateGoldProgress()
        {
            var region = (int)_game.CurrentRegion - 1;
            if (region >= 0)
            {
                int wins = Config.Instance.GoldProgress[region];
                if (wins >= 0)
                {
                    LblGoldProgress.Text = string.Format("Wins: {0}/3 ({1}/100G)", wins,
                        Config.Instance.GoldProgressTotal[region]);
                }
            }
        }

        private void SetWinRates()
        {
            var selectedDeck = DeckList.Instance.ActiveDeck;
            if (selectedDeck == null)
                return;

            LblWins.Text = string.Format("{0} ({1})", selectedDeck.WinLossString, selectedDeck.WinPercentString);

            if (!string.IsNullOrEmpty(_game.Opponent.Class))
            {
                var winsVS = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Win && g.OpponentHero == _game.Opponent.Class);
                var lossesVS = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Loss && g.OpponentHero == _game.Opponent.Class);
                var percent = (winsVS + lossesVS) > 0 ? Math.Round(winsVS * 100.0 / (winsVS + lossesVS), 0).ToString() : "-";
                LblWinRateAgainst.Text = string.Format("VS {0}: {1} - {2} ({3}%)", _game.Opponent.Class, winsVS, lossesVS, percent);
            }
        }

        private void SetDeckTitle()
        {
            var selectedDeck = DeckList.Instance.ActiveDeckVersion;
            LblDeckTitle.Text = selectedDeck != null ? selectedDeck.Name : string.Empty;
        }

        public bool PointInsideControl(Point pos, double actualWidth, double actualHeight)
        {
            return PointInsideControl(pos, actualWidth, actualHeight, new Thickness(0));
        }

        public bool PointInsideControl(Point pos, double actualWidth, double actualHeight, Thickness margin)
        {
            if (pos.X > 0 - margin.Left && pos.X < actualWidth + margin.Right)
            {
                if (pos.Y > 0 - margin.Top && pos.Y < actualHeight + margin.Bottom)
                    return true;
            }
            return false;
        }

        private async Task UpdateCardTooltip()
        {
            var pos = User32.GetMousePos();
            var relativePlayerDeckPos = ListViewPlayer.PointFromScreen(new Point(pos.X, pos.Y));
            var relativeOpponentDeckPos = ListViewOpponent.PointFromScreen(new Point(pos.X, pos.Y));
            var relativeSecretsPos = StackPanelSecrets.PointFromScreen(new Point(pos.X, pos.Y));
            var relativeCardMark = _cardMarks.Select(x => new { Label = x, Pos = x.PointFromScreen(new Point(pos.X, pos.Y)) });
            var visibility = (Config.Instance.OverlayCardToolTips && !Config.Instance.OverlaySecretToolTipsOnly)
                                 ? Visibility.Visible : Visibility.Hidden;

            var cardMark =
                relativeCardMark.FirstOrDefault(
                                                x =>
                                                x.Label.IsVisible
                                                && PointInsideControl(x.Pos, x.Label.ActualWidth, x.Label.ActualHeight, new Thickness(3, 1, 7, 1)));
            if (!Config.Instance.HideOpponentCardMarks && cardMark != null)
            {
                var index = _cardMarks.IndexOf(cardMark.Label);
                var card = Database.GetCardFromId(_game.Opponent.Hand[index].CardId);
                if (card != null)
                {
                    ToolTipCard.SetValue(DataContextProperty, card);
                    var topOffset = Canvas.GetTop(_cardMarks[index]) + _cardMarks[index].ActualHeight;
                    var leftOffset = Canvas.GetLeft(_cardMarks[index]) + _cardMarks[index].ActualWidth * index;
                    Canvas.SetTop(ToolTipCard, topOffset);
                    Canvas.SetLeft(ToolTipCard, leftOffset);
                    ToolTipCard.Visibility = Config.Instance.OverlayCardMarkToolTips ? Visibility.Visible : Visibility.Hidden;
                }
            }
            //player card tooltips
            else if (ListViewPlayer.Visibility == Visibility.Visible && StackPanelPlayer.Visibility == Visibility.Visible
                    && PointInsideControl(relativePlayerDeckPos, ListViewPlayer.ActualWidth, ListViewPlayer.ActualHeight))
            {
                //card size = card list height / ammount of cards
                var cardSize = ListViewPlayer.ActualHeight / ListViewPlayer.Items.Count;
                var cardIndex = (int)(relativePlayerDeckPos.Y / cardSize);
                if (cardIndex < 0 || cardIndex >= ListViewPlayer.Items.Count)
                    return;

                ToolTipCard.SetValue(DataContextProperty, ((AnimatedCard)ListViewPlayer.Items[cardIndex]).Card);

                //offset is affected by scaling
                var topOffset = Canvas.GetTop(StackPanelPlayer) + GetListViewOffset(StackPanelPlayer)
                                + cardIndex * cardSize * Config.Instance.OverlayPlayerScaling / 100;

                //prevent tooltip from going outside of the overlay
                if (topOffset + ToolTipCard.ActualHeight > Height)
                    topOffset = Height - ToolTipCard.ActualHeight;

                SetTooltipPosition(topOffset, StackPanelPlayer);

                ToolTipCard.Visibility = visibility;
            }
            //opponent card tooltips
            else if (ListViewOpponent.Visibility == Visibility.Visible && StackPanelOpponent.Visibility == Visibility.Visible
                    && PointInsideControl(relativeOpponentDeckPos, ListViewOpponent.ActualWidth, ListViewOpponent.ActualHeight))
            {
                //card size = card list height / ammount of cards
                var cardSize = ListViewOpponent.ActualHeight / ListViewOpponent.Items.Count;
                var cardIndex = (int)(relativeOpponentDeckPos.Y / cardSize);
                if (cardIndex < 0 || cardIndex >= ListViewOpponent.Items.Count)
                    return;

                ToolTipCard.SetValue(DataContextProperty, ((AnimatedCard)ListViewOpponent.Items[cardIndex]).Card);

                //offset is affected by scaling
                var topOffset = Canvas.GetTop(StackPanelOpponent) + GetListViewOffset(StackPanelOpponent)
                                + cardIndex * cardSize * Config.Instance.OverlayOpponentScaling / 100;

                //prevent tooltip from going outside of the overlay
                if (topOffset + ToolTipCard.ActualHeight > Height)
                    topOffset = Height - ToolTipCard.ActualHeight;

                SetTooltipPosition(topOffset, StackPanelOpponent);

                ToolTipCard.Visibility = visibility;
            }
            else if (StackPanelSecrets.Visibility == Visibility.Visible
                    && PointInsideControl(relativeSecretsPos, StackPanelSecrets.ActualWidth, StackPanelSecrets.ActualHeight))
            {
                //card size = card list height / ammount of cards
                var cardSize = StackPanelSecrets.ActualHeight / StackPanelSecrets.Children.Count;
                var cardIndex = (int)(relativeSecretsPos.Y / cardSize);
                if (cardIndex < 0 || cardIndex >= StackPanelSecrets.Children.Count)
                    return;

                ToolTipCard.SetValue(DataContextProperty, StackPanelSecrets.Children[cardIndex].GetValue(DataContextProperty));

                //offset is affected by scaling
                var topOffset = Canvas.GetTop(StackPanelSecrets) + cardIndex * cardSize * Config.Instance.OverlayOpponentScaling / 100;

                //prevent tooltip from going outside of the overlay
                if (topOffset + ToolTipCard.ActualHeight > Height)
                    topOffset = Height - ToolTipCard.ActualHeight;

                SetTooltipPosition(topOffset, StackPanelSecrets);

                ToolTipCard.Visibility = Config.Instance.OverlaySecretToolTipsOnly ? Visibility.Visible : visibility;
            }
            else
            {
                ToolTipCard.Visibility = Visibility.Hidden;
                HideAdditionalToolTips();
            }

            if (ToolTipCard.Visibility == Visibility.Visible)
            {
                var card = ToolTipCard.GetValue(DataContextProperty) as Card;
                if (card != null)
                {
                    if (_lastToolTipCardId != card.Id)
                    {
                        _lastToolTipCardId = card.Id;
                        ShowAdditionalToolTips();
                    }
                }
                else
                    HideAdditionalToolTips();
            }
            else
            {
                HideAdditionalToolTips();
                _lastToolTipCardId = string.Empty;
            }


            if (!Config.Instance.ForceMouseHook)
            {
                if (Config.Instance.ExtraFeatures)
                {
                    var relativePos = PointFromScreen(new Point(pos.X, pos.Y));
                    if ((StackPanelSecrets.IsVisible
                        && (PointInsideControl(StackPanelSecrets.PointFromScreen(new Point(pos.X, pos.Y)), StackPanelSecrets.ActualWidth,
                                               StackPanelSecrets.ActualHeight, new Thickness(20)))
                        || relativePos.X < 170 && relativePos.Y > Height - 120))
                    {
                        if (_mouseInput == null)
                            HookMouse();
                    }
                    else if (_mouseInput != null && !((_isFriendsListOpen.HasValue && _isFriendsListOpen.Value) || await Helper.FriendsListOpen()))
                        UnHookMouse();
                }
                else if (_mouseInput != null)
                    UnHookMouse();
            }

            if (!Config.Instance.AlwaysShowGoldProgress)
            {
                if (_game.IsInMenu
                   && PointInsideControl(RectGoldDisplay.PointFromScreen(new Point(pos.X, pos.Y)), RectGoldDisplay.ActualWidth,
                                         RectGoldDisplay.ActualHeight))
                {
                    UpdateGoldProgress();
                    GoldProgressGrid.Visibility = Visibility.Visible;
                }
                else
                    GoldProgressGrid.Visibility = Visibility.Hidden;
            }
        }

        private double GetListViewOffset(StackPanel stackPanel)
        {
            var offset = 0.0;
            foreach (var child in stackPanel.Children)
            {
                var text = child as HearthstoneTextBlock;
                if (text != null)
                    offset += text.ActualHeight;
                else
                {
                    if (child is ListView)
                        break;
                    var sp = child as StackPanel;
                    if (sp != null)
                        offset += sp.ActualHeight;
                }
            }
            return offset;
        }

        private void HideAdditionalToolTips()
        {
            StackPanelAdditionalTooltips.Visibility = Visibility.Hidden;
        }

        private void SetTooltipPosition(double yOffset, StackPanel stackpanel)
        {
            Canvas.SetTop(ToolTipCard, yOffset);

            if (Canvas.GetLeft(stackpanel) < Width / 2)
                Canvas.SetLeft(ToolTipCard, Canvas.GetLeft(stackpanel) + stackpanel.ActualWidth * Config.Instance.OverlayOpponentScaling / 100);
            else
                Canvas.SetLeft(ToolTipCard, Canvas.GetLeft(stackpanel) - ToolTipCard.ActualWidth);
        }

        public async void UpdatePosition()
        {
            //hide the overlay depenting on options
            ShowOverlay(
                        !((Config.Instance.HideInBackground && !User32.IsHearthstoneInForeground())
                          || (Config.Instance.HideOverlayInSpectator && _game.CurrentGameMode == GameMode.Spectator)
                          || Config.Instance.HideOverlay || ForceHidden));


            var hsRect = User32.GetHearthstoneRect(true);

            //hs window has height 0 if it just launched, screwing things up if the tracker is started before hs is. 
            //this prevents that from happening. 
            if (hsRect.Height == 0 || Visibility != Visibility.Visible)
                return;

            SetRect(hsRect.Top, hsRect.Left, hsRect.Width, hsRect.Height);
            ReSizePosLists();
            try
            {
                await UpdateCardTooltip();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString(), "UpdateOverlayPosition");
            }
        }

        internal void UpdateTurnTimer(TimerEventArgs timerEventArgs)
        {
            if (timerEventArgs.Running && (timerEventArgs.PlayerSeconds > 0 || timerEventArgs.OpponentSeconds > 0))
            {
                ShowTimers();

                LblTurnTime.Text = string.Format("{0:00}:{1:00}", (timerEventArgs.Seconds / 60) % 60, timerEventArgs.Seconds % 60);
                LblPlayerTurnTime.Text = string.Format("{0:00}:{1:00}", (timerEventArgs.PlayerSeconds / 60) % 60, timerEventArgs.PlayerSeconds % 60);
                LblOpponentTurnTime.Text = string.Format("{0:00}:{1:00}", (timerEventArgs.OpponentSeconds / 60) % 60,
                                                         timerEventArgs.OpponentSeconds % 60);

                if (Config.Instance.Debug)
                {
                    LblDebugLog.Text += string.Format("Current turn: {0} {1} {2} \n", timerEventArgs.CurrentActivePlayer, timerEventArgs.PlayerSeconds,
                                                      timerEventArgs.OpponentSeconds);
                    DebugViewer.ScrollToBottom();
                }
            }
        }

        public void UpdateScaling()
        {
            StackPanelPlayer.RenderTransform = new ScaleTransform(Config.Instance.OverlayPlayerScaling / 100,
                                                                  Config.Instance.OverlayPlayerScaling / 100);
			StackPanelOpponent.RenderTransform = new ScaleTransform(Config.Instance.OverlayOpponentScaling / 100,
																	Config.Instance.OverlayOpponentScaling / 100);
	        StackPanelSecrets.RenderTransform = new ScaleTransform(Config.Instance.SecretsPanelScaling,
	                                                               Config.Instance.SecretsPanelScaling);
        }

        public void HideTimers()
        {
            LblPlayerTurnTime.Visibility = LblOpponentTurnTime.Visibility = LblTurnTime.Visibility = Visibility.Hidden;
        }

        public void ShowTimers()
        {
            LblPlayerTurnTime.Visibility =
                LblOpponentTurnTime.Visibility = LblTurnTime.Visibility = Config.Instance.HideTimers ? Visibility.Hidden : Visibility.Visible;
        }

        public void UpdatePlayerLayout()
        {
            StackPanelPlayer.Children.Clear();
            foreach (var item in Config.Instance.PanelOrderPlayer)
            {
                switch (item)
                {
                    case "Cards":
                        StackPanelPlayer.Children.Add(ListViewPlayer);
                        break;
                    case "Draw Chances":
                        StackPanelPlayer.Children.Add(CanvasPlayerChance);
                        break;
                    case "Card Counter":
                        StackPanelPlayer.Children.Add(CanvasPlayerCount);
                        break;
                    case "Fatigue Counter":
                        StackPanelPlayer.Children.Add(LblPlayerFatigue);
                        break;
                    case "Deck Title":
                        StackPanelPlayer.Children.Add(LblDeckTitle);
                        break;
                    case "Wins":
                        StackPanelPlayer.Children.Add(LblWins);
                        break;
                }
            }
        }

        public void UpdateOpponentLayout()
        {
            StackPanelOpponent.Children.Clear();
            foreach (var item in Config.Instance.PanelOrderOpponent)
            {
                switch (item)
                {
                    case "Cards":
                        StackPanelOpponent.Children.Add(ListViewOpponent);
                        break;
                    case "Draw Chances":
                        StackPanelOpponent.Children.Add(CanvasOpponentChance);
                        break;
                    case "Card Counter":
                        StackPanelOpponent.Children.Add(CanvasOpponentCount);
                        break;
                    case "Fatigue Counter":
                        StackPanelOpponent.Children.Add(LblOpponentFatigue);
                        break;
                    case "Win Rate":
                        StackPanelOpponent.Children.Add(ViewBoxWinRateAgainst);
                        break;
                }
            }
        }

        public void ShowSecrets(bool force = false, HeroClass? heroClass = null)
        {
            if (Config.Instance.HideSecrets && !force)
                return;

            StackPanelSecrets.Children.Clear();
            var secrets = heroClass == null ? _game.OpponentSecrets.GetSecrets() : _game.OpponentSecrets.GetDefaultSecrets(heroClass.Value);
            foreach (var id in secrets)
            {
                var cardObj = new Controls.Card();
                var card = Database.GetCardFromId(id.CardId);
                card.Count = id.AdjustedCount(_game);
                cardObj.SetValue(DataContextProperty, card);
                StackPanelSecrets.Children.Add(cardObj);
            }

            StackPanelSecrets.Visibility = Visibility.Visible;
        }

        public void ShowAdditionalToolTips()
        {
            if (!Config.Instance.AdditionalOverlayTooltips)
                return;
            var card = ToolTipCard.DataContext as Card;
            if (card == null)
                return;
            if (!CardIds.SubCardIds.Keys.Contains(card.Id))
            {
                HideAdditionalToolTips();
                return;
            }

            StackPanelAdditionalTooltips.Children.Clear();
            foreach (var id in CardIds.SubCardIds[card.Id])
            {
                var tooltip = new CardToolTip();
                tooltip.SetValue(DataContextProperty, Database.GetCardFromId(id));
                StackPanelAdditionalTooltips.Children.Add(tooltip);
            }

            StackPanelAdditionalTooltips.UpdateLayout();

            //set position
            var tooltipLeft = Canvas.GetLeft(ToolTipCard);
            var left = tooltipLeft < Width / 2 ? tooltipLeft + ToolTipCard.ActualWidth : tooltipLeft - StackPanelAdditionalTooltips.ActualWidth;

            Canvas.SetLeft(StackPanelAdditionalTooltips, left);
            var top = Canvas.GetTop(ToolTipCard) - (StackPanelAdditionalTooltips.ActualHeight / 2 - ToolTipCard.ActualHeight / 2);
            if (top < 0)
                top = 0;
            else if (top + StackPanelAdditionalTooltips.ActualHeight > Height)
                top = Height - StackPanelAdditionalTooltips.ActualHeight;
            Canvas.SetTop(StackPanelAdditionalTooltips, top);

            StackPanelAdditionalTooltips.Visibility = Visibility.Visible;
        }

        public void HideSecrets()
        {
            StackPanelSecrets.Visibility = Visibility.Collapsed;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_mouseInput != null)
                UnHookMouse();
        }

        public async Task<bool> UnlockUI()
        {
            _uiMovable = !_uiMovable;
            Update(false);
            if (_uiMovable)
            {
                //if(!Config.Instance.ExtraFeatures)
                HookMouse();
                if (StackPanelSecrets.Visibility != Visibility.Visible)
                {
                    _secretsTempVisible = true;
                    ShowSecrets(true, HeroClass.Mage);
                    //need to wait for panel to actually show up
                    await Task.Delay(50);
                }
                if (LblTurnTime.Visibility != Visibility.Visible)
                    ShowTimers();
                foreach (var movableElement in _movableElements)
                {
                    try
                    {
                        if (!CanvasInfo.Children.Contains(movableElement.Value))
                            CanvasInfo.Children.Add(movableElement.Value);

                        movableElement.Value.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#4C0000FF");

                        Canvas.SetTop(movableElement.Value, Canvas.GetTop(movableElement.Key));
                        Canvas.SetLeft(movableElement.Value, Canvas.GetLeft(movableElement.Key));

                        var elementSize = GetUiElementSize(movableElement.Key);
                        if (movableElement.Key == StackPanelPlayer)
                        {
                            if (!TrySetResizeGripHeight(movableElement.Value, Config.Instance.PlayerDeckHeight * Height / 100))
                            {
                                Config.Instance.Reset("PlayerDeckHeight");
                                TrySetResizeGripHeight(movableElement.Value, Config.Instance.PlayerDeckHeight * Height / 100);
                            }
                        }
                        else if (movableElement.Key == StackPanelOpponent)
                        {
                            if (!TrySetResizeGripHeight(movableElement.Value, Config.Instance.OpponentDeckHeight * Height / 100))
                            {
                                Config.Instance.Reset("OpponentDeckHeight");
                                TrySetResizeGripHeight(movableElement.Value, Config.Instance.OpponentDeckHeight * Height / 100);
                            }
                        }
                        else if (movableElement.Key == StackPanelSecrets)
                            movableElement.Value.Height = StackPanelSecrets.ActualHeight > 0 ? StackPanelSecrets.ActualHeight : 0;
                        else
                            movableElement.Value.Height = elementSize.Height > 0 ? elementSize.Height : 0;
=======
		private void Window_SourceInitialized_1(object sender, EventArgs e)
		{
			var hwnd = new WindowInteropHelper(this).Handle;
			User32.SetWindowExStyle(hwnd, User32.WsExTransparent | User32.WsExToolWindow);
		}
>>>>>>> refs/remotes/Epix37/master


		public void HideTimers() => LblPlayerTurnTime.Visibility = LblOpponentTurnTime.Visibility = LblTurnTime.Visibility = Hidden;

		public void ShowTimers()
			=>
				LblPlayerTurnTime.Visibility =
				LblOpponentTurnTime.Visibility = LblTurnTime.Visibility = Config.Instance.HideTimers ? Hidden : Visible;

		public void ShowSecrets(bool force = false, HeroClass? heroClass = null)
		{
			if(Config.Instance.HideSecrets && !force)
				return;

			StackPanelSecrets.Children.Clear();
			var secrets = heroClass == null ? _game.OpponentSecrets.GetSecrets() : _game.OpponentSecrets.GetDefaultSecrets(heroClass.Value);
			foreach(var id in secrets)
			{
				var cardObj = new Controls.Card();
				var card = Database.GetCardFromId(id.CardId);
				card.Count = id.AdjustedCount(_game);
				cardObj.SetValue(DataContextProperty, card);
				StackPanelSecrets.Children.Add(cardObj);
			}

			StackPanelSecrets.Visibility = Visible;
		}

		public void HideSecrets() => StackPanelSecrets.Visibility = Collapsed;

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if(_mouseInput != null)
				UnHookMouse();
			DisableBatteryMonitor();
		}

		private void OverlayWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			//in addition to setting this in mainwindow_load: (in case of minimized)
			var presentationsource = PresentationSource.FromVisual(this);
			Helper.DpiScalingX = presentationsource?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
			Helper.DpiScalingY = presentationsource?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;
		}

<<<<<<< HEAD
	    private DateTime _lastPlayerUpdateReqest = DateTime.MinValue;
		public async void UpdatePlayerCards(bool reset)
		{
			_lastPlayerUpdateReqest = DateTime.Now;
			await Task.Delay(50);
			if((DateTime.Now - _lastPlayerUpdateReqest).Milliseconds < 50)
				return;
			//OnPropertyChanged("PlayerDeck");
			ListViewPlayer.Update(_game.Player.DisplayCards, true, reset);
		}

		private DateTime _lastOpponentUpdateReqest = DateTime.MinValue;
		public async void UpdateOpponentCards(bool reset)
		{
			_lastOpponentUpdateReqest = DateTime.Now;
			await Task.Delay(50);
			if((DateTime.Now - _lastOpponentUpdateReqest).Milliseconds < 50)
				return;
			//OnPropertyChanged("OpponentDeck");
			ListViewOpponent.Update(_game.Opponent.DisplayRevealedCards, false, reset);
=======
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool IsRankConvered(bool requireOpponentRank = false)
		{
			if(Canvas.GetLeft(StackPanelPlayer) < RankCoveredMaxLeft * Width)
			{
				if(Canvas.GetTop(StackPanelPlayer) + StackPanelPlayer.ActualHeight > PlayerRankCoveredMaxHeight * Height)
				{
					Log.Info("Player rank is potentially covered by player deck.");
					return true;
				}
				if(Canvas.GetTop(StackPanelPlayer) < OpponentRankCoveredMaxTop * Height)
				{
					Log.Info("Opponent rank is potentially covered by player deck.");
					if(requireOpponentRank)
						return true;
				}
			}
			if(Canvas.GetLeft(StackPanelOpponent) < RankCoveredMaxLeft * Width)
			{
				if(Canvas.GetTop(StackPanelOpponent) + StackPanelOpponent.ActualHeight > PlayerRankCoveredMaxHeight * Height)
				{
					Log.Info("Player rank is potentially covered by opponent deck.");
					return true;
				}
				if(Canvas.GetTop(StackPanelOpponent) < OpponentRankCoveredMaxTop * Height)
				{
					Log.Info("Opponent rank is potentially covered by opponent deck.");
					if(requireOpponentRank)
						return true;
				}
			}
			Log.Info("No ranks should be covered by any decks.");
			return false;
>>>>>>> refs/remotes/Epix37/master
		}

		public void ShowFriendsListWarning(bool show) => StackPanelFriendsListWarning.Visibility = show ? Visible : Collapsed;

		public void ShowRestartRequiredWarning() => TextBlockRestartWarning.Visibility = Visible;

		public void HideRestartRequiredWarning() => TextBlockRestartWarning.Visibility = Collapsed;
	}
}