namespace GUI
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            //Start of columns
            for (int i = 64; i <= 90; i++)
            {
                if (i == 64)
                {
                    TopLabels.Add(
               new Border
               {
                   Stroke = Color.FromRgb(0, 0, 0),
                   StrokeThickness = 1,
                   HeightRequest = 20,
                   WidthRequest = 75,
                   HorizontalOptions = LayoutOptions.Center,
                   Content =
                       new Label
                       {
                           Text = "",
                           BackgroundColor = Color.FromRgb(200, 200, 250),
                           HorizontalTextAlignment = TextAlignment.Center
                       }
               }
               );
                    continue;

                }
                char label = (char)i;
                TopLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"{label}",
                            BackgroundColor = Color.FromRgb(200, 200, 250),
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }
            //Start of rows
            for (int i = 1; i <= 99; i++)
            {

                LeftLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Start,
                    Content =
                        new Label
                        {
                            Text = $"{i}",
                            BackgroundColor = Color.FromRgb(200, 200, 250),
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }

            //grid
            for (int col = 1; col <= 26; col++)
            {
                for (int row = 1; row <= 99; row++)
                {
                    Grid gridCell = new Grid();

                    // Add a border
                    Border border = new Border
                    {
                        Stroke = Color.FromRgb(0, 0, 0),
                        HeightRequest = 20,
                        WidthRequest = 75,
                        StrokeThickness = 1,
                        HorizontalOptions = LayoutOptions.Start,
                    };
                    gridCell.Add(border);

                    // Add a entry
                    Label label = new Label
                    {
                        Text = $"{row}",
                        BackgroundColor = Color.FromRgb(200, 200, 250),  
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                   
                    gridCell.Add(label);

                    // Add the grid containing the border and label to the main grid
                    Grid.Add(gridCell, col, row); // Add grid to column col and row row

                }
            }
        }
      
            void FileMenuNew(object sender, EventArgs e)
            {
            }

            void FileMenuOpenAsync(object sender, EventArgs e)
            {

            }

        // Make sure everything is Synchronize
        private void OnTopLabelsScrolled(object sender, ScrolledEventArgs e)
        {
            TopLabels.TranslationX = -e.ScrollX;
        }
    }
}
