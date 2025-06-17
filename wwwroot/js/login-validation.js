$(document).ready(function () {
    $('form').on('submit', function (e) {
        const email = $('#inputEmailAddress').val().trim();
        const password = $('#inputChoosePassword').val().trim();

        if (!email || !password) {
            toastr.error('Vui lòng nhập đầy đủ Email và Mật khẩu.');
            e.preventDefault();
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            toastr.warning('Email không hợp lệ.');
            e.preventDefault();
        }
    });
});
