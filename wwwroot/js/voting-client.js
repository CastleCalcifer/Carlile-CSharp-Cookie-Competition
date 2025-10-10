// voting-client.js (updated)
// Assumes there is a container with id="cookiesContainer" and a submit button id="submitVotes"
const YEAR = 2024; // or compute dynamically
const cookiesContainer = document.getElementById('cookiesContainer');
const submitBtn = document.getElementById('submitVotes');

if (!cookiesContainer) {
    console.error('No #cookiesContainer element found on page.');
}

// Fetch cookies with optional exclude baker id
async function fetchCookies(year, excludeBakerId = null) {
    const base = window.location.origin;
    const url = excludeBakerId
        ? `${base}/api/cookies?year=${year}&excludeBakerId=${excludeBakerId}`
        : `${base}/api/cookies?year=${year}`;

    console.log('[debug] fetching cookies from:', url);
    const res = await fetch(url, { credentials: 'same-origin' });
    console.log('[debug] fetch status:', res.status, res.statusText);

    const text = await res.text();
    console.log('[debug] raw response body:', text);

    try {
        const payload = JSON.parse(text);
        // if server uses envelope { success: true, data: [...] }
        const data = payload?.data ?? payload;
        return data;
    } catch (err) {
        throw new Error(`Failed to parse cookies JSON: ${err.message} (raw: ${text})`);
    }
}

function createCookieCard(cookie, idx, ranks) {
    // cookie: { id, cookieName, imageUrl }
    const wrapper = document.createElement('div');
    wrapper.className = 'd-flex flex-column justify-content-center align-items-center cookieOption p-2';

    const imgDiv = document.createElement('div');
    imgDiv.className = 'p-2';
    const img = document.createElement('img');
    img.src = cookie.imageUrl || cookie.image || '/images/placeholder.jpg';
    img.className = 'cookieImage';
    imgDiv.appendChild(img);

    const labelDiv = document.createElement('div');
    labelDiv.className = 'd-inline-flex p-2 gap-4';
    const p = document.createElement('p');
    p.textContent = cookie.cookieName;
    const selectContainer = document.createElement('div');
    selectContainer.className = 'rankDropdown';

    // Create select element, add rank-select class and data attributes
    const select = document.createElement('select');
    select.className = 'rank-select form-select';
    select.dataset.index = idx;
    select.dataset.cookieId = cookie.id;

    // placeholder option
    const placeholder = document.createElement('option');
    placeholder.value = '';
    placeholder.text = '---';
    placeholder.selected = true;
    select.appendChild(placeholder);

    // create ranking options (1..ranks)
    for (let r = 1; r <= ranks; r++) {
        const opt = document.createElement('option');
        opt.value = String(r);
        opt.className = 'ranking';
        opt.textContent = `${r}${r === 1 ? 'st' : r === 2 ? 'nd' : r === 3 ? 'rd' : 'th'}`;
        select.appendChild(opt);
    }

    selectContainer.appendChild(select);
    labelDiv.appendChild(p);
    labelDiv.appendChild(selectContainer);

    wrapper.appendChild(imgDiv);
    wrapper.appendChild(labelDiv);
    return wrapper;
}

async function initPage() {
    try {
        // 1) detect current baker (if any)
        let bakerId = null;
        try {
            const bres = await fetch('/api/bakers/current', { credentials: 'same-origin' });
            if (bres.ok) {
                const bp = await bres.json();
                if (bp?.data?.id) bakerId = bp.data.id;
            } else {
                console.warn('[debug] /api/bakers/current returned', bres.status);
            }
        } catch (err) {
            console.warn('[debug] failed to fetch current baker', err);
        }

        // 2) fetch cookies once, passing excludeBakerId when baker logged in
        const cookies = await fetchCookies(YEAR, bakerId);
        if (!Array.isArray(cookies) || cookies.length === 0) {
            cookiesContainer.innerHTML = '<p>No cookies available.</p>';
            return;
        }

        // 3) render cookie cards and prepare selects
        cookiesContainer.innerHTML = '';
        const ranks = cookies.length; // make ranking options match available choices

        cookies.forEach((c, idx) => {
            const card = createCookieCard(c, idx, ranks);
            cookiesContainer.appendChild(card);
        });

        // 4) Initialize unique-select behavior if provided
        // If your uniqueSelect script exposes an init function, call it here.
        if (window.initRankSelects) {
            try { window.initRankSelects(); } catch (err) { console.warn('initRankSelects failed', err); }
        } else {
            // fallback: attach a simple event to update disabled options (if uniqueSelect isn't present)
            attachSimpleUniqueSelectHandler();
        }

    } catch (err) {
        console.error(err);
        if (cookiesContainer) cookiesContainer.innerHTML = '<p>Failed to load cookies.</p>';
    }
}

// fallback simple uniqueness handler (safe to keep even if you have your own)
function attachSimpleUniqueSelectHandler() {
    const handler = (e) => {
        const selects = Array.from(document.querySelectorAll('select.rank-select'));
        // build selected values
        const selected = selects.map(s => s.value).filter(v => v);
        selects.forEach(s => {
            const options = Array.from(s.options);
            options.forEach(opt => {
                if (!opt.value) return; // placeholder
                // disable option if selected in other select
                const isSelectedElsewhere = selected.includes(opt.value) && s.value !== opt.value;
                opt.disabled = isSelectedElsewhere;
            });
        });
    };

    const selects = Array.from(document.querySelectorAll('select.rank-select'));
    selects.forEach(s => s.addEventListener('change', handler));
}

// Submit handler
submitBtn.addEventListener('click', async (e) => {
    e.preventDefault();

    const selects = Array.from(document.querySelectorAll('select.rank-select'));
    const ranksCount = selects.length;
    if (ranksCount === 0) {
        alert('No ranks to submit.');
        return;
    }

    const ordered = new Array(ranksCount).fill(null);

    for (const s of selects) {
        const rankVal = s.value; // '' or '1'..'N'
        const cookieId = Number(s.dataset.cookieId);
        if (rankVal) {
            const idx = Number(rankVal) - 1;
            // guard: ensure idx is in bounds
            if (idx >= 0 && idx < ranksCount) ordered[idx] = cookieId;
        }
    }

    if (ordered.some(v => v === null)) {
        alert('Please select all ranks before submitting.');
        return;
    }

    const req = {
        year: YEAR,
        cookieIds: ordered,
    };

    try {
        const res = await fetch(`${window.location.origin}/api/voter/vote`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'same-origin',
            body: JSON.stringify(req),
        });

        const payload = await res.json();
        if (!res.ok) {
            const msg = payload?.message || `Server returned ${res.status}`;
            alert(`Error: ${msg}`);
            return;
        }

        alert('Thanks — your vote has been recorded!');
        window.location.href = '/awards.html';
    } catch (err) {
        console.error('submit error', err);
        alert('Network error submitting votes. Please try again.');
    }
});

// kick off
initPage();
