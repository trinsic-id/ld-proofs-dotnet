﻿using System;

namespace LinkedDataProofs
{
    public interface IVerifyData
    {
    }
    public class ByteArray : IVerifyData
    {
        public byte[] Data { get; set; }

        public static implicit operator ByteArray(byte[] data) => new ByteArray { Data = data };
    }
    public class StringArray : IVerifyData
    {
        public string[] Data { get; set; }

        public static implicit operator StringArray(string[] data) => new StringArray { Data = data };

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Data);
        }
    }
}
