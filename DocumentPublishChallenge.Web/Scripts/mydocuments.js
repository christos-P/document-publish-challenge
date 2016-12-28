$(function() {
    $.ajax({
            type: 'GET',
            cache: false,
            url: 'http://localhost:8080/api/document/getuserdocuments',
            headers: { "api_key": "X-some-key" }
        })
        .done(function (e) {
            if (e.length) {
                debugger;
                var data = JSON.parse(e);
                console.log(data);
                var template = $("#document-template").html();
                var documents = $(".js-documents");
                data.forEach(function(item) {
                    var document = Mustache.render(template, item);
                    documents.append(document);
                });
                //$(".js-documents").show();
            } 
        })
        .fail(function(e) {
            $(".js-message").append(e).show();
        });
});