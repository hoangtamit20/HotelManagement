// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


setTimeout(function () {
    $('.errormessage').alert('close');
}, 8000);


setTimeout(function () {
    $('.successmessage').alert('close');
}, 8000);

$("#imageupload").change(function (event) {
    var files = event.target.files;
    $("#imageprofile").attr("src", window.URL.createObjectURL(files[0]));
});


function Contains(text_one, text_two) {
    if (text_one.indexOf(text_two) != -1)
        return true;
    return false;
}
$("#Search").keyup(function () {
    var searchText = $("#Search").val().toLowerCase();
    $(".card").each(function () {
        if (!Contains($(this).text().toLowerCase(), searchText)) {
            $(this).hide();
        }
        else {
            $(this).show();
        }
    });
});

var listDatPhong = new Array();
var listDatDichVu = new Array();

var listButton = document.querySelectorAll(".btnThem");
listButton.forEach(element => {
    element.addEventListener('click', () => {
        var id = (element.value);
        console.log(document.getElementById('NgayDatDV' + id).value);
        var datPhongModel = {
            TenDv: document.getElementById('TenDv' + id).value,
            SoLuong: parseInt(document.getElementById('SoLuong' + id).value),
            DonGia: parseFloat(document.getElementById('DonGia' + id).value),
            ThanhTien: parseFloat(document.getElementById('ThanhTien' + id).value),
            MaDv: parseInt(document.getElementById('MaDv' + id).value)
        };

        var datDichVuModel = {
            SoLuong: document.getElementById('SoLuong' + id).value,
            NgayDatDichVu: document.getElementById('NgayDatDV' + id).value,
            MaDv: document.getElementById('MaDv' + id).value,
            MaKh: document.getElementById('MaKh' + id).value,
            MaDp: document.getElementById('MaDp' + id).value,
            TongTien: document.getElementById('ThanhTien' + id).value
        };


        let dem = 0;
        for (let i = 0; i < listDatPhong.length; i++) {
            if (listDatPhong[i].TenDv === datPhongModel.TenDv) {
                dem++;
                listDatPhong[i].SoLuong += 1;
                listDatPhong[i].ThanhTien = listDatPhong[i].SoLuong * listDatPhong[i].DonGia;
            }
        }
        if (dem == 0) {
            listDatPhong.push(datPhongModel);
        }
        else {
            dem = 0;
        }

        let dem1 = 0;
        for (let j = 0; j < listDatDichVu.length; j++) {
            if (listDatDichVu[j].MaDv === datDichVuModel.MaDv) {
                listDatDichVu[j].SoLuong += 1;
                listDatDichVu[j].TongTien = (listDatDichVu[j].TongTien / (listDatDichVu[j].SoLuong - 1)) * listDatDichVu[j].SoLuong;
            }
        }

        if (dem1 == 0) {
            listDatDichVu.push(datDichVuModel);
        }
        else {
            dem1 = 0;
        }
        render();
    })
});

function render() {
    table = `<tr>
                <th style="font-size: 10px; width: 5%; height: 20px;">#</th>
                <th style="font-size: 10px; width: 25%; height: 20px;">Tên DV</th>
                <th style="font-size: 10px; width: 20%; height: 20px;">S.Lượng</th>
                <th style="font-size: 10px; width: 15%; height: 20px;">Đơn Giá</th>
                <th style="font-size: 10px; width: 15%; height: 20px;">Thành Tiền</th>
                <th style="font-size: 10px; width: 20%; height: 20px;">Action</th>
            </tr>`;
    for (let i = 0; i < listDatPhong.length; i++) {
        table += `<tr data-MaDv='${listDatPhong[i].MaDv}' style="width: 100%; height: 20px;">
                    <td style="font-size: 12px; width: 5%; height: 20px;">${i + 1}</td>
                    <td style="font-size: 12px; width: 25%; height: 20px;">${listDatPhong[i].TenDv}</td>
                    <td style="font-size: 12px; width: 20%; height: 20px;" class="text-center">
                        <div class="input-group number-spinner">
                            <span class="input-group-btn">
                                <button class="btn btn-default p-0 m-0" data-dir="dwn"><span class="bi bi-dash"></span></button>
                            </span>
                            <input class="valueInputHidden" value='${listDatPhong[i].MaDv}' hidden>
                            <input style="font-size: 12px;" type="number" min="1" class="form-control rounded text-center p-0 m-0 removearrows" value='${listDatPhong[i].SoLuong}'>
                            <span class="input-group-btn">
                                <button class="btn btn-default p-0 m-0" data-dir="up"><span class="bi bi-plus"></span></button>
                            </span>
                        </div>
                    </td>
                    <td style="font-size: 12px; width: 15%; height: 20px;">${listDatPhong[i].DonGia}</td>
                    <td style="font-size: 12px; width: 15%; height: 20px;">${listDatPhong[i].ThanhTien}</td>
                    <td style="font-size: 12px; width: 20%; height: 20px;"><button class="bi bi-trash btn btn-outline-danger btnXoa" value='${listDatPhong[i].MaDv}'></button></td>
                </tr>`
    }
    document.getElementById('tableRender').innerHTML = table;

    listDatPhong.forEach(element => {
        const btnXoa = document.querySelector(`[data-MaDv = '${element.MaDv}'] .btnXoa`);
        btnXoa.addEventListener('click', (e) => {
            listDatDichVu.splice(listDatDichVu.findIndex(e => (e.MaDv + "") === btnXoa.value), 1);
            listDatPhong.splice(listDatPhong.findIndex(e => (e.MaDv + "") === btnXoa.value), 1);
            render();
        });



        const inputChangeSoLuong = document.querySelector(`[data-MaDv = '${element.MaDv}'] .removearrows`);
        const valueInputHidden = document.querySelector(`[data-MaDv = '${element.MaDv}'] .valueInputHidden`);
        inputChangeSoLuong.addEventListener('change', () => {
            for(let i = 0 ; i < listDatPhong.length ; i++)
            {
                if ((listDatPhong[i].MaDv + "") === valueInputHidden.value)
                {
                    listDatPhong[i].SoLuong = inputChangeSoLuong.value;
                    listDatPhong[i].ThanhTien = listDatPhong[i].SoLuong * listDatPhong[i].DonGia;
                    console.log(listDatPhong[i].ThanhTien);
                }
            }

            for(let i = 0 ; i < listDatPhong.length ; i++)
            {
                if ((listDatDichVu[i].MaDv + "") === valueInputHidden.value)
                {
                    var donGia = (listDatDichVu[i].TongTien/listDatDichVu[i].SoLuong);
                    console.log(donGia);
                    listDatDichVu[i].SoLuong = inputChangeSoLuong.value;
                    listDatDichVu[i].TongTien = listDatDichVu[i].SoLuong * donGia;
                    console.log(listDatDichVu[i].TongTien);
                }
            }
            render();
        });
    });

    var data = "";
    for (let i = 0; i < listDatDichVu.length; i++) {
        if (i == listDatDichVu.length - 1)
        {
            data += (listDatDichVu[i].SoLuong + "," + listDatDichVu[i].NgayDatDichVu + "," + listDatDichVu[i].MaDv + "," + listDatDichVu[i].MaKh + "," + listDatDichVu[i].MaDp + "," + listDatDichVu[i].TongTien);
        }
        else
        {
            data += (listDatDichVu[i].SoLuong + "," + listDatDichVu[i].NgayDatDichVu + "," + listDatDichVu[i].MaDv + "," + listDatDichVu[i].MaKh + "," + listDatDichVu[i].MaDp + "," + listDatDichVu[i].TongTien) + ";";
        }
    }
    $('#passData').val(data); 
}


// binding list object to controller using ajax
// $("#idFormDatDichVu").submit(function (e) {
//     // e.preventDefault();
//     var data = [];
//     for (let i = 0; i < listDatDichVu.length; i++) {
//         var a = (listDatDichVu[i].SoLuong + "," + listDatDichVu[i].NgayDatDichVu + "," + listDatDichVu[i].MaDv + "," + listDatDichVu[i].MaKh + "," + listDatDichVu[i].MaDp + "," + listDatDichVu[i].TongTien);
//         data.push(a);
//     }
//     console.log(data);
//     $.ajax({
//         type: 'post',
//         dataType: 'json',
//         url: '/DatDichVu/Index',
//         traditional: true,
//         data: {
//             firstName: "TamHoang",
//             danhSachDatPhong: data
//         },
//         success: function () {
//             location.reload();
//             // $('#result').html('successfully called.');
//         },
//         failure: function (response) {
//             $('#result').html(response);
//         }
//     });

// });


// custom input number
$(document).on('click', '.number-spinner button', function () {
    var btn = $(this),
        oldValue = btn.closest('.number-spinner').find('input').val().trim(),
        newVal = 0;

    if (btn.attr('data-dir') == 'up') {
        newVal = parseInt(oldValue) + 1;
    } else {
        if (oldValue > 1) {
            newVal = parseInt(oldValue) - 1;
        } else {
            newVal = 1;
        }
    }
    btn.closest('.number-spinner').find('input').val(newVal);
});

// checked all dich vu
const checkAll = document.querySelector('#checkAllDichVu');
const checkBox = document.querySelectorAll('.checkDichVu');
checkAll.addEventListener('change', (e) => {
    // console.log('dachange');
    if (checkAll.checked === true) {
        checkBox.forEach(cb => cb.checked = true);
        document.querySelector('#btnThemHangLoat').style.display = "block";
    }
    else if (checkAll.checked === false) {
        checkBox.forEach(cb => cb.checked = false);
        document.querySelector('#btnThemHangLoat').style.display = "none";
    }
});
//show button dathangloat
document.querySelector('#btnThemHangLoat').style.display = "none";
checkBox.forEach(element => {
    element.addEventListener('change', () => {
        if (element.checked == true) {
            document.querySelector('#btnThemHangLoat').style.display = "block";
        }
        else if (element.checked == false) {
            let dem = 0;
            checkBox.forEach(element => {
                if (element.checked == true) {
                    dem++;
                }
            });
            if (dem == 0) {
                document.querySelector('#btnThemHangLoat').style.display = "none";
            }
            else {
                document.querySelector('#btnThemHangLoat').style.display = "block";
            }
        }
    });
});

document.querySelector('#btnThemHangLoat').addEventListener('click', () => {
    checkBox.forEach(check => {

        if (check.checked == true) {
            var id = (check.value);
            var datPhongModel = {
                TenDv: document.getElementById('TenDv' + id).value,
                SoLuong: parseInt(document.getElementById('SoLuong' + id).value),
                DonGia: parseFloat(document.getElementById('DonGia' + id).value),
                ThanhTien: parseFloat(document.getElementById('ThanhTien' + id).value),
                MaDv: parseInt(document.getElementById('MaDv' + id).value)
            };

            var datDichVuModel = {
                SoLuong: parseInt(document.getElementById('SoLuong' + id).value),
                NgayDatDichVu: document.getElementById('NgayDatDV' + id).value,
                MaDv: parseInt(document.getElementById('MaDv' + id).value),
                MaKh: parseInt(document.getElementById('MaKh' + id).value),
                MaDp: parseInt(document.getElementById('MaDp' + id).value),
                TongTien: parseFloat(document.getElementById('ThanhTien' + id).value)
            };


            let dem = 0;
            for (let i = 0; i < listDatPhong.length; i++) {
                if (listDatPhong[i].TenDv === datPhongModel.TenDv) {
                    dem++;
                    listDatPhong[i].SoLuong += 1;
                    listDatPhong[i].ThanhTien = listDatPhong[i].SoLuong * listDatPhong[i].DonGia;
                }
            }
            if (dem == 0) {
                listDatPhong.push(datPhongModel);
            }
            else {
                dem = 0;
            }

            let dem1 = 0;
            for (let j = 0; j < listDatDichVu.length; j++) {
                if (listDatDichVu[j].MaDv === datDichVuModel.MaDv) {
                    listDatDichVu[j].SoLuong += 1;
                    listDatDichVu[j].TongTien = (listDatDichVu[j].TongTien / (listDatDichVu[j].SoLuong - 1)) * listDatDichVu[j].SoLuong;
                }
            }

            if (dem1 == 0) {
                listDatDichVu.push(datDichVuModel);
            }
            else {
                dem1 = 0;
            }
            render();
        }
    });
});
