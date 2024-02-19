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
using System.Xml;
using static FormulaEvaluator.Evaluator;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{
    /// <summary>
    /// Author:    Phuc Hoang
    /// Partner:   -None-
    /// Date:      14-Feb-2024
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
    ///    [This file contains the updated implementation of a spreadsheet application 
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
        private Dictionary<string, Cell> cells;
        // Dependency graph to track dependencies between cells
        private DependencyGraph dg;
        // Boolean to check change
        private bool changed;
        public override bool Changed
        {
            get { return changed; }
            protected set { changed = value; }
        }

        /// <summary>
        /// Constructor set up for zero-argument constructor that creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            // Initialize
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            changed = false;
        }
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> Normalizer, string version) : base(isValid, Normalizer, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            changed = false;
        }


        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : this(isValid, normalize, version)
        {
            if (!File.Exists(filepath))
            {
                throw new SpreadsheetReadWriteException("File not found: " + filepath);
            }

            string savedVersion = GetSavedVersion(filepath);

            if (!version.Equals(savedVersion))
            {
                throw new SpreadsheetReadWriteException("Incorrect version: Expected " + version + ", but found " + savedVersion);
            }
            LoadXml(filepath);
        }

        private void LoadXml(string filepath)
        {
            using (XmlReader reader = XmlReader.Create(filepath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "cell")
                    {
                        LoadCell(reader);
                    }
                }
            }
        }

        private void LoadCell(XmlReader reader)
        {
            string cellName = null;
            string cellContent = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "name")
                    {
                        cellName = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "contents")
                    {
                        cellContent = reader.ReadElementContentAsString();
                    }
                }

                if (cellName != null && cellContent != null)
                {
                    SetContentsOfCell(cellName, cellContent);
                    cellName = null;
                    cellContent = null;
                }
            }
        }


        internal class Cell
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
        ///   Returns the names of all non-empty cells.
        /// </summary>
        /// 
        /// <returns>
        ///     Returns an Enumerable that can be used to enumerate
        ///     the names of all the non-empty cells in the spreadsheet.  If 
        ///     all cells are empty then an IEnumerable with zero values will be returned.
        /// </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //return all the key that have value
            List<string> nonEmptyCellKeys = new List<string>();

            foreach (var keyValue in cells)
            {
                // Check if the content is not null and not an empty string
                if (keyValue.Value.Content != null && !string.IsNullOrEmpty(keyValue.Value.Content.ToString()))
                {
                    // Add the key to the list of non-empty cell keys
                    nonEmptyCellKeys.Add(keyValue.Key);
                }
            }

            return nonEmptyCellKeys;
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is invalid: blank/empty/""
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public override object GetCellContents(string name)
        {
            //If name is null and invalid then throw exception
            if (name is null || !isValid(name))
            {
                throw new InvalidNameException();
            }
            //Make sure every cell return an empty string
            if (cells.TryGetValue(Normalize(name), out var cell))
            {
                return cell.Content;
            }
            else
            {
                //Get the contents
                return "";
            }
        }
        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid: non-empty/not ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Update the cell with the given number
            Cell cell = new Cell(number);
            cells[name] = cell;
            changed = true;
            //Replace the dependents of 'name' in the dependency graph with an empty set
            dg.ReplaceDependees(name, new HashSet<string>());
            //Get cells that need to be recalculated then add name
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
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>       
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Update the cell
            Cell cell = new Cell(text);
            cells[name] = cell;
            changed = true;
            //Get cells that need to be recalculated then add name
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
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            // Store the original dependees to restore in case of a circular exception
            IEnumerable<string> originalDependees = dg.GetDependees(name);

            try
            {
                // Update dependency graph with new dependees from the formula
                dg.ReplaceDependees(name, formula.GetVariables());

                // Get the cells that need to be recalculated
                HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name));

                // Update the cell with the new formula
                Cell cell = new Cell(formula);
                cells[name] = cell;
                changed = true;
                // Convert the HashSet<string> to IList<string>
                IList<string> dependentCellsList = dependentCells.ToList();

                // Returns an enumeration, without duplicates, of the names of all cells that contain formulas containing name.
                return dependentCellsList;
            }
            catch (CircularException)
            {
                // Restore original dependees if a circular exception is detected
                dg.ReplaceDependees(name, originalDependees);
                throw;
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <required>
        ///    The name must be valid upon entry to the function.
        /// </required>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Get the dependents by calling the GetDependents
            return dg.GetDependents(name);
        }

        //Check for valid name
        private bool isValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9]*$");
        }

        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // If the name parameter is invalid, throw an InvalidNameException
            if (!IsValid(name)) { throw new InvalidNameException(); }
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
        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   1Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                // Create an XmlReader to read the XML file
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    // Read until the start of the XML document
                    while (reader.Read())
                    {
                        // Check if the reader is positioned on the start element of the spreadsheet
                        if (reader.IsStartElement() && reader.Name == "spreadsheet")
                        {
                            // Get the value of the version attribute
                            string version = reader.GetAttribute("version");
                            if (version != null)
                            {
                                // Return the version information
                                return version;
                            }
                            else
                            {
                                // If the version attribute is missing, throw an exception
                                throw new SpreadsheetReadWriteException("Cannot find file version" + filename);
                            }
                        }
                    }
                }
                // If the start element of the spreadsheet was not found, throw an exception
                throw new SpreadsheetReadWriteException("Spreadsheet element not found in the XML file.");
            }
            catch (Exception e)
            {
                // If any exception occurs while reading the file, throw a SpreadsheetReadWriteException
                throw new SpreadsheetReadWriteException("Error reading file or retrieving version information.");
            }
        }



        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                // Get the XML representation of the spreadsheet's contents
                string xmlContent = GetXML();

                // Write the XML content to the specified file
                File.WriteAllText(filename, xmlContent);

                // Update the Changed property after successful save
                Changed = false;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Error saving spreadsheet: " + e.Message);
            }
        }

        /// <summary>
        ///   Return an XML representation of the spreadsheet's contents
        /// </summary>
        /// <returns> contents in XML form </returns>
        public override string GetXML()
        {
            // Create a MemoryStream to capture XML content
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create an XML writer with indentation for readability and UTF-8 encoding
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = Encoding.UTF8, // Specify UTF-8 encoding
                    OmitXmlDeclaration = false // Include XML declaration
                };

                // Create the XML writer with the MemoryStream and settings
                using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                {
                    // Start the root element
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    // Iterate over each cell in the spreadsheet
                    foreach (var kvp in cells)
                    {
                        string cellName = kvp.Key;
                        Cell cell = kvp.Value;

                        // Start the cell element
                        writer.WriteStartElement("cell");

                        // Write the name element
                        writer.WriteElementString("name", cellName);

                        // Write the contents element based on the type of content
                        if (cell.Content is double)
                        {
                            writer.WriteElementString("contents", cell.Content.ToString());
                        }
                        else if (cell.Content is string)
                        {
                            writer.WriteElementString("contents", (string)cell.Content);
                        }
                        else if (cell.Content is Formula)
                        {
                            Formula formula = (Formula)cell.Content;
                            writer.WriteElementString("contents", "=" + formula.ToString());
                        }

                        // End the cell element
                        writer.WriteEndElement();
                    }

                    // End the spreadsheet element
                    writer.WriteEndElement();

                    // Flush the writer to ensure all content is written to the stream
                    writer.Flush();
                }

                // Convert the XML content to a string using UTF-8 encoding
                string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());

                // Return the XML content as a string
                return xmlString;
            }
        }





        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        // make it to have evaluate function
        public override object GetCellValue(string name)
        {
            // If name is invalid, throws an InvalidNameException.
            if (!isValid(name) || name is null)
            {
                throw new InvalidNameException();
            }
            // Assign the value type depending on the content
            object value = GetCellContents(name);
            // If formula
            if (value.GetType() == typeof(Formula))
            {
                Formula formula = (Formula)value;
                try
                {
                    // Recursively evaluate the formula
                    return formula.Evaluate(s => (double)GetCellValue(s));
                }
                catch (InvalidCastException)
                {
                    // Return FormulaError if there is an InvalidCastException during evaluation
                    return new FormulaError();
                }
            }
            return value;
        }
    }

}
