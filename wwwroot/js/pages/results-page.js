/**
 * Results page controller - handles the results page functionality
 */
class ResultsPage {
    constructor() {
        this.resultsService = new ResultsService(ApiClient, Config);
        this.cookieCard = new CookieCard(Config);
        
        this.placementsContainer = null;
        this.awardsContainer = null;
        this.errorElement = null;
    }

    /**
     * Initializes the results page
     */
    async init() {
        try {
            this._getDOMElements();
            await this._loadResults();
        } catch (error) {
            ErrorHandler.handleError('ResultsPage.init', error);
            this._showError('Failed to initialize results page');
        }
    }

    /**
     * Gets required DOM elements
     */
    _getDOMElements() {
        this.placementsContainer = DomUtils.getElement('placements');
        this.awardsContainer = DomUtils.getElement('awards');
        this.errorElement = DomUtils.getElement('resultsError');

        if (!this.placementsContainer) {
            throw new Error('Placements container not found');
        }
        if (!this.awardsContainer) {
            throw new Error('Awards container not found');
        }
    }

    /**
     * Loads and displays results
     */
    async _loadResults() {
        try {
            const results = await this.resultsService.getResults(Config.getYear());
            
            if (!this.resultsService.validateResults(results)) {
                throw new Error('Invalid results data received');
            }

            this._renderPlacements(results.ranked);
            this._renderAwards(results.awards);

        } catch (error) {
            ErrorHandler.handleError('ResultsPage._loadResults', error);
            this._showError('Failed to load results');
        }
    }

    /**
     * Renders placement results
     * @param {Array} rankedCookies - Array of ranked cookie objects
     */
    _renderPlacements(rankedCookies) {
        DomUtils.clearElement(this.placementsContainer);

        // Get top 4 cookies in display order (4th to 1st)
        const top4 = this.resultsService.createDisplayOrder(rankedCookies);
        const placeLabels = this.resultsService.getPlaceLabels(4);

        for (let i = 0; i < 4; i++) {
            const cookie = top4[i] || this._createEmptyPlaceholder();
            const placeLabel = placeLabels[i] || `${i + 1}th Place`;
            
            const card = this.cookieCard.createResultsCard(cookie, placeLabel, {
                showScore: true,
                showPoints: false
            });
            
            this.placementsContainer.appendChild(card);
        }
    }

    /**
     * Renders award results
     * @param {Object} awards - Awards data
     */
    _renderAwards(awards) {
        DomUtils.clearElement(this.awardsContainer);

        // Presentation award
        const presentationCard = this.cookieCard.createResultsCard(
            awards.presentation || this._createEmptyPlaceholder(),
            'Best Presentation',
            {
                showScore: false,
                showPoints: true
            }
        );
        this.awardsContainer.appendChild(presentationCard);

        // Creative award
        const creativeCard = this.cookieCard.createResultsCard(
            awards.creative || this._createEmptyPlaceholder(),
            'Most Creative',
            {
                showScore: false,
                showPoints: true
            }
        );
        this.awardsContainer.appendChild(creativeCard);
    }

    /**
     * Creates an empty placeholder for missing data
     * @returns {Object} Empty placeholder object
     */
    _createEmptyPlaceholder() {
        return {
            id: 0,
            cookieName: '—',
            imageUrl: Config.getImageUrl(''),
            score: 0,
            creativePoints: 0,
            presentationPoints: 0
        };
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
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ResultsPage().init();
});
