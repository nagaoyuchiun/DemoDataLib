using Newtonsoft.Json;
using System.Data.SqlClient;

namespace DemoDataLibTest
{
    [TestClass]
    public class OrderServiceTest
    {
        [TestMethod]
        public void GetFullDatasTest()
        {
            //建立連線
            SqlConnection conn;
            using (conn = new SqlConnection("Data Source=localhost;Initial Catalog=DemoDb;Persist Security Info=True;User ID=sa;Password=aa1111;MultipleActiveResultSets=True"))
            {
                conn.Open();

                //開啟交易
                SqlTransaction trans = conn.BeginTransaction();

                //宣告Services並且套入交易
                DemoDataLib.Services.OrderSevices service = new DemoDataLib.Services.OrderSevices(new DemoDataLib.Core.Connection(trans));

                //執行動作
                var actionResp = service.QueryOrderWithAllInfo(1);

                //將動作結果印出
                Console.WriteLine(JsonConvert.SerializeObject(actionResp, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));

                trans.Rollback();

                //判斷動作是否成功
                if (actionResp.DoResult.Status == PgPointLibrary.Global.ExcuteLibrary.ExcuteBase.Statuses.Success)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    Assert.Fail();
                }

            }

        }
    }
}
