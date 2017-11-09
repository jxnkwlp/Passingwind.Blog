/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/jquery-validation/dist/jquery.validate.js" />

/*  comment.js */

"use strict";

function asyncRequest(element) {
    if (!element) return;

    var loading = $(element.getAttribute("data-ajax-loading"));

    $.ajax({
        type: 'post',
        url: element.getAttribute("action") || undefined,
        data: $(element).serializeArray(),
        beforeSend: function beforeSend(xhr) {
            if (loading) loading.show();
        },
        complete: function complete() {
            if (loading) loading.hide();
        },
        success: function success(data, status, xhr) {
            asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
        },
        error: function error() {
            console.error('post comment error.');
        }
    });
}

function asyncOnSuccess(element, data, contentType) {

    if (data.parentId) $('#comment-' + data.parentId).find('#comment-respond').remove();

    if (!data.result) {
        if (data.message) {
            alert(data.message);
        }
        return;
    }

    if (data.commentId) {
        $.get(data.url, function (html) {
            var parentId = data.parentId;
            if (parentId === null) {
                $('.comment-list > ul').append(html);
            } else {
                $(html).prependTo($('#comment-' + parentId).find(' > ul'));
            }

            initCommentForm();
        });
    }
    //var commentData = data.comment;
    //// parse and format data
    //var $templateContainer = $("<div/>");
    //$templateContainer.html($('#commentTemplate').html());

    //$templateContainer.find('li').attr('id', 'comment-' + commentData.id).attr('data-id', commentData.id);
    //$templateContainer.find('.comment-actions a').data('id', commentData.id);
    //$templateContainer.find('.comment-time time').attr('datetime', commentData.createOn).html(new Date(commentData.createOn).toLocaleString().replace(/年|月/g, "-").replace(/日|上午|下午/g, " "));
    //$templateContainer.find('.comment-author .avatar').attr('src', commentData.avatarUrl);
    //$templateContainer.find('.comment-author .author-name a').attr('href', commentData.website);
    //$templateContainer.find('.comment-author .author-name a').html(commentData.author);
    //$templateContainer.find('.comment-content p').html(commentData.content);

    //// end parse and format data

    ////console.log($templateContainer);

    ////var commentsContainer = commentData.parentId == null ? $('.comment-list > ul') : $('#comment-' + parentId + ' > ul');
    ////console.log(commentsContainer);
    ////$("<span>Hello world!</span>").insertAfter(commentsContainer);
    //// $($templateContainer.html()).insertAfter(commentsContainer);

    //var parentId = commentData.parentId;
    //if (parentId == null) {
    //    $('.comment-list > ul').append($templateContainer.html());
    //} else {
    //    $($templateContainer.html()).prependTo($('#comment-' + parentId).find(' > ul'));
    //}

    //initCommentForm();
}
function initCommentForm() {
    $('#comment-respond').find('textarea').val('');
}
function moveCommentForm(commentId) {

    $('#comment-respond').hide(); // remove all

    var container = $('#comment-' + commentId); //li#comment
    if (container.find($('#comment-respond')).size() > 0) {
        container.find($('#comment-respond')).remove();
    }

    // container.find($('#comment-respond')).show();

    $($('#comment-respond').prop('outerHTML')).show().insertAfter(container.find('>.comment-body'));

    var $form = container.find('#commentForm');

    $form.find('input[type=hidden][name=ParentId]').val(commentId);
    $form.find('.cancel').show();

    $.validator.unobtrusive.parse($form);
}
function canelCommentForm(obj) {
    $(obj).closest('#comment-respond').remove();
}
function showCommentForm() {
    $('#comment-respond').show();
    $('html,body').animate({ scrollTop: $('#comment-respond').offset().top }, 'fast');
}
$(document).on("submit", "form[id=commentForm]", function (evt) {
    evt.preventDefault();
    $(this).find('[type=submit]').attr('disabled', 'disabled');
    asyncRequest(this);
    $(this).find('[type=submit]').removeAttr('disabled');
});

