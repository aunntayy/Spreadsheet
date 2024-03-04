using SS;
using System.Xml;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        // Initialize needed variables

        // Spreadsheet instance
        private Spreadsheet ss;
        // List to store rows
        private readonly List<HorizontalStackLayout> Rows = new List<HorizontalStackLayout>();
        // Variable to store the current file path
        private string currentFilePath = "";
        // Entry column for the spreadsheet
        private MyEntry[,] EntryColumn = new MyEntry[10, 10];
        private readonly int col = 10;
        private readonly int row = 10;

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            ss = new Spreadsheet(s => true, s => s.ToUpper(), "six");
            CreateGrid();
            Focused += (sender, e) =>
            {
                ShowSelectedCell();
                ShowContent();
            };
            showContent.Completed += ChangeContent;
        }

        /// <summary>
        /// Handles the completion of the content change animation.
        /// </summary>
        /// <param name="e">ignore</param>
        /// <param name="sender">ignore</param>
        private void ChangeContent(object? sender, EventArgs e)
        {
            foreach (var entry in EntryColumn)
            {
                if (selectedCell.Text.Equals(entry.GetCellName()))
                {
                    entry.Text = showContent.Text;
                    entry.CellUpdated();
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the content shown based on the focused cell.
        /// </summary>
        private void ShowContent()
        {
            foreach (var entry in EntryColumn)
            {
                if (entry.IsFocused)
                {
                    showContent.Text = entry.Text;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the selected cell indicator based on the focused cell.
        /// </summary>
        private void ShowSelectedCell()
        {
            foreach (var entry in EntryColumn)
            {
                if (entry.IsFocused)
                {
                    selectedCell.Text = entry.GetCellName();
                    break;
                }
            }
        }

        /// <summary>
        /// Creates the grid layout for the spreadsheet.
        /// </summary>
        private void CreateGrid()
        {
            EntryColumn = new MyEntry[col, row];
            // Start of columns
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
            // Start of rows
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

            // grid
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
        /// Clears the content of the grid.
        /// </summary>
        private void ClearGrid()
        {
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    // Clear cell content
                    string cellValue = "";
                    // Update the corresponding MyEntry value
                    EntryColumn[j, i].Text = cellValue;
                    // Update the cell
                    EntryColumn[j, i].CellUpdated();
                }
            }
        }


        /// <summary>
        /// Creates a new spreadsheet.
        /// </summary>
        /// <param name="sender">ignore</param>
        /// <param name="e">ignore</param>
        private async void FileMenuNew(object sender, EventArgs e)
        {
            // Check for unsaved changes
            if (ss.Changed)
            {
                // Prompt the user to save changes before creating a new file
                bool saveChanges = await DisplayAlert("Unsaved changes", "Do you want to save the changes before creating a new file?", "Yes", "No");

                if (saveChanges)
                {
                    // Save the changes
                    await SaveChanges();
                }
            }

            // Clear the grid
            ClearGrid();
            currentFilePath = "";
        }


        private async void FileMenuOpenAsync(object sender, EventArgs e)
        {
            // Store the current spreadsheet to check for unsaved changes
            Spreadsheet currentSpread = ss;

            try
            {
                // Check if there are unsaved changes in the current spreadsheet
                if (ss.Changed)
                {
                    // Prompt the user to save changes before opening a new file
                    bool saveChange = await DisplayAlert("Unsaved changes", "Do you want to save the changes before opening a new file?", "Yes", "Cancel");
                    if (saveChange)
                    {
                        // Save the current spreadsheet
                        ss.Save(currentFilePath);
                        await DisplayAlert("Success", $"File saved successfully at: {currentFilePath}", "OK");
                    }
                }

                // Use FilePicker to select a file
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a file"
                });

                if (fileResult != null)
                {
                    string selectedFilePath = fileResult.FullPath;
                    string extension = Path.GetExtension(selectedFilePath);

                    // Check if the selected file has the correct extension
                    if (extension != ".sprd")
                    {
                        await DisplayAlert("Invalid extension", extension, "OK");
                        return;
                    }

                    // Clear the current grid
                    ClearGrid();

                    // Update the current file path
                    currentFilePath = selectedFilePath;

                    // Load the contents of the selected file into the spreadsheet
                    LoadSpreadsheet(currentFilePath);
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

        private void LoadSpreadsheet(string filepath)
        {
            try
            {
                // Load the XML file into the spreadsheet
                ss.LoadXml(filepath);

                // Update the grid with the contents of the spreadsheet
                foreach (string cell in ss.GetNamesOfAllNonemptyCells())
                {
                    // Extract row and column indices from the cell name
                    string cellName = cell.ToUpper();
                    int colIndex = cellName[0] - 'A';
                    int rowIndex = int.Parse(cellName.Substring(1)) - 1;

                    // Check if the indices are within the valid range
                    if (colIndex >= 0 && colIndex < col && rowIndex >= 0 && rowIndex < row)
                    {
                        // Retrieve cell value from spreadsheet
                        string cellValue = ss.GetCellValue(cell).ToString();

                        // Update the corresponding MyEntry value
                        EntryColumn[rowIndex, colIndex].Text = cellValue;

                        // Update the cell
                        EntryColumn[rowIndex, colIndex].CellUpdated();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during loading
                Console.WriteLine("Error loading spreadsheet: " + ex.Message);
            }
        }


        /// <summary>
        /// Saves the changes made to the spreadsheet.
        /// </summary>
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

        /// <summary>
        /// Saves the changes made to the spreadsheet when the Save button is clicked.
        /// </summary>
        /// <param name="sender">ignore</param>
        /// <param name="e">ignore</param>
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
        /// Navigates to the help page.
        /// </summary>
        /// <param name="sender">ignore</param>
        /// <param name="e">ignore</param>
        void Help(object sender, EventArgs e)
        {
            HelpPage help = new HelpPage();
            Navigation.PushAsync(help, true);
        }

        /// <summary>
        /// Handles the horizontal scrolling of top labels.
        /// </summary>
        /// <param name="sender">ignore</param>
        /// <param name="e">ignore</param>
        private void OnTopLabelsScrolled(object sender, ScrolledEventArgs e)
        {
            TopLabels.TranslationX = -e.ScrollX;
        }

        /// <summary>
        /// Sets focus to the first cell when the page is loaded.
        /// </summary>
        /// <param name="sender">ignore</param>
        /// <param name="e">ignore</param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            // Set the focus to the widget (e.g., entry) that you want
            EntryColumn[0, 0].Focus();
        }
    }
}
