using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ExcelReaderManager:BaseManager<ExcelReaderManager>
{
    public static string BinaryFile_Path = Application.streamingAssetsPath + "/BinaryFile/";
    public static string fieldClassPath = Application.dataPath + "/Scripts/ExcelData/fieldClass/";//生成的字段文件存放路径
    public static string dicClassPath = Application.dataPath + "/Scripts/ExcelData/dicClass/";//生成的字典文件存放路径
    public static string ExcelFilePath = Application.dataPath + "/Editor/Excel/ExcelFile/";//excel文件存放路径

    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    public  ExcelReaderManager()
    {
        InitDate();
    }
    public void InitDate()
    {
        if (!Directory.Exists(dicClassPath))
        {
            Directory.CreateDirectory(dicClassPath);
        }

        DirectoryInfo directory = new DirectoryInfo(dicClassPath);
        FileInfo[] files = directory.GetFiles();
        string fileName;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".cs")
            {
                fileName = files[i].Name.Remove(files[i].Name.Length - 3);
                Type dicClass = Type.GetType(fileName);
                fileName += "fieldClass";
                Type fieldClass = Type.GetType(fileName);
                LoadTable(dicClass, fieldClass);
            }
        }
        
    }
    /// <summary>
    /// 加载Excel表的2进制数据到字典中
    /// </summary>
    /// <typeparam name="dicClass">字典类</typeparam>
    /// <typeparam name="fieldClass">字段类</typeparam>
    private void LoadTable(Type dicClass, Type FieldClass)
    {
        //读取 excel表对应的2进制文件 来进行解析
        using (FileStream fs=new FileStream(BinaryFile_Path+dicClass.Name+".CP3",FileMode.Open,FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();

            int index = 0;
            int count = BitConverter.ToInt32(bytes,index);//表中总共count行数据
            index += 4;

            int keyLength = BitConverter.ToInt32(bytes, index);//key的长度
            index += 4;

            string keyName = Encoding.UTF8.GetString(bytes,index, keyLength);//读取key本身
            index += keyLength;

            Type fieldType = FieldClass;
            FieldInfo[] fieldInfos = fieldType.GetFields();//拿到字段类中的字段信息

            Type dicType = dicClass;
            object dicObj = Activator.CreateInstance(dicType);//实例化字典类对象 

            for (int i = 0; i < count; i++)
            {
                object fieldObj = Activator.CreateInstance(fieldType);
                foreach (FieldInfo field in fieldInfos)
                {
                    if (field.FieldType == typeof(int))
                    {
                        //相当于就是把2进制数据转为int 然后赋值给了对应的字段
                        field.SetValue(fieldObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(fieldObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(fieldObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        //读取字符串字节数组的长度
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        field.SetValue(fieldObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }                    
                }
                object dic = dicType.GetField("dataDic").GetValue(dicObj);
                MethodInfo method = dic.GetType().GetMethod("Add");
                object keyValue = fieldType.GetField(keyName).GetValue(fieldObj);
                method.Invoke(dic, new object[] { keyValue, fieldObj });
            }
            tableDic.Add(dicClass.Name,dicObj);
            fs.Close();
        }
    }
    /// <summary>
    /// 得到表的数据
    /// </summary>
    /// <typeparam name="T">表名</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T:class
    {
       string tableName= typeof(T).Name;
       if(tableDic.ContainsKey(tableName))
       {
           return tableDic[tableName] as T;
       }
        return default(T);
    }
}
