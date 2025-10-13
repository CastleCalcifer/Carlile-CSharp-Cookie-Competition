/**
 * Baker login page controller - handles baker login functionality
 */
class BakerLoginPage {
    constructor() {
        this.bakerService = new BakerService(ApiClient, Config);
        this.bakers = [];
        
        this.bakerListContainer = null;
        this.modal = null;
        this.modalText = null;
        this.pinInput = null;
        this.pinError = null;
        this.pinSubmit = null;
        this.pinCancel = null;
        
        this.selectedBakerName = null;
    }

    /**
     * Initializes the baker login page
     */
    async init() {
        try {
            this._getDOMElements();
            this._setupEventListeners();
            await this._loadBakers();
        } catch (error) {
            ErrorHandler.handleError('BakerLoginPage.init', error);
            this._showError('Failed to initialize baker login page');
        }
    }

    /**
     * Gets required DOM elements
     */
    _getDOMElements() {
        this.bakerListContainer = DomUtils.getElement('bakerList');
        this.modal = DomUtils.getElement('pinModal');
        this.modalText = DomUtils.getElement('modalText');
        this.pinInput = DomUtils.getElement('pinInput');
        this.pinError = DomUtils.getElement('pinError');
        this.pinSubmit = DomUtils.getElement('pinSubmit');
        this.pinCancel = DomUtils.getElement('pinCancel');

        if (!this.bakerListContainer) {
            throw new Error('Baker list container not found');
        }
        if (!this.modal) {
            throw new Error('PIN modal not found');
        }
    }

    /**
     * Sets up event listeners
     */
    _setupEventListeners() {
        if (this.pinSubmit) {
            this.pinSubmit.addEventListener('click', () => this._handlePinSubmit());
        }
        if (this.pinCancel) {
            this.pinCancel.addEventListener('click', () => this._hideModal());
        }
    }

    /**
     * Loads and displays bakers
     */
    async _loadBakers() {
        try {
            this.bakers = await this.bakerService.getBakers();
            this._renderBakers();
        } catch (error) {
            ErrorHandler.handleError('BakerLoginPage._loadBakers', error);
            this._showError('Failed to load bakers');
        }
    }

    /**
     * Renders the list of bakers
     */
    _renderBakers() {
        DomUtils.clearElement(this.bakerListContainer);

        if (!this.bakers || this.bakers.length === 0) {
            this.bakerListContainer.innerHTML = '<p>No bakers found.</p>';
            return;
        }

        this.bakers.forEach(baker => {
            const button = DomUtils.createButton(
                baker.bakerName + (baker.hasPin ? '' : ' (set PIN)'),
                {
                    className: 'list-group-item list-group-item-action',
                    onclick: () => this._openPinModal(baker.bakerName, baker.hasPin)
                }
            );
            this.bakerListContainer.appendChild(button);
        });
    }

    /**
     * Opens the PIN modal for a baker
     * @param {string} bakerName - Baker name
     * @param {boolean} hasPin - Whether baker has a PIN
     */
    _openPinModal(bakerName, hasPin) {
        this.selectedBakerName = bakerName;
        
        if (this.modalText) {
            this.modalText.textContent = hasPin 
                ? `Enter PIN for ${bakerName}` 
                : `No PIN found for ${bakerName}. Create a PIN to claim this account.`;
        }
        
        if (this.pinInput) {
            this.pinInput.value = '';
        }
        
        DomUtils.clearError(this.pinError);
        
        // Show Bootstrap modal
        if (window.bootstrap && this.modal) {
            const modalInstance = new bootstrap.Modal(this.modal);
            modalInstance.show();
        }
    }

    /**
     * Hides the PIN modal
     */
    _hideModal() {
        if (window.bootstrap && this.modal) {
            const modalInstance = bootstrap.Modal.getInstance(this.modal);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
    }

    /**
     * Handles PIN submission
     */
    async _handlePinSubmit() {
        if (!this.pinInput || !this.selectedBakerName) return;

        const pin = this.pinInput.value.trim();
        if (!pin) {
            this._showPinError('PIN required');
            return;
        }

        try {
            const result = await this.bakerService.login(this.selectedBakerName, pin);
            
            this._hideModal();
            this._showSuccess('Login successful!');
            
            // Redirect to baker area
            setTimeout(() => {
                window.location.href = '/baker/area';
            }, 1000);

        } catch (error) {
            ErrorHandler.handleError('BakerLoginPage._handlePinSubmit', error);
            this._showPinError('Invalid PIN or network error');
        }
    }

    /**
     * Shows PIN error message
     * @param {string} message - Error message
     */
    _showPinError(message) {
        if (this.pinError) {
            DomUtils.setError(this.pinError, message);
        }
    }

    /**
     * Shows error message
     * @param {string} message - Error message
     */
    _showError(message) {
        ErrorHandler.showAlert(message, 'error');
    }

    /**
     * Shows success message
     * @param {string} message - Success message
     */
    _showSuccess(message) {
        ErrorHandler.showAlert(message, 'success');
    }
}

// Auto-initialize when DOM is ready and dependencies are loaded
document.addEventListener('DOMContentLoaded', () => {
    // Wait for dependencies to be available
    const initBakerLoginPage = () => {
        if (window.ApiClient && window.Config && window.DomUtils && window.ErrorHandler) {
            new BakerLoginPage().init();
        } else {
            setTimeout(initBakerLoginPage, 100);
        }
    };
    initBakerLoginPage();
});
