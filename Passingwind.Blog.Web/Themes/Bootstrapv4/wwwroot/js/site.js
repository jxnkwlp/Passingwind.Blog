
function toggleWidgetArchiveShow(ele) {
	var $this = $(ele).closest('li');
	$this.toggleClass('show');
}
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
