//using System;
using System.Collections.Generic;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using System.Windows.Forms; 

namespace Battleship
{
    public class Cell
    {
        public int Column
        {
            get;set;
        }

        public int Row
        {
            get; set;
        }

        public CellType Type
        {
            get; set;
        }

        public Button Btn
        {
            get;set;
        }

        public List<Cell> Ship
        {
            get;set;
        }

        public Cell()
        {
            Ship = new List<Cell>();
        }
    }

}
