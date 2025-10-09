// awards-client.js
const YEAR = 2024; // adjust as needed or compute dynamically
const creativeSelect = document.getElementById('mostCreative');
const presentationSelect = document.getElementById('bestPresentation');
const awardImage0 = document.getElementById('awardImage0');
const awardImage1 = document.getElementById('awardImage1');
const errorMsg = document.getElementById('errorMsg');

// fetch cookies and populate selects
async function initAwards() {
    try {
        const base = window.location.origin;
        const res = await fetch(`${base}/api/cookies?year=${YEAR}`);
        if (!res.ok) throw new Error(`Server returned ${res.status}`);
        const payload = await res.json();
        // accept either enveloped { success, data } or bare array
        const cookies = Array.isArray(payload) ? payload : (payload.data ?? []);
        populateSelects(cookies);
    } catch (err) {
        console.error('Failed to load cookies', err);
        errorMsg.textContent = 'Failed to load cookies. Try reloading.';
    }
}

function populateSelects(cookies) {
    // clear
    [creativeSelect, presentationSelect].forEach(s => s.innerHTML = '');
    // placeholder
    const placeholder0 = new Option('--- Select cookie ---', '');
    placeholder0.selected = true;
    creativeSelect.add(placeholder0);
    const placeholder1 = new Option('--- Select cookie ---', '');
    placeholder1.selected = true;
    presentationSelect.add(placeholder1);

    cookies.forEach(c => {
        const opt1 = new Option(c.cookieName, c.id);
        opt1.dataset.image = c.imageUrl || c.image || `/images/${c.imageFileName ?? ''}`;
        creativeSelect.add(opt1);

        const opt2 = new Option(c.cookieName, c.id);
        opt2.dataset.image = opt1.dataset.image;
        presentationSelect.add(opt2);
    });

    // update image on change
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

// submit handler
document.getElementById('submitAwards').addEventListener('click', async () => {
    errorMsg.textContent = '';
    const mostCreativeId = Number(creativeSelect.value || 0);
    const bestPresentationId = Number(presentationSelect.value || 0);
    if (!mostCreativeId || !bestPresentationId) {
        errorMsg.textContent = 'Please select a cookie for both awards.';
        return;
    }
    // optional: do not allow same cookie for both awards
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
            body: JSON.stringify(req)
        });

        const payload = await res.json();
        if (!res.ok) {
            const msg = payload?.message || `Server returned ${res.status}`;
            errorMsg.textContent = `Error: ${msg}`;
            return;
        }

        // success: redirect or show message
        alert('Thanks! Awards recorded.');
        window.location.href = '/results.html'; // adjust to your awards/results page
    } catch (err) {
        console.error('Error submitting awards', err);
        errorMsg.textContent = 'Network error submitting awards.';
    }
});

// Initialize page
initAwards();
