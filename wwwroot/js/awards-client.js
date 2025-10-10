/**
 * Handles the awards voting UI and submission logic.
 * Fetches eligible cookies, populates dropdowns, and submits votes.
 */

const YEAR = 2024;

// DOM elements for award selection and feedback
const creativeSelect = document.getElementById('mostCreative');
const presentationSelect = document.getElementById('bestPresentation');
const awardImage0 = document.getElementById('awardImage0');
const awardImage1 = document.getElementById('awardImage1');
const errorMsg = document.getElementById('errorMsg');

// Warn if any required DOM elements are missing
if (!creativeSelect || !presentationSelect || !awardImage0 || !awardImage1 || !errorMsg) {
    console.warn('awards-client: some DOM elements are missing — check IDs (mostCreative, bestPresentation, awardImage0/1, errorMsg).');
}

/**
 * Fetches the current baker's ID from the server.
 * @returns {Promise<number|null>} Baker ID or null if not found.
 */
async function getCurrentBakerId() {
    try {
        const res = await fetch('/api/bakers/current', { credentials: 'same-origin', cache: 'no-store' });
        const text = await res.text();

        try {
            const payload = JSON.parse(text);
            // Defensive: payload?.data?.id may be undefined
            return payload?.data?.id ?? null;
        } catch (parseErr) {
            console.error('[awards-client] JSON parse error for /api/bakers/current:', parseErr);
            return null;
        }
    } catch (err) {
        console.warn('[awards-client] network error fetching /api/bakers/current:', err);
        return null;
    }
}

/**
 * Fetches cookies for the given year, optionally excluding a baker's own cookies.
 * @param {number} year - Competition year.
 * @param {number|null} excludeBakerId - Baker ID to exclude, or null.
 * @returns {Promise<Array>} Array of cookie objects.
 */
async function fetchCookies(year, excludeBakerId = null) {
    const base = window.location.origin;
    const url = excludeBakerId
        ? `${base}/api/cookies?year=${year}&excludeBakerId=${excludeBakerId}`
        : `${base}/api/cookies?year=${year}`;

    try {
        const res = await fetch(url, { credentials: 'same-origin', cache: 'no-store' });
        const text = await res.text();

        try {
            const payload = JSON.parse(text);
            // API may return array or { data: [...] }
            return Array.isArray(payload) ? payload : (payload.data ?? []);
        } catch (err) {
            throw new Error(`Failed to parse cookies JSON: ${err.message} (raw: ${text})`);
        }
    } catch (err) {
        throw err;
    }
}

/**
 * Removes all options from a select element.
 * @param {HTMLSelectElement} sel
 */
function clearSelect(sel) {
    while (sel.options.length) sel.remove(0);
}

/**
 * Populates the award dropdowns with cookie options.
 * Also sets up image preview on selection.
 * @param {Array} cookies - Array of cookie objects.
 */
function populateSelects(cookies) {
    [creativeSelect, presentationSelect].forEach(s => { clearSelect(s); });

    // Add placeholder options
    const placeholder0 = new Option('--- Select cookie ---', '');
    placeholder0.selected = true;
    creativeSelect.add(placeholder0);
    const placeholder1 = new Option('--- Select cookie ---', '');
    placeholder1.selected = true;
    presentationSelect.add(placeholder1);

    if (!Array.isArray(cookies) || cookies.length === 0) return;

    cookies.forEach(c => {
        // Determine image URL for cookie
        const imageUrl = c.imageUrl ?? c.image ?? (c.imageFileName ? `/images/${c.imageFileName}` : '/images/placeholder.jpg');

        // Add option to both selects
        const opt1 = new Option(c.cookieName, c.id);
        opt1.dataset.image = imageUrl;
        creativeSelect.add(opt1);

        const opt2 = new Option(c.cookieName, c.id);
        opt2.dataset.image = imageUrl;
        presentationSelect.add(opt2);
    });

    // Show cookie image when selection changes
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

// Handle award submission
document.getElementById('submitAwards').addEventListener('click', async () => {
    errorMsg.textContent = '';
    const mostCreativeId = Number(creativeSelect.value || 0);
    const bestPresentationId = Number(presentationSelect.value || 0);

    // Validate selections
    if (!mostCreativeId || !bestPresentationId) {
        errorMsg.textContent = 'Please select a cookie for both awards.';
        return;
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

/**
 * Initializes the awards page:
 * - Fetches current baker ID (to exclude their own cookies)
 * - Loads eligible cookies and populates dropdowns
 * Retries baker ID fetch once if null.
 */
async function initAwards() {
    try {
        let bakerId = await getCurrentBakerId();

        // Retry once if bakerId is null (may be due to async login)
        if (bakerId == null) {
            await new Promise(r => setTimeout(r, 250));
            bakerId = await getCurrentBakerId();
        }

        const cookies = await fetchCookies(YEAR, bakerId);
        populateSelects(cookies);
    } catch (err) {
        console.error('Failed to load cookies', err);
        if (errorMsg) errorMsg.textContent = 'Failed to load cookies. Try reloading.';
    }
}

// Start page logic
initAwards();
