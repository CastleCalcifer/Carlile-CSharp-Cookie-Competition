/**
 * Awards page controller - handles the awards page functionality
 */
class AwardsPage {
    constructor() {
        this.cookieService = new CookieService(ApiClient, Config);
        this.bakerService = new BakerService(ApiClient, Config);
        this.awardsService = new AwardsService(ApiClient, Config);
        this.awardSelector = new AwardSelector(Config);
        
        this.submitButton = null;
        this.errorElement = null;
        this.voterIdInput = null;
    }

    /**
     * Initializes the awards page
     */
    async init() {
        try {
            this._getDOMElements();
            this._setupEventListeners();
            await this._loadPageData();
        } catch (error) {
            ErrorHandler.handleError('AwardsPage.init', error);
            this._showError('Failed to initialize awards page');
        }
    }

    /**
     * Gets required DOM elements
     */
    _getDOMElements() {
        this.submitButton = DomUtils.getElement('submitAwards');
        this.errorElement = DomUtils.getElement('errorMsg');
        this.voterIdInput = DomUtils.getElement('voterId');

        if (!this.submitButton) {
            throw new Error('Submit button not found');
        }
    }

    /**
     * Sets up event listeners
     */
    _setupEventListeners() {
        if (this.submitButton) {
            this.submitButton.addEventListener('click', (e) => this._handleSubmit(e));
        }
    }

    /**
     * Loads page data (cookies and current baker)
     */
    async _loadPageData() {
        try {
            // Get current baker to exclude their cookies
            const currentBaker = await this.bakerService.getCurrentBaker();
            const bakerId = currentBaker?.id || null;

            // Retry once if no baker found (may be due to async login)
            if (!bakerId) {
                await new Promise(resolve => setTimeout(resolve, Config.getUiConfig('retryDelay')));
                const retryBaker = await this.bakerService.getCurrentBaker();
                if (retryBaker) {
                    bakerId = retryBaker.id;
                }
            }

            // Load cookies
            const cookies = await this.cookieService.getCookies(Config.getYear(), bakerId);
            
            if (!cookies || cookies.length === 0) {
                this._showError('No cookies available for awards');
                return;
            }

            // Initialize award selector
            this.awardSelector.init(
                'mostCreative',
                'bestPresentation',
                'awardImage0',
                'awardImage1'
            );

            // Populate with cookies
            this.awardSelector.populate(cookies);

        } catch (error) {
            ErrorHandler.handleError('AwardsPage._loadPageData', error);
            this._showError('Failed to load cookies');
        }
    }

    /**
     * Handles form submission
     * @param {Event} event - Submit event
     */
    async _handleSubmit(event) {
        event.preventDefault();

        if (!this.submitButton) return;

        try {
            this.submitButton.disabled = true;
            DomUtils.clearError(this.errorElement);

            // Validate selections
            const validation = this.awardSelector.validate();
            if (!validation.isValid) {
                this._showError(validation.errors.join(', '));
                return;
            }

            // Get voter ID
            const voterId = this.voterIdInput ? this.voterIdInput.value.trim() : null;

            // Submit awards
            const result = await this.awardsService.submitAwards(
                validation.mostCreativeId,
                validation.bestPresentationId,
                voterId
            );

            this._showSuccess('Thanks! Awards recorded.');
            
            // Redirect to results page after a short delay
            setTimeout(() => {
                window.location.href = '/results.html';
            }, 1000);

        } catch (error) {
            ErrorHandler.handleApiError(error, this.errorElement);
        } finally {
            this.submitButton.disabled = false;
        }
    }

    /**
     * Shows error message
     * @param {string} message - Error message
     */
    _showError(message) {
        if (this.errorElement) {
            DomUtils.setError(this.errorElement, message);
        } else {
            ErrorHandler.showAlert(message, 'error');
        }
    }

    /**
     * Shows success message
     * @param {string} message - Success message
     */
    _showSuccess(message) {
        ErrorHandler.showAlert(message, 'success');
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new AwardsPage().init();
});
