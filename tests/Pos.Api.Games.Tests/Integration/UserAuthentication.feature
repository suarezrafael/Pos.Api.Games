Feature: User Authentication
    As a user
    I want to register and login to the system
    So that I can access the game library

Background:
    Given the authentication service is available

Scenario: User registers with valid credentials
    Given I have a valid registration request with:
        | Field    | Value              |
        | Name     | John Doe           |
        | Email    | john@example.com   |
        | Password | StrongPass@123     |
    When I submit the registration request
    Then the registration should be successful
    And I should receive a JWT token

Scenario: User cannot register with weak password
    Given I have a registration request with weak password:
        | Field    | Value              |
        | Name     | Jane Doe           |
        | Email    | jane@example.com   |
        | Password | weak               |
    When I submit the registration request
    Then the registration should fail
    And I should receive a password validation error

Scenario: User logs in with valid credentials
    Given I have registered a user with email "test@example.com" and password "StrongPass@123"
    When I login with email "test@example.com" and password "StrongPass@123"
    Then the login should be successful
    And I should receive a JWT token

Scenario: User cannot login with invalid credentials
    Given I have registered a user with email "test@example.com" and password "StrongPass@123"
    When I login with email "test@example.com" and password "WrongPassword@123"
    Then the login should fail
    And I should receive an authentication error
