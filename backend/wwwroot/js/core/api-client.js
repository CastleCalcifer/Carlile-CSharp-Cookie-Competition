/**
 * Centralized API client for all HTTP requests
 * Handles authentication, error handling, and response parsing
 */
class ApiClient {
    constructor() {
        this.baseUrl = window.location.origin;
        this.defaultOptions = {
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        };
    }

    /**
     * Makes a GET request
     * @param {string} endpoint - API endpoint
     * @param {Object} options - Additional options
     * @returns {Promise<any>} Parsed response data
     */
    async get(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = { ...this.defaultOptions, ...options, method: 'GET' };
        
        try {
            const response = await fetch(url, config);
            return await this._handleResponse(response);
        } catch (error) {
            throw new ApiError(`GET ${endpoint} failed`, error);
        }
    }

    /**
     * Makes a POST request
     * @param {string} endpoint - API endpoint
     * @param {Object} data - Request body data
     * @param {Object} options - Additional options
     * @returns {Promise<any>} Parsed response data
     */
    async post(endpoint, data, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = {
            ...this.defaultOptions,
            ...options,
            method: 'POST',
            body: JSON.stringify(data)
        };
        
        try {
            const response = await fetch(url, config);
            return await this._handleResponse(response);
        } catch (error) {
            throw new ApiError(`POST ${endpoint} failed`, error);
        }
    }

    /**
     * Handles response parsing and error checking
     * @param {Response} response - Fetch response object
     * @returns {Promise<any>} Parsed response data
     */
    async _handleResponse(response) {
        const text = await response.text();
        
        let data;
        try {
            data = JSON.parse(text);
        } catch (parseError) {
            throw new ApiError(`Invalid JSON response: ${text}`, parseError);
        }

        if (!response.ok) {
            const message = data?.message || `Server returned ${response.status}`;
            throw new ApiError(message, null, response.status);
        }

        // Handle different response formats
        return Array.isArray(data) ? data : (data.data ?? data);
    }
}

/**
 * Custom error class for API errors
 */
class ApiError extends Error {
    constructor(message, originalError = null, status = null) {
        super(message);
        this.name = 'ApiError';
        this.originalError = originalError;
        this.status = status;
    }
}

// Export singleton instance
window.ApiClient = new ApiClient();
