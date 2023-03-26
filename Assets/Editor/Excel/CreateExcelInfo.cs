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

    private static int dataRow = 3;//������ʵ�������ڵĿ�ʼ�У�ǰ�漸�ж��ǹ����У���������
    private static string BinaryFilePath= ExcelReaderManager.BinaryFile_Path;//���ɵĶ������ļ����·��
    private static string fieldClassPath = ExcelReaderManager.fieldClassPath;//���ɵ��ֶ��ļ����·��
    private static string dicClassPath = ExcelReaderManager.dicClassPath;//���ɵ��ֵ��ļ����·��
    private static string ExcelFilePath = ExcelReaderManager.ExcelFilePath;//excel�ļ����·��
    private static Dictionary<string, string> sheetRepeatName = new Dictionary<string, string>();//keyΪ������valueΪExcel�ļ���+���� �����жϱ����Ƿ��ظ�
    /// <summary>
    /// ��ȡExcel���е�����  ����3���ļ�
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
            using (FileStream fs = files[i].Open(FileMode.Open,FileAccess.Read))//��ȡ��i���ļ������б������ �̶�д��
            {
                IExcelDataReader excelDataReader;
                if (files[i].Extension == ".xlsx")
                {
                    excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);//��ȡ.xlsx
                }else
                excelDataReader = ExcelReaderFactory.CreateBinaryReader(fs);//��ȡ.xls
                dataTable = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }
            for (int j = 0; j < dataTable.Count; j++)//���ļ�����Count�ű�,ÿ�ű����ɶ�Ӧ��3���ļ�
            {
                string fileName = null;//Excel�ļ���
                fileName = GetFileNameNoExtention(files[i]);
                if (sheetRepeatName.ContainsKey(dataTable[j].TableName))//�жϱ����Ƿ��ظ�
                {
                    if (!(sheetRepeatName[dataTable[j].TableName] == fileName + dataTable[j].TableName))//��ǰ����ı������ļ�������Ѿ������ֵ���  ˵�����޸ľɱ� ������������
                    {
                        Debug.LogError("�����ظ����޸�!:" + dataTable[j].TableName + " ���ظ�����λ��Excel�ļ�:" + fileName);
                        continue;
                    }
                }
                if(!sheetRepeatName.ContainsKey(dataTable[j].TableName))
                {
                    sheetRepeatName.Add(dataTable[j].TableName, fileName + dataTable[j].TableName);//��������ı������ļ���+���� ���ȥ �����ж��ظ�����
                }                
                GenateBinaryFile(dataTable[j]);
                GenateFieldFile(dataTable[j]);
                GenateDicFilePath(dataTable[j]);          
            }
        }
    }/// <summary>
     /// ����Excel���Ӧ��2��������
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
    /// ����Excel���Ӧ���ֶ���
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
        AssetDatabase.Refresh();//ˢ��ProjecetĿ¼
    }
    /// <summary>
    /// ����Excel���Ӧ���ֵ���
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
