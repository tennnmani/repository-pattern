﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
    }
}
