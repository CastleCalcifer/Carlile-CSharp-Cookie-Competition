/**
 * Baker service - handles all baker-related business logic
 */
class BakerService {
    constructor(apiClient, config) {
        this.api = apiClient;
        this.config = config;
        this.currentBaker = null;
    }

    /**
     * Gets all bakers
     * @returns {Promise<Array>} Array of baker objects
     */
    async getBakers() {
        try {
            const bakers = await this.api.get(this.config.getApiEndpoint('bakers'));
            return this._normalizeBakers(bakers);
        } catch (error) {
            ErrorHandler.handleError('BakerService.getBakers', error);
            throw error;
        }
    }

    /**
     * Gets current logged-in baker
     * @returns {Promise<Object|null>} Current baker or null
     */
    async getCurrentBaker() {
        try {
            const baker = await this.api.get('/api/bakers/current');
            this.currentBaker = baker;
            return baker;
        } catch (error) {
            // Not an error if no baker is logged in
            this.currentBaker = null;
            return null;
        }
    }

    /**
     * Logs in a baker
     * @param {string} bakerName - Baker name
     * @param {string} pin - Baker PIN
     * @returns {Promise<Object>} Login result
     */
    async login(bakerName, pin) {
        try {
            const result = await this.api.post(this.config.getApiEndpoint('bakers') + '/login', {
                bakerName,
                pin
            });
            
            this.currentBaker = result;
            return result;
        } catch (error) {
            ErrorHandler.handleError('BakerService.login', error);
            throw error;
        }
    }

    /**
     * Logs out current baker
     */
    logout() {
        this.currentBaker = null;
        // Could make API call to logout if needed
    }

    /**
     * Normalizes baker data
     * @param {Array|Object} bakers - Raw baker data
     * @returns {Array|Object} Normalized baker data
     */
    _normalizeBakers(bakers) {
        if (Array.isArray(bakers)) {
            return bakers.map(baker => this._normalizeBaker(baker));
        }
        return this._normalizeBaker(bakers);
    }

    /**
     * Normalizes a single baker object
     * @param {Object} baker - Raw baker object
     * @returns {Object} Normalized baker object
     */
    _normalizeBaker(baker) {
        return {
            id: baker.id,
            bakerName: baker.bakerName || baker.name || 'Unknown Baker',
            hasPin: Boolean(baker.hasPin),
            isLoggedIn: Boolean(baker.isLoggedIn)
        };
    }

    /**
     * Validates baker data
     * @param {Object} baker - Baker object to validate
     * @returns {boolean} True if valid
     */
    validateBaker(baker) {
        return baker && 
               typeof baker.bakerName === 'string' && 
               baker.bakerName.trim() !== '';
    }

    /**
     * Checks if baker is logged in
     * @returns {boolean} True if logged in
     */
    isLoggedIn() {
        return this.currentBaker !== null;
    }

    /**
     * Gets current baker ID
     * @returns {number|null} Baker ID or null
     */
    getCurrentBakerId() {
        return this.currentBaker?.id || null;
    }

    /**
     * Gets current baker name
     * @returns {string|null} Baker name or null
     */
    getCurrentBakerName() {
        return this.currentBaker?.bakerName || null;
    }
}

// Export for global use
window.BakerService = BakerService;
