const btnPdf = document.querySelector('#btnPdf');
btnPdf.addEventListener('click', (e) => {
    e.preventDefault();
    console.log('OK');
    // var strHtml = document.querySelector('#pdfContainer').innerHTML;
    // strHtml = strHtml.replace(/</g, "StrTag").replace(/>/g, "EndTag");
    // console.log(strHtml);
    // document.querySelector('#pdfValue').value = strHtml;
    // document.querySelector('#formPDF').submit();



    //credit : https://ekoopmans.github.io/html2pdf.js

    var element = document.querySelector('#pdfContainer');

    var check = document.querySelector('#check');

    //easy
    //html2pdf().from(element).save();

    //custom file name
    //html2pdf().set({filename: 'code_with_mark_'+js.AutoCode()+'.pdf'}).from(element).save();

    //more custom settings
    var opt =
    {
        margin: 1,
        filename: (check ? 'hoadon' : 'hoadondichvu') + (new Date()).toString() + '.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
    };

    // New Promise-based usage:
    html2pdf().set(opt).from(element).save();
});