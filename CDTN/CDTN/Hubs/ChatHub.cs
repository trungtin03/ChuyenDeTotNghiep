using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using CDTN.Models;
using CDTN.Session;

namespace CDTN.Hubs
{
    public class ChatHub : Hub
    {
        private storeDB db = new storeDB();

        public async Task SendMessage(int maTK, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var user = db.TaiKhoanNguoiDung.FirstOrDefault(u => u.MaTK == maTK);
            if (user == null)
            {
                await Clients.Caller.receiveError("Không tìm thấy tài khoản người dùng.");
                return;
            }

            // Lưu tin nhắn user gửi
            var newMessage = new PhanHoiHoTro
            {
                MaTK = user.MaTK,
                Subject = "Tin nhắn hỗ trợ",
                Message = message,
                NgayGui = DateTime.Now,
                TrangThai = 0,
                PhanHoi = null
            };

            db.PhanHoiHoTro.Add(newMessage);
            db.SaveChanges();

            string formattedTime = newMessage.NgayGui.ToString("yyyy-MM-ddTHH:mm:ss");

            // Gửi tin nhắn lại cho mọi client
            await Clients.All.receiveMessage(user.HoTen, newMessage.Message, formattedTime);

            // Gửi phản hồi tự động bằng bot
            var botService = new WitAiService();
            var autoReply = await botService.GetBotResponse(message);

            if (!string.IsNullOrWhiteSpace(autoReply))
            {
                var botReply = new PhanHoiHoTro
                {
                    MaTK = user.MaTK,
                    Subject = "Phản hồi tự động",
                    Message = autoReply,
                    NgayGui = DateTime.Now,
                    TrangThai = 1,
                    PhanHoi = autoReply
                };

                db.PhanHoiHoTro.Add(botReply);
                db.SaveChanges();

                string botTime = botReply.NgayGui.ToString("yyyy-MM-ddTHH:mm:ss");
                await Clients.All.receiveAdminMessage("Admin", autoReply, botTime);
            }
        }
    }
}
