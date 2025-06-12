using System.Linq;

namespace CDTN.Models
{
    public static class ThongBaoHelper
    {
        public static int LaySoLuongChuaXem()
        {
            using (var db = new storeDB())
            {
                return db.ThongBao.Count(t => !t.DaXem);
            }
        }
    }
}
