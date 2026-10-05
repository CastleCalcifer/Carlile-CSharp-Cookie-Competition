# Carlile Cookie Competition - Test Suite

This directory contains comprehensive xUnit tests for the Carlile Cookie Competition API.

## Test Structure

### Models Tests
- **BakerTests.cs** - Tests for the Baker model including constructors, properties, and validation
- **CookieTests.cs** - Tests for the Cookie model including constructors, properties, and validation
- **VoteTests.cs** - Tests for the Vote model including default values and property validation
- **YearRecordTests.cs** - Tests for the YearRecord model including properties and validation

### Controller Tests
- **BakersControllerTests.cs** - Tests for baker authentication, login, and current user functionality
- **CookiesControllerTests.cs** - Tests for cookie retrieval with filtering and exclusion
- **VotesControllerTests.cs** - Tests for vote submission with validation and scoring
- **ResultsControllerTests.cs** - Tests for results retrieval with ranking and awards
- **AwardsControllerTests.cs** - Tests for award submission with validation and conflict checking
- **BakerAreaControllerTests.cs** - Tests for protected area access and cookie validation

### Data Tests
- **DbSeederTests.cs** - Tests for database seeding functionality including data creation and linking

### DTO Tests
- **LoginRequestTests.cs** - Tests for login request DTO validation
- **SubmitVotesRequestTests.cs** - Tests for vote submission request DTO validation
- **AwardSubmitRequestTests.cs** - Tests for award submission request DTO validation

### Integration Tests
- **IntegrationTests.cs** - End-to-end tests for API endpoints and workflows

## Test Features

### Unit Tests
- Model validation and property testing
- Controller action testing with mocked dependencies
- DTO validation and immutability testing
- Database seeder functionality testing

### Integration Tests
- Full API workflow testing
- Database integration testing
- Authentication flow testing
- Cross-controller functionality testing

### Test Infrastructure
- **TestWebApplicationFactory.cs** - Custom web application factory for integration testing
- In-memory database for isolated testing
- Mocked dependencies for unit testing
- FluentAssertions for readable test assertions

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Coverage

The test suite covers:
- ✅ All model classes and their properties
- ✅ All controller actions and their responses
- ✅ All DTO classes and validation
- ✅ Database seeding functionality
- ✅ Authentication and authorization flows
- ✅ Error handling and edge cases
- ✅ Integration workflows

## Dependencies

- **xUnit** - Testing framework
- **FluentAssertions** - Fluent assertion library
- **Moq** - Mocking framework
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for testing
