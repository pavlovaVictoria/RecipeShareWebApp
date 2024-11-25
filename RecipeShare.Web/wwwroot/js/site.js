function goBack() {
    sessionStorage.setItem('refreshPrevious', 'true');
    window.history.back();
}
window.addEventListener('load', function () {
    if (sessionStorage.getItem('refreshPrevious') === 'true') {
        sessionStorage.removeItem('refreshPrevious');
        window.location.reload();
    }
});

$(function () {
    $(document).on('click', '.like-btn', function () {
        var button = $(this);
        var recipeId = button.data('recipe-id');
        var likesCount = button.siblings('.likes-count');
        var url = button.data('url')

        $.ajax({
            url: url,
            type: 'POST',
            data: { recipeId: recipeId },
            success: function (response) {
                if (response.success) {
                    if (response.isLiked) {
                        button.text("Unlike");
                    } else {
                        button.text("Like");
                    }

                    likesCount.text(response.likes);
                } else if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
            },
            error: function () {
                window.location.href = 'Error/500';
            }
        });
    });
});
