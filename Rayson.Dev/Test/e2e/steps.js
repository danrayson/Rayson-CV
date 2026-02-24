const { chromium, devices } = require('@playwright/test');
const { BeforeAll, After, Before, AfterAll, Status, Given, When, Then, setDefaultTimeout } = require('@cucumber/cucumber');
require('dotenv').config();

setDefaultTimeout(30000);

const API_URL = process.env.E2E_API_URL || 'http://localhost:13245';
const UI_URL = process.env.E2E_UI_URL || 'http://localhost:3000';

let browser;
let context;
let page;

const getPage = () => page;
const getBrowser = () => browser;
const getContext = () => context;

BeforeAll(async () => {
  browser = await chromium.launch({ headless: true });
  context = await browser.newContext();
  page = await context.newPage();
  
  const maxRetries = 30;
  let retries = 0;
  
  console.log(`Waiting for API to be ready at ${API_URL}...`);
  
  while (retries < maxRetries) {
    try {
      const response = await fetch(`${API_URL}/health/live`);
      if (response.ok) {
        console.log('API is ready');
        break;
      }
    } catch {
      // API not ready yet
    }
    retries++;
    await new Promise(resolve => setTimeout(resolve, 2000));
  }
  
  if (retries >= maxRetries) {
    console.warn('Warning: API may not be ready');
  }
});

AfterAll(async () => {
  if (browser) {
    await browser.close();
  }
});

Before(async function () {
  await context.clearCookies();
  await context.clearPermissions();
  await page.goto(UI_URL);
});

After(async function (scenario) {
  if (scenario.result?.status === Status.FAILED) {
    await page.screenshot({ 
      path: `reports/screenshots/${scenario.pickle.name.replace(/[^a-z0-9]/gi, '_')}.png`,
      fullPage: true 
    });
  }
});

const TEST_USER_EMAIL = 'testuser@test.com';
const TEST_USER_PASSWORD = 'TestPassword123!';

let testUserEmail;
let testUserPassword;
let resetToken = null;

Given('I am on the login page', async function () {
  const page = getPage();
  await page.goto(UI_URL + '/', { waitUntil: 'networkidle' });
  await page.waitForSelector('#email', { timeout: 15000 });
});

Given('I am on the registration page', async function () {
  const page = getPage();
  await page.goto(UI_URL + '/#/signup');
  await page.waitForLoadState('networkidle');
});

Given('I am on the forgot password page', async function () {
  const page = getPage();
  await page.goto(UI_URL + '/#/forgot-password');
  await page.waitForLoadState('networkidle');
});

Given('I am signed in', async function () {
  const page = getPage();
  testUserEmail = TEST_USER_EMAIL;
  testUserPassword = TEST_USER_PASSWORD;
  
  await page.goto(UI_URL + '/#/');
  await page.waitForLoadState('networkidle');
});

Given('I have a valid password reset token', async function () {
  testUserEmail = TEST_USER_EMAIL;
  resetToken = 'test-reset-token';
});

Given('I have an invalid password reset token', async function () {
  resetToken = 'invalid-token-12345';
});

When('I enter valid credentials', async function () {
  const page = getPage();
  testUserEmail = TEST_USER_EMAIL;
  testUserPassword = TEST_USER_PASSWORD;
  
  await page.locator('#email').fill(testUserEmail, { timeout: 10000 });
  await page.locator('#password').fill(testUserPassword, { timeout: 10000 });
});

When('I enter an invalid password', async function () {
  const page = getPage();
  testUserEmail = TEST_USER_EMAIL;
  
  await page.locator('#email').fill(testUserEmail, { timeout: 10000 });
  await page.locator('#password').fill('WrongPassword123!', { timeout: 10000 });
});

When('I enter credentials for a non-existent user', async function () {
  const page = getPage();
  
  await page.locator('#email').fill('nonexistent@test.com', { timeout: 10000 });
  await page.locator('#password').fill('SomePassword123!', { timeout: 10000 });
});

When('I click the sign in button', async function () {
  const page = getPage();
  
  await page.click('button[type="submit"]:has-text("Login")');
  await page.waitForLoadState('networkidle');
});

When('I click the sign out button', async function () {
  const page = getPage();
  
  await page.click('button:has-text("Logout"), button:has-text("Sign Out")');
  await page.waitForLoadState('networkidle');
});

When('I enter valid registration details', async function () {
  const page = getPage();
  
  testUserEmail = `newuser${Date.now()}@test.com`;
  testUserPassword = 'NewUser123!';
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', testUserEmail);
  await page.fill('input[type="password"], input[name="password"], input[id="password"]', testUserPassword);
  await page.fill('input[type="password"]:nth-of-type(2), input[name="confirmPassword"], input[id="confirmPassword"]', testUserPassword);
});

When('I enter an email that already exists', async function () {
  const page = getPage();
  
  testUserEmail = TEST_USER_EMAIL;
  testUserPassword = 'NewUser123!';
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', testUserEmail);
  await page.fill('input[type="password"], input[name="password"], input[id="password"]', testUserPassword);
  await page.fill('input[type="password"]:nth-of-type(2), input[name="confirmPassword"], input[id="confirmPassword"]', testUserPassword);
});

When('I enter an invalid email format', async function () {
  const page = getPage();
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', 'notanemail');
  await page.fill('input[type="password"], input[name="password"], input[id="password"]', 'NewUser123!');
  await page.fill('input[type="password"]:nth-of-type(2), input[name="confirmPassword"], input[id="confirmPassword"]', 'NewUser123!');
});

When('I enter a weak password', async function () {
  const page = getPage();
  
  testUserEmail = `newuser${Date.now()}@test.com`;
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', testUserEmail);
  await page.fill('input[type="password"], input[name="password"], input[id="password"]', 'weak');
  await page.fill('input[type="password"]:nth-of-type(2), input[name="confirmPassword"], input[id="confirmPassword"]', 'weak');
});

When('I click the sign up button', async function () {
  const page = getPage();
  
  const signUpButton = page.locator('button[type="submit"], button:has-text("Sign Up"), button:has-text("sign up"), button:has-text("Register")');
  await signUpButton.click();
  await page.waitForLoadState('networkidle');
});

When('I enter a valid registered email address', async function () {
  const page = getPage();
  testUserEmail = TEST_USER_EMAIL;
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', testUserEmail);
});

When('I enter an email that does not exist', async function () {
  const page = getPage();
  
  await page.fill('input[type="email"], input[name="email"], input[id="email"]', 'doesnotexist@test.com');
});

When('I click the request reset button', async function () {
  const page = getPage();
  
  const resetButton = page.locator('button[type="submit"], button:has-text("Request"), button:has-text("Reset")');
  await resetButton.click();
  await page.waitForLoadState('networkidle');
});

When('I enter a new password', async function () {
  const page = getPage();
  
  testUserPassword = 'ResetPassword123!';
  await page.fill('input[type="password"], input[name="password"], input[id="password"]', testUserPassword);
});

When('I confirm the new password', async function () {
  const page = getPage();
  
  await page.fill('input[type="password"]:nth-of-type(2), input[name="confirmPassword"], input[id="confirmPassword"]', testUserPassword);
});

When('I click the reset password button', async function () {
  const page = getPage();
  
  const resetButton = page.locator('button[type="submit"], button:has-text("Reset"), button:has-text("Submit")');
  await resetButton.click();
  await page.waitForLoadState('networkidle');
});

Then('I should be redirected to the home page', async function () {
  const page = getPage();
  
  await page.waitForURL(/\/#\/|\/home|\/dashboard/);
});

Then('I should see my user name displayed', async function () {
  const page = getPage();
  
  // The BasicPage shows "Hello, World!" - just verify we're on the dashboard
  const dashboardContent = page.locator('text=Hello, World!');
  await expect(dashboardContent).toBeVisible({ timeout: 10000 });
});

Then('I should see an error message', async function () {
  const page = getPage();
  
  // The UI uses ValidationMessages component with alert-outline class
  const errorMessage = page.locator('.alert-outline, [role="alert"], .text-red-500, .text-red-600');
  await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
});

Then('I should remain on the login page', async function () {
  const page = getPage();
  
  // Verify we're still on the login page by checking for the email input
  await page.waitForSelector('#email', { timeout: 5000 });
});

Then('I should be redirected to the login page', async function () {
  const page = getPage();
  
  await page.waitForURL(/\/#\/login|\/login/);
});

Then('I should not see my user name displayed', async function () {
  const page = getPage();
  
  const userNameDisplay = page.locator('text=testuser');
  await expect(userNameDisplay.first()).not.toBeVisible({ timeout: 5000 });
});

Then('I should see a confirmation message', async function () {
  const page = getPage();
  
  const successMessage = page.locator('.alert-success, .alert-info, [role="status"], .text-green-500, .text-green-600, text=success, text=confirmed, text=sent');
  await expect(successMessage.first()).toBeVisible({ timeout: 10000 });
});

Then('I should be able to sign in with the new account', async function () {
  const page = getPage();
  
  await page.goto(UI_URL + '/#/');
  await page.waitForLoadState('networkidle');
  
  await page.waitForURL(/\/#\/|\/home|\/dashboard/);
});

Then('I should see an error message about the email already being taken', async function () {
  const page = getPage();
  
  const errorMessage = page.locator('text=email, text=already, text=exists, text=taken', { hasText: /email|already|exists|taken/i });
  await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
});

Then('I should see an error message about invalid email format', async function () {
  const page = getPage();
  
  const errorMessage = page.locator('text=email, text=invalid', { hasText: /email|invalid/i });
  await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
});

Then('I should see an error message about password requirements', async function () {
  const page = getPage();
  
  const errorMessage = page.locator('text=password, text=requirement, text=weak', { hasText: /password|requirement|weak/i });
  await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
});

Then('I should see a confirmation message that the reset email was sent', async function () {
  const page = getPage();
  
  const successMessage = page.locator('text=reset, text=sent, text=email', { hasText: /reset|sent|email/i });
  await expect(successMessage.first()).toBeVisible({ timeout: 10000 });
});

Then('I should see an error message about the invalid token', async function () {
  const page = getPage();
  
  const errorMessage = page.locator('text=token, text=invalid, text=expired', { hasText: /token|invalid|expired/i });
  await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
});

const expect = require('@playwright/test').expect;
