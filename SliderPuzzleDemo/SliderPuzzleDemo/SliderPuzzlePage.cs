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
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item;
                    if (counter == 16)
                    {

                        item = new GridItem(new GridPosition(row, col), "empty");
                        item.FinalImage = "16";
                    }

                    else
                    {
                        item = new GridItem(new GridPosition(row, col), counter.ToString());

                    }


                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);

                    _gridItems.Add(item.CurrentPosition, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            //Shuffle();
            // Shuffle();
            //Shuffle();
            // Shuffle();

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
        }




        private void Shuffle()
        {
            Random rand = new Random();
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];

                    int SwapRow = rand.Next(0, 4);
                    int SwapCol = rand.Next(0, 4);
                    GridItem swapItem = _gridItems[new GridPosition(SwapRow, SwapCol)];

                    swap(item, swapItem);
                }
            }
        }

        private void OnContentViewSizeChanged(object sender, EventArgs e)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / SIZE;

            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
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

            int row = 0;
            int col = 0;

            //did we click on empty? if so do nothing
            if (item.isEmptySpot() == true)
            {
                return;
            }












            //we know we didnt click on empty spot

            //check up, down, left, right until we find empty
            var counter = 0;

            while (counter < 4)
            {

                GridPosition pos = null;
                if (counter == 0 && item.CurrentPosition.Row != 0)
                {
                    // Get position of square above current item
                    pos = new GridPosition(item.CurrentPosition.Row - 1, item.CurrentPosition.Column);
                }

                else if (counter == 1 && item.CurrentPosition.Column != SIZE - 1)
                {
                    // Get position of square to right of current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column + 1);
                }
                else if (counter == 2 && item.CurrentPosition.Row != SIZE - 1)
                {
                    // Get position of square below current item
                    pos = new GridPosition(item.CurrentPosition.Row + 1, item.CurrentPosition.Column);
                }
                else if (counter == 3 && item.CurrentPosition.Column != 0)
                {
                    // Get position of square to left of current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column - 1);
                }

                if (pos != null) //dont have to check because of edge
                {



                    GridItem swapWith = _gridItems[pos];
                    if (swapWith.isEmptySpot())
                    {
                        swap(item, swapWith);
                        if(isPuzzleWon())
                        {
                            swapWith.ShowFinalImage();
                           

                        }
                        break;
                    }

               



                }
                counter++;

            }





            OnContentViewSizeChanged(this.Content, null);

        }

       


        public Boolean isPuzzleWon()
        {
            Boolean won = true;
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    if (!item.isPositionCorrect())
                    {
                        won = false;
                        break;
                    }
                }
            }

            return won;
        }


        void swap(GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.CurrentPosition;
            item1.CurrentPosition = item2.CurrentPosition;
            item2.CurrentPosition = temp;

            _gridItems[item1.CurrentPosition] = item1;
            _gridItems[item2.CurrentPosition] = item2;
        }

        internal class GridItem : Image
        {
            public GridPosition CurrentPosition
            {
                get; set;
            }
            private GridPosition _FinalPosition;
            private Boolean _isEmptySpot;
            public String FinalImage
            {
                get; set;
            }

            public GridItem(GridPosition position, String src)
            {
                _FinalPosition = position;
                CurrentPosition = position;
                String path = "SliderPuzzleDemo." + src + ".jpeg";
                Source = ImageSource.FromResource(path);
                if (src == "empty")
                {
                    _isEmptySpot = true;



                }

                else
                {
                    _isEmptySpot = false;
                }
                BackgroundColor = Color.Black;

                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.FillAndExpand;
            }

            public void ShowFinalImage()
            {
            
                
                String path = "SliderPuzzleDemo." + "16" + ".jpeg";
                Source = ImageSource.FromResource(path);



            }

            
            

  
            public Boolean isEmptySpot()
            {
                return _isEmptySpot;
            }



            public Boolean isPositionCorrect()
            {



                return _FinalPosition.Equals(CurrentPosition);
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

    

