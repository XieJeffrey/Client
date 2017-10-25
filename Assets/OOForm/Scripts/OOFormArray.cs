using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class OOFormArray:object
{
	/// <summary>
	/// The column count of form.
	/// </summary>
    private int _mColumnCount = 0;
	public int mColumnCount
	{
		get
		{
			return _mColumnCount;
		}
	}
	
	
	/// <summary>
	/// The row count of form.
	/// </summary>
    private int _mRowCount = 0;
	public int mRowCount
	{
		get
		{
			return _mRowCount;
		}
	}
	
	/// <summary>
	/// The data of form.  mData[column][row]
	/// </summary>
    public List<List<string>> mData = new List<List<string>>();
	
	/// <summary>
	/// The column name dictionary of form.
	/// </summary>
	public Dictionary<string, int> mColumnDic;
	
	/// <summary>
	/// The row name dictionary of form.
	/// </summary>
	public Dictionary<string, int> mRowDic;
	
	/// <summary>
	/// The current reading row.
	/// </summary>
	private int mCurrentRow = -1;
	
	/// <summary>
	/// Set the current reading row
	/// </summary>
	/// <param name='currentRow'>
	/// The current reading row.
	/// </param>
	public void SetCurrentRow(int currentRow)
	{
		mCurrentRow = currentRow;
	}
	
    /// <summary>
    /// Insert a column
    /// </summary>
    /// <param name="_index">Insert position</param>
    public void InsertColumn(int _index)
    {
        List<string> tmp = new List<string>();
        for (int i = 0; i < _mRowCount; i++)
        {
            tmp.Add("");
        }
        mData.Insert(_index, tmp);
        _mColumnCount++;
    }

    /// <summary>
    /// Move column left
    /// </summary>
    /// <param name="columnIndex">moving column</param>
    public void MoveColumnLeft(int columnIndex)
    {
        List<string> tmp = mData[columnIndex];
        int move_index = columnIndex - 1;
        if (move_index < 0)
        {
            move_index = mColumnCount - 1;
        }
        mData[columnIndex] = mData[move_index];
        mData[move_index] = tmp;
    }

    /// <summary>
    /// Move column right
    /// </summary>
    /// <param name="columnIndex">moving column</param>
    public void MoveColumnRight(int columnIndex)
    {
        List<string> tmp = mData[columnIndex];
        int move_index = columnIndex + 1;
        if (move_index >= mColumnCount)
        {
            move_index = 0;
        }
        mData[columnIndex] = mData[move_index];
        mData[move_index] = tmp;
    }

    /// <summary>
    /// Deletes the column.
    /// </summary>
    /// <param name='_index'>
    /// Deleting column
    /// </param>
    public void DeleteColumn(int _index)
    {

        mData.RemoveAt(_index);
        _mColumnCount--;
        if (_mColumnCount == 0)
        {
            _mRowCount = 0;
        }
    }

    /// <summary>
    /// Insert a row
    /// </summary>
    /// <param name="_index">Row to insert data</param>
    public void InsertRow(int _index)
    {
        foreach (List<string> list in mData)
        {
            list.Insert(_index, "");
        }
        _mRowCount++;
    }

    /// <summary>
    /// Delete a row
    /// </summary>
    /// <param name="_index">Deleting row</param>
    public void DeleteRow(int _index)
    {
        foreach (List<string> list in mData)
        {
            list.RemoveAt(_index);
        }
        _mRowCount--;
    }
	
	/// <summary>
	/// Checks the column name dic.
	/// </summary>
	void CheckColumnDic()
	{
		if(_mRowCount >= 1 && _mColumnCount >= 1 && mColumnDic == null)
		{
            RefreshColumnDic();
		}
	}

    /// <summary>
    /// Refresh column name dic.
    /// </summary>
    public void RefreshColumnDic()
    {
        mColumnDic = new Dictionary<string, int>();
        for (int i = 0; i < _mColumnCount; i++)
        {
            string dic_key = GetString(i, 0);
            mColumnDic[dic_key.Split(',')[0]] = i;
        }
    }

    /// <summary>
    /// Add a column name to column name dic.
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="columnIndex"></param>
    public void AddColumnName(string columnName, int columnIndex)
    {
        mColumnDic[columnName] = columnIndex;
    }
	
	/// <summary>
	/// Checks the row name dic.
	/// </summary>
	void CheckRowDic()
	{
		if(_mRowCount >= 1 && _mColumnCount >= 1 && mRowDic == null)
		{
            RefreshRowDic();
		}
	}

    /// <summary>
    /// Refresh row name dic.
    /// </summary>
    public void RefreshRowDic()
    {
        mRowDic = new Dictionary<string, int>();
        for (int i = 0; i < _mRowCount; i++)
        {
            string dic_key = GetString(0, i);
            mRowDic[dic_key.Split(',')[0]] = i;
        }
    }

    /// <summary>
    /// Add a row name to row name dic.
    /// </summary>
    /// <param name="rowName"></param>
    /// <param name="rowIndex"></param>
    public void AddRowName(string rowName, int rowIndex)
    {
        mRowDic[rowName] = rowIndex;
    }
	
	/// <summary>
	/// Gets the column.
	/// </summary>
	/// <returns>
	/// The column.
	/// </returns>
	/// <param name='columnKey'>
	/// Column key.
	/// </param>
	public int GetColumn(string columnKey)
	{
		CheckColumnDic();
		if(mColumnDic.ContainsKey(columnKey))
		{
			return mColumnDic[columnKey];
		}
		return -1;
	}
	
	/// <summary>
	/// Get the row index by row name.
	/// </summary>
	/// <returns>
	/// The row index.
	/// </returns>
	/// <param name='rowKey'>
	/// Row key.
	/// </param>
	public int GetRow(string rowKey)
	{
		CheckRowDic();
		if(mRowDic.ContainsKey(rowKey))
		{
			return mRowDic[rowKey];
		}
		return -1;
	}
	
	/// <summary>
	/// Object2s the int.
	/// </summary>
	/// <returns>
	/// The int.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	int Object2Int(object obj)
	{
			return (int)obj;
	}
	
	
	/// <summary>
	/// Gets the string data.
	/// </summary>
	/// <returns>
	/// The string data.
	/// </returns>
	/// <param name='args'>
	/// Arguments.(column, row)
	/// </param>
	string GetStringData(object[] args)
	{
		if(args.Length <= 0)
			return "";
		
		int column = -1;
		int row = -1;
		
		if(typeof(System.Int32) == args[0].GetType())
		{
			column = (int)args[0];
		}
		else if(typeof(System.String) == args[0].GetType())
		{
			column = GetColumn((string)args[0]);
		}
		else
		{
			column = Object2Int(args[0]);
		}
		
		if(args.Length >= 2)
		{
			if(typeof(System.Int32) == args[1].GetType())
			{
				row = (int)args[1];
			}
			else if(typeof(System.String) == args[1].GetType())
			{
				row = GetRow((string)args[1]);
			}
			else
			{
				row = Object2Int(args[1]);
			}
		}
		else
		{
			row = mCurrentRow;
		}
		
		if(column < _mColumnCount && column >= 0 && row < _mRowCount &&  row >= 0)
        {
            return mData[column][row];
        }
        return "";
	}

	
	/// <summary>
    /// Gets the data in string type. GetString(column[, row])
	/// </summary>
	/// <returns>
	/// The string.
	/// </returns>
	/// <param name='args'>
    /// GetString(column[, row])
	/// </param>
	public string GetString(params object[] args)
	{
		return GetStringData(args);
	}
	
	/// <summary>
    /// Gets the data in int type. GetInt(column[, row])
	/// </summary>
	/// <returns>
	/// The int.
	/// </returns>
	/// <param name='arg'>
	/// GetInt(column[, row])
	/// </param>
	public int GetInt(params object[] args)
	{
		string str = GetStringData(args);
		int ret;
		if(int.TryParse(str, out ret))
		{
			return ret;
		}
		else
		{
			return 0;
		}
	}
	
	/// <summary>
    /// Gets the data in float type. GetFloat(column[, row])
	/// </summary>
	/// <returns>
	/// The float.
	/// </returns>
	/// <param name='arg'>
	/// GetFloat(column[, row])
	/// </param>
	public float GetFloat(params object[] args)
	{
		string str = GetStringData(args);
		float ret;
		if(float.TryParse(str, out ret))
		{
			return ret;
		}
		else
		{
			return 0.0f;
		}
	}
	
	/// <summary>
    /// Gets the data in type bool.  GetBool(column[, row])
	/// </summary>
	/// <returns>
	/// The bool.
	/// </returns>
	/// <param name='args'>
	/// GetBool(column[, row])
	/// </param>
	public bool GetBool(params object[] args)
	{
		string str = GetStringData(args);
		bool ret;
		if(bool.TryParse(str, out ret))
		{
			return ret;
		}
		else
		{
			return false;
		}
	}
	
	
    /// <summary>
    /// Parse string to float
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
	float StringToFloat(string str)
	{
		float ret;
		if(float.TryParse(str, out ret))
		{
			return ret;
		}
		else
		{
			return 0;
		}
	}
	
	/// <summary>
    /// Gets the data in type Rect.  GetRect(column[, row])
	/// </summary>
	/// <returns>
	/// The rect.
	/// </returns>
	/// <param name='args'>
	/// GetRect(column[, row])
	/// </param>
	public Rect GetRect(params object[] args)
	{
		string str = GetStringData(args);
		Rect ret = new Rect(0, 0, 0, 0);
		string[] strs = str.Split(',');
		if(strs.Length >= 1)
			ret.x = StringToFloat(strs[0]);
		if(strs.Length >= 2)
			ret.y = StringToFloat(strs[1]);
		if(strs.Length >= 3)
			ret.width = StringToFloat(strs[2]);
		if(strs.Length >= 4)
			ret.height = StringToFloat(strs[3]);
		
		return ret;
	}
	
	/// <summary>
    /// Gets the data in type Vector2. GetVector2(column[, row])
	/// </summary>
	/// <returns>
	/// The vector2.
	/// </returns>
	/// <param name='args'>
    /// GetVector2(column[, row])
	/// </param>
	public Vector2 GetVector2(params object[] args)
	{
		string str = GetStringData(args);
		Vector2 ret = new Vector2(0, 0);
		string[] strs = str.Split(',');
		if(strs.Length >= 1)
			ret.x = StringToFloat(strs[0]);
		if(strs.Length >= 2)
			ret.y = StringToFloat(strs[1]);
		
		return ret;
	}
	
	
	/// <summary>
    /// Gets the data in type vector3. GetVector3(column[, row])
	/// </summary>
	/// <returns>
	/// The vector3.
	/// </returns>
	/// <param name='args'>
    /// GetVector3(column[, row])
	/// </param>
	public Vector3 GetVector3(params object[] args)
	{
		string str = GetStringData(args);
		Vector3 ret = new Vector3(0, 0, 0);
		string[] strs = str.Split(',');
		if(strs.Length >= 1)
			ret.x = StringToFloat(strs[0]);
		if(strs.Length >= 2)
			ret.y = StringToFloat(strs[1]);
		if(strs.Length >= 3)
			ret.z = StringToFloat(strs[2]);
		
		return ret;
	}
	
	
	/// <summary>
    /// Gets the data in type vector4. GetVector4(column[, row])
	/// </summary>
	/// <returns>
	/// The vector4.
	/// </returns>
	/// <param name='args'>
    /// GetVector4(column[, row])
	/// </param>
	public Vector4 GetVector4(params object[] args)
	{
		string str = GetStringData(args);
		Vector4 ret = new Vector4(0, 0, 0, 0);
		string[] strs = str.Split(',');
		if(strs.Length >= 1)
			ret.x = StringToFloat(strs[0]);
		if(strs.Length >= 2)
			ret.y = StringToFloat(strs[1]);
		if(strs.Length >= 3)
			ret.z = StringToFloat(strs[2]);
		if(strs.Length >= 4)
			ret.w = StringToFloat(strs[3]);
		
		return ret;
	}
	
	
	
	
	/// <summary>
    /// Sets the string data. SetStringData(setValue, column, row)
	/// </summary>
	/// <returns>
	/// The string data.
	/// </returns>
	/// <param name='stringValue'>
	/// If set to <c>true</c> string value.
	/// </param>
	/// <param name='args'>
    /// SetStringData(setValue, column, row)
	/// </param>
	public bool SetStringData(string stringValue, object[] args)
	{
		if(args.Length <= 0)
			return false;
		
		int column = -1;
		int row = -1;
		
		if(typeof(System.Int32) == args[0].GetType())
		{
			column = (int)args[0];
		}
		else if(typeof(System.String) == args[0].GetType())
		{
			column = GetColumn((string)args[0]);
		}
		else
		{
			column = Object2Int(args[0]);
		}
		
		if(args.Length >= 2)
		{
			if(typeof(System.Int32) == args[1].GetType())
			{
				row = (int)args[1];
			}
			else if(typeof(System.String) == args[1].GetType())
			{
				row = GetRow((string)args[1]);
			}
			else
			{
				row = Object2Int(args[1]);
			}
		}
		else
		{
			row = mCurrentRow;
		}
		
		if(column < _mColumnCount && column >= 0 && row < _mRowCount &&  row >= 0)
        {
            mData[column][row] = stringValue;
			return true;
        }
        return false;
		
	}
	
	/// <summary>
	/// Sets the string.
	/// </summary>
	/// <returns>
	/// The string.
	/// </returns>
	/// <param name='stringValue'>
	/// If set to <c>true</c> string value.
	/// </param>
	/// <param name='args'>
	/// SetString(stringValue, column [,row])
	/// </param>
	public bool SetString(string stringValue, params object[] args)
	{
		return SetStringData(stringValue, args);
	}
	
	/// <summary>
	/// Sets the int.
	/// </summary>
	/// <returns>
	/// The int.
	/// </returns>
	/// <param name='intValue'>
	/// If set to <c>true</c> int value.
	/// </param>
	/// <param name='args'>
	/// SetInt(intValue, column [,row])
	/// </param>
	public bool SetInt(int intValue, params object[] args)
	{
		return SetStringData(intValue.ToString(), args);
	}
	
	/// <summary>
	/// Sets the float.
	/// </summary>
	/// <returns>
	/// The float.
	/// </returns>
	/// <param name='floatValue'>
	/// If set to <c>true</c> float value.
	/// </param>
	/// <param name='args'>
	/// SetFloat(floatValue, column [,row])
	/// </param>
	public bool SetFloat(float floatValue, params object[] args)
	{
		return SetStringData(floatValue.ToString(), args);
	}
	
	/// <summary>
	/// Sets the bool.
	/// </summary>
	/// <returns>
	/// The bool.
	/// </returns>
	/// <param name='boolValue'>
	/// If set to <c>true</c> bool value.
	/// </param>
	/// <param name='args'>
	/// SetBool(boolValue, column [,row])
	/// </param>
	public bool SetBool(bool boolValue, params object[] args)
	{
		return SetStringData(boolValue.ToString(), args);
	}
	
	
	/// <summary>
	/// Sets the rect.
	/// </summary>
	/// <returns>
	/// The rect.
	/// </returns>
	/// <param name='rect'>
	/// If set to <c>true</c> rect.
	/// </param>
	/// <param name='args'>
	/// SetRect(rectValue, column [,row])
	/// </param>
	public bool SetRect(Rect rect, params object[] args)
	{
		string str = rect.x.ToString() + "," + rect.y.ToString() + "," + rect.width.ToString() + "," + rect.height.ToString();
		return SetStringData(str, args);
	}
	 
	/// <summary>
	/// Sets the vector2.
	/// </summary>
	/// <returns>
	/// The vector2.
	/// </returns>
	/// <param name='vec2'>
	/// If set to <c>true</c> vec2.
	/// </param>
	/// <param name='args'>
	/// SetRect(vc2, column [,row])
	/// </param>
	public bool SetVector2(Vector2 vec2, params object[] args)
	{
		string str = vec2.x.ToString() + "," + vec2.y.ToString();
		return SetStringData(str, args);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="vec3"></param>
	/// <param name="args">SetVector3(vec3, column, [row])</param>
	/// <returns></returns>
	public bool SetVector3(Vector3 vec3, params object[] args)
	{
		string str = vec3.x.ToString() + "," + vec3.y.ToString() + "," + vec3.z.ToString();
		return SetStringData(str, args);
	}
	
	/// <summary>
	/// Sets the vector4.
	/// </summary>
	/// <returns>
	/// The vector4.
	/// </returns>
	/// <param name='vec4'>
	/// If set to <c>true</c> vec4.
	/// </param>
	/// <param name='args'>
	/// SetVector4(vec4, column[, row])
	/// </param>
	public bool SetVector4(Vector4 vec4, params object[] args)
	{
		string str = vec4.x.ToString() + "," + vec4.y.ToString() + "," + vec4.z.ToString() + "," + vec4.w.ToString();
		return SetStringData(str, args);
	}
	
	
	
	
    /// <summary>
    /// Read form data by asset name in "Resources" folder.
    /// </summary>
    /// <param name="formPath"></param>
    /// <returns></returns>
    public static OOFormArray ReadFromResources(string formPath)
    { 
        TextAsset ta = (TextAsset)Resources.Load(formPath);
        string form_string = ta.text;

        return GetForm(form_string);
    }
	
	/// <summary>
	/// Read form data from TextAsset object.
	/// </summary>
	/// <returns>
	/// The form text asset.
	/// </returns>
	/// <param name='asset'>
	/// Asset.
	/// </param>
	public static OOFormArray ReadFromTextAsset(TextAsset asset)
	{
		if(asset != null)
		{
			return GetForm(asset.text);
		}
		return GetForm("");
	}
	
    /// <summary>
    /// Read form data form file formPath.
    /// </summary>
    /// <param name="formPath"></param>
    /// <returns></returns>
    public static OOFormArray ReadFromFile(string formPath)
    {
        string form_string = OOFormTools.ReadFileText(formPath);

        return GetForm(form_string);
    }
	
    /// <summary>
    /// Save form data to a file
    /// </summary>
    /// <param name="fileName"></param>
    public void SaveFormFile(string fileName)
    {
        OOFormTools.WriteFileText(fileName, ToString());
    }

    /// <summary>
    /// Read form data from json file
    /// </summary>
    /// <param name="formPath"></param>
    /// <returns></returns>
    public static OOFormArray ReadFromJsonFile(string formPath)
    {
        string form_string = OOFormTools.ReadFileText(formPath);

        return GetFormByJsonString(form_string);
    }

    /// <summary>
    /// Read form data from xml file
    /// </summary>
    /// <param name="formPath"></param>
    /// <returns></returns>
    public static OOFormArray ReadFromXMLFile(string formPath)
    {
        string form_string = OOFormTools.ReadFileText(formPath);

        return GetFormByXMLString(form_string);
    }

    /// <summary>
    /// Read form data from csv file
    /// </summary>
    /// <param name="formPath"></param>
    /// <returns></returns>
    public static OOFormArray ReadFormCSVFile(string formPath)
    {
        string form_string = OOFormTools.ReadFileText(formPath);

        return GetFormByCSVString(form_string);
    }

    /// <summary>
    /// Get OOFormArray from xml string;
    /// </summary>
    /// <param name="xmlString"></param>
    /// <returns></returns>
    public static OOFormArray GetFormByXMLString(string xmlString)
    {
        //if string with BOM,remove it.
        if ((int)xmlString[0] == 65279)
        {
            xmlString = xmlString.Substring(1);
        }

        OOFormArray form_array = new OOFormArray();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);

        form_array.InsertRow(0);
        form_array.InsertColumn(0);
        XmlNode node = xmlDoc.FirstChild;
        XmlNodeList component_list = node.ChildNodes;
        foreach (XmlNode row_node in component_list)
        {
            form_array.InsertRow(form_array.mRowCount);
            foreach (XmlAttribute attr in row_node.Attributes)
            {
                if (form_array.GetColumn(attr.Name) == -1)
                {
                    if (form_array.mColumnDic.Count <= 1)
                    {
                        form_array.SetString(attr.Name, 0, 0);
                        form_array.AddColumnName(attr.Name, 0);
                    }
                    else
                    {
                        form_array.InsertColumn(form_array.mColumnCount);
                        form_array.SetString(attr.Name, form_array.mColumnCount - 1, 0);
                        form_array.AddColumnName(attr.Name, form_array.mColumnCount - 1);
                    }
                }
                form_array.SetString(attr.Value.ToString(), form_array.GetColumn(attr.Name), form_array.mRowCount - 1);
            }
        }
        return form_array;
    }

    /// <summary>
    /// Get OOFormArray from json string.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static OOFormArray GetFormByJsonString(string jsonString)
    {
        OOFormArray form_array = new OOFormArray();
        List<Hashtable> table_list = OOLitJson.JsonMapper.ToObject<List<Hashtable>>(jsonString);

        form_array.InsertRow(0);
        form_array.InsertColumn(0);
        //int row = 0;
        foreach (Hashtable table in table_list)
        {
            form_array.InsertRow(form_array.mRowCount);
            foreach(string key_str in table.Keys)
            {
                if (form_array.GetColumn(key_str) == -1)
                {
                    if (form_array.mColumnDic.Count <= 1)
                    {
                        form_array.SetString(key_str, 0, 0);
                        form_array.AddColumnName(key_str,0);
                    }
                    else
                    {
                        form_array.InsertColumn(form_array.mColumnCount);
                        form_array.SetString(key_str, form_array.mColumnCount - 1, 0);
                        form_array.AddColumnName(key_str, form_array.mColumnCount - 1);
                    }
                }
                form_array.SetString(table[key_str].ToString(), form_array.GetColumn(key_str), form_array.mRowCount - 1);
            }
        }
        return form_array;
    }

    /// <summary>
    /// Get one row data from csv string
    /// </summary>
    /// <param name="csvLine"></param>
    /// <returns></returns>
    public static string[] GetCSVStringList(string csvLine)
    {
        List<string> string_list = new List<string>();
        string str = "";
        bool start_string = false;
        for (int i = 0; i < csvLine.Length; i++)
        {
            if (csvLine[i] == ',')
            {
                if (start_string == false)
                {
                    string_list.Add(str);
                    str = "";
                }
                else
                {
                    str += csvLine[i];
                }
            }
            else if (csvLine[i] == '\"')
            {
                if (i != csvLine.Length - 1)
                {
                    if (start_string == false)
                    {
                        start_string = true;
                    }
                    else
                    {
                        if (i + 1 <= csvLine.Length - 1)
                        {
                            if (csvLine[i + 1] == '\"')
                            {
                                str += '\"';
                                i++;
                            }
                            else if (csvLine[i + 1] == ',')
                            {
                                start_string = false;
                            }
                        }
                    }
                }
                else
                {
                    string_list.Add(str);
                    str = "";
                    start_string = false;
                }
            }
            else
            {
                str += csvLine[i];
            }
        }

        if (str == "")
        {
            if (csvLine.Length > 0 && csvLine[csvLine.Length - 1] == ',')
            {
                string_list.Add("");
            }
        }
        else
        {
            string_list.Add(str);
        }
        return string_list.ToArray();
    }


    /// <summary>
    /// Get OOFormArray from CSV string
    /// </summary>
    /// <param name="formString"></param>
    /// <returns></returns>
    public static OOFormArray GetFormByCSVString(string formString)
    {
        OOFormArray form_array = new OOFormArray();
        string[] string_rows = formString.Split('\n');

        int column_count = 0;
        int row_count = string_rows.Length;

        if (row_count > 0)
        {
            string[] string_firstRow = GetCSVStringList(string_rows[0]);

            column_count = string_firstRow.Length;
            form_array.InsertRow(0);
            for (int i = 0; i < column_count; i++)
            {
                form_array.InsertColumn(i);
                form_array.mData[i][0] = string_firstRow[i].Replace("\r", "");
            }

            for (int j = 1; j < row_count; j++)
            {
                string[] string_oneRow = GetCSVStringList(string_rows[j]);
                int nod_count = Mathf.Min(column_count, string_oneRow.Length);

                form_array.InsertRow(j);
                for (int i = 0; i < nod_count; i++)
                {
                    form_array.mData[i][j] = string_oneRow[i].Replace("\r", "");
                }
            }
        }

        return form_array;
    }

    /// <summary>
    /// Convert text to a OOFormArray
    /// </summary>
    /// <param name="formString"></param>
    /// <returns></returns>
    public static OOFormArray GetForm(string formString)
    {
        OOFormArray form_array = new OOFormArray();
        string[] string_rows = formString.Split('\n');

        int column_count = 0;
        int row_count = string_rows.Length;

        if (row_count > 0)
        {
            string[] string_firstRow = string_rows[0].Split('\t');

            
            column_count = string_firstRow.Length;
            form_array.InsertRow(0);
            for (int i = 0; i < column_count; i++)
            {
                form_array.InsertColumn(i);
                form_array.mData[i][0] = string_firstRow[i].Replace("\r", "");
            }

            for (int j = 1; j < row_count; j++)
            {
                string[] string_oneRow = string_rows[j].Split('\t');
                int nod_count = Mathf.Min(column_count, string_oneRow.Length);

                form_array.InsertRow(j);
                for (int i = 0; i < nod_count; i++)
                {
                    form_array.mData[i][j] = string_oneRow[i].Replace("\r", "");
                }
            }
        }
        
        return form_array;
    }

	/// <summary>
	/// Convert table data to string.
	/// </summary>
	/// <returns></returns>
	public override string ToString ()
	{
		string str = "";
        for (int i = 0; i < _mRowCount; i++)
        {
            for (int j = 0; j < _mColumnCount; j++)
            {
                str = str + mData[j][i] + '\t';
            }
            str = str.Substring(0, str.Length - 1);

            str = str + "\r\n";
        }
        if (str.Length > 0)
        {
            str = str.Substring(0, str.Length - 1);
        }
		return str;
	}

    /// <summary>
    /// Export OOFormArray to json string
    /// </summary>
    /// <returns></returns>
    public string ToJsonString()
    {
        string ret = "{}";
        List<Hashtable> has_list = new List<Hashtable>();
        for (int j = 1; j < mRowCount; j++)
        {
            Hashtable hash = new Hashtable();
            for (int i = 0; i < mColumnCount; i++)
            {
                string key_string = mData[i][0];
                string add_key = key_string.Split(',')[0];
                if (key_string.Contains("[float]"))
                {
                    hash[add_key] = GetFloat(i, j);
                }
                else if (key_string.Contains("[int]"))
                {
                    hash[add_key] = GetInt(i, j);
                }
                else if (key_string.Contains("[bool]"))
                {
                    hash[add_key] = GetBool(i, j);
                }
                else
                {
                    hash[add_key] = GetString(i, j);
                }
            }
            has_list.Add(hash);
        }
        ret = OOLitJson.JsonMapper.ToJson(has_list);
        return ret;
    }

    /// <summary>
    /// Export OOFormArray to XML string
    /// </summary>
    /// <returns></returns>
    public string ToXMLString()
    {
        string ret = "";
        XmlDocument doc = new XmlDocument();

        XmlElement root = doc.CreateElement("Root");
        doc.AppendChild(root);

        for (int j = 1; j < mRowCount; j++)
        {
            XmlElement row = doc.CreateElement("Data");
            for (int i = 0; i < mColumnCount; i++)
            {
                string key_string = mData[i][0];
                if (key_string == "")
                    continue;
                string add_key = key_string.Split(',')[0];

                row.SetAttribute(add_key, GetString(i,j));
            }
            root.AppendChild(row);
        }
        ret = doc.InnerXml;
        return ret;
    }

    /// <summary>
    /// Export OOFormArray to CSV string
    /// </summary>
    /// <returns></returns>
    public string ToCSVString()
    {
        string ret = "";
        for (int j = 0; j < mRowCount; j++)
        {
            for (int i = 0; i < mColumnCount; i++)
            {
                string data_node = GetString(i, j);
                data_node = data_node.Replace("\"", "\"\"");
                if (data_node.Contains(","))
                {
                    data_node = "\"" + data_node + "\"";
                }
                ret = ret + data_node + ',';
            }
            ret = ret.Substring(0, ret.Length - 1);
            ret = ret + "\r\n";
        }

        ret = ret.Substring(0, ret.Length - 1);

        return ret;
    }

    /// <summary>
    /// Get a row data with json format.
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    string GetJsonString(int rowIndex)
    {
        if (rowIndex >= mRowCount && rowIndex < 1)
        {
            return "{}";
        }

        string ret = "";
        Hashtable hash = new Hashtable();
        for (int i = 0; i < mColumnCount; i++)
        {
            string key_string =  mData[i][0];
            string add_key = key_string.Split(',')[0];
            if ( key_string.Contains("[float]"))
            {
                hash[add_key] = GetFloat(i, rowIndex);
            }
            else if (key_string.Contains("[int]"))
            {
                hash[add_key] = GetInt(i, rowIndex);
            }
            else if (key_string.Contains("[bool]"))
            {
                hash[add_key] = GetBool(i, rowIndex);
            }
            else
            {
                hash[add_key] = GetString(i, rowIndex);
            }
            
        }
        ret = OOLitJson.JsonMapper.ToJson(hash);
        return ret;
    }

    /// <summary>
    /// Get one row data as T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public T GetObject<T>(int rowIndex)
    {
        return OOLitJson.JsonMapper.ToObject<T>(GetJsonString(rowIndex));
    }


    
}