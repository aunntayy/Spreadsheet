using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SS
{   
    /// <summary>
    /// 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> cells;
        DependencyGraph dg;

        /// <summary>
        /// Constructor set up for zero-argument constructor that creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet ()
        {
            cells = new Dictionary<string, Cell> ();
            dg = new DependencyGraph ();
        }
        /// <summary>
        /// Get the name of all non empty cell
        /// </summary>
        /// <returns> the cell name </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //return all the key that have value
            return cells.Keys;
        }

        public override object GetCellContents(string name)
        {
            //If name is null and invalid then throw exception
            if (name is null || !isValid(name)) 
            {
                throw new InvalidNameException();
            }
            //Get the contents
            return cells[name].Content;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            //If name is null and invalid then throw exception
            if (name is null || !isValid(name))
            {
                throw new InvalidNameException();
            }
            throw new InvalidNameException();
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Get the dependents by calling the GetDependents
            return dg.GetDependents(name);
        }

        //Check for valid name
        private bool isValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        //Cell class set up
        private class Cell
        {
            public object Content { get; private set; }
            public object Value {  get; private set; }
            
            public Cell(string name)
            {
                Content = name;
                Value = name;
            }
            public Cell(double number)
            {
                Content = number;
                Value = number;
            }
            public Cell(Formula formula)
            {
               //If it was a formula maybe it was valid already ???
               Content = formula;
               Value = formula;
            }
        }
    }

}
