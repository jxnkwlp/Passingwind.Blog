$(function (e) {
    $('table thead .item-checkbox input[type=checkbox]').on('click', function (e) {
        var that = $(this);
        if (that.prop('checked')) {
            that.closest('table').find('tbody tr .item-checkbox input[type=checkbox]').prop('checked', 'checked');
        } else {
            that.closest('table').find('tbody tr .item-checkbox input[type=checkbox]').removeAttr('checked');
        }
    });
})