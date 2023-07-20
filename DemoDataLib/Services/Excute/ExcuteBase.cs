using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PgPointLibrary.Global;
using Newtonsoft.Json.Linq;
using System.Collections;
using DemoDataLib.Core;
using DemoDataLib.Global;

namespace PgPointLibrary.Global.ExcuteLibrary
{
    public abstract class ExcuteBase
    {
        /// <summary>
        /// 執行結果
        /// </summary>
        public ProcessResult DoResult { get; protected set; }

        /// <summary>
        /// 連線相關物件
        /// </summary>
        [JsonIgnore]
        public Connection SqlDatabase { get; protected set; }

        /// <summary>
        /// 執行設定物件
        /// </summary>
        public Setting SettingContent { get; set; }

        /// <summary>
        /// 檔案儲存相關路徑
        /// </summary>
        public FilePath FilePathContent { get; protected set; }

        /// <summary>
        /// 預設執行設定
        /// </summary>
        public static readonly Setting DefaultSetting = new Setting
        {
            IsAutoCloseConn = false,
            IsAutoGetStackTrace = false,
            SqlConnectionString = DemoDataLib.Properties.Resources.DefaultConnectString,
            IsOpenSqlConnection = true,
            IsBeginSqlTransaction = true,
            IsAutoHandle = true,
            LogAllEnabled = false
        };

        private bool IsSelfTrans = false;

        protected void Handle()
        {
            DoResult = new ProcessResult();
            bool hasError = false;
            try
            {
                HandleHelper();

                DoResult.Status = Statuses.Success;

                if (IsSelfTrans)
                {
                    SqlDatabase.Trans.Commit();
                }
            }
            catch(SqlException ex)
            {
                if (ex.Number == -2)
                {
                    DoResult.Status = Statuses.SqlTimeOut;
                }
                else
                {
                    DoResult.Status = Statuses.SqlError;
                }
            }
            catch(CustomException ex)
            {
                DoResult.Status = Statuses.KnowError;
            }

            catch (Exception ex)
            {
                DoResult.Status = Statuses.UnknowError;
                DoResult.Exception = ex;

                hasError = true;
            }
            finally
            {
                if ((SettingContent.IsAutoCloseConn && IsSelfTrans))
                {
                    if (SqlDatabase.Conn.State == System.Data.ConnectionState.Open)
                    {
                        SqlDatabase.Conn.Close();
                    }
                }


                if (hasError)
                {
                    LogSave(FilePathContent.ErrorPath);
                }
                else
                {
                    if (SettingContent.LogAllEnabled)
                    {
                        LogSave(FilePathContent.GeneralLogPath);
                    }
                }
            }
        }

        protected abstract void HandleHelper();

        protected virtual void GeneratePath()
        {
            FilePathContent = new FilePath();

            string path_base = @$"c:\{Assembly.GetCallingAssembly().GetName().Name}";

            FilePathContent.ErrorPath = $@"{path_base}\{DemoDataLib.Properties.Resources.ErrorPath}\default.txt";
            FilePathContent.GeneralLogPath = $@"{path_base}\{DemoDataLib.Properties.Resources.GeneralLogPath}\default.txt";
        }


        protected void SetMySetting(Setting setting)
        {
            if (setting == null)
            {
                SettingContent = DefaultSetting;
            }
            else
            {
                SettingContent = setting;
            }
        }

        protected void SetMyConnection(Connection connection)
        {

            if (connection == null)
            {
                connection = new Connection();
            }

            if (connection.Trans == null)
            {
                if (connection.Conn == null)
                {
                    connection.Conn = new SqlConnection(SettingContent.SqlConnectionString);
                    SettingContent.IsAutoCloseConn = true;
                }

                if (SettingContent.IsOpenSqlConnection)
                {
                    connection.Conn.Open();
                    if (SettingContent.IsBeginSqlTransaction)
                    {
                        connection.Trans = connection.Conn.BeginTransaction();
                    }
                }

                IsSelfTrans = true;
            }

            SqlDatabase = connection;
        }

        public enum Statuses
        {
            Success,
            UnknowError,
            /// <summary>
            /// SQL錯誤
            /// </summary>
            SqlError,
            /// <summary>
            /// SQL Time Out
            /// </summary>
            SqlTimeOut,
            /// <summary>
            /// 已知錯誤(需設定CustomException)
            /// </summary>
            KnowError
        }

        public class Setting
        {
            /// <summary>
            /// 是否自動取得錯誤追蹤(只有在有例外狀況時才會生效)
            /// 預設為false
            /// </summary>
            public bool IsAutoGetStackTrace { get; set; }
            /// <summary>
            /// 指定錯誤LOG路徑(未指定的話會使用預設錯誤LOG路徑)
            /// </summary>
            public string ErrorLogPath { get; set; }
            /// <summary>
            /// 指定一般LOG路徑(未指定的話會使用預設錯誤LOG路徑)
            /// 當LogAllEnabled=true時，未發生錯誤的LOG會儲存到這個路徑
            /// </summary>
            public string GeneralLogPath { get; set; }
            /// <summary>
            /// 是否自動關閉連線(外部交易也可開啟，但注意設定之後會在該動作之後關閉連線)
            /// 預設為false
            /// </summary>
            public bool IsAutoCloseConn { get; set; }
            /// <summary>
            /// 指定連線字串(未設定會使用Define內的設定)
            /// </summary>
            public string SqlConnectionString { get; set; }
            /// <summary>
            /// 是否開啟連線(使用外部交易時，此設定無效)
            /// 預設為true
            /// </summary>
            public bool IsOpenSqlConnection { get; set; }
            /// <summary>
            /// 是否開啟交易(使用外部交易時，此設定無效，IsOpenSqlConnection=true才會生效)
            /// 預設為true
            /// </summary>
            public bool IsBeginSqlTransaction { get; set; }
            /// <summary>
            /// 是否自動執行
            /// 預設為true
            /// </summary>
            public bool IsAutoHandle { get; set; }

            /// <summary>
            /// 是否在所有情況下都記錄LOG
            /// true:任何情況都會記錄
            /// false:只在有例外狀況時才會記錄
            /// 預設為false
            /// </summary>
            public bool LogAllEnabled { get; set; }

        }

        public class ProcessResult
        {
            public Statuses Status { get; set; }

            public Exception Exception { get; set; }

            public StackTrace StackTrace { get; set; }
        }

        public class FilePath
        {
            public string ErrorPath { get; set; }
            public string GeneralLogPath { get; set; }
        }

        private void LogSave(string path)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new DynamicParametersConverter());
            string saveString = JsonConvert.SerializeObject(this, settings);

            //SaveToFile
        }
    }
}
