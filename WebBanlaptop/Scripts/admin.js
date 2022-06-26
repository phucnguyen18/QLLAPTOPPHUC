////const { EALREADY } = require("constants");
////const { type } = require("jquery");

var admin = {
    init: function () {
        admin.event();
    },
    event: function () {
        
        //================================
        // modal khóa tải khoản admin
        $('.modal-lock-admin').off('click').on('click', function () {
            var id = $(this).data('account');
            $('#hiddenAccount').val(id);
        });
        $('#btn-lock-admin').off('click').on('click', function () {
            var id = $('#hiddenAccount').val();
            admin.lockOrUnlockAdmin(id);
        });
        //================================
        // modal đọc góp ý
        $('.modal-lock-admin').off('click').on('click', function () {
            var id = $(this).data('account');
            $('#hiddenAccount').val(id);
        });
        $('#btn-lock-admin').off('click').on('click', function () {
            var id = $('#hiddenAccount').val();
            admin.lockOrUnlockGopY(id);
        });
    },

    lockOrUnlockAdmin: function (id) {
        $.ajax({
            url: '/Admin/LockOrUnlockAdmin',
            data: {
                id: id
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) { // trả về giá trị khóa tài khoản thành công
                    if (response.lockStatus) alert('Khóa tài khoản thành công');
                    else alert('mở khóa tài khoản thành công');
                    $('#lockAccount').modal('hide');
                    location.reload(); // reload lại trang
                } else { // khóa lỗi do người bị khóa là chủ sở hữu
                    alert(response.exit);
                }
            },
            error: function (err) {
                console.log(err);
                alert(err.errorMessage)
            }
        });
    },

    lockOrUnlockGopY: function (id) {
        $.ajax({
            url: '/Admin/LockOrUnlockGopY',
            data: {
                id: id
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) { // trả về giá trị khóa tài khoản thành công
                    if (response.lockStatus) alert('Đã đọc');
                    else alert('chưa đọc');
                    $('#lockAccount').modal('hide');
                    location.reload(); // reload lại trang
                } else { // khóa lỗi do người bị khóa là chủ sở hữu
                    alert(response.exit);
                }
            },
            error: function (err) {
                console.log(err);
                alert(err.errorMessage)
            }
        });
    }


};

admin.init();