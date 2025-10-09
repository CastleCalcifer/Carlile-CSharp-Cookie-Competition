
document.addEventListener('DOMContentLoaded', async () => {
    const headerContainer = document.getElementById('header-container');
    if (!headerContainer) return;

    try {
        const res = await fetch('/_header.html');
        const html = await res.text();
        headerContainer.innerHTML = html;
    } catch (err) {
        console.error('Failed to load header:', err);
    }
});
