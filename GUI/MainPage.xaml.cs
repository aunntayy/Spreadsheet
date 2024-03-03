using SS;


namespace GUI
{
    public partial class MainPage : ContentPage
    {
        // Initialize needed var
        private Dictionary<string, Entry> Cell;
        private Spreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "six");
        private List<HorizontalStackLayout> Rows = new List<HorizontalStackLayout>();
        private string currentFilePath = ""; // Variable to store the current file path
        // If the spread sheet size need to be change
        private MyEntry[,] EntryColumn = new MyEntry[10, 10];
        private int col = 10;
        private int row = 10;
        public MainPage()
        {
            Cell = new Dictionary<string, Entry>();
            InitializeComponent();
            createGrid();
        }

        /// <summary>
        /// Create the grid 
        /// </summary>
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
        /// <summary>
        /// Function for button new
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FileMenuNew(object sender, EventArgs e)
        {
            // Check for saving changes
            bool saveChange = false;
            Spreadsheet currentSpread = ss;
            if (ss.Changed)
            {
                // prompt the user if they want to save
                saveChange = await DisplayAlert("Unsaved change", "Do you want to save the changed before open new file ?", "yes", "Cancel");
                if (saveChange)
                {
                    // Restore the original spreadsheet if changes are not saved
                    ss = currentSpread;
                    return;
                }
            }
            // If no save
            if (!ss.Changed || !saveChange)
            {
                ClearGrid();
                ss = new Spreadsheet();
            }
        }

        /// <summary>
        /// Helper funtion to clear up the grid
        /// </summary>
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
        

        // Function for button Open
        private async void FileMenuOpenAsync(object sender, EventArgs e)
        {
            Spreadsheet currentSpread = ss;
            // Check to know if the user wants to save their current spreadsheet
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
                    // Check for the right extension
                    if (extension != ".sprd")
                    {
                        await DisplayAlert("Invalid extension", extension, "OK");
                        return;
                    }
                    // Clear the current grid
                    ClearGrid();

                    currentFilePath = selectedFilePath; // Update the current file path
                    ss = new Spreadsheet(selectedFilePath, s => true, s => s, "six");
                    // Populate the new open spreadsheet with the open file content
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
                    // If no file was selected
                    await DisplayAlert("Error", "No file selected", "OK");
                }
            }
            catch (Exception ex)
            {
                // If any errors occur
                await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
            }
        }

        // Function to save changes
        private async Task SaveChanges()
        {
            try
            {
                // If the current file path is not empty, save the changes directly
                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    ss?.Save(currentFilePath);
                    await DisplayAlert("Success", $"File saved successfully at: {currentFilePath}", "OK");
                }
                else
                {
                        // If the current file path is empty, prompt the user to enter a new file path
                        string filename = await DisplayPromptAsync("Enter file name", "Please enter the file name:");
                        if (string.IsNullOrEmpty(filename))
                        {
                            return;
                        }
                        filename = filename + ".sprd";
                        string filepath = await DisplayPromptAsync("Enter file path", "Please enter the file path:");
                        if (string.IsNullOrEmpty(filepath))
                        {
                            return;
                        }
                        if (string.IsNullOrEmpty(filepath))
                        {
                            filepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        }

                        string filePath = Path.Combine(filepath, filename);

                        ss?.Save(filePath);
                        await DisplayAlert("Success", $"File saved successfully at: {filePath}", "OK");
                        currentFilePath = filePath; // Update the current file path
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
            }
        }

        // Function for the Save button
        private async void Save(object sender, EventArgs e)
        {
            if (ss.Changed)
            {
                await SaveChanges();
            }
            else
            {
                await DisplayAlert("No changes", "No changes have been made since the file was opened.", "OK");
            }
        }

        /// <summary>
        /// Function for the Help button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Help(object sender, EventArgs e)
        {
            HelpPage help = new HelpPage();
            Navigation.PushAsync(help, true);
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