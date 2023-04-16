

var listButtonChiTietNV = document.querySelectorAll('.btnChiTiet');
listButton.forEach(btn => {
    btn.addEventListener('click', () => {
        var MaNv = btn.value;
    });
})