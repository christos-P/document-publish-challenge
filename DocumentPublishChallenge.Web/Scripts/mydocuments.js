$(function() {
    $.ajax({
            type: 'GET',
            cache: false,
            url: 'http://localhost:8080/api/document/getuserdocuments',
            data: { userId: 1 }
        })
        .done(function (e) {
            if (e.length) {
                var data = JSON.parse(e);
                var template = $("#document-template").html();
                var documents = $(".js-documents");
                data.forEach(function(item) {
                    var document = Mustache.render(template, item);
                    documents.append(document);
                });
            } 
        })
        .fail(function(e) {
            $(".js-message").append(e).show();
        });
});