$(document).ready(function() {
    var requestVerificationToken = $('input[name="__RequestVerificationToken"]:first').val();

    // Like button
    $('body').on('click', '.like-button', function() {
        var button = $(this);
        var postId = button.data('post-id');
        var icon = button.find('i');
        $.ajax({
            url: '/api/postinteraction/like',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ postId: postId }),
            success: function(response) {
                button.next('.like-count').text(response.likeCount);
                button.toggleClass('liked', response.isLiked);
                icon.removeClass('far fas').addClass(response.isLiked ? 'fas' : 'far');
            },
            error: function() { console.error("Failed to like the post."); }
        });
    });

    // Author Hover Card
    var hoverTimeout;
    $('body').on('mouseenter', '.post-author-container', function() {
        clearTimeout(hoverTimeout);
        var triggerElement = $(this);
        var cardId = triggerElement.data('hovercard-id');
        var card = $('#' + cardId);

        if (card.length === 0) return;

        card.appendTo('body');
        var triggerPos = triggerElement.offset();
        var cardHeight = card.outerHeight();

        card.css({
            top: triggerPos.top - cardHeight - 5,
            left: triggerPos.left,
            display: 'block'
        }).hide().fadeIn(150);

        card.off('mouseenter').on('mouseenter', function() { clearTimeout(hoverTimeout); });
        card.off('mouseleave').on('mouseleave', function() { $(this).fadeOut(150); });

    }).on('mouseleave', '.post-author-container', function() {
        var cardId = $(this).data('hovercard-id');
        var card = $('#' + cardId);
        hoverTimeout = setTimeout(function() {
            card.fadeOut(150);
        }, 200);
    });

    // Comment functionality
    $('body').on('click', '.comment-toggle-button', function() {
        $('#comment-section-' + $(this).data('post-id')).slideToggle('fast');
    });

    $('body').on('keyup', '.comment-input', function() {
        var button = $('.post-comment-button[data-post-id="' + $(this).data('post-id') + '"]');
        $(this).val().trim().length > 0 ? button.show() : button.hide();
    });

    function postComment(postId) {
        var input = $('.comment-input[data-post-id="' + postId + '"]');
        var content = input.val().trim();
        if (!content) return;
        $.ajax({
            url: '/api/postinteraction/comment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ postId: postId, content: content }),
            success: function(response) {
                var escapedContent = $('<div/>').text(content).html();
                var newCommentHtml = `
                    <div class="comment-item" style="display:none;">
                        <img src="${response.profilePicture || '/images/pf.png'}" alt="Avatar" class="comment-avatar" />
                        <div class="comment-body">
                            <a href="/Profile?id=${response.userId}" class="comment-user-name">${response.userName}</a>
                            <p class="comment-content">${escapedContent}</p>
                        </div>
                    </div>
                `;
                var commentList = $('#comment-list-' + postId);
                $(newCommentHtml).prependTo(commentList).fadeIn('slow');
                input.val('');
                $('.post-comment-button[data-post-id="' + postId + '"]').hide();
                var countSpan = $('.comment-toggle-button[data-post-id="' + postId + '"]').next('.comment-count');
                countSpan.text((parseInt(countSpan.text()) || 0) + 1);
            },
            error: function() { alert('Failed to post comment.'); }
        });
    }

    $('body').on('click', '.post-comment-button', function() { postComment($(this).data('post-id')); });
    $('body').on('keypress', '.comment-input', function(e) {
        if (e.which === 13 && !e.shiftKey) {
            e.preventDefault();
            postComment($(this).data('post-id'));
        }
    });

    // Global functions
    window.deletePost = function(postId) {
        if (confirm('Are you sure?')) $.ajax({ url: '/Home/DeletePost', type: 'POST', data: { id: postId, __RequestVerificationToken: requestVerificationToken }, success: function(r) { if(r.success) $('#post-' + postId).fadeOut('slow', function() { $(this).remove(); }); else alert(r.message || 'Failed'); }, error: function() { alert('Error'); } });
    }
    window.sendFriendRequest = function(receiverId) {
        $.ajax({ url: '/Friends/SendRequest', type: 'POST', data: { receiverId: receiverId, __RequestVerificationToken: requestVerificationToken }, success: function(r) { if(r.success) location.reload(); else alert('Failed'); }, error: function() { alert('Error'); } });
    }
    window.acceptRequest = function(friendshipId) {
        $.ajax({ url: '/Friends/AcceptRequest', type: 'POST', data: { friendshipId: friendshipId, __RequestVerificationToken: requestVerificationToken }, success: function(r) { if(r.success) $('#request-' + friendshipId).fadeOut('slow', function() { $(this).remove(); }); else alert('Failed'); }, error: function() { alert('Error'); } });
    }
    window.declineRequest = function(friendshipId) {
        $.ajax({ url: '/Friends/DeclineRequest', type: 'POST', data: { friendshipId: friendshipId, __RequestVerificationToken: requestVerificationToken }, success: function(r) { if(r.success) $('#request-' + friendshipId).fadeOut('slow', function() { $(this).remove(); }); else alert('Failed'); }, error: function() { alert('Error'); } });
    }
    window.removeFriend = function(friendId) {
        if (confirm('Are you sure you want to unfriend this user?')) {
            $.ajax({ url: '/Friends/RemoveFriend', type: 'POST', data: { friendId: friendId, __RequestVerificationToken: requestVerificationToken }, success: function(r) { if(r.success) location.reload(); else alert('Failed'); }, error: function() { alert('Error'); } });
        }
    }
});