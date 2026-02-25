import { Page, Locator, expect } from '@playwright/test';

export async function waitForPageReady(page: Page, urlPattern?: string | RegExp): Promise<void> {
  if (urlPattern) {
    await page.waitForURL(urlPattern);
  }
  await page.waitForLoadState('networkidle');
}

export async function clickButton(page: Page, buttonText: string): Promise<void> {
  const button = page.locator(`button:has-text("${buttonText}")`);
  await button.click();
  await page.waitForLoadState('networkidle');
}

export async function fillInput(page: Page, selector: string, value: string): Promise<void> {
  const input = page.locator(selector);
  await input.fill(value);
}

export async function getText(page: Page, selector: string): Promise<string> {
  const element = page.locator(selector);
  return element.textContent() || '';
}

export async function isVisible(page: Page, selector: string): Promise<boolean> {
  try {
    const element = page.locator(selector);
    await expect(element).toBeVisible({ timeout: 5000 });
    return true;
  } catch {
    return false;
  }
}

export async function isEnabled(page: Page, selector: string): Promise<boolean> {
  const element = page.locator(selector);
  return element.isEnabled();
}

export async function waitForSelector(page: Page, selector: string, timeout = 10000): Promise<Locator> {
  return page.locator(selector).first();
}

export async function takeScreenshot(page: Page, name: string): Promise<void> {
  await page.screenshot({ 
    path: `reports/screenshots/${name.replace(/[^a-z0-9]/gi, '_')}.png`,
    fullPage: true 
  });
}

export async function clearCookiesAndStorage(page: Page): Promise<void> {
  await page.context().clearCookies();
  await page.evaluate(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
}

export function getUniqueEmail(prefix = 'test'): string {
  const timestamp = Date.now();
  const random = Math.floor(Math.random() * 10000);
  return `${prefix}${timestamp}${random}@test.com`;
}
