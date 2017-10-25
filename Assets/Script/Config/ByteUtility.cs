using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ByteUtility
{
    #region WriteByte
    public static byte[] Write(object data, byte[] array)
    {
        if (data is byte[])
            return WriteByte(data as byte[], array);
        if (data is uint)
            return WriteUint32((uint)data, array);
        if (data is int)
            return WriteInt32((int)data, array);
        if (data is short)
            return WriteShort((short)data, array);
        if (data is string)
            return WriteString((string)data, array);
        Console.WriteLine("不支持的类型");
        return new byte[0];
    }
    private static byte[] WriteByte(byte[] data, byte[] array)
    {
        byte[] newArray = new byte[array.Length + data.Length];
        array.CopyTo(newArray, 0);
        data.CopyTo(newArray, array.Length);
        return newArray;
    }

    private static byte[] WriteUint32(uint data, byte[] array)
    {
        byte[] tmp = BitConverter.GetBytes(data);
        byte[] newArray = new byte[tmp.Length + array.Length];
        array.CopyTo(newArray, 0);
        tmp.CopyTo(newArray, array.Length);
        return newArray;
    }

    private static byte[] WriteInt32(int data, byte[] array)
    {
        byte[] tmp = BitConverter.GetBytes(data);
        byte[] newArray = new byte[tmp.Length + array.Length];
        array.CopyTo(newArray, 0);
        tmp.CopyTo(newArray, array.Length);
        return newArray;
    }

    private static byte[] WriteShort(short data, byte[] array)
    {
        byte[] tmp = BitConverter.GetBytes(data);
        byte[] newArray = new byte[tmp.Length + array.Length];
        array.CopyTo(newArray, 0);
        tmp.CopyTo(newArray, array.Length);
        return newArray;
    }

    private static byte[] WriteString(string data, byte[] array)
    {
        byte[] tmp2 = System.Text.Encoding.UTF8.GetBytes(data);
        short strLength = (short)tmp2.Length;
        byte[] tmp1 = BitConverter.GetBytes(strLength);
        byte[] newArray = new byte[tmp1.Length + tmp2.Length + array.Length];
        array.CopyTo(newArray, 0);
        tmp1.CopyTo(newArray, array.Length);
        tmp2.CopyTo(newArray, array.Length + tmp1.Length);
        return newArray;
    }

    private static byte[] WriteBoolen(bool data, byte[] array)
    {
        byte[] tmp = BitConverter.GetBytes(data);
        byte[] newArray = new byte[tmp.Length + array.Length];
        array.CopyTo(newArray, 0);
        tmp.CopyTo(newArray, array.Length);
        return newArray;
    }
    #endregion

    #region ReadByte
    public static uint ReadUint(ref byte[] data)
    {
        uint tmp = BitConverter.ToUInt32(data, 0);
        byte[] newData = new byte[data.Length - 4];
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = data[i + 4];
        }
        data = newData;
        return tmp;
    }

    public static short ReadShort(ref byte[] data)
    {
        short tmp = BitConverter.ToInt16(data, 0);
        byte[] newData = new byte[data.Length - 2];
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = data[i + 2];
        }
        data = newData;
        return tmp;
    }

    public static string ReadString(ref byte[] data)
    {
        short length = ReadShort(ref data);
        byte[] tmpStr = new byte[length];
        byte[] newData = new byte[data.Length - length];
        for (int i = 0; i < data.Length; i++)
        {
            if (i < length)
            {
                tmpStr[i] = data[i];
            }
            else
            {
                newData[i - length] = data[i];
            }
        }
        string str = System.Text.Encoding.UTF8.GetString(tmpStr);
        data = newData;
        return str;
    }

    #endregion

}

