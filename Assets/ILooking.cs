using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ILooking
{

    int Sensitivity { get; set; } // чувствительность мыши
    float HeadMinY { get; set; } // ограничение угла для головы
    float HeadMaxY { get; set; }

}
