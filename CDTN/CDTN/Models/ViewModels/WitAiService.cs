using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WitAiService
{
    private readonly string token = "H3XF6YFIU2LRHRH4IXT3ZTFTOTRTN7BM";

    public async Task<string> GetIntent(string userMessage)
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var url = $"https://api.wit.ai/message?v=20230517&q={Uri.EscapeDataString(userMessage)}";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(jsonString);

                string intent = (json?.intents != null && json.intents.Count > 0) ? json.intents[0].name : null;
                return intent ?? "unknown";
            }
        }
        catch (Exception ex)
        {
            // log lỗi nếu cần
            return "unknown";
        }
    }

    public async Task<string> GetBotResponse(string userMessage)
    {
        var intent = await GetIntent(userMessage);
        string lowerMsg = userMessage.ToLower();

        switch (intent)
        {
            case "hoi_gia":
                if (lowerMsg.Contains("áo sơ mi"))
                    return "Áo sơ mi hiện có giá từ 199K. Bạn muốn mẫu basic hay form Hàn Quốc để shop gửi link ạ?";
                if (lowerMsg.Contains("quần tây"))
                    return "Quần tây dao động từ 250K đến 450K tùy mẫu. Bạn cần quần công sở hay đi chơi ạ?";
                if (lowerMsg.Contains("quần jean"))
                    return "Quần jean nữ bên shop chỉ từ 220K. Có nhiều form ôm và ống rộng cực trend. Bạn muốn loại nào để shop gửi link?";
                if (lowerMsg.Contains("ưu đãi") || lowerMsg.Contains("khuyến mãi") || lowerMsg.Contains("sale"))
                    return "Hiện shop đang có nhiều ưu đãi đến 30%. Bạn có thể xem danh sách sản phẩm giảm giá tại mục 'Khuyến mãi' nhé!";
                return "Sản phẩm đang có khuyến mãi 10%. Bạn muốn xem loại nào?";

            case "hoi_size":
                if (lowerMsg.Contains("1m65") && lowerMsg.Contains("55kg"))
                    return "Với chiều cao 1m65 và cân nặng 55kg, bạn có thể mặc size M hoặc L tùy form. Shop có thể tư vấn kỹ hơn nếu bạn inbox.";
                if (lowerMsg.Contains("size l"))
                    return "Size L thường phù hợp với người từ 55–65kg. Bạn vui lòng cho shop thêm chiều cao và cân nặng để tư vấn chính xác hơn.";
                if (lowerMsg.Contains("bảng size"))
                    return "Dạ có ạ! Mỗi sản phẩm đều có bảng size chi tiết, bạn có thể xem ngay bên dưới hình ảnh sản phẩm.";
                if (lowerMsg.Contains("size nhỏ nhất"))
                    return "Size nhỏ nhất thường là S hoặc XS tùy sản phẩm. Bạn có thể xem thông tin cụ thể trên từng sản phẩm.";
                if (lowerMsg.Contains("dưới 50kg"))
                    return "Với nữ dưới 50kg, thường mặc size S hoặc XS. Tuy nhiên còn phụ thuộc chiều cao và dáng người. Inbox để shop tư vấn kỹ hơn nhé!";
                return "Bạn vui lòng cung cấp chiều cao và cân nặng để shop tư vấn size phù hợp nhé.";

            case "hoi_thanh_toan":
                if (lowerMsg.Contains("cod") || lowerMsg.Contains("nhận hàng"))
                    return "Dạ có ạ! Shop hỗ trợ thanh toán khi nhận hàng (COD) toàn quốc.";
                if (lowerMsg.Contains("vat"))
                    return "Shop hỗ trợ xuất hoá đơn VAT cho đơn hàng công ty. Bạn vui lòng ghi chú khi đặt hàng nhé.";
                if (lowerMsg.Contains("đăng ký") && lowerMsg.Contains("tài khoản"))
                    return "Không cần đâu ạ! Bạn có thể đặt hàng với tư cách khách lẻ mà không cần đăng ký tài khoản.";
                return "Shop hỗ trợ COD, chuyển khoản, Momo, ZaloPay và thẻ ngân hàng.";

            case "hoi_doi_tra":
                if (lowerMsg.Contains("sau khi") && lowerMsg.Contains("đổi ý"))
                    return "Shop hỗ trợ đổi trả trong 7 ngày nếu sản phẩm còn tem mác và chưa sử dụng.";
                if (lowerMsg.Contains("trả sản phẩm"))
                    return "Bạn vui lòng liên hệ hotline hoặc nhắn tin để shop hỗ trợ quy trình đổi trả nhanh nhất.";
                if (lowerMsg.Contains("bị lỗi"))
                    return "Dĩ nhiên rồi ạ! Shop sẽ đổi miễn phí nếu sản phẩm lỗi từ phía chúng tôi.";
                if (lowerMsg.Contains("đổi hàng trong mấy ngày") || lowerMsg.Contains("mấy ngày"))
                    return "Bạn có thể đổi hàng trong vòng 7 ngày kể từ ngày nhận hàng.";
                return "Bạn có thể đổi hàng trong 7 ngày nếu còn tem mác và chưa sử dụng.";

            case "hoi_giao_hang":
                if (lowerMsg.Contains("trong ngày"))
                    return "Shop có hỗ trợ giao trong ngày với khu vực nội thành HCM/HN, áp dụng cho đơn đặt trước 14h nhé.";
                if (lowerMsg.Contains("bao lâu") || lowerMsg.Contains("mất mấy ngày"))
                    return "Thời gian giao hàng từ 1–3 ngày tùy khu vực. Nội thành thường nhận trong 24–48h.";
                if (lowerMsg.Contains("hà nội"))
                    return "Nếu bạn ở nội thành Hà Nội, thường sẽ nhận trong vòng 1–2 ngày làm việc.";
                if (lowerMsg.Contains("toàn quốc"))
                    return "Dạ có! Shop giao hàng toàn quốc, hỗ trợ kiểm hàng trước khi thanh toán.";
                return "Thời gian giao hàng dao động từ 1–3 ngày tuỳ khu vực.";

            default:
                return "Xin lỗi, mình chưa hiểu rõ. Bạn có thể hỏi lại chi tiết hơn không?";
        }
    }

}
