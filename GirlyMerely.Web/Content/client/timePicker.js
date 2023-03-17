var defaults = {
	calendarWeeks: true,
	showClear: true,
	showClose: true,
	allowInputToggle: true,
	useCurrent: false,
	ignoreReadonly: true,
	// minDate: new Date(),
	toolbarPlacement: 'top',
	locale: 'nl',
	icons: {
		time: 'fa fa-clock-o',
		date: 'fa fa-calendar',
		up: 'fa fa-angle-up',
		down: 'fa fa-angle-down',
		previous: 'fa fa-angle-left',
		next: 'fa fa-angle-right',
		today: 'fa fa-dot-circle-o',
		clear: 'fa fa-trash',
		close: 'fa fa-times'
	}
};

// $(function() {
// 	var optionsDatetime = $.extend({}, defaults, {format:'DD-MM-YYYY HH:mm'});
// 	var optionsDate = $.extend({}, defaults, {format:'DD-MM-YYYY'});
	var optionsTime = $.extend({}, defaults, {format:'HH:mm'});
	
// 	// $('.datepicker').datetimepicker(optionsDate);
	$('.timepicker').datetimepicker(optionsTime);
// 	// $('.datetimepicker').datetimepicker(optionsDatetime);
// });

// $("#datepicker").css("font-family", "sans-serif");