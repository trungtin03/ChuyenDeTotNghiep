// Hiển thị popup thêm danh mục
$(document).ready(function () {
    $("#addPopup").on("click", function () {
        $("#popUp").fadeIn(200);
        $("#overlay").fadeIn(200);
    });

    $(".cancelPopup").on("click", function () {
        $(".popupPrivate").fadeOut(200);
        $("#overlay").fadeOut(200);
    });
});

// Load dữ liệu danh mục lên form sửa
function loadData(id) {
    $.ajax({
        type: 'POST',
        url: '/Admin/Category/Index',
        data: { id: id },
        success: function (response) {
            if (response && response.MaDM) {
                $("#madm").val(response.MaDM);
                $("#tendanhmuc").val(response.TenDanhMuc);
                $(".changePopUp").fadeIn(200);
                $("#overlay").fadeIn(200);
            } else {
                alert("Không tìm thấy danh mục.");
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Đã xảy ra lỗi khi tải danh mục.");
        }
    });
}

// Gửi AJAX thêm danh mục
function themDanhMuc() {
    let data = {};
    let formData = $('#add-form').serializeArray();
    $.each(formData, function (index, value) {
        data[value.name] = value.value;
    });

    $.ajax({
        url: '/Admin/Category/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (respone) {
            $("#add-message").html(respone.message);
            if (respone.status === true) {
                $("#add-message").addClass("text-warning");
                $(".popupPrivate").fadeOut(200);
                $("#overlay").fadeOut(200);
                setTimeout(function () {
                    window.location.replace("/Admin/Category");
                }, 800);
            } else {
                $("#add-message").addClass("text-danger");
            }
        },
        error: function (respone) {
            console.log(respone);
            alert("Lỗi khi thêm danh mục.");
        }
    });

    return false;
}

// Gửi AJAX sửa danh mục
function suaDanhMuc() {
    let data = {};
    let formData = $('#update-form').serializeArray();
    $.each(formData, function (index, value) {
        data[value.name] = value.value;
    });

    $.ajax({
        url: '/Admin/Category/Update',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (respone) {
            $("#update-message").html(respone.message);
            if (respone.status === true) {
                $("#update-message").addClass("text-warning");
                $(".popupPrivate").fadeOut(200);
                $("#overlay").fadeOut(200);
                setTimeout(function () {
                    window.location.replace("/Admin/Category");
                }, 800);
            } else {
                $("#update-message").addClass("text-danger");
            }
        },
        error: function (respone) {
            console.log(respone);
            alert("Lỗi khi cập nhật danh mục.");
        }
    });

    return false;
}

// Gán ID cần xóa vào form xóa
function deleteData(id) {
    $("#delete-madm").val(id);
    $(".deletePopUp").fadeIn(200);
    $("#overlay").fadeIn(200);
}

// Gửi AJAX xóa danh mục
function xoaDanhMuc() {
    let id = $("#delete-madm").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/Category/Delete',
        data: { id: id },
        success: function (response) {
            if (response.status === true) {
                $("#row-" + id).fadeOut(200);
                $(".popupPrivate").fadeOut(200);
                $("#overlay").fadeOut(200);
            } else {
                alert("Không thể xóa danh mục.");
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            alert("Đã xảy ra lỗi khi xóa danh mục.");
        }
    });
}
