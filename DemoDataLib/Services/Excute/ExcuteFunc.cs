using DemoDataLib.Core;
using Newtonsoft.Json;
using PgPointLibrary.Global.ExcuteLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Services.Excute
{
    public class ExcuteFunc<TApply, TReturn> : ExcuteGeneric<TApply>
    {
        public TReturn ResultData { get; private set; }

        /// <summary>
        /// 執行動作定義
        /// </summary>
        private Func<ExcuteBase, TReturn> Func { get; set; }

        /// <summary>
        /// 執行動作，此動作會自動開啟和關閉連線
        /// <param name="action">動作定義</param>
        /// <param name="setting">執行設定</param>
        public ExcuteFunc(Func<ExcuteBase, TReturn> func, Setting setting = null)
        {
            Init();

            Func = func;
            SetMySetting(setting);
            SettingContent.IsAutoCloseConn = true;

            SetMyConnection(new Connection(new SqlConnection(SettingContent.SqlConnectionString)));

            AutoHandle();
        }

        /// <summary>
        /// 執行動作(使用外部連線)
        /// </summary>
        /// <param name="action">動作定義</param>
        /// <param name="conn">外部連線</param>
        /// <param name="setting">執行設定</param>
        public ExcuteFunc(Func<ExcuteBase, TReturn> func, SqlConnection conn, Setting setting = null)
        {
            Init();

            Func = func;
            SetMySetting(setting);

            SetMyConnection(new Connection(conn));

            AutoHandle();
        }

        /// <summary>
        /// 執行動作(使用外部交易)
        /// </summary>
        /// <param name="action">動作定義</param>
        /// <param name="trans">外部交易</param>
        /// <param name="setting">執行設定</param>
        public ExcuteFunc(Func<ExcuteBase, TReturn> func, SqlTransaction trans, Setting setting = null)
        {
            Init();

            Func = func;
            SetMySetting(setting);

            SetMyConnection(new Connection(trans));


            AutoHandle();
        }

        /// <summary>
        /// 執行動作(不指定連線方式)
        /// </summary>
        /// <param name="func">執行函式</param>
        /// <param name="connection">連線物件</param>
        /// <param name="setting">設定檔</param>

        public ExcuteFunc(Func<ExcuteBase, TReturn> func, Connection connection, Setting setting = null)
        {
            Init();

            Func = func;
            SetMySetting(setting);
            SetMyConnection(connection);

            AutoHandle();
        }

        protected override void HandleHelper()
        {
            ResultData = Func(this);
        }
        /// <summary>
        /// 執行Function
        /// </summary>
    }

}
