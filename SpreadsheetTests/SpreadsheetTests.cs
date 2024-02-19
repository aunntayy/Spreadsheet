using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Xml;

namespace SpreadsheetTests
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
    ///    [This file contains the upadted test for Spreedsheet Class]
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
            ss.Save("garbage/text4.xml");
        }
        /// <summary>
        /// Test from a4
        /// </summary>
        [TestMethod]
        public void TestChangedProperty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("A1", "123");
            Assert.IsTrue(ss.Changed);
        }
        // CIRCULAR FORMULA DETECTION
        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCircular() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2"));
            s.SetContentsOfCell("A2", ("=A1"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(CircularException))]
        public void TestComplexCircular() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A3", ("=A4+A5"));
            s.SetContentsOfCell("A5", ("=A6+A7"));
            s.SetContentsOfCell("A7", ("=A1+A1"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircular() {
            Spreadsheet s = new Spreadsheet();
            try {
                s.SetContentsOfCell("A1", ("=A2+A3"));
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", ("=A3*A1"));
            } catch (CircularException e) {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        [TestMethod()]
        [TestCategory("17b")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCellsCircular() {
            Spreadsheet s = new Spreadsheet();
            try {
                s.SetContentsOfCell("A1", ("=A2"));
                s.SetContentsOfCell("A2", ("=A1"));
            } catch (CircularException e) {
                Assert.AreEqual("", s.GetCellContents("A2"));
                Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        public void TestEmptyNames() {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        public void TestExplicitEmptySet() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        public void TestSimpleNamesString() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestSimpleNamesDouble() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestSimpleNamesFormula() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", ("=3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestMixedNames() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", ("=3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestSetSingletonDouble() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", ("=5"));
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestSetSingletonString() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", ("=5"));
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestSetSingletonFormula() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", ("=5")).SequenceEqual(new List<string>() { "C1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestSetChain() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestChangeFtoD() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestChangeFtoS() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestChangeStoF() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", ("=23"));
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestStress1() {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=B1+B2"));
            s.SetContentsOfCell("B1", ("=C1-C2"));
            s.SetContentsOfCell("B2", ("=C3*C4"));
            s.SetContentsOfCell("C1", ("=D1*D2"));
            s.SetContentsOfCell("C2", ("=D3*D4"));
            s.SetContentsOfCell("C3", ("=D5*D6"));
            s.SetContentsOfCell("C4", ("=D7*D8"));
            s.SetContentsOfCell("D1", ("=E1"));
            s.SetContentsOfCell("D2", ("=E1"));
            s.SetContentsOfCell("D3", ("=E1"));
            s.SetContentsOfCell("D4", ("=E1"));
            s.SetContentsOfCell("D5", ("=E1"));
            s.SetContentsOfCell("D6", ("=E1"));
            s.SetContentsOfCell("D7", ("=E1"));
            s.SetContentsOfCell("D8", ("=E1"));
            IList<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestStress1a() {
            TestStress1();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestStress1b() {
            TestStress1();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestStress1c() {
            TestStress1();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestStress2() {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++) {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, ("=A" + (i + 1)))));
            }
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestStress2a() {
            TestStress2();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestStress2b() {
            TestStress2();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestStress2c() {
            TestStress2();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestStress3() {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++) {
                s.SetContentsOfCell("A" + i, ("=A" + (i + 1)));
            }
            try {
                s.SetContentsOfCell("A150", ("=A50"));
                Assert.Fail();
            } catch (CircularException) {
            }
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestStress3a() {
            TestStress3();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestStress3b() {
            TestStress3();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestStress3c() {
            TestStress3();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestStress4() {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++) {
                s.SetContentsOfCell("A1" + i, ("=A1" + (i + 1)));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++) {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestStress4a() {
            TestStress4();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestStress4b() {
            TestStress4();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestStress4c() {
            TestStress4();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestStress5() {
            RunRandomizedTest(47, 2519);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestStress6() {
            RunRandomizedTest(48, 2521);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestStress7() {
            RunRandomizedTest(49, 2526);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestStress8() {
            RunRandomizedTest(50, 2521);
        }

        /// <summary>
        /// Sets random contents for a random cell 10000 times
        /// </summary>
        /// <param name="seed">Random seed</param>
        /// <param name="size">The known resulting ss size, given the seed</param>
        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        /// <summary>
        /// Generates a random cell name with a capital letter and number between 1 - 99
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        /// <summary>
        /// Generates a random Formula
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }


    }
}