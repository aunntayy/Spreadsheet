using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    }
}