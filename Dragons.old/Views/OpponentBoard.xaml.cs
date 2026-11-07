using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dragons.Shared;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Grid = Windows.UI.Xaml.Controls.Grid;
using Dragons.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Dragons.Views
{
    public sealed partial class OpponentBoard : UserControl
    {
        public event EventHandler<Coordinate> CoordinateSelected;

        private List<List<BoardButton>> buttons = new List<List<BoardButton>>();

        private Shared.Grid _data;

         public OpponentBoard()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

         public OpponentBoard(Shared.Grid data)
             : this()
        {
            _data = data;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var rows = Enumerable.Range(0, 14).ToArray();
            var columns = Enumerable.Range(0, 14).ToArray();

            foreach (var row in rows) Board.RowDefinitions.Add(new RowDefinition());
            foreach (var column in columns) Board.ColumnDefinitions.Add(new ColumnDefinition());

            foreach (var row in rows)
            {
                buttons.Add(new List<BoardButton>());
                foreach (var column in columns)
                {
                    BoardButton boardButton;
                    if(_data != null)
                        boardButton = new BoardButton(row, column, _data[row][column]);
                    else
                        boardButton = new BoardButton(row, column);

                    boardButton.CoordinateSelected += boardButton_CoordinateSelected;
                    boardButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                    boardButton.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                    boardButton.Margin = new Thickness();
                    Board.Children.Add(boardButton);
                    Grid.SetRow(boardButton, row);
                    Grid.SetColumn(boardButton, column);
                    buttons[row].Add(boardButton);
                }
            }
        }

        private void boardButton_CoordinateSelected(object sender, Coordinate e)
        {
            if (CoordinateSelected != null)
                CoordinateSelected(sender, e);
        }

        public async Task RefreshGrid(Shared.Grid grid)
        {
            foreach (var row in grid)
            {
                foreach (var cell in row)
                {
                    if (cell.LastMoves.Count > 0)
                    {
                        await buttons[cell.Coordinate.x][cell.Coordinate.y].Update(cell);
                        await Task.Delay(100);
                    }
                }
            }
        }

        public async Task RefreshGridAfterKill(List<Cell> piece)
        {
            foreach (var cell in piece)
            {
                await buttons[cell.Coordinate.x][cell.Coordinate.y].UpdateKill(cell);
                await Task.Delay(100);
            }
        }
    }
}
