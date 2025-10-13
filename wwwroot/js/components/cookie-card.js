/**
 * Cookie card component - reusable cookie display component
 */
class CookieCard {
    constructor(config) {
        this.config = config;
    }

    /**
     * Creates a cookie card element
     * @param {Object} cookie - Cookie data
     * @param {Object} options - Display options
     * @returns {HTMLElement} Cookie card element
     */
    create(cookie, options = {}) {
        const {
            showRank = false,
            rank = null,
            showScore = false,
            showPoints = false,
            className = 'd-flex flex-column justify-content-center align-items-center cookieOption p-2',
            imageClassName = 'cookieImage',
            titleClassName = '',
            scoreClassName = ''
        } = options;

        const card = DomUtils.createElement('div', { className });

        // Image container
        const imageContainer = DomUtils.createElement('div', { className: 'p-2' });
        const image = DomUtils.createElement('img', {
            src: this.config.getImageUrl(cookie.imageUrl),
            alt: cookie.cookieName,
            className: imageClassName
        });
        imageContainer.appendChild(image);

        // Content container
        const contentContainer = DomUtils.createElement('div', { className: 'd-inline-flex p-2 gap-4' });

        // Title
        const title = DomUtils.createElement('p', { 
            className: titleClassName 
        }, cookie.cookieName);

        contentContainer.appendChild(title);

        // Add rank if specified
        if (showRank && rank !== null) {
            const rankElement = DomUtils.createElement('h2', {}, this._formatRank(rank));
            contentContainer.appendChild(rankElement);
        }

        // Add score if specified
        if (showScore) {
            const scoreElement = DomUtils.createElement('h2', { 
                className: scoreClassName 
            }, `Total Score: ${cookie.score || 0}`);
            contentContainer.appendChild(scoreElement);
        }

        // Add points if specified
        if (showPoints) {
            const pointsContainer = DomUtils.createElement('div', { className: 'points-container' });
            
            if (cookie.creativePoints !== undefined) {
                const creativePoints = DomUtils.createElement('p', {}, 
                    `Creative: ${cookie.creativePoints}`);
                pointsContainer.appendChild(creativePoints);
            }

            if (cookie.presentationPoints !== undefined) {
                const presentationPoints = DomUtils.createElement('p', {}, 
                    `Presentation: ${cookie.presentationPoints}`);
                pointsContainer.appendChild(presentationPoints);
            }

            contentContainer.appendChild(pointsContainer);
        }

        card.appendChild(imageContainer);
        card.appendChild(contentContainer);

        return card;
    }

    /**
     * Creates a cookie card for voting with rank selector
     * @param {Object} cookie - Cookie data
     * @param {number} index - Cookie index
     * @param {number} maxRanks - Maximum number of ranks
     * @returns {HTMLElement} Voting cookie card
     */
    createVotingCard(cookie, index, maxRanks) {
        const card = DomUtils.createElement('div', {
            className: 'd-flex flex-column justify-content-center align-items-center cookieOption p-2'
        });

        // Image container
        const imageContainer = DomUtils.createElement('div', { className: 'p-2' });
        const image = DomUtils.createElement('img', {
            src: this.config.getImageUrl(cookie.imageUrl),
            alt: cookie.cookieName,
            className: 'cookieImage'
        });
        imageContainer.appendChild(image);

        // Content container
        const contentContainer = DomUtils.createElement('div', { className: 'd-inline-flex p-2 gap-4' });
        
        // Title
        const title = DomUtils.createElement('p', {}, cookie.cookieName);
        contentContainer.appendChild(title);

        // Rank selector
        const selectorContainer = DomUtils.createElement('div', { className: 'rankDropdown' });
        const select = this._createRankSelector(cookie, index, maxRanks);
        selectorContainer.appendChild(select);
        contentContainer.appendChild(selectorContainer);

        card.appendChild(imageContainer);
        card.appendChild(contentContainer);

        return card;
    }

    /**
     * Creates a rank selector dropdown
     * @param {Object} cookie - Cookie data
     * @param {number} index - Cookie index
     * @param {number} maxRanks - Maximum number of ranks
     * @returns {HTMLSelectElement} Rank selector
     */
    _createRankSelector(cookie, index, maxRanks) {
        const options = [
            { value: '', text: '---', selected: true }
        ];

        for (let rank = 1; rank <= maxRanks; rank++) {
            options.push({
                value: String(rank),
                text: this._formatRank(rank),
                className: 'ranking'
            });
        }

        const select = DomUtils.createSelect(options, {
            className: 'rank-select form-select',
            dataset: {
                index: String(index),
                cookieId: String(cookie.id)
            }
        });

        return select;
    }

    /**
     * Formats rank number with ordinal suffix
     * @param {number} rank - Rank number
     * @returns {string} Formatted rank
     */
    _formatRank(rank) {
        if (rank === 1) return '1st';
        if (rank === 2) return '2nd';
        if (rank === 3) return '3rd';
        return `${rank}th`;
    }

    /**
     * Creates a results card for display
     * @param {Object} cookie - Cookie data
     * @param {string} positionLabel - Position label (e.g., "1st Place")
     * @param {Object} options - Display options
     * @returns {HTMLElement} Results card
     */
    createResultsCard(cookie, positionLabel, options = {}) {
        const {
            showScore = true,
            showPoints = false,
            className = 'd-flex flex-column justify-content-center align-items-center cookieOption'
        } = options;

        const card = DomUtils.createElement('div', { className });

        const content = DomUtils.createElement('div', { className: 'p-2' });

        // Position label
        const positionElement = DomUtils.createElement('h2', {}, positionLabel);
        content.appendChild(positionElement);

        // Cookie name
        const nameElement = DomUtils.createElement('h2', {}, 
            DomUtils.escapeHtml(cookie.cookieName));
        content.appendChild(nameElement);

        // Image
        const image = DomUtils.createElement('img', {
            src: this.config.getImageUrl(cookie.imageUrl),
            alt: cookie.cookieName,
            className: 'cookieImage'
        });
        content.appendChild(image);

        // Score
        if (showScore) {
            const scoreElement = DomUtils.createElement('h2', {}, 
                `Total Score: ${cookie.score || 0}`);
            content.appendChild(scoreElement);
        }

        // Points
        if (showPoints) {
            if (cookie.creativePoints !== undefined) {
                const creativePoints = DomUtils.createElement('p', {}, 
                    `Creative Points: ${cookie.creativePoints}`);
                content.appendChild(creativePoints);
            }

            if (cookie.presentationPoints !== undefined) {
                const presentationPoints = DomUtils.createElement('p', {}, 
                    `Presentation Points: ${cookie.presentationPoints}`);
                content.appendChild(presentationPoints);
            }
        }

        card.appendChild(content);
        return card;
    }
}

// Export for global use
window.CookieCard = CookieCard;
