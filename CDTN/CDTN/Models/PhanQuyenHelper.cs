using System.Collections.Generic;
using System.Linq;
using CDTN.Models;

namespace CDTN.Helpers
{
    public static class PhanQuyenHelper
    {
        private static storeDB db = new storeDB();

        // Lấy danh sách mã quyền của một tài khoản quản trị
        public static List<int> LayDanhSachMaQuyen(int idTaiKhoan)
        {
            return db.PhanQuyen
                .Where(pq => pq.IDTaiKhoan == idTaiKhoan)
                .Select(pq => pq.MaQuyen)
                .ToList();
        }

        // Kiểm tra xem một tài khoản có quyền cụ thể hay không
        public static bool CoQuyen(int idTaiKhoan, string tenQuyen)
        {
            return db.PhanQuyen
                .Any(pq => pq.IDTaiKhoan == idTaiKhoan && pq.Quyen.TenQuyen == tenQuyen);
        }

        // Gán danh sách quyền cho tài khoản (xóa quyền cũ trước khi thêm mới)
        public static void GanQuyen(int idTaiKhoan, List<int> danhSachQuyenMoi)
        {
            var quyenCu = db.PhanQuyen.Where(p => p.IDTaiKhoan == idTaiKhoan).ToList();
            db.PhanQuyen.RemoveRange(quyenCu);

            foreach (var maQuyen in danhSachQuyenMoi)
            {
                db.PhanQuyen.Add(new PhanQuyen
                {
                    IDTaiKhoan = idTaiKhoan,
                    MaQuyen = maQuyen
                });
            }

            db.SaveChanges();
        }
    }
}
