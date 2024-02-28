using Microsoft.UI.Xaml.Automation;
using SS;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
     
        private Spreadsheet ss = new Spreadsheet();
        private MyEntry[,] EntryColumn = new MyEntry[99,26];
        List<HorizontalStackLayout> Rows = new List<HorizontalStackLayout>();

        private readonly int col = 99;
        private readonly int row = 26;
        public MainPage()
        {
            InitializeComponent();
            createGrid();
        }

        private void createGrid()
        {

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
                    HeightRequest = 32,
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
            for (int i = 0; i < col; i++) {
                HorizontalStackLayout verRow = new HorizontalStackLayout(); // Create a new HorizontalStackLayout for each row
                Rows.Add(verRow); // Add the row to the list

                for (int j = 0; j < row; j++) {
                    EntryColumn[i, j] = new MyEntry(i, j); // Adjusting indices to start from 0
                    verRow.Add(EntryColumn[i, j]); // Add each MyEntry to the row
                }

                bar.Add(verRow); // Add the row to the vertical layout
            }
        }

        void FileMenuNew(object sender, EventArgs e)
        {
            
        }

        void FileMenuOpenAsync(object sender, EventArgs e)
        {
            
        }
        void  Help(object sender, EventArgs e)
        {
           
        }

        private async void Save(object sender, EventArgs e)
        { // Ask user for filename
            string filename = await DisplayPromptAsync("Enter file name", "Please enter the file name:");

            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    // Get the directory where the application is running
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // Combine the base directory with a subdirectory (if needed)
                    string targetDirectory = Path.Combine(baseDirectory, "SavedFiles");

                    // Ensure the directory exists, create it if it doesn't
                    Directory.CreateDirectory(targetDirectory);

                    // Combine the directory path with the filename
                    string filePath = Path.Combine(targetDirectory, filename);

                    // Save the file
                    ss?.Save(filePath);

                    await DisplayAlert("Success", "File saved successfully", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
                }
            }

        }

        // Make sure everything is Synchronize
        private void OnTopLabelsScrolled(object sender, ScrolledEventArgs e)
        {
            TopLabels.TranslationX = -e.ScrollX;
        }
    }
}
