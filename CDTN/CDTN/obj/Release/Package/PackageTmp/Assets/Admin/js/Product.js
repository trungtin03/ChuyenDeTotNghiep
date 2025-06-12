// Load ảnh preview khi Thêm
var loadFile = function (event) {
    var image = document.getElementById('output');
    image.src = URL.createObjectURL(event.target.files[0]);
};

// Load ảnh preview khi Sửa
var loadFileUpdate = function (event) {
    var image = document.getElementById('anhsp');
    image.src = URL.createObjectURL(event.target.files[0]);
};

// Thêm dòng màu-size-số lượng
function themDong(listId) {
    const html = $('#template-mau-size').html();
    $(listId).append(html);
}




// Thêm sản phẩm
function themSanPham() {
    let formData = new FormData(); // <- phải tạo trước

    let sanpham = {};
    $('#add-form').serializeArray().forEach(field => {
        sanpham[field.name] = field.value;
    });

    let chiTietMauSize = [];
    $('#listMauSize_Add .item-mau-size').each(function () {
        chiTietMauSize.push({
            MaMau: $(this).find('.chon-mau').val(),
            MaKichCo: $(this).find('.chon-size').val(),
            SoLuong: $(this).find('.so-luong').val()
        });
    });

    formData.append('sanpham', JSON.stringify(sanpham));
    formData.append('chiTietMauSize', JSON.stringify(chiTietMauSize));

    const imageFile = $('#imageFile')[0]?.files[0];
    if (imageFile) formData.append('hinhanh', imageFile);

    $.ajax({
        url: '/Admin/Product/Create',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.status) {
                alert('Thêm thành công!');
                window.location.reload();
            } else {
                alert(res.message);
            }
        },
        error: function () {
            alert('Lỗi hệ thống!');
        }
    });

    return false;
}


function suaSanPham() {
    let formData = new FormData();

    let sanpham = {};
    $('#update-form').serializeArray().forEach(field => {
        sanpham[field.name] = field.value;
    });

    let chiTietMauSize = [];
    $('#listMauSize_Update .item-mau-size').each(function () {
        chiTietMauSize.push({
            MaMau: $(this).find('.chon-mau').val(),
            MaKichCo: $(this).find('.chon-size').val(),
            SoLuong: $(this).find('.so-luong').val()
        });
    });

    formData.append('sanpham', JSON.stringify(sanpham));
    formData.append('chiTietMauSize', JSON.stringify(chiTietMauSize));

    const updateImage = $('#updateImage')[0]?.files[0];
    if (updateImage) formData.append('hinhanh', updateImage);

    $.ajax({
        url: '/Admin/Product/Update',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.status) {
                alert('Cập nhật thành công!');
                window.location.reload();
            } else {
                alert(res.message);
            }
        },
        error: function () {
            alert('Lỗi hệ thống!');
        }
    });

    return false;
}


$(document).ready(function () {
    $('#deletePopUp .btn-danger').click(xoaSanPham);
    // Mở popup thêm
    $('#addPopup').click(function () {
        $('#overlay').fadeIn();
        $('#popUp').fadeIn();
        console.log('Đã click nút thêm sản phẩm');

    });

    // Mở popup sửa – gọi qua loadData(id) rồi show popup
    window.loadData = function (id) {
        $.ajax({
            type: 'POST',
            url: '/Admin/Product/GetSanPhamById', // ✅ đúng route
            data: { id: id },
            success: function (res) {
                if (!res) return alert("Không tìm thấy sản phẩm");

                // Đổ dữ liệu vào form sửa
                $('#masp').val(res.MaSP);
                $('#tensanpham').val(res.TenSP);
                $('#gia').val(res.Gia);
                $('#danhmuc').val(res.MaDM);
                $('#chatlieu').val(res.ChatLieu);
                $('#mota').val(res.MoTa);
                $('#huongdan').val(res.HuongDan);
                $('#anhsp').attr('src', res.HinhAnh); // nếu res.HinhAnh là null => hiển thị noImage

                // Đổ màu-size
                $('#listMauSize_Update').empty();
                $.each(res.ListMauSize, function (i, item) {
                    themDong('#listMauSize_Update');
                    var row = $('#listMauSize_Update .item-mau-size').last();
                    row.find('.chon-mau').val(item.MaMau);
                    row.find('.chon-size').val(item.MaKichCo);
                    row.find('.so-luong').val(item.SoLuong);
                });

                // Hiển thị popup
                $('#overlay').fadeIn();
                $('#changePopUp').fadeIn();
            },
            error: function (xhr) {
                console.error('Lỗi khi load sản phẩm:', xhr);
                alert('Không thể tải dữ liệu sản phẩm');
            }
        });
    };



    // Mở popup xóa
    window.deleteData = function (id) {
        if (confirm('Bạn có chắc muốn xóa?')) {
            $.post('/Admin/Product/Delete', { id: id }, function (res) {
                if (res.status) {
                    alert('Đã xóa!');
                    location.reload();
                } else {
                    alert('Xóa thất bại');
                }
            });
        }
    }

    // Đóng tất cả popup
    $(document).on('click', '.cancelPopup', function () {
        $('#overlay').fadeOut();
        $('#popUp, #changePopUp, #deletePopUp').fadeOut();
    });
});
function xoaSanPham() {
    const id = $('#delete-masp').val();
    if (!id) return;

    $.ajax({
        url: '/Admin/Product/Delete',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            if (res.status) {
                alert('Xóa thành công!');
                window.location.reload();
            } else {
                alert('Xóa thất bại!');
            }
        },
        error: function () {
            alert('Lỗi hệ thống!');
        }
    });
}
// Gắn sự kiện XÓA cho các dòng màu-size bằng delegation
$(document).on('click', '.btn-remove', function () {
    $(this).closest('.item-mau-size').remove();
});

// Gắn sự kiện THÊM dòng màu-size (cả ở thêm và sửa)
$('#btnThemMauSize_Add').click(function () {
    themDong('#listMauSize_Add');
});
$('#btnThemMauSize_Update').click(function () {
    themDong('#listMauSize_Update');
});
