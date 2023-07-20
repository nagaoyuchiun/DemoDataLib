using PgPointLibrary.Global.ExcuteLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Services.Excute
{
    public abstract class ExcuteGeneric<TApply> : ExcuteBase
    {
        protected void Init()
        {
            SettingContent = DefaultSetting;
            GenerateLogPath();
        }

        protected void GenerateLogPath()
        {
            GeneratePath();

            if (!string.IsNullOrEmpty(SettingContent.ErrorLogPath))
                FilePathContent.ErrorPath = SettingContent.ErrorLogPath;

            if (!string.IsNullOrEmpty(SettingContent.GeneralLogPath))
                FilePathContent.GeneralLogPath = SettingContent.GeneralLogPath;
        }

        protected override void GeneratePath()
        {
            string tName = typeof(TApply).FullName;
            FilePathContent = new FilePath();

            string path_base = @$"c:\{Assembly.GetCallingAssembly().GetName().Name}";

            FilePathContent.ErrorPath = $@"{path_base}\{DemoDataLib.Properties.Resources.ErrorPath}\{tName}.txt";
            FilePathContent.GeneralLogPath = $@"{path_base}\{DemoDataLib.Properties.Resources.GeneralLogPath}\{tName}.txt";
        }

        protected void AutoHandle()
        {
            if (SettingContent.IsAutoHandle)
            {
                Handle();
            }
        }

    }
}
