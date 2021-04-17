using System;

namespace TauCode.Db.MySql.Tests.DbCruder
{
    public class SuperTableRowDto
    {
        public int TheInt { get; set; }
        public uint TheIntUnsigned { get; set; }

        public ulong TheBit { get; set; }
        public ulong TheBit9 { get; set; }
        public ulong TheBit17 { get; set; }
        public ulong TheBit33 { get; set; }

        public sbyte TheTinyInt { get; set; }
        public byte TheTinyIntUnsigned { get; set; }

        public sbyte TheBool { get; set; }
        public sbyte TheBoolean { get; set; }

        public short TheSmallInt { get; set; }
        public ushort TheSmallIntUnsigned { get; set; }


        public int TheMediumInt { get; set; }
        public uint TheMediumIntUnsigned { get; set; }

        public long TheBigInt { get; set; }
        public ulong TheBigIntUnsigned { get; set; }

        public decimal TheDecimal { get; set; }
        public decimal TheNumeric { get; set; }

        public float TheFloat { get; set; }
        public double TheDouble { get; set; }

        public DateTime TheDate { get; set; }
        public DateTime TheDateTime { get; set; }
        public DateTime TheTimeStamp { get; set; }
        public TimeSpan TheTime { get; set; }
        public short TheYear { get; set; }

        public string TheChar { get; set; }
        public string TheVarChar { get; set; }

        public byte[] TheBinary { get; set; }
        public byte[] TheVarBinary { get; set; }

        public string TheTinyText { get; set; }
        public string TheText { get; set; }
        public string TheMediumText { get; set; }
        public string TheLongText { get; set; }

        public byte[] TheTinyBlob { get; set; }
        public byte[] TheBlob { get; set; }
        public byte[] TheMediumBlob { get; set; }
        public byte[] TheLongBlob { get; set; }

        public int? NotExisting { get; set; }
    }
}
