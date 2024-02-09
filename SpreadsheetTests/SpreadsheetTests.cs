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
        
        public void invalidCellValue()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.Equals("", sheet.GetNamesOfAllNonemptyCells());
        }
    }
}