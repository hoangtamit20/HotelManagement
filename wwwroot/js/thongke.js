if ($('#ngayBatDau').val() === "" || $('#ngayKetThuc').val() === "") {
    $('#btnXacNhanThongKe').hide();
}
else {
    $('#btnXacNhanThongKe').show();
}


$("body").on("change", "#dropdownyear", function (e) {
    e.preventDefault();
    if ($('#dropdownyear').val() == -1) {

    }
    else {
        $('#ngayBatDau').val("");
        $('#ngayKetThuc').val("");
        $("#idFormThongKe").submit();
    }
});

$("body").on("change", "#ngayBatDau", function (e) {
    if ($('#ngayBatDau').val() === "" || $('#ngayKetThuc').val() === "") {
        $('#btnXacNhanThongKe').hide();
    }
    else {
        $('#btnXacNhanThongKe').show();
        $("#dropdownyear").val(-1).change();
    }
    var dateMin = new Date($('#ngayBatDau').val());
    dateMin.setDate(dateMin.getDate() + 1);
    $("#ngayKetThuc[type='datetime-local']").prop('min', dateMin.toISOString().split(":00.")[0]).change();
});

$("body").on("change", "#ngayKetThuc", function (e) {
    if ($('#ngayBatDau').val() === "" || $('#ngayKetThuc').val() === "") {
        $('#btnXacNhanThongKe').hide();
    }
    else {
        $('#btnXacNhanThongKe').show();
        $("#dropdownyear").val(-1).change();
    }
    var dateMin = new Date($('#ngayKetThuc').val());
    dateMin.setDate(dateMin.getDate() - 1);
    $("#ngayBatDau[type='datetime-local']").prop('max', dateMin.toISOString().split(":00.")[0]).change();
});

$("body").on('click', '#btnXacNhanThongKe', function (e) {
    e.preventDefault();
    $('#idFormThongKe').submit();
});