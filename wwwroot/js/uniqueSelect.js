// uniqueSelect.js - robust implementation
(() => {
    // Get all selects we want to manage
    const selects = Array.from(document.querySelectorAll('select.rank-select'));

    if (selects.length === 0) return; // nothing to do

    // Make a slot for each select's currently selected value ('' means no selection)
    const selectedOptions = new Array(selects.length).fill('');

    // Helper: update disabled state on all selects based on selectedOptions
    function refreshDisabledStates() {
        // create a Set for faster lookup (ignores empty strings)
        const chosen = new Set(selectedOptions.filter(v => v !== ''));

        for (let i = 0; i < selects.length; i++) {
            const s = selects[i];
            const options = Array.from(s.querySelectorAll('option.ranking'));

            options.forEach(opt => {
                // allow the option if it's the one currently chosen in the same select
                // or if it's not chosen in any other select
                const isChosenElsewhere = chosen.has(opt.value) && selectedOptions[i] !== opt.value;
                opt.disabled = isChosenElsewhere;
            });
        }
    }

    // Attach listeners and set data-index if not present
    selects.forEach((s, idx) => {
        // standardize: add a class and data-index if missing
        if (!s.classList.contains('rank-select')) s.classList.add('rank-select');
        s.dataset.index = s.dataset.index ?? String(idx);

        // initialize selectedOptions from any pre-selected option with value (non '---')
        const currentValue = s.value === '---' ? '' : s.value;
        selectedOptions[idx] = currentValue === '---' ? '' : currentValue;

        s.addEventListener('change', (e) => {
            const index = Number(s.dataset.index);
            const value = e.target.value === '---' ? '' : e.target.value;
            selectedOptions[index] = value;
            refreshDisabledStates();
        });
    });

    // initial refresh in case some options were already selected in markup
    refreshDisabledStates();
})();
