$(function () {
	$('#light span').on("click", function () {
		$('#light span').removeClass('active');
		$(this).addClass('active');
		$('#score').val($(this).data('value'));
	});
});