/**
 * Awards service - handles all awards-related business logic
 */
class AwardsService {
    constructor(apiClient, config) {
        this.api = apiClient;
        this.config = config;
    }

    /**
     * Submits awards votes
     * @param {number} mostCreativeId - Most creative cookie ID
     * @param {number} bestPresentationId - Best presentation cookie ID
     * @param {string|null} voterId - Optional voter ID
     * @returns {Promise<Object>} Submission result
     */
    async submitAwards(mostCreativeId, bestPresentationId, voterId = null) {
        const year = this.config.getYear();
        
        const requestData = {
            year,
            mostCreativeId: this._validateCookieId(mostCreativeId),
            bestPresentationId: this._validateCookieId(bestPresentationId)
        };

        if (voterId && voterId.trim() !== '') {
            requestData.voterId = voterId.trim();
        }

        try {
            const result = await this.api.post(this.config.getApiEndpoint('awards'), requestData);
            return result;
        } catch (error) {
            ErrorHandler.handleError('AwardsService.submitAwards', error);
            throw error;
        }
    }

    /**
     * Validates a cookie ID
     * @param {number} cookieId - Cookie ID to validate
     * @returns {number} Validated cookie ID
     */
    _validateCookieId(cookieId) {
        const id = parseInt(cookieId);
        if (isNaN(id) || id <= 0) {
            throw new Error('Invalid cookie ID');
        }
        return id;
    }

    /**
     * Validates awards data before submission
     * @param {number} mostCreativeId - Most creative cookie ID
     * @param {number} bestPresentationId - Best presentation cookie ID
     * @param {string|null} voterId - Optional voter ID
     * @returns {Object} Validation result
     */
    validateAwardsData(mostCreativeId, bestPresentationId, voterId = null) {
        const errors = [];

        if (!mostCreativeId || typeof mostCreativeId !== 'number' || mostCreativeId <= 0) {
            errors.push('Please select a cookie for Most Creative');
        }

        if (!bestPresentationId || typeof bestPresentationId !== 'number' || bestPresentationId <= 0) {
            errors.push('Please select a cookie for Best Presentation');
        }

        if (mostCreativeId === bestPresentationId) {
            errors.push('Most Creative and Best Presentation must be different cookies');
        }

        if (voterId !== null && typeof voterId !== 'string') {
            errors.push('Voter ID must be a string');
        }

        return {
            isValid: errors.length === 0,
            errors
        };
    }

    /**
     * Creates awards data from form inputs
     * @param {HTMLSelectElement} creativeSelect - Creative select element
     * @param {HTMLSelectElement} presentationSelect - Presentation select element
     * @param {HTMLInputElement|null} voterIdInput - Voter ID input element
     * @returns {Object} Awards data
     */
    createAwardsDataFromForm(creativeSelect, presentationSelect, voterIdInput = null) {
        const mostCreativeId = parseInt(creativeSelect.value) || 0;
        const bestPresentationId = parseInt(presentationSelect.value) || 0;
        const voterId = voterIdInput ? voterIdInput.value.trim() : null;

        const validation = this.validateAwardsData(mostCreativeId, bestPresentationId, voterId);

        return {
            mostCreativeId: validation.isValid ? mostCreativeId : 0,
            bestPresentationId: validation.isValid ? bestPresentationId : 0,
            voterId: validation.isValid ? voterId : null,
            validation
        };
    }
}

// Export for global use
window.AwardsService = AwardsService;
