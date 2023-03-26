using Excel;
using PlasticGui.WorkspaceWindow.Home;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateExcelInfo 
{

    private static int dataRow = 3;//表中真实数据所在的开始行，前面几行都是规则行，不是数据
    private static string BinaryFilePath= ExcelReaderManager.BinaryFile_Path;//生成的二进制文件存放路径
    private static string fieldClassPath = ExcelReaderManager.fieldClassPath;//生成的字段文件存放路径
    private static string dicClassPath = ExcelReaderManager.dicClassPath;//生成的字典文件存放路径
    private static string ExcelFilePath = ExcelReaderManager.ExcelFilePath;//excel文件存放路径
    private static Dictionary<string, string> sheetRepeatName = new Dictionary<string, string>();//key为表名，value为Excel文件名+表名 用来判断表名是否重复
    /// <summary>
    /// 读取Excel表中的数据  生成3个文件
    /// </summary>
    [MenuItem("Tool/CreateExcelInfo")]
    private static void ReadExcelInfo()
    {
        DirectoryInfo directory = Directory.CreateDirectory(ExcelFilePath);
        FileInfo[] files = directory.GetFiles();
        DataTableCollection dataTable;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension!=".xlsx"&& files[i].Extension != ".xls")
            {
                continue;
            }
            using (FileStream fs = files[i].Open(FileMode.Open,FileAccess.Read))//读取第i个文件中所有表的数据 固定写法
            {
                IExcelDataReader excelDataReader;
                if (files[i].Extension == ".xlsx")
                {
                    excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);//读取.xlsx
                }else
                excelDataReader = ExcelReaderFactory.CreateBinaryReader(fs);//读取.xls
                dataTable = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }
            for (int j = 0; j < dataTable.Count; j++)//该文件中有Count张表,每张表生成对应的3个文件
            {
                string fileName = null;//Excel文件名
                fileName = GetFileNameNoExtention(files[i]);
                if (sheetRepeatName.ContainsKey(dataTable[j].TableName))//判断表名是否重复
                {
                    if (!(sheetRepeatName[dataTable[j].TableName] == fileName + dataTable[j].TableName))//当前处理的表名和文件名如果已经存在字典中  说明是修改旧表 而不是新增表
                    {
                        Debug.LogError("表名重复请修改!:" + dataTable[j].TableName + " 该重复表名位于Excel文件:" + fileName);
                        continue;
                    }
                }
                if(!sheetRepeatName.ContainsKey(dataTable[j].TableName))
                {
                    sheetRepeatName.Add(dataTable[j].TableName, fileName + dataTable[j].TableName);//把新增表的表名和文件名+表名 存进去 方便判断重复表名
                }                
                GenateBinaryFile(dataTable[j]);
                GenateFieldFile(dataTable[j]);
                GenateDicFilePath(dataTable[j]);          
            }
        }
    }/// <summary>
     /// 生成Excel表对应的2进制数据
     /// </summary>
     /// <param name="table"></param>
    private static void GenateBinaryFile(DataTable table)
    {
        if (!Directory.Exists(BinaryFilePath))
        {
            Directory.CreateDirectory(BinaryFilePath);
        }
        using (FileStream fs=new FileStream(BinaryFilePath+table.TableName+".CP3",FileMode.OpenOrCreate,FileAccess.Write))
        {
            fs.Write(BitConverter.GetBytes(table.Rows.Count- dataRow),0,4);
            string keyName = GetVariableNameRow(table)[GetkeyColumn(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyName);
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes,0, bytes.Length);
            DataRow row;
            DataRow rowType=GetVariableTypeRow(table);
            for (int i = dataRow; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {                   
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            if (row[j].ToString() == "")
                                fs.Write(BitConverter.GetBytes(default(int)), 0, 4);
                            else
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())),0,4);
                            break;
                        case "float":
                            if (row[j].ToString() == "")
                                fs.Write(BitConverter.GetBytes(default(float)), 0, 4);
                            else
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            if (row[j].ToString() == "")
                                fs.Write(BitConverter.GetBytes(default(bool)), 0, 4);
                            else
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                                byte[] bytesStr = Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytesStr.Length), 0, 4);
                                fs.Write(bytesStr, 0, bytesStr.Length);                           
                            break;                           
                    }
                }
            }
            fs.Close();
        }
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 生成Excel表对应的字段类
    /// </summary>
    /// <param name="table"></param>
    private static void GenateFieldFile(DataTable table)
    {
        if(!Directory.Exists(fieldClassPath))
        {
           Directory.CreateDirectory(fieldClassPath);
        }
        string str = "public class " + table.TableName + "fieldClass\n{\n";
        DataRow rowName = GetVariableNameRow(table);
        DataRow rowType = GetVariableTypeRow(table);
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "    public " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
        }
        str += "}";
        File.WriteAllText(fieldClassPath+table.TableName+ "fieldClass.cs", str);
        AssetDatabase.Refresh();//刷新Projecet目录
    }
    /// <summary>
    /// 生成Excel表对应的字典类
    /// </summary>
    /// <param name="table"></param>    
    private static void GenateDicFilePath(DataTable table)
    {
        int keyColumn = GetkeyColumn(table);
        DataRow rowType = GetVariableTypeRow(table);
        if (!Directory.Exists(dicClassPath))
        {
            Directory.CreateDirectory(dicClassPath);
        }
        string str = "using System.Collections.Generic;\n";
        str +="public class " + table.TableName + "\n{\n";
        str +="    public Dictionary<" + rowType[keyColumn].ToString() + "," + table.TableName+ "fieldClass" + "> dataDic = new Dictionary<" + rowType[keyColumn].ToString() + "," + table.TableName + "fieldClass" + ">();\n";
        str += "}";
        File.WriteAllText(dicClassPath+table.TableName+".cs",str);
        AssetDatabase.Refresh();
}
    private static DataRow GetVariableNameRow(DataTable table)
    {
       return  table.Rows[0];
    }
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        return table.Rows[1];
    }
    private static int GetkeyColumn(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString()=="key")
            {
                return i;
            }
        }
        return 0;
    }
    private static string GetFileNameNoExtention(FileInfo file)
    {
        if (file.Extension.Length == 5)
        {
            return file.Name.Remove(file.Name.Length - 5);
        }
        else
        {
            return  file.Name.Remove(file.Name.Length - 4);
        }
    }
}
