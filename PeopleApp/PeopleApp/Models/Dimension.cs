﻿using PeopleApp.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleApp.Models
{
    public class Dimension : TableData
    {
        public string Label { get; set; }
        public bool? Interval { get; set; }
    }
}
