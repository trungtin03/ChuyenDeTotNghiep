// chat.js

$(document).ready(function () {
    var chat = $.connection.chatHub;

    // Hàm thêm tin nhắn vào khung chat
    function appendMessage(userName, message, time, isAdmin) {
        var messageDiv = $('<div>').addClass('chat-message ' + (isAdmin ? 'admin' : 'user'));
        var content = `
            <div class="chat-bubble">
                <div><strong>${userName}</strong></div>
                <div>${message}</div>
                <div class="chat-time">${time}</div>
            </div>
        `;
        messageDiv.html(content);
        $('#chatMessages').append(messageDiv);
        $('#chatMessages').scrollTop($('#chatMessages')[0].scrollHeight);
    }

    // Load toàn bộ lịch sử tin nhắn giữa user và admin
    function loadUserMessages() {
        $.get('/ChatSupport/GetUserMessages', function (res) {
            if (res.success) {
                $('#chatMessages').empty();
                res.data.forEach(msg => {
                    const align = msg.laAdmin ? 'admin' : 'user';
                    const sender = align === 'admin' ? 'Admin' : 'Bạn';
                    const time = msg.thoiGian ? new Date(msg.thoiGian).toLocaleString('vi-VN') : 'Chưa rõ';
                    appendMessage(sender, msg.noiDung, time, align === 'admin');
                });
            }
        });
    }

    // Khi nhận tin nhắn từ server (nếu dùng SignalR real-time)
    chat.client.receiveMessage = function (userName, message, time) {
        appendMessage(userName, message, time, false);
    };

    chat.client.receiveAdminMessage = function (userName, message, time) {
        appendMessage(userName, message, time, true);
    };

    // Bắt đầu kết nối SignalR
    $.connection.hub.start().done(function () {
        console.log('SignalR connected');

        // Gửi tin nhắn
        $('#sendButton').click(function (e) {
            e.preventDefault();

            var message = $('#messageInput').val().trim();
            if (message !== '') {
                chat.server.sendMessage(currentUserId, message); // Gửi lên server
                $('#messageInput').val('');
                $('#messageInput').focus();

                // Tải lại toàn bộ tin nhắn (đồng bộ UI)
                setTimeout(loadUserMessages, 300);
            }
        });
    }).fail(function (error) {
        console.error('SignalR connection failed: ' + error);
    });

    // Toggle mở/đóng chatbox và load lịch sử khi mở
    $('#chatButton').click(function () {
        $('#chatBox').fadeToggle();
        loadUserMessages();
    });
});
