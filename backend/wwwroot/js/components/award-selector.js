/**
 * Award selector component - manages award selection dropdowns
 */
class AwardSelector {
    constructor(config) {
        this.config = config;
        this.creativeSelect = null;
        this.presentationSelect = null;
        this.creativeImage = null;
        this.presentationImage = null;
        this.cookies = [];
    }

    /**
     * Initializes the award selector
     * @param {string} creativeSelectId - Creative select element ID
     * @param {string} presentationSelectId - Presentation select element ID
     * @param {string} creativeImageId - Creative image element ID
     * @param {string} presentationImageId - Presentation image element ID
     */
    init(creativeSelectId, presentationSelectId, creativeImageId, presentationImageId) {
        this.creativeSelect = DomUtils.getElement(creativeSelectId);
        this.presentationSelect = DomUtils.getElement(presentationSelectId);
        this.creativeImage = DomUtils.getElement(creativeImageId);
        this.presentationImage = DomUtils.getElement(presentationImageId);

        if (!this.creativeSelect || !this.presentationSelect) {
            throw new Error('Required select elements not found');
        }

        this._attachEventListeners();
    }

    /**
     * Populates the selectors with cookies
     * @param {Array} cookies - Array of cookie objects
     */
    populate(cookies) {
        this.cookies = cookies || [];
        this._clearSelects();
        this._addPlaceholderOptions();
        this._addCookieOptions();
    }

    /**
     * Clears both select elements
     */
    _clearSelects() {
        [this.creativeSelect, this.presentationSelect].forEach(select => {
            if (select) {
                while (select.options.length) {
                    select.remove(0);
                }
            }
        });
    }

    /**
     * Adds placeholder options to both selects
     */
    _addPlaceholderOptions() {
        const placeholder = { value: '', text: '--- Select cookie ---', selected: true };
        
        [this.creativeSelect, this.presentationSelect].forEach(select => {
            const option = DomUtils.createElement('option', {
                value: placeholder.value,
                selected: placeholder.selected
            }, placeholder.text);
            select.appendChild(option);
        });
    }

    /**
     * Adds cookie options to both selects
     */
    _addCookieOptions() {
        this.cookies.forEach(cookie => {
            const imageUrl = this.config.getImageUrl(cookie.imageUrl);
            
            // Add to creative select
            const creativeOption = DomUtils.createElement('option', {
                value: cookie.id,
                dataset: { image: imageUrl }
            }, cookie.cookieName);
            this.creativeSelect.appendChild(creativeOption);

            // Add to presentation select
            const presentationOption = DomUtils.createElement('option', {
                value: cookie.id,
                dataset: { image: imageUrl }
            }, cookie.cookieName);
            this.presentationSelect.appendChild(presentationOption);
        });
    }

    /**
     * Attaches event listeners for image preview
     */
    _attachEventListeners() {
        if (this.creativeSelect && this.creativeImage) {
            this.creativeSelect.addEventListener('change', (event) => {
                this._updateImagePreview(event.target, this.creativeImage);
            });
        }

        if (this.presentationSelect && this.presentationImage) {
            this.presentationSelect.addEventListener('change', (event) => {
                this._updateImagePreview(event.target, this.presentationImage);
            });
        }
    }

    /**
     * Updates image preview based on selection
     * @param {HTMLSelectElement} select - Select element
     * @param {HTMLImageElement} image - Image element
     */
    _updateImagePreview(select, image) {
        const selectedOption = select.selectedOptions[0];
        if (selectedOption && selectedOption.dataset.image) {
            image.src = selectedOption.dataset.image;
        }
    }

    /**
     * Gets current selections
     * @returns {Object} Current selections
     */
    getSelections() {
        return {
            mostCreativeId: parseInt(this.creativeSelect.value) || 0,
            bestPresentationId: parseInt(this.presentationSelect.value) || 0
        };
    }

    /**
     * Validates current selections
     * @returns {Object} Validation result
     */
    validate() {
        const { mostCreativeId, bestPresentationId } = this.getSelections();
        const errors = [];

        if (!mostCreativeId) {
            errors.push('Please select a cookie for Most Creative');
        }

        if (!bestPresentationId) {
            errors.push('Please select a cookie for Best Presentation');
        }

        if (mostCreativeId && bestPresentationId && mostCreativeId === bestPresentationId) {
            errors.push('Most Creative and Best Presentation must be different cookies');
        }

        return {
            isValid: errors.length === 0,
            errors,
            mostCreativeId: errors.length === 0 ? mostCreativeId : 0,
            bestPresentationId: errors.length === 0 ? bestPresentationId : 0
        };
    }

    /**
     * Resets both selectors
     */
    reset() {
        if (this.creativeSelect) {
            this.creativeSelect.value = '';
        }
        if (this.presentationSelect) {
            this.presentationSelect.value = '';
        }
        if (this.creativeImage) {
            this.creativeImage.src = this.config.getImageUrl('');
        }
        if (this.presentationImage) {
            this.presentationImage.src = this.config.getImageUrl('');
        }
    }

    /**
     * Gets selected cookies
     * @returns {Object} Selected cookie objects
     */
    getSelectedCookies() {
        const { mostCreativeId, bestPresentationId } = this.getSelections();
        
        return {
            mostCreative: this.cookies.find(cookie => cookie.id === mostCreativeId) || null,
            bestPresentation: this.cookies.find(cookie => cookie.id === bestPresentationId) || null
        };
    }
}

// Export for global use
window.AwardSelector = AwardSelector;
