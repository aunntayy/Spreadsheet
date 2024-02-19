using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Xml;

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
            sheet.SetContentsOfCell("1A1", "1.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setNullCellName1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "1.0");
        }

        //Set Cell content with text
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setInvalidCellName2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1A", "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void setNullCellName2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "hello");
        }

        //Set Cell content with formula
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setInvalidCellName3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1A", "=A2+2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void setNullCellName3()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "=A2+2");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void setTextContentNull()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void setFormulaContentNull()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void circularDependency()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1");
            sheet.SetContentsOfCell("A1", "=B1");
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
            sheet.SetContentsOfCell("A1", "43.0");
            Assert.AreEqual(43.0, sheet.GetCellContents("A1"));
            //Update Cell number type with new content
            sheet.SetContentsOfCell("A1", "69.0");
            Assert.AreEqual(69.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void setCellText()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a new cell and set it
            sheet.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", sheet.GetCellContents("A1"));
            //Update Cell text type with new content
            sheet.SetContentsOfCell("A1", "Konichiwa");
            Assert.AreEqual("Konichiwa", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void setCelNumberFormula()
        {
            AbstractSpreadsheet sheet2 = new Spreadsheet(s => true, s => s, "default");
            //Create a new cell and set it
            var cell = sheet2.SetContentsOfCell("A1", "=A2+1");
            Assert.AreEqual("A2+1", sheet2.GetCellContents("A1").ToString());
            //Update Cell formula type with new content
            sheet2.SetContentsOfCell("A1", "=A3+3");
            Assert.AreEqual("A3+3", sheet2.GetCellContents("A1").ToString());
        }

        [TestMethod]
        public void getNameOfAllNonEmptyCellTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2");
            ss.SetContentsOfCell("A2", "=A3+3");
            ss.SetContentsOfCell("A3", "=2+3+A4");
            var cell = ss.GetNamesOfAllNonemptyCells();
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
        
        //Get Cell content test
        [TestMethod()]
        public void formulaEvaluateTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("A2", "7");
            ss.SetContentsOfCell("A3", "8");
            ss.SetContentsOfCell("A4", "=A3 + A2");
            ss.SetContentsOfCell("B1", "=A1 + A2 + A3 + A4");
            Assert.AreEqual(36, (double)ss.GetCellValue("B1"), 1e-9);
        }
        [TestMethod]
        public void formulaEvaluateTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1","=A2+A3");
            ss.SetContentsOfCell("A2", "=A4 + 5");
            ss.SetContentsOfCell("A3", "=8 + A4");
            ss.SetContentsOfCell("A4", "=13.0");
            Assert.AreEqual(39.0,ss.GetCellValue("A1"));
            Assert.AreEqual(18.0, ss.GetCellValue("A2"));
            Assert.AreEqual(21.0, ss.GetCellValue("A3"));
            ss.SetContentsOfCell("A2","=A4 + 5 + 6");
            Assert.AreEqual(45.0,ss.GetCellValue("A1"));
        }

        [TestMethod()]
        public void getCellTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("A2", "7");
            ss.SetContentsOfCell("A3", "8");
            ss.SetContentsOfCell("A4", "=A3 + A2");
            ss.SetContentsOfCell("B1", "=A1 + A2 + A3 + A4");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getNullCell() 
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetCellValue("_A1");
        }

        [TestMethod]
        public void getErrorFormulaCell()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=heelo + 5");
            Assert.IsInstanceOfType(ss.GetCellValue("A1"), typeof(FormulaError));
        }
        /// <summary>
        /// Test for saving, get xml
        /// </summary>
        [TestMethod]
        public void TestSaveMethod()
        {
            // Arrange: Create a new instance of your Spreadsheet class
            Spreadsheet ss = new Spreadsheet();

            // Act: Set some contents in the spreadsheet
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "=A1+10");
            ss.SetContentsOfCell("C1", "hello");

            // Save the spreadsheet to a file
            string filename = "save1.txt";
            ss.Save(filename);

            ss = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual(5.0, ss.GetCellContents("A1"));
            Assert.AreEqual("A1+10", ss.GetCellContents("B1").ToString());
            Assert.AreEqual("hello", ss.GetCellContents("C1"));
        }

        //Incorrect file path test
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestIncorrectFilePath()
        {
            Spreadsheet ss = new Spreadsheet("blabla/blabla", s => true, s => s, "default");
        }

        //Incorrect verion test
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestIncorrectVersion()
        {
            Spreadsheet ss = new Spreadsheet("save1.txt", s => true, s => s, "v1");
        }
        //Missing version atributte
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestVersionAttributeNotFound()
        {
            // Create a temporary XML file without the version attribute
            string filename = "save2.txt";
            using (XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartElement("spreadsheet");
                writer.WriteEndElement();
            }

            // Call GetSavedVersion with the temporary XML file
            Spreadsheet ss = new Spreadsheet(); 
            string version = ss.GetSavedVersion(filename); 
        }
        //Missing element
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSSElementNotFound()
        {
            // Create a temporary XML file without the version attribute
            string filename = "save3.txt";
            using (XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartElement("blabla");
                writer.WriteEndElement();
            }

            // Call GetSavedVersion with the temporary XML file
            Spreadsheet ss = new Spreadsheet(); 
            string version = ss.GetSavedVersion(filename);
        }

        [TestMethod]
        public void TestErrorReadingFile()
        {
            // Create a new instance of your spreadsheet class
            Spreadsheet ss = new Spreadsheet();

            // Populate the spreadsheet with some data (cells)
            // For demonstration purposes, let's assume cells contain some values
            ss.SetContentsOfCell("A1", "Hello");
            ss.SetContentsOfCell("B2", "123");
            ss.SetContentsOfCell("C3", "=A1 + B2");

            // Call the GetXML method to get the XML representation of the spreadsheet
            string xml = ss.GetXML();

            // Assert that the XML string is not empty
            Assert.IsFalse(string.IsNullOrEmpty(xml));
        }

        //Missing version atributte
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadFile()
        {
            // Create a temporary XML file without the version attribute
            string filename = "save4.txt";

            // Call GetSavedVersion with the temporary XML file
            Spreadsheet ss = new Spreadsheet("save4.txt", s => true, s => s, "");
            string version = ss.GetSavedVersion(filename);
        }

        [TestMethod]
        public void TestSaveFile()
        {
            // Create a temporary XML file without the version attribute
            string filename = "save4.txt";

            // Create a new instance of your spreadsheet class
            Spreadsheet ss = new Spreadsheet();

            // Call the constructor with the temporary XML file, which should throw an exception
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet(filename, s => true, s => s, ""));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        // Save to non exist path
        public void SaveNonExist()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("/save4.txt");
        }

    }
}