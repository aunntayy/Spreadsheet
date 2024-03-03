using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUI {
    public class MyEntry : Entry {
        // Reference to the spreadsheet associated with this entry
        private readonly Spreadsheet ss;

        // Column and row indices of the cell represented by this entry
        private readonly int col;
        private readonly int row;

        /// <summary>
        /// Initializes a new instance of the MyEntry class.
        /// </summary>
        /// <param name="ss">The Spreadsheet instance.</param>
        /// <param name="row">The row index of the entry.</param>
        /// <param name="col">The column index of the entry.</param>
        public MyEntry(Spreadsheet ss, int row, int col) : base() {
            this.ss = ss;
            this.row = row + 1;
            this.col = col;
            this.StyleId = $"{row}-{col}";
            this.WidthRequest = 75;
            this.HeightRequest = 20;
            this.BackgroundColor = Colors.White;
            this.TextColor = Colors.Black;
            //this.TextChanged += OnTextChanged;
            this.Focused += (sender, e) => ShowFormula();
            this.Unfocused += (sender, e) => CellUpdated();
            this.Focused += (sender, e) => HighLightedCell();
            this.Unfocused += (sender, e) => UnhighlightedCell();
        }

        /// <summary>
        /// Handles the text changed event of the entry.
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs e) {
            string content = e.NewTextValue;
            string cellName = GetCellName();
            ss.SetContentsOfCell(cellName, content);
        }

        /// <summary>
        /// Sets the background color and text color to unhighlighted state.
        /// </summary>
        private void UnhighlightedCell() {
            this.BackgroundColor = Colors.White;
            this.TextColor = Colors.Black;
        }

        /// <summary>
        /// Sets the background color and text color to highlighted state.
        /// </summary>
        private void HighLightedCell() {
            this.BackgroundColor = Colors.Salmon;
            this.TextColor = Colors.Yellow;
        }

        /// <summary>
        /// Retrieves the cell name based on row and column indices.
        /// </summary>
        /// <returns>The cell name.</returns>
        internal string GetCellName() {
            string colChar = ((char)('A' + col)).ToString();
            string rowChar = row.ToString();
            string cell = colChar + rowChar;
            return cell;
        }

        /// <summary>
        /// Handles cell update event.
        /// </summary>
        public void CellUpdated() {
            string content = this.Text;
            string cell = GetCellName();

            try {
                ss.SetContentsOfCell(cell, content);
                object cellValue = ss.GetCellValue(cell);

                if (cellValue is string stringValue) {
                    // If the cell value is a string, set the text of the entry box to the string value
                    this.Text = stringValue;
                } else if (cellValue is double doubleValue) {
                    // If the cell value is a double, convert it to a string and set the text of the entry box
                    this.Text = doubleValue.ToString();
                } else if (cellValue is Formula formula) {
                    // If the cell value is a formula, set the text of the entry box to the formula representation
                    this.Text = "=" + formula.ToString();
                } else if (cellValue is FormulaError formulaError) {
                    this.Text = "ERROR";
                }
            } catch (FormulaFormatException) {
                Text = "ERROR";
            } catch (Exception) {
                Text = "ERROR2";
            }
        }

        /// <summary>
        /// Displays formula when the cell is focused.
        /// </summary>
        private void ShowFormula() {
            string cell = GetCellName();
            object cellContent = ss.GetCellContents(cell);

            if (cellContent is string stringValue) {
                // If the cell value is a string, set the text of the entry box to the string value
                this.Text = stringValue;
            } else if (cellContent is double doubleValue) {
                // If the cell value is a double, convert it to a string and set the text of the entry box
                this.Text = doubleValue.ToString();
            } else if (cellContent is Formula formula) {
                // If the cell value is a formula, set the text of the entry box to the formula representation
                this.Text = "=" + formula.ToString();
            }
        }

        /// <summary>
        /// Sets the cell value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetCellValue(string value) {
            this.Text = value;
        }
    }
}
