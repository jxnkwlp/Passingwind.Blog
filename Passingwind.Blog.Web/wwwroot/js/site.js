// Write your Javascript code.
$(function () {
  // $(document).pjax('a[data-pjax]', '.content-area');

  $('#togglemenu').click(function (e) {
    $('#main-navigation').toggleClass('open');
    e.preventDefault();
  });
});


window._alert = function (obj) {
  if (swal) {
    swal({
      title: obj.title,
      text: obj.text,
      icon: obj.icon || 'info',
    })
  } else {
    window.alert(obj.text);
  }
};