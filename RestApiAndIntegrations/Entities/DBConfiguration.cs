using System;
using System.Collections.Generic;
using System.Text;

namespace AD.CAAPS.Entities
{
    public class DBConfiguration
    {
        public string ConnectionString { get; set; }
        public string DateFormat { get; set; } = "dd/MM/yyyy";
    }
}