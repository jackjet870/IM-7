﻿using System;
using System.Configuration;
using System.Reflection;

namespace IM.DAL.DataAccessFactory
{
    /// <summary>
    /// 抽象工厂模式创建DAL。
    /// web.config 需要加入配置：(利用工厂模式+反射机制+缓存机制,实现动态创建不同的数据层对象接口)  
    /// DataCache类在导出代码的文件夹里
    /// <appSettings>  
    /// <add key="DAL" value="SQLServerDAL" /> (这里的命名空间根据实际情况更改为自己项目的命名空间)
    /// </appSettings> 
    /// </summary>
    public sealed class DataAccessFactory
    {
        #region 属性

        /// <summary>
        /// 调用类命名空间
        /// </summary>
        private static readonly string DLLPATH = "IM.DAL.";

        /// <summary>
        /// 数据库类型
        /// </summary>
        private static readonly string DBTYPE = ConfigurationManager.AppSettings["DALTYPE"];

        #endregion 属性

        #region 默认方法
        /// <summary>
        /// 创建对象或从缓存获取
        /// </summary>
        public static object CreateObject(string classNamespace)
        {
            object objType = DataCache.GetCache(classNamespace);//从缓存读取
            if (objType == null)
            {
                try
                {
                    objType = Assembly.Load(DLLPATH + DBTYPE).CreateInstance(classNamespace);//反射创建
                    DataCache.SetCache(classNamespace, objType);// 写入缓存
                }
                catch (Exception er)
                {
                    throw er;
                }
            }
            return objType;
        }
        #endregion 默认方法

        #region 功能：创建接口通用方法（接口名称必须等于“I”+ 数据库实现层名称） 
        /// <summary>
        /// 功能：创建接口通用方法（接口名称必须等于“I”+ 数据库实现层名称） 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInterface<T>()
        {
            string ClassNamespace = DLLPATH + DBTYPE + "." + typeof(T).Name.Substring(1);
            object objType = CreateObject(ClassNamespace);
            return (T)objType;
        } 
        #endregion

        #region 自定义接口

        /// <summary>
        /// 接口IUser>
        /// <returns></returns>
        public static IM.IDAL.IUser CreateIUser()
        {
            string ClassNamespace = DLLPATH + DBTYPE + ".User";
            object objType = CreateObject(ClassNamespace);
            return (IM.IDAL.IUser)objType;
        }

        /// <summary>
        /// 接口IDTOUser>
        /// <returns></returns>
        public static IM.IDAL.IDTOUser CreateIDTOUser()
        {
            string ClassNamespace = DLLPATH + DBTYPE + ".DTOUser";
            object objType = CreateObject(ClassNamespace);
            return (IM.IDAL.IDTOUser)objType;
        } 
        #endregion
    }
}