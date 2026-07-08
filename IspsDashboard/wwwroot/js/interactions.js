// Écouteurs délégués pour les motifs jusqu'ici en attributs onclick="" inline (bloqués par la
// Content-Security-Policy : script-src n'autorise plus 'unsafe-inline', voir Program.cs).
(function () {
    document.addEventListener('click', function (e) {
        var confirmEl = e.target.closest('[data-confirm]');
        if (confirmEl && !window.confirm(confirmEl.getAttribute('data-confirm'))) {
            e.preventDefault();
            return;
        }

        var rowEl = e.target.closest('[data-href]');
        if (rowEl && !e.target.closest('a, button, input, select, textarea, label')) {
            window.location = rowEl.getAttribute('data-href');
        }
    });
})();
