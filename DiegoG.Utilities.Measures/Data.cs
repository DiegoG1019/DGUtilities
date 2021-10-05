using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiegoG.Utilities.Measures.Data;

namespace DiegoG.Utilities.Measures
{
    public class Data : Measure<Data>
    {
        public enum DataPrefix
        {
            n, Kilo, Mega, Giga, Tera, Peta, Exa, Zetta, Yotta
        }

        public double TotalBytes
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }

        public double GetBytesAs(DataPrefix prefix) => TotalBytes / DataPrefixValues[prefix];

        public Data(double bytes, DataPrefix prefix = DataPrefix.n) => TotalBytes = bytes * DataPrefixValues[prefix];
        public Data() : this(0) { }

        /// <summary>
        /// Gets the total amount of bytes the amount would represent with the given prefix. 15 kilobytes would return 15360
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static double GetTotalBytes(double bytes, DataPrefix prefix = DataPrefix.n) => bytes * DataPrefixValues[prefix];

        /// <summary>
        /// Gets the total prefixed bytes the amount would represent. 15360 bytes as kilobytes would return 15
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static double GetBytesAs(double bytes, DataPrefix prefix) => bytes / DataPrefixValues[prefix];

        private static readonly Dictionary<DataPrefix, double> DataPrefixValues = new()
        {
            { DataPrefix.Yotta, Math.Pow(1024, 8) },
            { DataPrefix.Zetta, Math.Pow(1024, 7) },
            { DataPrefix.Exa, Math.Pow(1024, 6) },
            { DataPrefix.Peta, Math.Pow(1024, 5) },
            { DataPrefix.Tera, Math.Pow(1024, 4) },
            { DataPrefix.Giga, Math.Pow(1024, 3) },
            { DataPrefix.Mega, Math.Pow(1024, 2) },
            { DataPrefix.Kilo, 1024 },
            { DataPrefix.n, 1 }
        };

        public static readonly Data Zero = new(0);
        public static readonly Data Byte = new(1);
        public static readonly Data KiloByte = new(1, DataPrefix.Kilo);
        public static readonly Data MegaByte = new(1, DataPrefix.Mega);
        public static readonly Data GigaByte = new(1, DataPrefix.Giga);
        public static readonly Data TeraByte = new(1, DataPrefix.Tera);
        public static readonly Data PetaByte = new(1, DataPrefix.Peta);
        public static readonly Data ExaByte = new(1, DataPrefix.Exa);
        public static readonly Data ZettaByte = new(1, DataPrefix.Zetta);
        public static readonly Data YottaByte = new(1, DataPrefix.Yotta);
    }
}
