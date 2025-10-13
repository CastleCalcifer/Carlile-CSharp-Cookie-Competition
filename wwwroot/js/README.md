# JavaScript Architecture - Cookie Competition

This document describes the refactored JavaScript architecture with proper separation of concerns.

## 📁 Directory Structure

```
wwwroot/js/
├── core/                    # Core infrastructure
│   ├── api-client.js       # Centralized API communication
│   ├── config.js           # Application configuration
│   ├── dom-utils.js        # DOM manipulation utilities
│   └── error-handler.js    # Centralized error handling
├── services/               # Business logic layer
│   ├── awards-service.js   # Awards-related business logic
│   ├── baker-service.js    # Baker-related business logic
│   ├── cookie-service.js   # Cookie-related business logic
│   ├── results-service.js  # Results-related business logic
│   └── voting-service.js   # Voting-related business logic
├── components/             # Reusable UI components
│   ├── award-selector.js   # Award selection component
│   ├── cookie-card.js      # Cookie display component
│   └── rank-selector.js    # Ranking selection component
├── pages/                  # Page controllers
│   ├── awards-page.js      # Awards page controller
│   ├── baker-login-page.js # Baker login page controller
│   ├── results-page.js     # Results page controller
│   └── voting-page.js      # Voting page controller
├── app.js                  # Main application entry point
└── README.md              # This file
```

## 🏗️ Architecture Principles

### 1. **Separation of Concerns**
- **Core**: Infrastructure and utilities
- **Services**: Business logic and data management
- **Components**: Reusable UI elements
- **Pages**: Page-specific controllers

### 2. **Dependency Injection**
- Services receive dependencies through constructor
- Components are configurable and reusable
- No global state pollution

### 3. **Error Handling**
- Centralized error handling system
- User-friendly error messages
- Comprehensive logging

### 4. **Configuration Management**
- Centralized configuration
- Environment-specific settings
- Easy to modify and extend

## 🔧 Core Infrastructure

### ApiClient
Centralized HTTP client with:
- Automatic error handling
- Response parsing
- Authentication support
- Retry logic

### DomUtils
DOM manipulation utilities:
- Element creation helpers
- Safe element access
- Event handling utilities
- HTML escaping

### ErrorHandler
Centralized error management:
- Global error catching
- User-friendly messages
- Error logging
- Alert system

### Config
Application configuration:
- API endpoints
- UI settings
- Image defaults
- Year configuration

## 🏢 Services Layer

Each service handles specific business logic:

### CookieService
- Cookie data management
- Validation and normalization
- Sorting and filtering
- Image URL handling

### BakerService
- Baker authentication
- Session management
- Data validation
- Login/logout logic

### VotingService
- Vote submission
- Data validation
- Form processing
- Error handling

### AwardsService
- Award submission
- Selection validation
- Form processing
- Duplicate prevention

### ResultsService
- Results fetching
- Data normalization
- Sorting and ranking
- Display formatting

## 🧩 Components

Reusable UI components:

### CookieCard
- Cookie display component
- Multiple display modes
- Image handling
- Score display

### RankSelector
- Unique ranking selection
- Validation
- Event handling
- State management

### AwardSelector
- Award selection dropdowns
- Image preview
- Validation
- Form integration

## 📄 Page Controllers

Page-specific logic:

### VotingPage
- Cookie loading
- Rank selection
- Form submission
- Error handling

### AwardsPage
- Cookie loading
- Award selection
- Form submission
- Image preview

### ResultsPage
- Results loading
- Display formatting
- Error handling
- Data presentation

### BakerLoginPage
- Baker listing
- Authentication
- Modal handling
- Session management

## 🚀 Usage

### Loading Order
1. Core infrastructure (config, dom-utils, error-handler, api-client)
2. Services (business logic)
3. Components (UI components)
4. Page controllers (page-specific logic)
5. Main app (initialization)

### Example HTML
```html
<!-- Load core infrastructure first -->
<script src="/js/core/config.js"></script>
<script src="/js/core/dom-utils.js"></script>
<script src="/js/core/error-handler.js"></script>
<script src="/js/core/api-client.js"></script>

<!-- Load services -->
<script src="/js/services/cookie-service.js"></script>
<script src="/js/services/baker-service.js"></script>

<!-- Load components -->
<script src="/js/components/cookie-card.js"></script>

<!-- Load page controller -->
<script src="/js/pages/voting-page.js"></script>

<!-- Load main app -->
<script src="/js/app.js"></script>
```

## 🔄 Migration from Old Structure

### Before (Old Structure)
- Mixed responsibilities
- Code duplication
- Global state
- Tight coupling
- No error handling

### After (New Structure)
- Clear separation of concerns
- Reusable components
- Dependency injection
- Centralized error handling
- Maintainable code

## 🧪 Testing

The new structure is designed to be easily testable:
- Services can be unit tested
- Components can be integration tested
- Page controllers can be end-to-end tested
- Mocking is straightforward

## 📈 Benefits

1. **Maintainability**: Clear structure makes code easy to understand and modify
2. **Reusability**: Components and services can be reused across pages
3. **Testability**: Each layer can be tested independently
4. **Scalability**: Easy to add new features and pages
5. **Error Handling**: Centralized and consistent error management
6. **Performance**: Optimized loading and execution
7. **Developer Experience**: Clear APIs and documentation

## 🔧 Configuration

Update `wwwroot/js/core/config.js` to modify:
- API endpoints
- Image defaults
- UI settings
- Year configuration

## 🐛 Debugging

Enable debug mode by setting `DEBUG = true` in the relevant files or use browser dev tools to inspect the global objects:
- `window.App` - Main application instance
- `window.Services` - All services
- `window.Config` - Configuration
- `window.ErrorHandler` - Error handling
