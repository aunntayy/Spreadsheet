using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static FormulaEvaluator.Evaluator;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{
    /// <summary>
    /// Author:    Phuc Hoang
    /// Partner:   -None-
    /// Date:      10-Feb-2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Phuc Hoang - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Phuc Hoang, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    [This file contains the implementation of a spreadsheet application 
    ///    for CS 3500 course. It includes classes and methods related to managing
    ///    and manipulating spreadsheet data.]
    ///    
    /// </summary>

    /// <summary>
    /// Represents a spreadsheet application capable of storing and manipulating data.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Dictionary to store cells in the spreadsheet
        Dictionary<string, Cell> cells;
        // Dependency graph to track dependencies between cells
        DependencyGraph dg;
        // Boolean to check change
        bool change;
        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <summary>
        /// Constructor set up for zero-argument constructor that creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            // Initialize
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            bool change;
        }
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> Normalizer, string version) : base(isValid, Normalizer, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            change = false;
        }

        public Spreadsheet(string Filepath, Func<string, bool> isValid, Func<string, string> Normalizer, string version) : base(isValid, Normalizer, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            change = false;
        }
        private class Cell
        {
            /// <summary>
            /// Gets or sets the content of the cell.
            /// </summary>
            public object Content { get; private set; }

            /// <summary>
            /// Gets or sets the value of the cell.
            /// </summary>
            public object Value { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Cell class with a string content.
            /// </summary>
            /// <param name="content">The content of the cell.</param>
            public Cell(object content)
            {
                Content = content;
                Value = content;
            }
        }
        /// <summary>
        /// Get the name of all non empty cell
        /// </summary>
        /// <returns> All the cell name </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Where(keyValue => keyValue.Value.Content != null && !string.IsNullOrEmpty(keyValue.Value.Content.ToString())).Select(KeyValue => KeyValue.Key);
        }

        /// <summary>
        /// Get the cell content with using the cell name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The content of the named cell</returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            //If name is null and invalid then throw exception
            if (name is null || !isValid(name) || name == "")
            {
                throw new InvalidNameException();
            }
            //Make sure every cell return an empty string
            if (!cells.ContainsKey(name)) { return ""; }
            //Get the contents
            return cells[name].Content;
        }
        /// <summary>
        /// Set the content of the named cell with a double number
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="number">Double type input</param>
        /// <returns>an enumeration, without duplicates, of the names of all cells that contain formulas containing name.</returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, double number)
        {
            // If name is null or invalid, throw an exception
            if (name is null || !isValid(name) || name == "")
            {
                throw new InvalidNameException();
            }

            Cell cell;

            // Update the cell
            cell = new Cell(number);
            cells[name] = cell;

            // Get cells that need to be recalculated then add name
            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name))
                {
                    name
                };
            // Convert the HashSet<string> to IList<string>
            IList<string> dependentCellsList = dependentCells.ToList();
            // Returns an enumeration, without duplicates, of the names of all cells that contain formulas containing name.
            return dependentCellsList;

        }

        /// <summary>
        /// Set the content of the named cell with a text string
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="text">String type input</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if the name is null</exception>
        /// <exception cref="InvalidNameException">Thrown if the name is invalid</exception>
        /// <exception cref="ArgumentNullException">If text parameter is null, throw an ArgumentNullException</exception>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // If name is null or invalid, throw an exception
            if (name is null || !isValid(name) || name == "")
            {
                throw new InvalidNameException();
            }

            Cell cell;

            // Update the cell
            cell = new Cell(text);
            cells[name] = cell;

            // Get cells that need to be recalculated then add name
            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name))
                {
                    name
                };

            // Convert the HashSet<string> to IList<string>
            IList<string> dependentCellsList = dependentCells.ToList();
            // Returns an enumeration, without duplicates, of the names of all cells that contain formulas containing name.
            return dependentCellsList;

        }

        /// <summary>
        /// Set the content of the named cell with a Formula
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="text">Formula type input</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if the name is null</exception>
        /// <exception cref="InvalidNameException">Thrown if the name is invalid</exception>
        /// <exception cref="ArgumentNullException">If formula parameter is null, throw an ArgumentNullException</exception>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // If name is null or invalid, throw an exception
            if (name is null || !isValid(name) || name == "")
            {
                throw new InvalidNameException();
            }
            //Check if formula is null
            if (formula == null)
            {
                throw new ArgumentNullException("Content cannot be null");
            }

            Cell cell;

            //Update the cell
            cell = new Cell(formula);
            cells[name] = cell;
            //Add dependency for each var in formula to cell
            foreach (var Var in formula.GetVariables())
            {
                dg.AddDependency(Var, name);
            }
            // Get cells that need to be recalculated then add name
            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name))
                {
                    name
                };
            // Convert the HashSet<string> to IList<string>
            IList<string> dependentCellsList = dependentCells.ToList();
            // Returns an enumeration, without duplicates, of the names of all cells that contain formulas containing name.
            return dependentCellsList;
        }
        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.
        /// </summary>
        /// <param name="name">Named cell</param>
        /// <returns></returns>
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

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // If the name parameter is invalid, throw an InvalidNameException
            if (!isValid(name) || name is null) { throw new InvalidNameException(); }
            if (content is null) { throw new ArgumentNullException(); }

            if (name == "") { return SetCellContents(name, content); }


            if (double.TryParse(content, out double number))
            {
                return SetCellContents(name, number);
            }

            if (content.StartsWith("="))
            {
                //trim of the "="
                content = content.Substring(1);
                //pass into formula constructor
                Formula f = new Formula(content, Normalize, IsValid);
                return SetCellContents(name, f);
            }


            return SetCellContents(name, content);
        }

        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override string GetXML()
        {
            throw new NotImplementedException();
        }
        // make it to have evaluate function
        public override object GetCellValue(string name)
        {
            // If name is invalid, throws an InvalidNameException.
            if (!isValid(name) || name is null)
            {
                throw new InvalidNameException();
            }
            //Assign the value type depend on the content
            object value = GetCellContents(name);
            //If formula
            if (value.GetType() == typeof(Formula))
            {
                Formula formula = (Formula)value;
                //Recursivly evaluate the formula 
                return formula.Evaluate(s => (double)GetCellValue(s));
            }
            return value;

        }
    }

}
