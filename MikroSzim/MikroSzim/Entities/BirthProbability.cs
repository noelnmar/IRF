﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroSzim.Entities
{
    public class BirthProbability
    {
        public int Age { get; set; }
        public int NumberOfChildren { get; set; }
        public double P { get; set; }
    }
}
