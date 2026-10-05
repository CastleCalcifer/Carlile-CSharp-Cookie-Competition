/**
 * DOM manipulation utilities
 * Provides common DOM operations and element creation helpers
 */
class DomUtils {
    /**
     * Safely gets an element by ID
     * @param {string} id - Element ID
     * @returns {HTMLElement|null} Element or null if not found
     */
    static getElement(id) {
        const element = document.getElementById(id);
        if (!element) {
            console.warn(`Element with ID '${id}' not found`);
        }
        return element;
    }

    /**
     * Creates an element with attributes and content
     * @param {string} tag - HTML tag name
     * @param {Object} attributes - Element attributes
     * @param {string|HTMLElement|Array} content - Element content
     * @returns {HTMLElement} Created element
     */
    static createElement(tag, attributes = {}, content = '') {
        const element = document.createElement(tag);
        
        // Set attributes
        Object.entries(attributes).forEach(([key, value]) => {
            if (key === 'className') {
                element.className = value;
            } else if (key === 'dataset') {
                Object.entries(value).forEach(([dataKey, dataValue]) => {
                    element.dataset[dataKey] = dataValue;
                });
            } else {
                element.setAttribute(key, value);
            }
        });

        // Set content
        if (typeof content === 'string') {
            element.textContent = content;
        } else if (content instanceof HTMLElement) {
            element.appendChild(content);
        } else if (Array.isArray(content)) {
            content.forEach(item => {
                if (typeof item === 'string') {
                    element.appendChild(document.createTextNode(item));
                } else if (item instanceof HTMLElement) {
                    element.appendChild(item);
                }
            });
        }

        return element;
    }

    /**
     * Creates a Bootstrap button element
     * @param {string} text - Button text
     * @param {Object} options - Button options
     * @returns {HTMLButtonElement} Button element
     */
    static createButton(text, options = {}) {
        const {
            type = 'button',
            className = 'btn btn-primary',
            onclick = null,
            disabled = false,
            ...attributes
        } = options;

        const button = this.createElement('button', {
            type,
            className,
            disabled,
            ...attributes
        }, text);

        if (onclick) {
            button.onclick = onclick;
        }

        return button;
    }

    /**
     * Creates a select element with options
     * @param {Array} options - Array of {value, text, selected} objects
     * @param {Object} attributes - Select attributes
     * @returns {HTMLSelectElement} Select element
     */
    static createSelect(options = [], attributes = {}) {
        const select = this.createElement('select', attributes);
        
        options.forEach(option => {
            const optionElement = this.createElement('option', {
                value: option.value || '',
                selected: option.selected || false
            }, option.text || '');
            select.appendChild(optionElement);
        });

        return select;
    }

    /**
     * Clears all children from an element
     * @param {HTMLElement} element - Element to clear
     */
    static clearElement(element) {
        if (element) {
            element.innerHTML = '';
        }
    }

    /**
     * Shows an element by removing 'd-none' class
     * @param {HTMLElement} element - Element to show
     */
    static showElement(element) {
        if (element) {
            element.classList.remove('d-none');
        }
    }

    /**
     * Hides an element by adding 'd-none' class
     * @param {HTMLElement} element - Element to hide
     */
    static hideElement(element) {
        if (element) {
            element.classList.add('d-none');
        }
    }

    /**
     * Sets error message in an element
     * @param {HTMLElement} element - Error element
     * @param {string} message - Error message
     */
    static setError(element, message) {
        if (element) {
            element.textContent = message;
            this.showElement(element);
        }
    }

    /**
     * Clears error message from an element
     * @param {HTMLElement} element - Error element
     */
    static clearError(element) {
        if (element) {
            element.textContent = '';
            this.hideElement(element);
        }
    }

    /**
     * Escapes HTML to prevent XSS
     * @param {string} text - Text to escape
     * @returns {string} Escaped text
     */
    static escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Export for global use
window.DomUtils = DomUtils;
