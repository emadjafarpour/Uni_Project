$("#hide").hide();
// $(document).ready(function() {
$("#datepicker ").persianDatepicker({
    inline: false,
    autoClose: true,
    format: "L",
    toolbox: {
        calendarSwitch: {
            enabled: true,
        },
    },
    // onSelect: function(date){
    // 	$("#hide").hide();
    // 	$(".alert").last().html(new persianDate(date).format())
    // 	$("#hide").show();
    // }
});
// });
