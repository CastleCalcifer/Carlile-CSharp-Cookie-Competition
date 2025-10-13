/**
 * Centralized error handling system
 * Provides consistent error handling across the application
 */
class ErrorHandler {
    constructor() {
        this.errorCallbacks = [];
        this.setupGlobalErrorHandling();
    }

    /**
     * Sets up global error handling for unhandled errors
     */
    setupGlobalErrorHandling() {
        window.addEventListener('error', (event) => {
            this.handleError('Unhandled Error', event.error);
        });

        window.addEventListener('unhandledrejection', (event) => {
            this.handleError('Unhandled Promise Rejection', event.reason);
        });
    }

    /**
     * Registers an error callback
     * @param {Function} callback - Error callback function
     */
    onError(callback) {
        this.errorCallbacks.push(callback);
    }

    /**
     * Handles an error by logging and notifying callbacks
     * @param {string} context - Error context
     * @param {Error|any} error - Error object or message
     * @param {Object} options - Additional options
     */
    handleError(context, error, options = {}) {
        const errorInfo = {
            context,
            error: error instanceof Error ? error : new Error(String(error)),
            timestamp: new Date().toISOString(),
            userAgent: navigator.userAgent,
            url: window.location.href,
            ...options
        };

        // Log to console
        console.error(`[${context}]`, errorInfo);

        // Notify callbacks
        this.errorCallbacks.forEach(callback => {
            try {
                callback(errorInfo);
            } catch (callbackError) {
                console.error('Error in error callback:', callbackError);
            }
        });
    }

    /**
     * Handles API errors specifically
     * @param {ApiError} apiError - API error object
     * @param {HTMLElement} errorElement - Element to display error in
     * @param {Object} options - Additional options
     */
    handleApiError(apiError, errorElement = null, options = {}) {
        const { showToUser = true, logError = true } = options;

        if (logError) {
            this.handleError('API Error', apiError, {
                endpoint: apiError.endpoint,
                status: apiError.status
            });
        }

        if (showToUser && errorElement && window.DomUtils) {
            const message = this.getUserFriendlyMessage(apiError);
            DomUtils.setError(errorElement, message);
        }
    }

    /**
     * Gets user-friendly error message
     * @param {ApiError} apiError - API error object
     * @returns {string} User-friendly message
     */
    getUserFriendlyMessage(apiError) {
        if (apiError.status === 401) {
            return 'Please log in to continue.';
        } else if (apiError.status === 403) {
            return 'You do not have permission to perform this action.';
        } else if (apiError.status === 404) {
            return 'The requested resource was not found.';
        } else if (apiError.status >= 500) {
            return 'Server error. Please try again later.';
        } else if (apiError.status === 0) {
            return 'Network error. Please check your connection.';
        } else {
            return apiError.message || 'An unexpected error occurred.';
        }
    }

    /**
     * Shows a user-friendly alert
     * @param {string} message - Message to show
     * @param {string} type - Alert type (error, warning, info, success)
     */
    showAlert(message, type = 'error') {
        // Create Bootstrap alert using native DOM methods
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show`;
        alertDiv.setAttribute('role', 'alert');

        const content = document.createElement('div');
        content.textContent = message;

        const closeButton = document.createElement('button');
        closeButton.type = 'button';
        closeButton.className = 'btn-close';
        closeButton.setAttribute('data-bs-dismiss', 'alert');
        closeButton.setAttribute('aria-label', 'Close');

        content.appendChild(closeButton);
        alertDiv.appendChild(content);

        // Insert at top of body
        document.body.insertBefore(alertDiv, document.body.firstChild);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.parentNode.removeChild(alertDiv);
            }
        }, 5000);
    }
}

// Export singleton instance
window.ErrorHandler = new ErrorHandler();
