/**
 * Cookie service - handles all cookie-related business logic
 */
class CookieService {
    constructor(apiClient, config) {
        this.api = apiClient;
        this.config = config;
    }

    /**
     * Fetches cookies for a given year
     * @param {number} year - Competition year
     * @param {number|null} excludeBakerId - Baker ID to exclude
     * @returns {Promise<Array>} Array of cookie objects
     */
    async getCookies(year = null, excludeBakerId = null) {
        const actualYear = year || this.config.getYear();
        let endpoint = `${this.config.getApiEndpoint('cookies')}?year=${actualYear}`;
        
        if (excludeBakerId) {
            endpoint += `&excludeBakerId=${excludeBakerId}`;
        }

        console.log('CookieService: Fetching from endpoint:', endpoint);

        try {
            const cookies = await this.api.get(endpoint);
            console.log('CookieService: Raw response:', cookies);
            const normalized = this._normalizeCookies(cookies);
            console.log('CookieService: Normalized cookies:', normalized);
            return normalized;
        } catch (error) {
            console.error('CookieService: Error fetching cookies:', error);
            ErrorHandler.handleError('CookieService.getCookies', error);
            throw error;
        }
    }

    /**
     * Normalizes cookie data to ensure consistent structure
     * @param {Array} cookies - Raw cookie data
     * @returns {Array} Normalized cookie data
     */
    _normalizeCookies(cookies) {
        if (!Array.isArray(cookies)) {
            return [];
        }

        return cookies.map(cookie => ({
            id: cookie.id,
            cookieName: cookie.cookieName || cookie.name || 'Unknown Cookie',
            imageUrl: this.config.getImageUrl(cookie.imageUrl || cookie.image),
            year: cookie.year || this.config.getYear(),
            score: cookie.score || cookie.Score || 0,
            creativePoints: cookie.creativePoints || cookie.creative_points || 0,
            presentationPoints: cookie.presentationPoints || cookie.presentation_points || 0
        }));
    }

    /**
     * Validates cookie data
     * @param {Object} cookie - Cookie object to validate
     * @returns {boolean} True if valid
     */
    validateCookie(cookie) {
        return cookie && 
               typeof cookie.id === 'number' && 
               cookie.id > 0 &&
               typeof cookie.cookieName === 'string' && 
               cookie.cookieName.trim() !== '';
    }

    /**
     * Sorts cookies by score (descending)
     * @param {Array} cookies - Array of cookie objects
     * @returns {Array} Sorted cookies
     */
    sortByScore(cookies) {
        return [...cookies].sort((a, b) => (b.score || 0) - (a.score || 0));
    }

    /**
     * Gets top N cookies by score
     * @param {Array} cookies - Array of cookie objects
     * @param {number} count - Number of top cookies to return
     * @returns {Array} Top N cookies
     */
    getTopCookies(cookies, count = 4) {
        return this.sortByScore(cookies).slice(0, count);
    }

    /**
     * Finds cookie by ID
     * @param {Array} cookies - Array of cookie objects
     * @param {number} id - Cookie ID
     * @returns {Object|null} Cookie object or null
     */
    findById(cookies, id) {
        return cookies.find(cookie => cookie.id === id) || null;
    }

    /**
     * Gets cookies for a specific baker
     * @param {Array} cookies - Array of cookie objects
     * @param {number} bakerId - Baker ID
     * @returns {Array} Baker's cookies
     */
    getByBaker(cookies, bakerId) {
        return cookies.filter(cookie => cookie.bakerId === bakerId);
    }
}

// Export for global use
window.CookieService = CookieService;
