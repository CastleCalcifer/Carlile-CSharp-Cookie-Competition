/**
 * Handles the baker login registration and submission logic.
 * Logins in or registers pin, then gives the baker access to the voting page using cookies.
 */
let selectedBakerName = null;

async function loadBakers() {
    const res = await fetch('/api/bakers');
    const payload = await res.json();
    const bakers = payload.data ?? payload;
    const container = document.getElementById('bakerList');
    container.innerHTML = '';
    console.log("LoadBakers()");
    bakers.forEach(b => {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'list-group-item list-group-item-action';
        btn.textContent = b.bakerName + (b.hasPin ? '' : ' (set PIN)');
        btn.onclick = () => openPinModal(b.bakerName, b.hasPin);
        container.appendChild(btn);
        console.log(b.bakerName);
    });
}

function openPinModal(bakerName, hasPin) {
    selectedBakerName = bakerName;
    document.getElementById('modalText').textContent = hasPin ? `Enter PIN for ${bakerName}` : `No PIN found for ${bakerName}. Create a PIN to claim this account.`;
    document.getElementById('pinInput').value = '';
    document.getElementById('pinError').textContent = '';
    // show bootstrap modal manually
    const modal = new bootstrap.Modal(document.getElementById('pinModal'));
    modal.show();
    document.getElementById('pinSubmit').onclick = () => submitPin(modal);
    document.getElementById('pinCancel').onclick = () => modal.hide();
}

async function submitPin(modal) {
    const pin = document.getElementById('pinInput').value.trim();
    if (!pin) {
        document.getElementById('pinError').textContent = 'PIN required';
        return;
    }
    try {
        const res = await fetch('/api/bakers/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ bakerName: selectedBakerName, pin })
        });
        const payload = await res.json();
        if (!res.ok) {
            document.getElementById('pinError').textContent = payload.message || 'Invalid PIN';
            return;
        }

        // success: payload.message may indicate created or login
        modal.hide();
        window.location.href = `/baker/area`;
    } catch (err) {
        document.getElementById('pinError').textContent = 'Network error';
    }
}

// On page load
document.addEventListener('DOMContentLoaded', () => {
    loadBakers();
});
