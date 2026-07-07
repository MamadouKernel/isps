// Confirmation avant soumission pour tout formulaire portant data-confirm="message"
// (boutons de suppression / corbeille). Le message est un attribut HTML classique,
// donc échappé automatiquement par Razor — pas de JS inline à écrire par vue.
document.addEventListener('submit', function (e) {
    var form = e.target;
    var message = form.getAttribute && form.getAttribute('data-confirm');
    if (message && !confirm(message)) {
        e.preventDefault();
    }
});
