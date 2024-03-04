using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    /// <summary>
    /// Author:    Phuc Hoang
    /// Partner:   Chanphone Visathip
    /// Date:      24-2-2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Phuc Hoang- This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Phuc Hoang, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source. All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    /// 
    /// File Contents
    /// 
    /// This class represents a customized Entry control used in the GUI for a spreadsheet application.
    /// It extends the functionality of the base Entry control to handle spreadsheet-specific operations,
    /// such as displaying cell formulas, setting cell values, and updating cell content.
    /// </summary>
    public class MyEntry : Entry
    {
        // Reference to the spreadsheet associated with this entry
        private readonly Spreadsheet ss;
        private Dictionary<string, MyEntry> cellEntries = new Dictionary<string, MyEntry>();

        // Column and row indices of the cell represented by this entry
        private readonly int col;
        private readonly int row;

        /// <summary>
        /// Initializes a new instance of the MyEntry class.
        /// </summary>
        /// <param name="ss">The Spreadsheet instance.</param>
        /// <param name="row">The row index of the entry.</param>
        /// <param name="col">The column index of the entry.</param>
        public MyEntry(Spreadsheet ss, int row, int col) : base()
        {
            this.ss = ss;
            this.row = row + 1;
            this.col = col;
            this.StyleId = $"{row}-{col}";
            this.WidthRequest = 75;
            this.HeightRequest = 20;
            this.BackgroundColor = Colors.White;
            this.TextColor = Colors.Black;
            this.Focused += (sender, e) => ShowFormula();
            this.Unfocused += (sender, e) => CellUpdated();
            this.Focused += (sender, e) => HighLightedCell();
            this.Unfocused += (sender, e) => UnhighlightedCell();
            string cellName = GetCellName();
            cellEntries[cellName] = this;
        }

   

        /// <summary>
        /// Sets the background color and text color to unhighlighted state.
        /// </summary>
        public void UnhighlightedCell()
        {
            this.BackgroundColor = Colors.White;
            this.TextColor = Colors.Black;
        }

        /// <summary>
        /// Sets the background color and text color to highlighted state.
        /// </summary>
        public void HighLightedCell()
        {
            this.BackgroundColor = Colors.Salmon;
            this.TextColor = Colors.Yellow;
        }

        /// <summary>
        /// Retrieves the cell name based on row and column indices.
        /// </summary>
        /// <returns>The cell name.</returns>
        internal string GetCellName()
        {
            string colChar = ((char)('A' + col)).ToString();
            string rowChar = row.ToString();
            string cell = colChar + rowChar;
            return cell;
        }

        public void CellUpdated()
        {
            string cell = GetCellName();
            string content = this.Text;

            try
            {
                // Set the contents of the cell and get the list of dependent cells
                IList<string> dependentCells = ss.SetContentsOfCell(cell, content);

                // Update the UI with the new content of the cell
                object cellValue = ss.GetCellValue(cell);
                if (cellValue is string stringValue)
                {
                    // If the cell value is a string, set the text of the entry box to the string value
                    this.Text = stringValue;
                }
                else if (cellValue is double doubleValue)
                {
                    // If the cell value is a double, convert it to a string and set the text of the entry box
                    this.Text = doubleValue.ToString();
                }
                else if (cellValue is Formula formula)
                {
                    // If the cell value is a formula, set the text of the entry box to the formula representation
                    this.Text = "=" + formula.ToString();
                }
                else if (cellValue is FormulaError formulaError)
                {
                    this.Text = "ERROR";
                }

                foreach (string dependentCell in dependentCells)
                {
                    // Retrieve the UI element corresponding to the dependent cell name
                    if (cellEntries.TryGetValue(dependentCell, out MyEntry dependentEntryToUpdate))
                    {
                        // Get the new value for the dependent cell from the spreadsheet
                        object newValue = ss.GetCellValue(dependentCell);

                        // Update the UI element (dependentEntryToUpdate) with the new value
                        if (newValue is string stringValueDependent)
                        {
                            // If the new value is a string, set the text of the entry box to the string value
                            dependentEntryToUpdate.Text = stringValueDependent;
                        }
                        else if (newValue is double doubleValue)
                        {
                            // If the new value is a double, convert it to a string and set the text of the entry box
                            dependentEntryToUpdate.Text = doubleValue.ToString();
                        }
                        else if (newValue is Formula formula)
                        {
                            // If the new value is a formula, set the text of the entry box to the formula representation
                            dependentEntryToUpdate.Text = "=" + formula.ToString();
                        }
                        else if (newValue is FormulaError formulaError)
                        {
                            // Handle formula error, if needed
                            dependentEntryToUpdate.Text = "ERROR";
                        }
                    }
                }
            }
            catch (FormulaFormatException)
            {
                Text = "ERROR";
            }
            catch (CircularException)
            {
                Text = "CIRCULAR DEPENDENCY";
            }
            catch (Exception)
            {
                Text = "ERROR2";
            }
        }



        /// <summary>
        /// Displays formula when the cell is focused.
        /// </summary>
        private void ShowFormula()
        {
            string cell = GetCellName();
            object cellContent = ss.GetCellContents(cell);

            if (cellContent is string stringValue)
            {
                // If the cell value is a string, set the text of the entry box to the string value
                this.Text = stringValue;
            }
            else if (cellContent is double doubleValue)
            {
                // If the cell value is a double, convert it to a string and set the text of the entry box
                this.Text = doubleValue.ToString();
            }
            else if (cellContent is Formula formula)
            {
                // If the cell value is a formula, set the text of the entry box to the formula representation
                this.Text = "=" + formula.ToString();
            }
        }
    }
}
