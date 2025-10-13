/**
 * Application configuration and constants
 * Centralized configuration management
 */
class Config {
    constructor() {
        this.year = 2024;
        this.apiEndpoints = {
            cookies: '/api/cookies',
            bakers: '/api/bakers',
            votes: '/api/voter/vote',
            awards: '/api/awards',
            results: '/api/results'
        };
        this.imageDefaults = {
            placeholder: '/images/placeholder.jpg',
            fallback: '/images/placeholder.jpg'
        };
        this.ui = {
            maxRankings: 10,
            alertTimeout: 5000,
            retryDelay: 250
        };
    }

    /**
     * Gets the current competition year
     * @returns {number} Current year
     */
    getYear() {
        return this.year;
    }

    /**
     * Gets API endpoint URL
     * @param {string} endpoint - Endpoint name
     * @returns {string} Full endpoint URL
     */
    getApiEndpoint(endpoint) {
        return this.apiEndpoints[endpoint] || endpoint;
    }

    /**
     * Gets image URL with fallback
     * @param {string} imageUrl - Original image URL
     * @param {string} fallback - Fallback image URL
     * @returns {string} Image URL with fallback
     */
    getImageUrl(imageUrl, fallback = null) {
        if (imageUrl && imageUrl !== '') {
            return imageUrl;
        }
        return fallback || this.imageDefaults.placeholder;
    }

    /**
     * Gets UI configuration value
     * @param {string} key - Configuration key
     * @returns {any} Configuration value
     */
    getUiConfig(key) {
        return this.ui[key];
    }

    /**
     * Updates configuration at runtime
     * @param {Object} updates - Configuration updates
     */
    update(updates) {
        Object.assign(this, updates);
    }
}

// Export singleton instance
window.Config = new Config();
