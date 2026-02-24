Feature: User Authentication
  As a registered user
  I want to sign in to my account
  So that I can access protected features

  Background:
    Given I am on the login page

  @smoke
  Scenario: Successful sign in with valid credentials
    When I enter valid credentials
    And I click the sign in button
    Then I should be redirected to the home page
    And I should see my user name displayed

  Scenario: Failed sign in with invalid password
    When I enter an invalid password
    And I click the sign in button
    Then I should see an error message
    And I should remain on the login page

  Scenario: Failed sign in with non-existent user
    When I enter credentials for a non-existent user
    And I click the sign in button
    Then I should see an error message
    And I should remain on the login page
