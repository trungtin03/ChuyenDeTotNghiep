//Ajax thêm tài khoản
let xhr = new XMLHttpRequest();

function themTaiKhoanAdmin() {
    let data = {};
    let formData = $('#add-form').serializeArray({
    });
    $.each(formData, function (index, value) {
        data["" + value.name + ""] = value.value;
    });
    if (data["matkhau"] != data["nhaplaimatkhau"]) {
        $("#add-message").html("Mật khẩu không khớp");
        return false;
    }
    $.ajax({
        url: '/AdminUser/Create',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (respone) {
            $("#add-message").html(respone.message);
            if (respone.status == true) {
                $("#add-message").addClass("text-warning");
                setTimeout(function () {
                    window.location.replace("/Admin/AdminUser");
                }, 1000)
            } else {
                $("#add-message").addClass("text-danger");
            }
        },
        error: function (xhr) {
            console.log(xhr);
        }
    });
    return false;
}

//load dữ liệu lên form sửa
function loadData(id) {
    $.ajax({
        url: '/Admin/AdminUser/GetById',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ id: id }),
        dataType: 'json',
        success: function (res) {
            if (!res.success) {
                alert(res.message);
                return;
            }

            const response = res.data;
            $("#matk").val(response.ID);
            $("#tendangnhap").val(response.TenDangNhap);
            $("#hoten").val(response.HoTen);
            response.LoaiTaiKhoan
                ? $("#admin-role").prop("checked", true)
                : $("#manager-role").prop("checked", true);
            response.TrangThai
                ? $("#actived").prop("checked", true)
                : $("#blocked").prop("checked", true);

            $("#overlay").show();
            $("#changePopUp").show();
            document.body.classList.add('overflow-hidden');
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Có lỗi xảy ra khi lấy thông tin tài khoản.");
        }
    });
}
//ajax sửa tài khoản quản trị
function suaTaiKhoanQuanTri() {
    let data = {};
    let formData = $('#update-form').serializeArray({
    });
    $.each(formData, function (index, value) {
        data["" + value.name + ""] = value.value;
    });
    $.ajax({
        url: '/AdminUser/Update',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (respone) {
            $("#update-message").html(respone.message);
            if (respone.status == true) {
                $("#update-message").addClass("text-warning");
                setTimeout(function () {
                    window.location.replace("/Admin/AdminUser");
                }, 1000)
            } else {
                $("#update-message").addClass("text-danger");
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText); 
        }
    });
    return false;
}

//load data lên form xóa
function deleteData(id) {
    $("#delete-message").html("");
    $("#delete-adminuser-id").val(id);
}

function xoaTaiKhoan() {
    let id = $("#delete-adminuser-id").val();
    $.ajax({
        type: 'POST',
        data: { "id": id },
        url: '/AdminUser/Delete',
        success: function (response) {
            if (response.status == true) {
                $(".cancelPopup").click();
                $("#row-" + id).remove();
            } else {
                $("#delete-message").html(response.message);
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Error has occurred..");
        }
    });  
}