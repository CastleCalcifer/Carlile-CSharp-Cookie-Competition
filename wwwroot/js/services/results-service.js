/**
 * Results service - handles all results-related business logic
 */
class ResultsService {
    constructor(apiClient, config) {
        this.api = apiClient;
        this.config = config;
    }

    /**
     * Fetches results for a given year
     * @param {number} year - Competition year
     * @returns {Promise<Object>} Results data
     */
    async getResults(year = null) {
        const actualYear = year || this.config.getYear();
        const endpoint = `${this.config.getApiEndpoint('results')}?year=${actualYear}`;

        try {
            const results = await this.api.get(endpoint);
            return this._normalizeResults(results);
        } catch (error) {
            ErrorHandler.handleError('ResultsService.getResults', error);
            throw error;
        }
    }

    /**
     * Normalizes results data
     * @param {Object} results - Raw results data
     * @returns {Object} Normalized results data
     */
    _normalizeResults(results) {
        const ranked = results.ranked || results || [];
        const awards = results.awards || {};

        return {
            ranked: this._normalizeRankedResults(ranked),
            awards: this._normalizeAwards(awards),
            year: results.year || this.config.getYear()
        };
    }

    /**
     * Normalizes ranked results
     * @param {Array} ranked - Raw ranked data
     * @returns {Array} Normalized ranked data
     */
    _normalizeRankedResults(ranked) {
        if (!Array.isArray(ranked)) {
            return [];
        }

        return ranked.map(cookie => ({
            id: cookie.id,
            cookieName: cookie.cookieName || cookie.name || 'Unknown Cookie',
            imageUrl: this.config.getImageUrl(cookie.imageUrl || cookie.image),
            score: cookie.score || cookie.Score || 0,
            creativePoints: cookie.creativePoints || cookie.creative_points || 0,
            presentationPoints: cookie.presentationPoints || cookie.presentation_points || 0
        }));
    }

    /**
     * Normalizes awards data
     * @param {Object} awards - Raw awards data
     * @returns {Object} Normalized awards data
     */
    _normalizeAwards(awards) {
        return {
            presentation: this._normalizeAward(awards.presentation),
            creative: this._normalizeAward(awards.creative)
        };
    }

    /**
     * Normalizes a single award
     * @param {Object|null} award - Raw award data
     * @returns {Object|null} Normalized award data
     */
    _normalizeAward(award) {
        if (!award) {
            return null;
        }

        return {
            id: award.id,
            cookieName: award.cookieName || award.name || 'Unknown Cookie',
            imageUrl: this.config.getImageUrl(award.imageUrl || award.image),
            score: award.score || award.Score || 0,
            creativePoints: award.creativePoints || award.creative_points || 0,
            presentationPoints: award.presentationPoints || award.presentation_points || 0
        };
    }

    /**
     * Sorts results by score (descending)
     * @param {Array} results - Array of result objects
     * @returns {Array} Sorted results
     */
    sortByScore(results) {
        return [...results].sort((a, b) => (b.score || 0) - (a.score || 0));
    }

    /**
     * Gets top N results
     * @param {Array} results - Array of result objects
     * @param {number} count - Number of top results to return
     * @returns {Array} Top N results
     */
    getTopResults(results, count = 4) {
        return this.sortByScore(results).slice(0, count);
    }

    /**
     * Creates display order (4th to 1st place)
     * @param {Array} results - Array of result objects
     * @returns {Array} Results in display order
     */
    createDisplayOrder(results) {
        const top4 = this.getTopResults(results, 4);
        return top4.slice().reverse(); // 4th -> 1st
    }

    /**
     * Gets place labels for display
     * @param {number} count - Number of places
     * @returns {Array} Array of place labels
     */
    getPlaceLabels(count = 4) {
        const labels = ['4th Place', '3rd Place', '2nd Place', '1st Place'];
        return labels.slice(0, count);
    }

    /**
     * Validates results data
     * @param {Object} results - Results object to validate
     * @returns {boolean} True if valid
     */
    validateResults(results) {
        return results && 
               Array.isArray(results.ranked) &&
               typeof results.awards === 'object';
    }
}

// Export for global use
window.ResultsService = ResultsService;
