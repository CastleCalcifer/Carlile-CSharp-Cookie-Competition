// uniqueSelect.js
// Robust rank-select manager for dynamically injected selects.
// Exposes window.initRankSelects() and also auto-observes DOM changes.

(function () {
    const DEBUG = false;

    function dbg(...args) { if (DEBUG) console.debug('[uniqueSelect]', ...args); }

    // Core function to (re)initialize all managed selects.
    function initRankSelects() {
        // Query the selects that should participate
        const selects = Array.from(document.querySelectorAll('select.rank-select'));
        if (selects.length === 0) {
            dbg('No rank-selects found.');
            return;
        }

        // Ensure each select has a data-index (unique per select)
        selects.forEach((s, idx) => {
            if (s.dataset.index === undefined || s.dataset.index === '') {
                s.dataset.index = String(idx);
            }
            // ensure placeholder option value is normalized to empty string
            const firstOpt = s.querySelector('option');
            if (firstOpt && (firstOpt.value === '---')) firstOpt.value = '';
        });

        // Create selectedOptions array based on current selects
        const selectedOptions = new Array(selects.length).fill('');
        selects.forEach((s) => {
            const idx = Number(s.dataset.index);
            const val = s.value === '' ? '' : s.value;
            if (!Number.isNaN(idx) && idx >= 0 && idx < selectedOptions.length) {
                selectedOptions[idx] = val;
            }
        });

        // helper: recompute disabled states from selectedOptions
        function refreshDisabledStates() {
            const chosen = new Set(selectedOptions.filter(v => v !== ''));
            selects.forEach((s) => {
                const idx = Number(s.dataset.index);
                const options = Array.from(s.querySelectorAll('option.ranking'));
                options.forEach(opt => {
                    // allow the option if it's the one currently selected in same select
                    const isChosenElsewhere = chosen.has(opt.value) && selectedOptions[idx] !== opt.value;
                    opt.disabled = isChosenElsewhere;
                });
            });
        }

        // Attach change listeners (remove duplicates first)
        selects.forEach((s) => {
            // remove any previously added handler to avoid duplicate listeners
            if (s._uniqueSelectHandler) {
                s.removeEventListener('change', s._uniqueSelectHandler);
            }

            const handler = (e) => {
                const index = Number(s.dataset.index);
                const value = e.target.value === '' ? '' : e.target.value;
                if (!Number.isNaN(index) && index >= 0 && index < selectedOptions.length) {
                    selectedOptions[index] = value;
                }
                refreshDisabledStates();
            };

            // store reference so we can remove it later
            s._uniqueSelectHandler = handler;
            s.addEventListener('change', handler);
        });

        // initial apply
        refreshDisabledStates();
        dbg('initRankSelects complete. selectedOptions=', selectedOptions);
    }

    // Expose init function to global so callers can manually run it after injecting DOM
    window.initRankSelects = initRankSelects;
})();
