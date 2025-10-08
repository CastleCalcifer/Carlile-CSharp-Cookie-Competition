// voting-client.js
const YEAR = 2024; // or compute dynamically
const cookiesContainer = document.getElementById('cookiesContainer');

async function fetchCookies(year) {
    // Build absolute URL using the page origin to avoid surprises
    const url = `${window.location.origin}/api/cookies?year=${year}`;
    console.log('[debug] fetching cookies from:', url);

    const res = await fetch(url);
    // log status and headers
    console.log('[debug] fetch status:', res.status, res.statusText);
    for (const [k, v] of res.headers) console.log(`[debug] res header: ${k} = ${v}`);

    const text = await res.text();
    console.log('[debug] raw response body:', text);

    // try to parse JSON, but surface parse errors
    try {
        const payload = JSON.parse(text);
        if (!payload.success) throw new Error(payload.message || 'API returned success:false');
        return payload.data;
    } catch (err) {
        throw new Error(`Failed to get cookies: ${err.message} (raw: ${text})`);
    }
}

function createCookieCard(cookie, idx) {
    // cookie: { id, cookieName, imageUrl }
    const wrapper = document.createElement('div');
    wrapper.className = 'd-flex flex-column justify-content-center align-items-center cookieOption p-2';

    const imgDiv = document.createElement('div');
    imgDiv.className = 'p-2';
    const img = document.createElement('img');
    img.src = cookie.image;
    console.log(cookie.image);
    img.className = 'cookieImage';
    imgDiv.appendChild(img);

    const labelDiv = document.createElement('div');
    labelDiv.className = 'd-inline-flex p-2 gap-4';
    const p = document.createElement('p');
    p.textContent = cookie.cookieName;
    const selectContainer = document.createElement('div');
    selectContainer.className = 'rankDropdown';

    // Create select element, add rank-select class and data-index
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

    // create ranking options (1..N)
    const ranks = 4; // or number of cookies
    for (let r = 1; r <= ranks; r++) {
        const opt = document.createElement('option');
        opt.value = String(r);        // value could also be the rank or cookie id; we'll collect cookie ids separately
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
        const cookies = await fetchCookies(YEAR);
        // clear container
        cookiesContainer.innerHTML = '';

        // render cookie cards
        cookies.forEach((c, idx) => {
            const card = createCookieCard(c, idx);
            cookiesContainer.appendChild(card);
        });

        // After DOM is built, the uniqueSelect.js (which expects selects with class rank-select)
        // should initialize automatically if it's designed that way. If not, expose a function in it to call.
        // e.g., if it exposes window.initRankSelects(), call it here:
        if (window.initRankSelects) window.initRankSelects();

    } catch (err) {
        console.error(err);
        cookiesContainer.innerHTML = '<p>Failed to load cookies.</p>';
    }
}

document.getElementById('submitVotes').addEventListener('click', async () => {
    // collect selected cookieIds in order of the rank
    // we used selects where each select corresponds to a cookie; we need to transform into an ordered list
    // Approach: read each select's selected rank and build array at position rank-1 with cookieId
    const selects = Array.from(document.querySelectorAll('select.rank-select'));
    // Build array of length selects.length filled with null
    const ordered = new Array(selects.length).fill(null);

    for (const s of selects) {
        const rankVal = s.value; // '' or '1'..'N'
        const cookieId = Number(s.dataset.cookieId);
        if (rankVal) {
            const idx = Number(rankVal) - 1; // position in ranking
            ordered[idx] = cookieId;
        }
    }

    // Validate: ensure required ranks are filled (e.g. no nulls)
    if (ordered.some(v => v === null)) {
        alert('Please select all ranks before submitting.');
        return;
    }

    // Prepare request
    const req = {
        year: YEAR,
        cookieIds: ordered,
        voterId: null
    };

    try {
        const res = await fetch('/api/votes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(req),
        });
        const payload = await res.json();
        if (!payload.success) throw new Error(payload.message || 'Failed to submit votes');
        // redirect to awards page (client-side)
        window.location.href = '/awards.html'; // or a route you implemented
    } catch (err) {
        console.error(err);
        alert('Error submitting votes. Please try again.');
    }
});

initPage();
