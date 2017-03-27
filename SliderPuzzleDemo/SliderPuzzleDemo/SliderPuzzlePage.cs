using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace SliderPuzzleDemo
{
    class SliderPuzzlePage : ContentPage
    {
        private const int SIZE = 4;
        private AbsoluteLayout _absoluteLayout;
        private Dictionary<GridPosition, GridItem> _gridItems;

        //constructor
        public SliderPuzzlePage()
        {
            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var counter = 1;
            for(var row = 0; row < SIZE; row++)
            {
                for(var col = 0; col < SIZE; col++)
                {

                    GridItem item = new GridItem(new GridPosition(row, col), counter.ToString());
       
                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);

                    _gridItems.Add(item.Position, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
  
        }

        private void OnContentViewSizeChanged(object sender, EventArgs e)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / SIZE;

            for(var row = 0; row < SIZE; row++)
            {
                for(var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);

                    AbsoluteLayout.SetLayoutBounds(item, rect);
                }
            }
        }

        private void OnLabelTapped(object sender, EventArgs args)
        {
            GridItem item = (GridItem)sender;
            Random rand = new Random();
            int move = rand.Next(0, 4);
            //adjust random move to account for edges
            //if im trying to move up and position is in top row move down
            if (move == 0 && item.Position.Row == 0)
            {
                move = 2;
            }
           else if(move == 1 && item.Position.Column == SIZE - 1) //try to move right
            {
                move = 3; //instead move left
            }
           else if (move == 2 && item.Position.Row == SIZE - 1) //try to move down
            {
                move = 0; //instead move up
            }
           else if(move == 3 && item.Position.Column == 0) //try to move left
            {
                move = 1; //instead move right
            }

            int row = item.Position.Row;
            int col = item.Position.Column;

            if(move == 0)
            {
                row = row - 1;

            }
            else if(move == 1)
            {
                col = col + 1;
            }
            else if(move == 2)
            {
                row = row + 1;
            }

            else
            {
                col = col - 1;
            }
            GridItem swapWith = _gridItems[new GridPosition(row, col)];
            swap(item, swapWith);
            OnContentViewSizeChanged(this.Content, null);
        }

        void swap (GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;

            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;
        }

        internal class GridItem : Image
        {
            public GridPosition Position
            {
                get; set;
            }

            public GridItem(GridPosition position, String src)
            {
                Position = position;
                String path = "SliderPuzzleDemo." + src + ".jpeg";
                Source = ImageSource.FromResource(path);
                


                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.FillAndExpand;
            }
        }

        internal class GridPosition
        {
            public int Row { get; set; }

            public int Column { get; set; }

            public GridPosition(int row, int col)
            {
                Row = row;
                Column = col;
            }

          

            public override bool Equals(object obj)
            {
                GridPosition other = obj as GridPosition;
                if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }
                return false;
            }


            public override int GetHashCode()
            {
                return 17 * (23 + this.Row.GetHashCode()) * (23 + this.Column.GetHashCode());
            }
        }

    }
}
