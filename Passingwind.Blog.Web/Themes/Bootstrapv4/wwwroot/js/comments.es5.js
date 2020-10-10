/// <reference path="../lib/jquery/jquery.js" />
/// <reference path="../lib/jquery-validate/jquery.validate.js" />
/// <reference path="../lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js" />
/// <reference path="../lib/sweetalert/sweetalert.min.js" />

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
			$(element).find('[type=submit]').removeAttr('disabled');
		},
		success: function success(data, status, xhr) {
			asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
		},
		error: function error(err) {
			asyncOnError(err);
		}
	});
}

function asyncOnError(err) {
	//console.log(err);
	_alert({ text: 'Failed to submit comment, please try again', icon: 'error' });
}

function asyncOnSuccess(element, data, contentType) {
	if (data.parentId) $('#comment-' + data.parentId).find('#comment-respond').remove();

	if (!data.result) {
		if (data.message) {
			_alert({ text: data.message, icon: 'error' });
		}
		return;
	}

	if (data.commentId) {
		_alert({ text: 'Submited', icon: 'success' });
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
}

function initCommentForm() {
	$('#comment-respond').find('textarea').val('');
	$('#comment-respond').find('[name=CaptchaCode]').val('');
	refreshCaptchaimg();
}

function moveCommentForm(commentId) {

	$('#comment-respond').hide(); // remove all

	var container = $('#comment-' + commentId); //li#comment
	if (container.find($('#comment-respond')).length > 0) {
		container.find($('#comment-respond')).remove();
	}

	// container.find($('#comment-respond')).show();

	$($('#comment-respond').prop('outerHTML')).show().appendTo(container.find('>.comment-body'));

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
	refreshCaptchaimg();
}

function refreshCaptchaimg() {
	$('#commentForm').find('.comment-form-captchaimg>img').click();
}

$(document).on("submit", "form[id=commentForm]", function (evt) {
	evt.preventDefault();
	$(this).find('[type=submit]').attr('disabled', 'disabled');
	asyncRequest(this);
});

