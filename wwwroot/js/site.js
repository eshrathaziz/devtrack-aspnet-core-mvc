/* DevTrack interactions: concise, deliberate, and accessible. */
$(function () {
    $('[data-sidebar-toggle]').on('click', function () { $('#sidebar').toggleClass('open'); });
    $('[data-table-search]').on('input', function () { var query = $(this).val().toLowerCase(); $($(this).data('table-search')).find('tbody tr').each(function () { $(this).toggle($(this).text().toLowerCase().indexOf(query) > -1); }); });
    $('.comment-form').on('submit', function (event) {
        event.preventDefault(); var form = $(this); var target = $(form.data('comment-target'));
        $.post(form.attr('action'), form.serialize()).done(function (data) { target.prepend('<div class="list-row"><span class="list-marker berry"></span><div><strong class="list-title"></strong><span class="list-desc"></span></div></div>'); target.find('.list-title').first().text(data.author); target.find('.list-desc').first().text(data.comment); form.trigger('reset'); }).fail(function () { form.find('.comment-error').text('The comment could not be recorded. Check the required fields and try again.'); });
    });
});
