using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace electricbill
{
    
    public class Bill
    {
        public static List<Bill> ItemsSource { get; internal set; }
        [PrimaryKey, AutoIncrement]
        public long meterNumber { get; set; }
        public double pAmount { get; set; }
        public double aPayable { get; set; }
    }
}



