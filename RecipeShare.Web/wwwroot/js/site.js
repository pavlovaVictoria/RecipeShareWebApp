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
        var csrfToken = $('#csrfToken').val();

        $.ajax({
            url: url,
            type: 'POST',
            data: { recipeId: recipeId },
            headers: {
                'RequestVerificationToken': csrfToken
            },
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

document.addEventListener('DOMContentLoaded', function () {
    let rowIndex = document.querySelectorAll('#productTable tbody tr').length;

    document.getElementById('addRowBtn').addEventListener('click', function () {
        const tableBody = document.querySelector('#productTable tbody');
        const firstRow = tableBody.querySelector('tr:first-child');

        
        const newRow = firstRow.cloneNode(true);

        
        newRow.querySelectorAll('select, input').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                
                const updatedName = name.replace(/\[\d+\]/, `[${rowIndex}]`);
                input.setAttribute('name', updatedName);
            }

            
            if (input.tagName === 'INPUT') {
                input.value = '';
            } else if (input.tagName === 'SELECT') {
                input.selectedIndex = 0;
            }
        });

        
        tableBody.appendChild(newRow);

        rowIndex++;
    });

    document.querySelector('#productTable').addEventListener('click', function (e) {
        if (e.target.classList.contains('btn-remove-row')) {
            const row = e.target.closest('tr');
            const tableBody = row.parentElement;

            
            if (tableBody.children.length > 1) {
                row.remove();

                // Optional: Recalculate row indexes for debugging or strict validation
                Array.from(tableBody.children).forEach((row, index) => {
                    row.querySelectorAll('select, input').forEach(input => {
                        const name = input.getAttribute('name');
                        if (name) {
                            const updatedName = name.replace(/\[\d+\]/, `[${index}]`);
                            input.setAttribute('name', updatedName);
                        }
                    });
                });

                rowIndex = tableBody.children.length;
            }
        }
    });
});
