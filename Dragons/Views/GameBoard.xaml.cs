using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dragons.Common;
using Dragons.Services;
using Dragons.Shared;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Dragons.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameBoard : Page
    {       
        private bool waitingForOpponent = true;
        private Player currentMover = Player.Player1;
        private Player currentPlayer;
        private Player currentOpponent;
        private int movesProcessed = 0;
        private Match currentMatch;
        private Shared.Grid playerBoard;
        private Shared.Grid opponentBoard;
        private Shared.Grid playerPieces = new Shared.Grid();
        private Shared.Grid opponentPieces = new Shared.Grid();
        private int currentMana;
        private bool gameOver = false;
        private string selectedMoveKey = "";
        private int selectedMoveCost = 0;
        private Coordinate selectedCoordinate;
        private object lastSender;
        private bool waitingDisplayHidden = false;
        private PlayerBoard _PlayerBoard;

        public GameBoard()
        {
            this.InitializeComponent();
            
            OpponentBoard.CoordinateSelected += OpponentBoard_CoordinateSelected;
            WaitingDisplay.Visibility = Visibility.Visible;
            TitleBar.Text = string.Format("{0} vs. ?", LobbyService.ClientName); 

            StartWaitToJoinLoop();
        }

        private async void OpponentBoard_CoordinateSelected(object sender, Coordinate e)
        {
            if(lastSender != null)
                ((BoardButton)lastSender).UnHighlight();
            
            lastSender = sender;
            selectedCoordinate = e;
            await UpdateButtonStates();
        }

        private void StartWaitToJoinLoop()
        {
            Task.Run(() => WaitLoop());
        }

        private async void WaitLoop()
        {
            currentMatch = await MatchService.StartMatch();
            waitingForOpponent = String.IsNullOrEmpty(currentMatch.Player2ClientId);
            currentPlayer = currentMatch.Player1ClientId == LobbyService.ClientId ? Player.Player1 : Player.Player2;
            currentOpponent = currentMatch.Player1ClientId != LobbyService.ClientId ? Player.Player1 : Player.Player2;
            
            playerBoard = currentMatch.Player1ClientId == LobbyService.ClientId ? currentMatch.Player1Layout : currentMatch.Player2Layout;
            
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _PlayerBoard = new PlayerBoard(playerBoard);
                    PlayerBoardGrid.Children.Add(_PlayerBoard);
                });

            opponentBoard = currentMatch.Player1ClientId != LobbyService.ClientId ? currentMatch.Player1Layout : currentMatch.Player2Layout;
            Shared.Grid allPlayerPieces = currentMatch.Player1ClientId == LobbyService.ClientId ? currentMatch.Player1Pieces : currentMatch.Player2Pieces;
            Shared.Grid allOpponentPieces = currentMatch.Player1ClientId != LobbyService.ClientId ? currentMatch.Player1Pieces : currentMatch.Player2Pieces;

            foreach (var piece in allPlayerPieces)
            {
                if (piece.First().PieceKey.StartsWith("Dragon"))
                    playerPieces.Add(piece);
            }
            foreach (var piece in allOpponentPieces)
            {
                if (piece.First().PieceKey.StartsWith("Dragon"))
                    opponentPieces.Add(piece);
            }

            if(currentPlayer == Player.Player1)
                currentMana = currentMatch.InitalMana;
            
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CurrentManaText.Text = currentMana.ToString(); });
            await UpdateButtonStates();
            while (waitingForOpponent && !gameOver)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        WaitingDisplayText.Text = "Waiting for opponent to join...";
                    });
                currentMatch = await MatchService.GetMatch(currentMatch.MatchId);
                waitingForOpponent = String.IsNullOrEmpty(currentMatch.Player2ClientId);
                await Task.Delay(100);
            }
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  async () => { await SoundManager.PlaySound("gamestarts.mp3"); });
            StartGameLoop();
        }

        #region Main game loop
        private void StartGameLoop()
        {
            Task.Run(() => GameLoop());
        }

        private async void GameLoop()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    WaitingDisplay.Visibility = Visibility.Collapsed; waitingDisplayHidden = true;
                    TitleBar.Text = string.Format("{0} vs. {1}", currentPlayer == Player.Player1 ? currentMatch.Player1ClientName : currentMatch.Player2ClientName, currentOpponent == Player.Player1 ? currentMatch.Player1ClientName : currentMatch.Player2ClientName);
                });
            
            while (String.IsNullOrEmpty(currentMatch.WinnerClientId) && !gameOver)
            {
                if (!waitingDisplayHidden)
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { WaitingDisplay.Visibility = Visibility.Collapsed;
                                                                                     waitingDisplayHidden = true;
                    });

                var moves = await MatchService.GetMoves(currentMatch.MatchId, movesProcessed);
                foreach (var move in moves)
                {
                    await ProcessMove(move);
                    if(move.Mover == currentOpponent)
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await _PlayerBoard.RefreshGrid(playerBoard));
                }
                if(moves.Count > 0)
                    await RefreshGameBoards();
                await Task.Delay(100);
            }
        }

        private async Task RefreshGameBoards()
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CurrentManaText.Text = currentMana.ToString(); });

                // update opponent board
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await OpponentBoard.RefreshGrid(opponentBoard));
                await Task.Delay(3000);
                //Check for killed dragons
                await ProcessPieces();
                await Task.Delay(500);
                //Check for win condition
                await CheckForWin();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
        }

        private bool HasDamage(Cell cell, Shared.Grid board)
        {
            if (board[cell.Coordinate.x][cell.Coordinate.y].LastMoves.Count > 0)
                return true;
            else
                return false;
        }

        private async Task ProcessPieces()
        {
            var playerPiecesToRemove = new List<int>();
            var opponentPiecesToRemove = new List<int>();

            for (int i = 0; i < playerPieces.Count; i++)
            {
                var piece = playerPieces[i];
                if (piece.All(cell => HasDamage(cell, playerBoard)))
                {
                    playerPiecesToRemove.Add(i);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>  await _PlayerBoard.RefreshGridAfterKill(piece));
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("dragon death roar.mp3"); });
                    await Task.Delay(500);
                }
            }
            foreach (var pieceIndex in playerPiecesToRemove)
                playerPieces.RemoveAt(pieceIndex);


            for (int i = 0; i < opponentPieces.Count; i++)
            {
                var piece = opponentPieces[i];
                if (piece.All(cell => HasDamage(cell, opponentBoard)))
                {
                    opponentPiecesToRemove.Add(i);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await OpponentBoard.RefreshGridAfterKill(piece));
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("dragon death roar.mp3"); });
                    await Task.Delay(500);
                }
            }
            foreach (var pieceIndex in opponentPiecesToRemove)
                opponentPieces.RemoveAt(pieceIndex);
        }
        
        private async Task CheckForWin()
        {
            if (playerPieces.Count == 0)
                await GameWon(currentOpponent);
                
            if (opponentPieces.Count == 0)
                await GameWon(currentPlayer);
        }

        private async Task GameWon(Player winner)
        {
            gameOver = true;

            if (winner == currentPlayer)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        WonDisplayText.Text = String.Format("{0} Wins!", currentPlayer == Player.Player1 ? currentMatch.Player1ClientName : currentMatch.Player2ClientName);
                        WonDisplay.Visibility = Visibility.Visible;
                    });
                currentMatch.WinnerClientId = currentPlayer == Player.Player1 ? currentMatch.Player1ClientId : currentMatch.Player2ClientId;
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    KillWizard.Begin();
                    WizardImage.Visibility = Visibility.Collapsed;
                    WizardImageDead.Visibility = Visibility.Visible;
                    await SoundManager.PlaySound("wizardfire.mp3");
                    await Task.Delay(500);
                    await SoundManager.PlaySound("wizard death scream.mp3");
                    await Task.Delay(1000);
                    await SoundManager.PlaySound("wizardlaugh.mp3");
                }); 

                //winnerText = currentPlayer == Player.Player1 ? currentMatch.Player2ClientName : currentMatch.Player1ClientName;
                currentMatch.WinnerClientId = currentPlayer == Player.Player1 ? currentMatch.Player2ClientId : currentMatch.Player1ClientId;
            }

        }

        private async Task ProcessMove(Move move)
        {
            switch (move.MoveKey)
            {
                case "vertical":
                    await ProcessVertical(move);
                    break;
                case "horizontal":
                    await ProcessHorizontal(move);
                    break;
                case "dragon":
                    await ProcessDragon(move);
                    break;
                case "explode":
                    await ProcessExplode(move);
                    break;
                case "rest":
                    await ProcessRest(move);
                    break;
                case "bolt":
                    await ProcessBolt(move);
                    break;
                case "quit":
                    await ProcessQuit(move);
                    break;
            }
            
            // Hand over the turn to the other play after you move.
            currentMover = move.Mover == Player.Player1 ? Player.Player2 : Player.Player1;
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("newround.mp3"); });
            if (move.Mover == currentPlayer)
                currentMana -= move.Cost;
            else
                currentMana += currentMatch.ManaIncrement;
            movesProcessed++;
            await UpdateButtonStates();
        }

        #endregion

        #region UpdateUI

        private async Task UpdateButtonStates()
        {
            bool canCast = currentMover == currentPlayer && !String.IsNullOrEmpty(selectedMoveKey) && (selectedCoordinate != null || selectedMoveKey == "dragon" || selectedMoveKey == "rest");
            var castVisible = canCast ?  Visibility.Visible : Visibility.Collapsed; 

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {


                    SpellButtons.Opacity = currentMover == currentPlayer ? 1.0 : 0.5;
                    BoltButton.IsEnabled = currentMover == currentPlayer && currentMana >= 5;
                    BoltButton.Opacity = BoltButton.IsEnabled ? 1.0 : 0.5;
                    DragonFuryButton.IsEnabled = currentMover == currentPlayer && currentMana >= 60;
                    DragonFuryButton.Opacity = DragonFuryButton.IsEnabled ? 1.0 : 0.5;
                    VerticalBlastButton.IsEnabled = currentMover == currentPlayer && currentMana >= 35;
                    VerticalBlastButton.Opacity = VerticalBlastButton.IsEnabled ? 1.0 : 0.5;
                    HorizontalBlastButton.IsEnabled = currentMover == currentPlayer && currentMana >= 35;
                    HorizontalBlastButton.Opacity = HorizontalBlastButton.IsEnabled ? 1.0 : 0.5;
                    ExplodeButton.IsEnabled = currentMover == currentPlayer && currentMana >= 20;
                    ExplodeButton.Opacity = ExplodeButton.IsEnabled ? 1.0 : 0.5;
                    CastButton.IsEnabled = canCast;
                    CastButton.Visibility = castVisible;
                });
        }
        #endregion

        #region Move processing

        private async Task ProcessHorizontal(Move move)
        {
            var board = move.Mover == currentPlayer ? opponentBoard : playerBoard;

            var cord = move.Coordinates[0];
            
            for (var col = 0; col < 14; col++)
            {
                var cell = board[cord.x][col];
                cell.LastMoves.Push(move.MoveKey);

                if (move.Mover == currentPlayer && cell.PieceKey.StartsWith("Mana"))
                    currentMana += cell.Value;    
            }
        }
        private async Task ProcessVertical(Move move)
        {
            var board = move.Mover == currentPlayer ? opponentBoard : playerBoard;

            var cord = move.Coordinates[0];

            for (var row = 0; row < 14; row++)
            {
                var cell = board[row][cord.y];
                cell.LastMoves.Push(move.MoveKey);

                if (move.Mover == currentPlayer && cell.PieceKey.StartsWith("Mana"))
                    currentMana += cell.Value;
            }
        }
        private async Task ProcessDragon(Move move)
        {
            var board = move.Mover == currentPlayer ? opponentBoard : playerBoard;

            foreach (var coordinate in move.Coordinates)
            {
                await AttackSquare(coordinate, move, board);
            }
        }

        private async Task AttackSquare(Coordinate cord, Move move, Shared.Grid board)
        {
            var cell = board[cord.x][cord.y];
            cell.LastMoves.Push(move.MoveKey);
            int xDirection = 1;
            int yDirection = 1;
            if (cord.x == 13)
                xDirection = -1;
            if (cord.y == 13)
                yDirection = -1;

            // Next column over
            cell = board[cord.x][cord.y + yDirection];
            cell.LastMoves.Push(move.MoveKey);
            if (move.Mover == currentPlayer && cell.PieceKey.StartsWith("Mana"))
                currentMana += cell.Value;

            // Next column over
            cell = board[cord.x + xDirection][cord.y];
            cell.LastMoves.Push(move.MoveKey);

            if (move.Mover == currentPlayer && cell.PieceKey.StartsWith("Mana"))
                currentMana += cell.Value;

            // Next column over, next row over 
            cell = board[cord.x + xDirection][cord.y + yDirection];
            cell.LastMoves.Push(move.MoveKey);

            if (move.Mover == currentPlayer && cell.PieceKey.StartsWith("Mana"))
                currentMana += cell.Value;
        }

        private async Task ProcessExplode(Move move)
        {
            //do something
            var board = move.Mover == currentPlayer ? opponentBoard : playerBoard;
            await AttackSquare(move.Coordinates[0], move, board);

        }
        private async Task ProcessRest(Move move)
        {
            //do something
        }

        private async Task ProcessQuit(Move move)
        {
            if (move.Mover == currentPlayer) await GameWon(currentOpponent);
            else await GameWon(currentPlayer);
        }

        private async Task ProcessBolt(Move move)
        {
            var board = move.Mover == currentPlayer ? opponentBoard : playerBoard;

            var cord = move.Coordinates[0];
            var cell = board[cord.x][cord.y];
            cell.LastMoves.Push(move.MoveKey);

            if (move.Mover==currentPlayer && cell.PieceKey.StartsWith("Mana"))
                currentMana += cell.Value;
        }

        #endregion

        #region Cancel and Quit 

        private async Task QuitGame()
        {
            //prompt for are you sure

            gameOver = true;
            if (currentMatch != null)
            {
                if (!waitingForOpponent)
                    await GameWon(currentOpponent);
          
                await MatchService.EndMatch(currentMatch.MatchId, currentMatch.WinnerClientId ?? "none");
                var move = new Move()
                {
                    MatchId = currentMatch.MatchId,
                    MoveKey = "quit",
                    Mover = currentPlayer,
                };
                await MatchService.SaveMove(move);
            }
            Frame.Navigate(typeof(StartPage));
        }

        private async void WaitingCancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            await QuitGame();
        }

        private async void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await QuitGame();
        }
        
        #endregion

        private async void CastButton_Click(object sender, RoutedEventArgs e)
        {
            CastSpell.Begin();
            switch (selectedMoveKey)
            {
                case "vertical":
                    await SoundManager.PlaySound("bloodstorm.mp3");
                    break;
                case "horizontal":
                    await SoundManager.PlaySound("icestrike2.mp3");
                    break;
                case "dragon":
                    await SoundManager.PlaySound("dragonfury.mp3");
                    break;
                case "explode":
                    await SoundManager.PlaySound("explode.mp3");
                    break;
                case "rest":
                    await SoundManager.PlaySound("recharge.mp3");
                    break;
                case "bolt":
                    await SoundManager.PlaySound("fireball.mp3");
                    break;
                default:
                    await SoundManager.PlaySound("cast.mp3");
                    break;
            }
            await Task.Delay(1500);
            var move = new Move()
            {
                MatchId = currentMatch.MatchId,
                Coordinates = new List<Coordinate>(),
                MoveKey = selectedMoveKey,
                Mover = currentPlayer,
                Cost = selectedMoveCost
            };
            if (selectedCoordinate != null)
                move.Coordinates.Add(new Coordinate() {x = selectedCoordinate.x, y = selectedCoordinate.y});
            if (selectedMoveKey == "dragon")
            {
                var random = new Random();
                move.Coordinates.Clear();

                foreach (var piece in Enumerable.Range(0, playerPieces.Count))
                    move.Coordinates.Add(new Coordinate() {x = random.Next(0, 14), y = random.Next(0, 14)});
            }
            await MatchService.SaveMove(move);
            selectedCoordinate = null;
            selectedMoveKey = "";
            selectedMoveCost = 0;
            selectedSpellButton.BorderThickness = new Thickness(0);
            if (lastSender != null)
                ((BoardButton)lastSender).UnHighlight();
        }

        private async void OpponentCastButton_Click(object sender, RoutedEventArgs e)
        {
            //if(selectedCoordinate == null)
            //    return;
            //var move = new Move()
            //{
            //    MatchId = currentMatch.MatchId,
            //    Coordinates = new List<Coordinate>() { new Coordinate() { x = selectedCoordinate.x, y = selectedCoordinate.y } },
            //    MoveKey = "explode",
            //    Mover = currentOpponent,
            //    Cost = 5
            //};
            var move = new Move()
            {
                MatchId = currentMatch.MatchId,
                Coordinates = new List<Coordinate>(),
                MoveKey = "rest",
                Mover = currentOpponent,
                Cost =0
            };
            await MatchService.SaveMove(move);
        }

        private Button selectedSpellButton;

        private async void SpellButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedSpellButton != null)
                selectedSpellButton.BorderThickness = new Thickness(0);

            selectedSpellButton = ((Button) sender);
            var buttonArgs =selectedSpellButton.Tag.ToString().Split(',');

            selectedMoveKey = buttonArgs[0];
            selectedMoveCost = Convert.ToInt32(buttonArgs[1]);
            selectedSpellButton.BorderThickness = new Thickness(5);

            if(selectedMoveKey == "dragon" || selectedMoveKey == "rest")
            {
                if (lastSender != null)
                    ((BoardButton)lastSender).UnHighlight();
            }
            await UpdateButtonStates();
        }

        private void WinExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }

        private async void ManaCheat_OnClick(object sender, RoutedEventArgs e)
        {
            currentMana += 100;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CurrentManaText.Text = currentMana.ToString(); });

        }
    }
}
