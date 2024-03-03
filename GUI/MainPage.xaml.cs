using Microsoft.UI.Xaml.Automation;
using SS;
using System.Threading.Channels;
using Windows.Storage.Pickers.Provider;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        private Dictionary<string, Entry> Cell;
        private Spreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "six");
        private MyEntry[,] EntryColumn = new MyEntry[10, 10];
        private List<HorizontalStackLayout> Rows = new List<HorizontalStackLayout>();

        private int col = 10;
        private int row = 10;
        public MainPage()
        {
            Cell = new Dictionary<string, Entry>();
            InitializeComponent();
            createGrid();
        }


        private void createGrid()
        {
            EntryColumn = new MyEntry[col, row];
            //Start of columns
            for (int i = 0; i <= col; i++)
            {
                var label = i == 0 ? "" : ((char)('A' + i - 1)).ToString();
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
            for (int i = 1; i <= row; i++)
            {

                LeftLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 32,//32
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
            for (int i = 0; i < col; i++)
            {
                HorizontalStackLayout verRow = new HorizontalStackLayout(); // Create a new HorizontalStackLayout for each row
                Rows.Add(verRow); // Add the row to the list
                for (int j = 0; j < row; j++)
                {

                    EntryColumn[i, j] = new MyEntry(ss, i, j); // Adjusting indices to start from 0
                    verRow.Add(EntryColumn[i, j]); // Add each MyEntry to the row
                }
                bar.Add(verRow); // Add the row to the vertical layout
            }
        }

        private async void FileMenuNew(object sender, EventArgs e)
        {
            bool saveChange = false;
            Spreadsheet currentSpread = ss;
            if (ss.Changed)
            {
                saveChange = await DisplayAlert("Unsaved change", "Do you want to save the changed before open new file ?", "yes", "Cancel");
                if (saveChange)
                {
                    // Restore the original spreadsheet if changes are not saved
                    ss = currentSpread;
                    return;
                }
            }
            if (!ss.Changed || !saveChange)
            {
                ClearGrid();
                ss = new Spreadsheet();
            }
        }
        
        private async void FileMenuOpenAsync(object sender, EventArgs e)
        {
            Spreadsheet currentSpread = ss;
            if (ss.Changed)
            {
                bool saveChange = await DisplayAlert("Unsaved change", "Do you want to save the changed before open new file ?", "yes", "Cancel");
                if (saveChange)
                {
                    // Restore the original spreadsheet if changes are not saved
                    ss = currentSpread;
                    return;
                }
            }

            try
            {
                // Use FilePicker to select a file
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a file"
                });

                if (fileResult != null)
                {
                    string selectedFilePath = fileResult.FullPath;
                    string extension = Path.GetExtension(selectedFilePath);
                    if (extension != ".sprd")
                    {
                        await DisplayAlert("Invalid extension", extension, "OK");
                        return;
                    }

                    ClearGrid();

                    ss = new Spreadsheet(selectedFilePath, s => true, s => s, "six");

                    foreach (string cell in ss.GetNamesOfAllNonemptyCells())
                    {
                        // Extract row and column indices from the cell name
                        string cellName = cell.ToUpper(); // Ensure the cell name is in upper case
                        int colIndex = cellName[0] - 'A'; // Convert the first character to column index
                        int rowIndex = int.Parse(cellName.Substring(1)) - 1; // Extract the numeric part and convert it to row index

                        // Check if the indices are within the valid range
                        if (colIndex >= 0 && colIndex < col && rowIndex >= 0 && rowIndex < row)
                        {
                            string cellValue = ss.GetCellValue(cell).ToString(); // Retrieve cell value from spreadsheet
                            EntryColumn[rowIndex, colIndex].Text = cellValue; // Update the corresponding MyEntry value
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No file selected", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
            }
        }

        private void ClearGrid()
        {
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    EntryColumn[i, j].Text = ""; // Clear cell content
                }
            }
        }
        void Help(object sender, EventArgs e)
        {

        }
        // Function for saving the sheet
        private async void Save(object sender, EventArgs e)
        {
            // Check for any changed
            if (ss.Changed)
            {
                try
                {
                    // Ask user for filename
                    string filename = await DisplayPromptAsync("Enter file name", "Please enter the file name:");
                    if (string.IsNullOrEmpty(filename))
                    {
                        return;
                    }
                    filename = filename + ".sprd";
                    // Ask user for file path
                    string filepath = await DisplayPromptAsync("Enter file path", "Please enter the file path:");
                    if (string.IsNullOrEmpty(filepath))
                    {
                        return;
                    }

                    // If filepath is null, set the base directory
                    if (string.IsNullOrEmpty(filepath))
                    {
                        filepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    }


                    // Combine the directory path with the filename
                    string filePath = Path.Combine(filepath, filename);

                    try
                    {
                        // Save the file
                        ss?.Save(filePath);
                        // Show success message
                        await DisplayAlert("Success", $"File saved successfully at: {filePath}", "OK");
                    }
                    catch (Exception)
                    {
                        // If saving fails due to invalid file path, save in base directory
                        string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        filePath = Path.Combine(baseDirectory, filename);

                        // Attempt to save in the base directory
                        ss?.Save(filePath);

                        // Show message to user
                        await DisplayAlert("Warning", $"Invalid file path. File saved in base directory at: {filePath}", "OK");

                    }
                }
                catch (Exception ex)
                {
                    // Show error message if any other exception occurs
                    await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
                }
            }
            // Prompt the user for unchange so no save
            else { await DisplayAlert("No change have been make", $"Failed to save file", "OK"); }
        }




        // Make sure everything is Synchronize
        private void OnTopLabelsScrolled(object sender, ScrolledEventArgs e)
        {
            TopLabels.TranslationX = -e.ScrollX;
        }


        private void OnPageLoaded(object sender, EventArgs e)
        {
            // set the focus to the widget (e.g., entry) that you want   
            EntryColumn[0, 0].Focus();

        }

    }
}