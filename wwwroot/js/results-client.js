// results-client.js
const YEAR = 2024; // change or compute dynamically if you like
const placementsEl = document.getElementById('placements');
const awardsEl = document.getElementById('awards');
const errorEl = document.getElementById('resultsError');

async function fetchResults(year) {
    const base = window.location.origin;
    const url = `${base}/api/results?year=${year}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`Server returned ${res.status}`);
    const payload = await res.json();
    // accept envelope or bare data
    return Array.isArray(payload) ? payload : (payload.data ?? payload);
}

function makeCookieCard(positionLabel, cookie) {
    const div = document.createElement('div');
    div.className = 'cookieOption';
    // use provided imageUrl or build from imageFileName
    const imageUrl = cookie.imageUrl ?? (cookie.imageFileName ? `/images/${cookie.imageFileName}` : '/images/placeholder.jpg');
    div.innerHTML = `
    <div class="p-2">
      <h2>${positionLabel}</h2>
      <h3>${escapeHtml(cookie.cookieName)}</h3>
      <img src="${imageUrl}" alt="${escapeHtml(cookie.cookieName)}" class="cookieImage"/>
      <h4>Total Score: ${cookie.score ?? cookie.Score ?? 0}</h4>
    </div>
  `;
    return div;
}

function makeAwardCard(title, cookie, pointsField) {
    const div = document.createElement('div');
    div.className = 'cookieOption';
    const imageUrl = cookie?.imageUrl ?? (cookie?.imageFileName ? `/images/${cookie.imageFileName}` : '/images/placeholder.jpg');
    const points = (cookie && (cookie[pointsField] ?? 0)) ?? 0;
    div.innerHTML = `
    <div class="p-2">
      <h2>${title}</h2>
      <h3>${cookie ? escapeHtml(cookie.cookieName) : '—'}</h3>
      <img src="${imageUrl}" alt="${cookie ? escapeHtml(cookie.cookieName) : ''}" class="award-image"/>
      <h4>Total Score: ${points}</h4>
    </div>
  `;
    return div;
}

// simple HTML escape
function escapeHtml(s) {
    if (!s) return '';
    return s.replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": "&#39;" }[c]));
}

async function initPage() {
    try {
        const result = await fetchResults(YEAR);

        const ranked = result.ranked ?? result; // if endpoint returns array directly it's the ranked list
        const awards = result.awards ?? {};

        const sorted = Array.isArray(ranked) ? ranked.slice().sort((a, b) => (b.score ?? b.Score ?? 0) - (a.score ?? a.Score ?? 0)) : [];

        // pick top 4 (if there are more, take first 4 highest)
        const top4Desc = sorted.slice(0, 4);
        // create display order: 4th -> 1st
        const displayOrder = top4Desc.slice().reverse();

        placementsEl.innerHTML = '';
        const placeLabels = ['4th Place', '3rd Place', '2nd Place', '1st Place'];
        for (let i = 0; i < 4; i++) {
            const cookie = displayOrder[i] ?? { cookieName: '—', score: 0, imageUrl: '/images/placeholder.jpg' };
            placementsEl.appendChild(makeCookieCard(placeLabels[i], cookie));
        }

        // Awards: presentation and creative winners (controller returns objects or null)
        awardsEl.innerHTML = '';
        const presentation = awards.presentation ?? null;
        const creative = awards.creative ?? null;

        awardsEl.appendChild(makeAwardCard('Best Presentation', presentation, 'presentationPoints' in (presentation ?? {}) ? 'presentationPoints' : 'presentation_points'));
        awardsEl.appendChild(makeAwardCard('Most Creative', creative, 'creativePoints' in (creative ?? {}) ? 'creativePoints' : 'creative_points'));
    } catch (err) {
        console.error('Failed to load results', err);
        errorEl.textContent = 'Failed to load results. Try reloading.';
    }
}

document.addEventListener('DOMContentLoaded', initPage);
