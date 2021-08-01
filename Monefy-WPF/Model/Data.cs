﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monefy_WPF.Model
{
    public class Data
    {
        private float precentAge;
        public Data()
        {

        }
        public string Cotegorie { get; set; }
        public string Note { get; set; }
        public string Color { get; set; }
        public DateTime TimeCreate { get; set; }
        public bool Income { get; set; }
        public double Money { get; set; }
        public float PrecentAge { get => precentAge; set => precentAge = value; }
    }
}
