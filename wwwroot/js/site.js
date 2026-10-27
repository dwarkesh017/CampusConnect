// Small progressive enhancement: confirm before deleting a report.
document.querySelectorAll('form[action*="Delete"]').forEach(form => {
  form.addEventListener('submit', event => {
    if (!confirm('Delete this report? This action cannot be undone.')) event.preventDefault();
  });
});
