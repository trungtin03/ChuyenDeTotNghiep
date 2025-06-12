using System;
using System.Linq;
using System.Web.Http;
using CDTN.Models;

namespace CDTN.Controllers
{
    [System.Web.Http.RoutePrefix("api/discounts")]
    public class DiscountApiController : ApiController
    {
        private storeDB db = new storeDB();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("apply")]
        public IHttpActionResult ApplyDiscount(DiscountRequest model)
        {
            var code = db.DiscountCodes.FirstOrDefault(d =>
                d.Code == model.Code &&
                d.IsActive &&
                d.StartDate <= DateTime.Now &&
                d.EndDate >= DateTime.Now);

            if (code == null)
            {
                return Ok(new { success = false, message = "Mã không hợp lệ hoặc đã hết hạn." });
            }

            decimal discountAmount = 0;
            if (code.Type == DiscountType.OrderTotal)
            {
                discountAmount = code.IsPercentage
                    ? model.TotalAmount * code.DiscountAmount / 100
                    : code.DiscountAmount;
            }
            else
            {
                return Ok(new { success = false, message = "Mã này chỉ áp dụng cho sản phẩm cụ thể." });
            }

            return Ok(new
            {
                success = true,
                discountAmount,
                finalPrice = model.TotalAmount - discountAmount
            });
        }

        public class DiscountRequest
        {
            public string Code { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
