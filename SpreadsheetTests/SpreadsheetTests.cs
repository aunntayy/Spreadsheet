using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void invalidCellName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("1A");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void nullCellName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }


        [TestMethod]
        public void setCellNumber()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 42.0);
            sheet.SetCellContents("A2", "hello");
            sheet.SetCellContents("A1", 43.0);
            Assert.AreEqual("hello", sheet.GetCellContents("A2"));
            Assert.AreEqual(43.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void setCelNumberFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("A2"));
            sheet.SetCellContents("A2", new Formula("A3+3"));
            sheet.SetCellContents("A3", new Formula("2+3+A4"));
            Assert.AreEqual("A3+3", sheet.GetCellContents("A2").ToString());
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
    }
}