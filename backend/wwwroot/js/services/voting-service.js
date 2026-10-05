/**
 * Voting service - handles all voting-related business logic
 */
class VotingService {
    constructor(apiClient, config) {
        this.api = apiClient;
        this.config = config;
    }

    /**
     * Submits votes for cookies
     * @param {Array} cookieIds - Array of cookie IDs in ranking order
     * @param {string|null} voterId - Optional voter ID
     * @returns {Promise<Object>} Submission result
     */
    async submitVotes(cookieIds, voterId = null) {
        const year = this.config.getYear();
        
        const requestData = {
            year,
            cookieIds: this._validateCookieIds(cookieIds)
        };

        if (voterId && voterId.trim() !== '') {
            requestData.voterId = voterId.trim();
        }

        try {
            const result = await this.api.post(this.config.getApiEndpoint('votes'), requestData);
            return result;
        } catch (error) {
            ErrorHandler.handleError('VotingService.submitVotes', error);
            throw error;
        }
    }

    /**
     * Validates cookie IDs array
     * @param {Array} cookieIds - Array of cookie IDs
     * @returns {Array} Validated cookie IDs
     */
    _validateCookieIds(cookieIds) {
        if (!Array.isArray(cookieIds)) {
            throw new Error('Cookie IDs must be an array');
        }

        const validIds = cookieIds.filter(id => 
            typeof id === 'number' && id > 0
        );

        if (validIds.length !== cookieIds.length) {
            throw new Error('All cookie IDs must be positive numbers');
        }

        return validIds;
    }

    /**
     * Validates voting data before submission
     * @param {Array} cookieIds - Array of cookie IDs
     * @param {string|null} voterId - Optional voter ID
     * @returns {Object} Validation result
     */
    validateVotingData(cookieIds, voterId = null) {
        const errors = [];

        if (!Array.isArray(cookieIds) || cookieIds.length === 0) {
            errors.push('No cookies selected for voting');
        }

        if (cookieIds && cookieIds.some(id => typeof id !== 'number' || id <= 0)) {
            errors.push('Invalid cookie IDs');
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
     * Creates voting data from form inputs
     * @param {Array} selects - Array of select elements
     * @returns {Object} Voting data
     */
    createVotingDataFromSelects(selects) {
        const cookieIds = [];
        const errors = [];

        selects.forEach((select, index) => {
            const value = select.value;
            const cookieId = parseInt(select.dataset.cookieId);

            if (!value || value === '') {
                errors.push(`Please select a rank for cookie ${index + 1}`);
                return;
            }

            if (isNaN(cookieId) || cookieId <= 0) {
                errors.push(`Invalid cookie ID for cookie ${index + 1}`);
                return;
            }

            const rank = parseInt(value) - 1; // Convert to 0-based index
            cookieIds[rank] = cookieId;
        });

        // Check for gaps in ranking
        if (cookieIds.some(id => id === undefined)) {
            errors.push('Please select all ranks before submitting');
        }

        return {
            cookieIds: errors.length === 0 ? cookieIds : [],
            errors
        };
    }
}

// Export for global use
window.VotingService = VotingService;
