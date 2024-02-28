using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI {
    public class MyEntry : Entry {

       
        public MyEntry(int row, int col) : base() {
            this.StyleId = $"{row}-{col}"; // Set a unique identifier for each MyEntry object
            this.WidthRequest = 75; // Set the desired width (in device-independent pixels)
            this.HeightRequest = 20; // Set the desired height (in device-independent pixels)

        }

    }
}
