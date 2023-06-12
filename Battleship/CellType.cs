using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship 
{
    public enum CellType
    {
        Open=1,
        Hide=2,
        Miss=3,
        Ship=4,
        Shot=5,
        Dead=6,
        HideShip=7
    }
}
