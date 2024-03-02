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
    public class MyEntry : Entry
    {
        private readonly Spreadsheet ss;
        private readonly int col;
        private readonly int row;
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
            this.TextChanged += OnTextChanged;
            this.Focused += (sender, e) => ShowFormula();
            this.Unfocused += (sender, e) => CellUpdated();
            this.Focused += (sender, e) => HighLightedCell();
            this.Unfocused += (sender, e) => UnhighlightedCell();

        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string content = e.NewTextValue;
            string cellName = GetCellName();
            ss.SetContentsOfCell(cellName, content);
        }

        private void UnhighlightedCell()
        {
            this.BackgroundColor = Colors.White;
            this.TextColor = Colors.Black;
        }

        private void HighLightedCell()
        {
            this.BackgroundColor = Colors.Salmon;
            this.TextColor = Colors.Yellow;
        }

        private string GetCellName()
        {
            string colChar = ((char)('A' + col)).ToString();
            string rowChar = row.ToString();
            string cell = colChar + rowChar;
            return cell;
        }

        //method for when user add content into cell and set it in spreadsheet
        public void CellUpdated()
        {
            string content = this.Text;
            string cell = GetCellName();
            ss.SetContentsOfCell(cell, content);
            try
            {
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
                    this.Text = formula.ToString();
                }
            }
            catch (Exception ex)
            {
                Text = ex.Message;
            }
        }

        //show formula when cell is focused
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

        public void SetCellValue(string value)
        {
            this.Text = value;
        }

    }
}