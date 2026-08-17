$(function () {
    $('.mobile-toggle').on('click', function () { $('.sidebar').toggleClass('open'); });
    $('#globalSearch').on('keydown', function (event) {
        if (event.key === 'Enter') {
            const query = $(this).val().trim();
            if (query.length) window.location.href = '/Projects?search=' + encodeURIComponent(query);
        }
    });
    $(document).on('keydown', function (event) {
        if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
            event.preventDefault(); $('#globalSearch').trigger('focus');
        }
    });
    $('.search-form').on('submit', function () {
        $(this).find('button').prop('disabled', true).text('Searching…');
    });
    $('.status-select').on('change', function () { $(this).closest('form').trigger('submit'); });
    $('.form-control, .form-select').on('focus', function () { $(this).closest('.modal-body').find('.field-error').remove(); });
});
