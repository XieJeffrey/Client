using UnityEngine;
using System.Collections;
using System.IO;

public class OOFormTools
{
    /// <summary>
    /// Read all text by file path name
    /// </summary>
    /// <param name="path">text file path</param>
    /// <returns></returns>
    public static string ReadFileText(string pathName)
    {
        string ret = "";
        if (File.Exists(pathName))
        {
            FileStream fs = File.Open(pathName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            System.Byte[] check_byte = br.ReadBytes(2);
            System.Text.Encoding decoder = System.Text.Encoding.Default;

            if (check_byte.Length > 0 && check_byte[0] >= 0xEF)
            {
                if (check_byte[0] == 0xEF && check_byte[1] == 0xBB)
                {
                    decoder = System.Text.Encoding.UTF8;
                }
                else if (check_byte[0] == 0xFE && check_byte[1] == 0xFF)
                {
                    decoder = System.Text.Encoding.BigEndianUnicode;
                }
                else if (check_byte[0] == 0xFF && check_byte[1] == 0xFE)
                {
                    decoder = System.Text.Encoding.Unicode;
                }
                else
                {
                    decoder = System.Text.Encoding.Default;
                }
            }
            else
            {
                decoder = System.Text.Encoding.Default;
            }

            fs.Position = 0;
            StreamReader reader = new StreamReader(fs, decoder);
            string row = "";
            string text_row = "";

            while ((row = reader.ReadLine()) != null)
            {
                text_row = text_row + row + "\n";
            }
            if(text_row.Length > 0)
                text_row = text_row.Substring(0, text_row.Length - 1);

            reader.Close();
            ret = text_row;
        }
        else
        {
            Debug.Log("Cannot Read File");
        }
        return ret;
    }

    /// <summary>
    /// Write text to file by path name
    /// </summary>
    /// <param name="pathName">text file path</param>
    /// <param name="textString">text to save</param>
    /// <returns></returns>
    public static bool WriteFileText(string pathName, string textString)
    {

        StreamWriter writer = new StreamWriter(File.Open(pathName, FileMode.Create), System.Text.Encoding.Unicode);
        writer.Write(textString);
        writer.Close();

        return true;
    }

    /// <summary>
    /// Write text to file with encode type
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="textString"></param>
    /// <param name="encodeType"></param>
    /// <returns></returns>
    public static bool WriteFileText(string pathName, string textString, System.Text.Encoding encodeType)
    {
        StreamWriter writer = new StreamWriter(File.Open(pathName, FileMode.Create), encodeType);
        writer.Write(textString);
        writer.Close();

        return true;
    }
	
	/// <summary>
	/// Check  the path
	/// </summary>
	/// <returns>
	/// The directory.
	/// </returns>
	/// <param name='pathName'>
	/// If set to <c>true</c> _path.
	/// </param>
    public static bool CheckDirectory(string pathName)
    {
        if (!Directory.Exists(pathName))
        {
            Directory.CreateDirectory(pathName);
        }
        return true;
    }
}
