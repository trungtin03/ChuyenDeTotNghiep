// Load sản phẩm lên modal
function loadSanPham(id) {
    $.ajax({
        type: 'POST',
        data: { "id": id },
        url: '/Product/Index',
        success: function (response) {
            $("#modal-a-hinhanh").attr("href", response.HinhAnh);
            $("#modal-hinhanh").attr("src", response.HinhAnh);
            $("#modal-tensp").html(response.TenSP);
            $("#modal-danhmuc").html(response.DanhMuc.TenDanhMuc);
            $("#modal-gia").html(response.Gia.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }));

            // Load danh sách màu
            let mauHtml = '';
            if (response.ListMau && response.ListMau.length > 0) {
                $.each(response.ListMau, function (index, item) {
                    mauHtml += `
                        <div class="form-check mr-3 mb-2">
                            <input class="form-check-input mau-radio" type="radio" name="mau" value="${item.MaMau}" id="mau-${item.MaMau}">
                            <label class="form-check-label" for="mau-${item.MaMau}">
                                <span style="display: inline-block; width: 20px; height: 20px; background: ${item.MaMauHex}; border-radius: 50%; border: 1px solid #ccc;"></span> ${item.TenMau}
                            </label>
                        </div>`;
                });
            } else {
                mauHtml = "<p>Không có màu sắc cho sản phẩm này</p>";
            }
            $("#chon-mau-list").html(mauHtml);

            // Load size
            $("#modal-kichco-soluong").html('');
            $.each(response.SanPhamChiTiets, function (index, item) {
                $("#modal-kichco-soluong").append(`<option value="${item.IDCTSP}">${item.KichCo.TenKichCo}</option>`);
            });
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Có lỗi xảy ra khi load sản phẩm.");
        }
    });
}

function themVaoGioHang() {
    const selectedSize = $('input[name="kichco"]:checked');
    const selectedColor = $('input[name="mau"]:checked');
    const quantity = parseInt($('#modal-soluong').val());

    if (selectedSize.length === 0) {
        alert("Vui lòng chọn kích cỡ!");
        return;
    }

    if (selectedColor.length === 0) {
        alert("Vui lòng chọn màu sắc!");
        return;
    }

    if (isNaN(quantity) || quantity <= 0) {
        alert("Số lượng không hợp lệ!");
        return;
    }

    const selectedBtn = selectedSize.closest('.kichco-btn');
    const stock = parseInt(selectedBtn.data('soluong')) || 0;
    const colorOfSize = selectedBtn.data('mamau').toString();
    const selectedColorVal = selectedColor.val();

    if (selectedColorVal !== colorOfSize) {
        alert("Màu bạn chọn không phù hợp với kích cỡ đã chọn!");
        return;
    }

    if (stock <= 0) {
        alert("Sản phẩm bạn chọn hiện đã hết hàng!");
        return;
    }
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            IDCTSP: selectedSize.val(),
            SoLuongMua: quantity
        }),
        success: function (res) {
            if (res.status === true) {
                alert("Thêm vào giỏ hàng thành công!");
                location.reload();
            } else {
                alert(res.message || "Không thể thêm vào giỏ hàng.");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi thêm vào giỏ hàng.");
        }
    });
}

// Ajax load số lượng theo size
$(document).on("change", "#modal-kichco-soluong", function () {
    let id = $(this).val();
    $.ajax({
        type: 'POST',
        data: { "id": id },
        url: '/Product/Detail',
        success: function (response) {
            if (response.SoLuong > 0) {
                $("#order-text").html("Thêm vào giỏ");
                $("#order-text").removeAttr("disabled");
            } else {
                $("#order-text").html("Hết hàng ! Hãy chọn kích cỡ khác");
                $("#order-text").attr("disabled", "disabled");
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Có lỗi xảy ra.");
        }
    });
});

//Ajax xóa sp trong giỏ hàng
function xoaGioHang(idctsp) {
    let total = 0;
    swal({
        title: "Bạn có chắc chắn",
        text: "Xóa sản phẩm này khỏi giỏ hàng",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: 'POST',
                    data: { "idctsp": idctsp },
                    url: '/Cart/DeleteFromCart',
                    success: function (response) {
                        if (response.length == 0) {
                            window.location = "/Cart/Orders";
                        } else {
                            $("#row-order-" + idctsp).remove();
                            $("#product-count").html(response.length);
                            $.each(response, function (index) {
                                total += response[index].SoLuongMua * response[index].GiaMua;
                            })
                            $("#order-total").html(total.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }));
                        }
                    },
                    error: function (response) {
                        //debugger;  
                        console.log(xhr.responseText);
                        alert("Error has occurred..");
                    }
                });
            }
        });
}
function datHang() {
    var formData = $('#add-bill-form').serialize();

    $.ajax({
        url: '/Bill/CreateBill',
        type: 'POST',
        data: formData,
        success: function (res) {
            if (res.status) {
                swal({
                    title: "Đặt hàng thành công!",
                    icon: "success"
                }).then(() => {
                    window.location.href = "/Bill/ListBills";
                });
            } else {
                $("#add-message").text("Có lỗi: " + res.message).addClass("text-danger");
            }
        },
        error: function () {
            $("#add-message").text("Không gửi được yêu cầu.").addClass("text-danger");
        }
    });
    return false; 
}


function kiemTraMaGiamGia() {
    const code = $('#discount-code-input').val().trim();
    if (!code) {
        $('#discount-message').text("⚠️ Vui lòng nhập mã giảm giá.");
        return;
    }

    let tongTien = 0;
    $(".product-total .amount").each(function () {
        const gia = $(this).text().replace(/[^\d]/g, '');
        tongTien += parseInt(gia);
    });

    $.ajax({
        url: '/api/discounts/apply',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            code: code,
            totalAmount: tongTien
        }),
        success: function (res) {
            if (res.success) {
                $('#discount-message').html("✅ Đã áp dụng mã. Giảm " + res.discountAmount.toLocaleString('vi-VN') + "đ");
                $(".total.amount").text((res.finalPrice).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }));
                $('#discount-amount-hidden').val(res.discountAmount);
                $('#discount-code-hidden').val(code);
            } else {
                $('#discount-message').html("❌ " + res.message);
            }
        },
        error: function () {
            $('#discount-message').text("⚠️ Không thể kiểm tra mã giảm giá.");
        }
    });
}

//Ajax Hủy đơn hàng
function huyDonHang(id) {
    swal({
        title: "Cảnh báo",
        text: "Bạn có chắc về việc hủy đơn hàng này!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: 'POST',
                    data: { "mahd": id, "stt": 0 },
                    url: '/Bill/ChangeStatus',
                    success: function (response) {
                        if (response.status == true) {
                            swal({
                                title: "Thành công!",
                                text: "Hủy đơn hàng thành công !",
                                type: "success",
                                icon: "success",
                                timer: 1500,
                                button: false
                            });
                        } else {
                            swal({
                                title: "Thất bại!",
                                text: "Bạn không thể hủy đơn hàng do đơn hàng đã đang giao",
                                type: "danger",
                                icon: "error",
                                timer: 1500,
                                button: false
                            });
                        }
                        setTimeout(function () {
                            window.location = "/Bill/ListBills";
                        }, 1500);
                    },
                    error: function (response) {
                        //debugger;  
                        console.log(xhr.responseText);
                        alert("Error has occurred..");
                    }
                });
            }
        });
}
// Xử lý thêm vào giỏ hàng và mua ngay
$(document).ready(function () {
    if (window.location.href.includes("/Product/ProductDetail")) {

        // Click chọn kích cỡ
        $(document).on('click', '.kichco-btn', function () {
            $('.kichco-btn').removeClass('active btn-primary').addClass('btn-outline-dark');
            $(this).removeClass('btn-outline-dark').addClass('active btn-primary');
            $(this).find('input[type="radio"]').prop('checked', true);
        });
        // Click chọn màu sắc
        $(document).on('click', '.mausac-btn', function () {
            $('.mausac-btn').removeClass('active btn-primary').addClass('btn-outline-dark');
            $(this).removeClass('btn-outline-dark').addClass('active btn-primary');
            $(this).find('input[type="radio"]').prop('checked', true);

            const selectedColor = $(this).find('input[type="radio"]').val();

            $('input[name="kichco"]').prop('checked', false);
            $('.kichco-btn').removeClass('active btn-primary');

            $('.kichco-btn').each(function () {
                const btn = $(this);
                const size = btn.data('kichco');

                const match = spcts.find(item =>
                    item.MaMau == selectedColor && item.TenKichCo === size
                );

                if (match && match.SoLuong > 0) {
                    btn.removeClass('disabled btn-secondary').addClass('btn-outline-dark');
                    btn.css({ 'pointer-events': 'auto', 'opacity': '1' });
                    btn.find('input[type="radio"]').prop('disabled', false);
                    btn.attr('data-idctsp', match.IDCTSP);
                    btn.attr('data-mamau', match.MaMau);
                    btn.attr('data-soluong', match.SoLuong);
                    btn.find('input[type="radio"]').val(match.IDCTSP);
                } else {
                    btn.removeClass('btn-outline-dark btn-primary active').addClass('disabled btn-secondary');
                    btn.css({ 'pointer-events': 'none', 'opacity': '0.5' });
                    btn.find('input[type="radio"]').prop('disabled', true).prop('checked', false);
                }
            });
        });

        // Hàm hiển thị lỗi
        function showError(message) {
            $('#product-error-message').text(message).show();
        }
        // Hàm xử lý khi bấm vào Thêm vào giỏ hoặc Mua ngay
        function handleAction(isBuyNow) {
            const selectedSize = $('input[name="kichco"]:checked');
            const selectedColor = $('input[name="mau"]:checked');
            const quantity = parseInt($('#modal-soluong').val());

            $('#product-error-message').hide();

            if (selectedSize.length === 0) {
                showError("Vui lòng chọn kích cỡ!");
                return;
            }

            if (selectedColor.length === 0) {
                showError("Vui lòng chọn màu sắc!");
                return;
            }

            if (isNaN(quantity) || quantity <= 0) {
                showError("Số lượng không hợp lệ!");
                return;
            }

            const selectedBtn = selectedSize.closest('.kichco-btn');
            const stock = parseInt(selectedBtn.data('soluong')) || 0;
            const colorOfSize = selectedBtn.data('mamau').toString();
            const selectedColorVal = selectedColor.val();

            if (selectedColorVal !== colorOfSize) {
                showError("Màu bạn chọn không phù hợp với kích cỡ đã chọn!");
                return;
            }

            if (stock <= 0) {
                showError("Sản phẩm bạn chọn hiện đã hết hàng!");
                return;
            }

            if (isBuyNow) {
                window.location.href = `/Cart/BuyNow?id=${selectedSize.val()}&quantity=${quantity}`;
            } else {
                $.ajax({
                    url: '/Cart/AddToCart',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        IDCTSP: selectedSize.val(),
                        SoLuongMua: quantity
                    }),
                    success: function (res) {
                        if (res.status === true) {
                            alert("Thêm vào giỏ hàng thành công!");
                            location.reload();
                        } else {
                            showError(res.message || "Không thể thêm vào giỏ hàng.");
                        }
                    },
                    error: function () {
                        showError("Có lỗi xảy ra khi thêm vào giỏ hàng.");
                    }
                });
            }
        }
        // Gán sự kiện cho 2 nút
        $('#btn-add-to-cart').on('click', function () {
            handleAction(false);
        });

        $('#btn-buy-now').on('click', function () {
            handleAction(true);
        });
    }
});