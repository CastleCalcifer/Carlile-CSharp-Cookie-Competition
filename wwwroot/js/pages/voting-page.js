/**
 * Voting page controller - handles the voting page functionality
 */
class VotingPage {
    constructor() {
        this.cookieService = new CookieService(ApiClient, Config);
        this.bakerService = new BakerService(ApiClient, Config);
        this.votingService = new VotingService(ApiClient, Config);
        this.cookieCard = new CookieCard(Config);
        this.rankSelector = new RankSelector();
        
        this.cookiesContainer = null;
        this.submitButton = null;
        this.errorElement = null;
        this.voterIdInput = null;
    }

    /**
     * Initializes the voting page
     */
    async init() {
        try {
            this._getDOMElements();
            this._setupEventListeners();
            await this._loadPageData();
        } catch (error) {
            ErrorHandler.handleError('VotingPage.init', error);
            this._showError('Failed to initialize voting page');
        }
    }

    /**
     * Gets required DOM elements
     */
    _getDOMElements() {
        this.cookiesContainer = DomUtils.getElement('cookiesContainer');
        this.submitButton = DomUtils.getElement('submitVotes');
        this.errorElement = DomUtils.getElement('errorMsg');
        this.voterIdInput = DomUtils.getElement('voterId');

        if (!this.cookiesContainer) {
            throw new Error('Cookies container not found');
        }
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

            // Load cookies
            const cookies = await this.cookieService.getCookies(Config.getYear(), bakerId);
            
            if (!cookies || cookies.length === 0) {
                this._showNoCookies();
                return;
            }

            this._renderCookies(cookies);
        } catch (error) {
            ErrorHandler.handleError('VotingPage._loadPageData', error);
            this._showError('Failed to load cookies');
        }
    }

    /**
     * Renders cookies in the container
     * @param {Array} cookies - Array of cookie objects
     */
    _renderCookies(cookies) {
        DomUtils.clearElement(this.cookiesContainer);

        cookies.forEach((cookie, index) => {
            const card = this.cookieCard.createVotingCard(cookie, index, cookies.length);
            this.cookiesContainer.appendChild(card);
        });

        // Initialize rank selector
        const selects = Array.from(document.querySelectorAll('select.rank-select'));
        this.rankSelector.init(selects);

        // Set up change callback for validation
        this.rankSelector.onChange(() => {
            this._updateSubmitButtonState();
        });

        this._updateSubmitButtonState();
    }

    /**
     * Updates submit button state based on selections
     */
    _updateSubmitButtonState() {
        if (!this.submitButton) return;

        const validation = this.rankSelector.validate();
        this.submitButton.disabled = !validation.isValid;
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

            const validation = this.rankSelector.validate();
            if (!validation.isValid) {
                this._showError(validation.errors.join(', '));
                return;
            }

            const voterId = this.voterIdInput ? this.voterIdInput.value.trim() : null;
            const result = await this.votingService.submitVotes(validation.cookieIds, voterId);

            this._showSuccess('Thanks — your vote has been recorded!');
            
            // Redirect to awards page after a short delay
            setTimeout(() => {
                window.location.href = '/awards.html';
            }, 1000);

        } catch (error) {
            ErrorHandler.handleApiError(error, this.errorElement);
        } finally {
            this.submitButton.disabled = false;
        }
    }

    /**
     * Shows no cookies message
     */
    _showNoCookies() {
        if (this.cookiesContainer) {
            this.cookiesContainer.innerHTML = '<p>No cookies available for voting.</p>';
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
    new VotingPage().init();
});
