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
                    HeightRequest = 44,
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
            for (int col = 1; col <= 24; col++)
            {
                for (int row = 1; row <= 99; row++)
                {
                    Grid gridCell = new Grid();

                    // Add a border
                    Border border = new Border
                    {
                        HeightRequest = 20,
                        WidthRequest = 75,
                        Stroke = Color.FromRgb(0, 0, 0),
                        StrokeThickness = 1
                    };
                    gridCell.Add(border);

                    // Add a entry
                    Entry label = new Entry
                    {
                     
                        BackgroundColor = Color.FromRgb(200, 200, 250),  
                        HeightRequest = 20,
                        WidthRequest = 75,
                       
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

        

    }
}
