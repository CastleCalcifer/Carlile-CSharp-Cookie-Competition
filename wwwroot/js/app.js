/**
 * Main application entry point
 * Initializes the application and sets up global error handling
 */
class CookieCompetitionApp {
    constructor() {
        this.isInitialized = false;
        this.services = {};
        this.components = {};
    }

    /**
     * Initializes the application
     */
    async init() {
        if (this.isInitialized) {
            console.warn('App already initialized');
            return;
        }

        try {
            console.log('Initializing Cookie Competition App...');
            
            // Initialize services
            this._initializeServices();
            
            // Set up global error handling
            this._setupGlobalErrorHandling();
            
            // Initialize page-specific functionality
            this._initializePage();
            
            this.isInitialized = true;
            console.log('App initialized successfully');
            
        } catch (error) {
            console.error('Failed to initialize app:', error);
            ErrorHandler.handleError('App.init', error);
        }
    }

    /**
     * Initializes all services
     */
    _initializeServices() {
        this.services = {
            cookieService: new CookieService(ApiClient, Config),
            bakerService: new BakerService(ApiClient, Config),
            votingService: new VotingService(ApiClient, Config),
            awardsService: new AwardsService(ApiClient, Config),
            resultsService: new ResultsService(ApiClient, Config)
        };

        // Make services globally available
        window.Services = this.services;
    }

    /**
     * Sets up global error handling
     */
    _setupGlobalErrorHandling() {
        // Add error callback to show user-friendly messages
        ErrorHandler.onError((errorInfo) => {
            // Log to console in development
            if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
                console.error('App Error:', errorInfo);
            }
        });
    }

    /**
     * Initializes page-specific functionality
     */
    _initializePage() {
        const currentPage = this._getCurrentPage();
        
        switch (currentPage) {
            case 'voting':
                // Voting page is auto-initialized by voting-page.js
                break;
            case 'awards':
                // Awards page is auto-initialized by awards-page.js
                break;
            case 'results':
                // Results page is auto-initialized by results-page.js
                break;
            case 'baker-login':
                // Baker login page is auto-initialized by baker-login-page.js
                break;
            default:
                console.log('No specific page initialization needed');
        }
    }

    /**
     * Determines the current page based on URL
     * @returns {string} Current page name
     */
    _getCurrentPage() {
        const path = window.location.pathname;
        
        if (path.includes('voting.html')) return 'voting';
        if (path.includes('awards.html')) return 'awards';
        if (path.includes('results.html')) return 'results';
        if (path.includes('baker-select.html')) return 'baker-login';
        
        return 'unknown';
    }

    /**
     * Gets a service by name
     * @param {string} serviceName - Service name
     * @returns {Object|null} Service instance or null
     */
    getService(serviceName) {
        return this.services[serviceName] || null;
    }

    /**
     * Gets application configuration
     * @returns {Object} Configuration object
     */
    getConfig() {
        return Config;
    }
}

// Initialize app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.App = new CookieCompetitionApp();
    window.App.init();
});

// Export for module systems if needed
if (typeof module !== 'undefined' && module.exports) {
    module.exports = CookieCompetitionApp;
}
