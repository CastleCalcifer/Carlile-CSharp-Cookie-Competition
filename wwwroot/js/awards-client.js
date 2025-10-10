// awards-client.js (instrumented + robust)
// Purpose: log baker-check step and ensure cookies fetch uses excludeBakerId when available

const YEAR = 2024;
const creativeSelect = document.getElementById('mostCreative');
const presentationSelect = document.getElementById('bestPresentation');
const awardImage0 = document.getElementById('awardImage0');
const awardImage1 = document.getElementById('awardImage1');
const errorMsg = document.getElementById('errorMsg');

if (!creativeSelect || !presentationSelect || !awardImage0 || !awardImage1 || !errorMsg) {
    console.warn('awards-client: some DOM elements are missing — check IDs (mostCreative, bestPresentation, awardImage0/1, errorMsg).');
}

// fetch current baker id; verbose logging
async function getCurrentBakerId() {
    try {
        console.log('[awards-client] calling /api/bakers/current ...');
        const res = await fetch('/api/bakers/current', { credentials: 'same-origin', cache: 'no-store' });
        console.log('[awards-client] /api/bakers/current status:', res.status);

        const text = await res.text();
        console.log('[awards-client] /api/bakers/current raw body:', text);

        try {
            const payload = JSON.parse(text);
            console.log('[awards-client] /api/bakers/current parsed payload:', payload);
            const id = payload?.data?.id ?? null;
            console.log('[awards-client] derived bakerId:', id);
            return id;
        } catch (parseErr) {
            console.error('[awards-client] JSON parse error for /api/bakers/current:', parseErr);
            return null;
        }
    } catch (err) {
        console.warn('[awards-client] network error fetching /api/bakers/current:', err);
        return null;
    }
}

// fetch cookies with optional exclude baker id; verbose logging
async function fetchCookies(year, excludeBakerId = null) {
    const base = window.location.origin;
    const url = excludeBakerId ? `${base}/api/cookies?year=${year}&excludeBakerId=${excludeBakerId}` : `${base}/api/cookies?year=${year}`;
    console.log('[awards-client] fetching cookies URL ->', url);

    try {
        const res = await fetch(url, { credentials: 'same-origin', cache: 'no-store' });
        console.log('[awards-client] cookies fetch status:', res.status);
        const text = await res.text();
        console.log('[awards-client] cookies raw body:', text);

        try {
            const payload = JSON.parse(text);
            const cookies = Array.isArray(payload) ? payload : (payload.data ?? []);
            console.log('[awards-client] cookies parsed count:', cookies.length);
            return cookies;
        } catch (err) {
            throw new Error(`Failed to parse cookies JSON: ${err.message} (raw: ${text})`);
        }
    } catch (err) {
        throw err;
    }
}

function clearSelect(sel) {
    while (sel.options.length) sel.remove(0);
}

function populateSelects(cookies) {
    [creativeSelect, presentationSelect].forEach(s => { clearSelect(s); });

    const placeholder0 = new Option('--- Select cookie ---', '');
    placeholder0.selected = true;
    creativeSelect.add(placeholder0);
    const placeholder1 = new Option('--- Select cookie ---', '');
    placeholder1.selected = true;
    presentationSelect.add(placeholder1);

    if (!Array.isArray(cookies) || cookies.length === 0) return;

    cookies.forEach(c => {
        const imageUrl = c.imageUrl ?? c.image ?? (c.imageFileName ? `/images/${c.imageFileName}` : '/images/placeholder.jpg');

        const opt1 = new Option(c.cookieName, c.id);
        opt1.dataset.image = imageUrl;
        creativeSelect.add(opt1);

        const opt2 = new Option(c.cookieName, c.id);
        opt2.dataset.image = imageUrl;
        presentationSelect.add(opt2);
    });

    creativeSelect.addEventListener('change', e => {
        const sel = e.target;
        const img = sel.selectedOptions[0]?.dataset?.image;
        if (img) awardImage0.src = img;
    });
    presentationSelect.addEventListener('change', e => {
        const sel = e.target;
        const img = sel.selectedOptions[0]?.dataset?.image;
        if (img) awardImage1.src = img;
    });
}

document.getElementById('submitAwards').addEventListener('click', async () => {
    errorMsg.textContent = '';
    const mostCreativeId = Number(creativeSelect.value || 0);
    const bestPresentationId = Number(presentationSelect.value || 0);
    if (!mostCreativeId || !bestPresentationId) {
        errorMsg.textContent = 'Please select a cookie for both awards.';
        return;
    }

    if (mostCreativeId === bestPresentationId) {
        if (!confirm('You selected the same cookie for both awards. Continue?')) return;
    }

    const voterIdInput = document.getElementById('voterId');
    const req = {
        year: YEAR,
        mostCreativeId: mostCreativeId,
        bestPresentationId: bestPresentationId,
        voterId: voterIdInput ? voterIdInput.value.trim() : null
    };

    try {
        const base = window.location.origin;
        const res = await fetch(`${base}/api/awards`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'same-origin',
            body: JSON.stringify(req)
        });

        const payload = await res.json();
        if (!res.ok) {
            const msg = payload?.message || `Server returned ${res.status}`;
            errorMsg.textContent = `Error: ${msg}`;
            return;
        }

        alert('Thanks! Awards recorded.');
        window.location.href = '/results.html';
    } catch (err) {
        console.error('Error submitting awards', err);
        errorMsg.textContent = 'Network error submitting awards.';
    }
});

// main init: try once, if result undefined/null, retry once after small delay
async function initAwards() {
    try {
        console.log('[awards-client] initAwards start');
        let bakerId = await getCurrentBakerId();

        // If bakerId is null *and* you are sure a baker is logged in, retry once
        if (bakerId == null) {
            console.log('[awards-client] bakerId null, retrying GET /api/bakers/current in 250ms');
            await new Promise(r => setTimeout(r, 250));
            bakerId = await getCurrentBakerId();
        }

        console.log('[awards-client] final bakerId:', bakerId);

        const cookies = await fetchCookies(YEAR, bakerId);
        populateSelects(cookies);
    } catch (err) {
        console.error('Failed to load cookies', err);
        if (errorMsg) errorMsg.textContent = 'Failed to load cookies. Try reloading.';
    }
}

initAwards();
