﻿// See https://aka.ms/new-console-template for more information
using ExcelDataReader;

using (var stream = File.Open("C:\\workroot\\1.xls", FileMode.Open, FileAccess.Read))
{
    // Auto-detect format, supports:
    //  - Binary Excel files (2.0-2003 format; *.xls)
    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
    using (var reader = ExcelReaderFactory.CreateReader(stream))
    {
        var result = reader.AsDataSet();
        int a = 0;
    }
}
