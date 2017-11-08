(function (undefined) {
    loop();

    function rewriteTitles() {
        var $info = $('.info_title', '#api_info');
        $('#logo').attr('href', '/').text($info.text());
        $info.text('.NET Services for Data Mart');
    }

    function loop() {
        if ($('#logo').text() === 'swagger') {
            rewriteTitles();
            setTimeout(loop, 100);
        }
    }

    window.document.title = 'HLI Datamart Services - Swagger UI';
})();
