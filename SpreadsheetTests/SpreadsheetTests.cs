using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
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
    ///    [This file contains the test for Spreedsheet Class]
    ///    
    /// </summary>
    
    [TestClass]
    public class SpreadsheetTests
    {
        ///<summary>
        ///Test for all Exception
        ///</summary>
        
        ///<paragraph>
        ///Start of exception test
        ///</paragraph>
        
        //Test for get cell content metho
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getInvalidCellName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("1A");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getNullCellName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        //Set Cell content with number
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setInvalidCellName1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("1A1", 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setNullCellName1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, 1.0);
        }

        //Set Cell content with text
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setInvalidCellName2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("1A", "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void setNullCellName2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "hello");
        }

        //Set Cell content with formula
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setInvalidCellName3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("1A", new Formula("A2+2"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void setNullCellName3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, new Formula("A2+2"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void setTextContentNull()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("B1", (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void setFormulaContentNull()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("B1", (Formula)null);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void circularDependency()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("B1", new Formula("A1"));
            sheet.SetCellContents("A1", new Formula("B1"));
        }
        ///<paragraph>
        ///End of exception test
        ///</paragraph>

        ///<summary>
        ///Functionality test
        ///</summary>

        ///<paragraph>
        ///Start of the Functionality test
        ///</paragraph>
        [TestMethod]
        public void setCellNumber()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a new cell and set it
            sheet.SetCellContents("A1", 43.0);
            Assert.AreEqual(43.0, sheet.GetCellContents("A1"));
            //Update Cell number type with new content
            sheet.SetCellContents("A1", 69.0);
            Assert.AreEqual(69.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void setCellText()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a new cell and set it
            sheet.SetCellContents("A1", "Hello");
            Assert.AreEqual("Hello", sheet.GetCellContents("A1"));
            //Update Cell text type with new content
            sheet.SetCellContents("A1", "Konichiwa");
            Assert.AreEqual("Konichiwa", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void setCelNumberFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a new cell and set it
            var cell = sheet.SetCellContents("A1", new Formula("A2+1"));
            Assert.AreEqual("A2+1", sheet.GetCellContents("A1").ToString());
            //Update Cell formula type with new content
            sheet.SetCellContents("A1", new Formula("A3+3"));
            Assert.AreEqual("A3+3", sheet.GetCellContents("A1").ToString());
        }

        [TestMethod]
        public void getNameOfAllNonEmptyCellTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("A2"));
            sheet.SetCellContents("A2", new Formula("A3+3"));
            sheet.SetCellContents("A3", new Formula("2+3+A4"));
            var cell = sheet.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(cell.Contains("A1"));
            Assert.IsTrue(cell.Contains("A2"));
        }

        [TestMethod]
        public void getContentOfEmptyCell()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual("", spreadsheet.GetCellContents("A1"));
        }
        ///<paragraph>
        ///End of the Functionality test
        ///</paragraph>
    }
}