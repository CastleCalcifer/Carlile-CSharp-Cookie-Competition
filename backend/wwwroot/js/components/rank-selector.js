/**
 * Rank selector component - manages unique ranking selections
 */
class RankSelector {
    constructor() {
        this.selects = [];
        this.selectedOptions = [];
        this.changeCallbacks = [];
    }

    /**
     * Initializes rank selectors
     * @param {Array} selects - Array of select elements
     */
    init(selects) {
        this.selects = Array.from(selects);
        this.selectedOptions = new Array(this.selects.length).fill('');

        // Set up data attributes
        this.selects.forEach((select, index) => {
            if (select.dataset.index === undefined || select.dataset.index === '') {
                select.dataset.index = String(index);
            }
            
            // Normalize placeholder option
            const firstOption = select.querySelector('option');
            if (firstOption && firstOption.value === '---') {
                firstOption.value = '';
            }
        });

        // Set up event listeners
        this._attachEventListeners();
        
        // Initial state
        this._refreshDisabledStates();
    }

    /**
     * Attaches event listeners to select elements
     */
    _attachEventListeners() {
        this.selects.forEach(select => {
            // Remove existing handler
            if (select._rankSelectorHandler) {
                select.removeEventListener('change', select._rankSelectorHandler);
            }

            const handler = (event) => {
                const index = parseInt(select.dataset.index);
                const value = event.target.value === '' ? '' : event.target.value;
                
                if (!isNaN(index) && index >= 0 && index < this.selectedOptions.length) {
                    this.selectedOptions[index] = value;
                    this._refreshDisabledStates();
                    this._notifyChangeCallbacks();
                }
            };

            select._rankSelectorHandler = handler;
            select.addEventListener('change', handler);
        });
    }

    /**
     * Refreshes disabled states of options
     */
    _refreshDisabledStates() {
        const chosen = new Set(this.selectedOptions.filter(value => value !== ''));
        
        this.selects.forEach(select => {
            const index = parseInt(select.dataset.index);
            const options = Array.from(select.querySelectorAll('option.ranking'));
            
            options.forEach(option => {
                const isChosenElsewhere = chosen.has(option.value) && 
                                        this.selectedOptions[index] !== option.value;
                option.disabled = isChosenElsewhere;
            });
        });
    }

    /**
     * Notifies change callbacks
     */
    _notifyChangeCallbacks() {
        this.changeCallbacks.forEach(callback => {
            try {
                callback(this.selectedOptions);
            } catch (error) {
                console.error('Error in rank selector callback:', error);
            }
        });
    }

    /**
     * Adds a change callback
     * @param {Function} callback - Callback function
     */
    onChange(callback) {
        this.changeCallbacks.push(callback);
    }

    /**
     * Gets current selections
     * @returns {Array} Array of selected values
     */
    getSelections() {
        return [...this.selectedOptions];
    }

    /**
     * Gets selections as cookie IDs in rank order
     * @returns {Array} Array of cookie IDs in rank order
     */
    getSelectionsAsCookieIds() {
        const cookieIds = new Array(this.selects.length).fill(null);
        
        this.selects.forEach(select => {
            const value = select.value;
            const cookieId = parseInt(select.dataset.cookieId);
            
            if (value && value !== '' && !isNaN(cookieId)) {
                const rank = parseInt(value) - 1; // Convert to 0-based index
                if (rank >= 0 && rank < cookieIds.length) {
                    cookieIds[rank] = cookieId;
                }
            }
        });

        return cookieIds;
    }

    /**
     * Validates selections
     * @returns {Object} Validation result
     */
    validate() {
        const errors = [];
        const cookieIds = this.getSelectionsAsCookieIds();

        // Check for missing selections
        if (cookieIds.some(id => id === null)) {
            errors.push('Please select all ranks before submitting');
        }

        // Check for duplicate selections
        const uniqueIds = new Set(cookieIds.filter(id => id !== null));
        if (uniqueIds.size !== cookieIds.filter(id => id !== null).length) {
            errors.push('Each cookie can only be ranked once');
        }

        return {
            isValid: errors.length === 0,
            errors,
            cookieIds: errors.length === 0 ? cookieIds : []
        };
    }

    /**
     * Resets all selections
     */
    reset() {
        this.selects.forEach(select => {
            select.value = '';
        });
        this.selectedOptions.fill('');
        this._refreshDisabledStates();
    }

    /**
     * Destroys the rank selector
     */
    destroy() {
        this.selects.forEach(select => {
            if (select._rankSelectorHandler) {
                select.removeEventListener('change', select._rankSelectorHandler);
                delete select._rankSelectorHandler;
            }
        });
        
        this.selects = [];
        this.selectedOptions = [];
        this.changeCallbacks = [];
    }
}

// Export for global use
window.RankSelector = RankSelector;
