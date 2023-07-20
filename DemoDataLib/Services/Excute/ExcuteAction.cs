using DemoDataLib.Core;
using PgPointLibrary.Global.ExcuteLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Services.Excute
{
    public class ExcuteAction<TApply> : ExcuteGeneric<TApply>
    {
        private Action<ExcuteBase> Action { get; set; }

        /// <summary>
        /// 執行動作，此動作會自動關閉連線
        /// </summary>
        /// <param name="action">動作定義</param>
        /// <param name="setting">執行設定</param>
        public ExcuteAction(Action<ExcuteBase> action, Setting setting = null)
        {
            Init();

            Action = action;
            SetMySetting(setting);
            setting.IsAutoCloseConn = true;

            SetMyConnection(new Connection(new SqlConnection(SettingContent.SqlConnectionString)));

            AutoHandle();
        }

        /// <summary>
        /// 執行動作(使用外部連線)
        /// </summary>
        /// <param name="action">動作定義</param>
        /// <param name="conn">外部連線</param>
        /// <param name="setting">執行設定</param>
        public ExcuteAction(Action<ExcuteBase> action, SqlConnection conn, Setting setting = null)
        {
            Init();

            Action = action;
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
        public ExcuteAction(Action<ExcuteBase> action, SqlTransaction trans, Setting setting = null)
        {
            Init();

            Action = action;
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

        public ExcuteAction(Action<ExcuteBase> action, Connection connection, Setting setting = null)
        {
            Init();

            Action = action;
            SetMySetting(setting);
            SetMyConnection(connection);

            AutoHandle();
        }

        protected override void HandleHelper()
        {
            Action(this);
        }



    }
}
