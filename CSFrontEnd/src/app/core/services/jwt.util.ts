/** Minimal, dependency-free JWT payload decoder (base64url -> JSON). */
export function decodeJwtPayload<T = Record<string, unknown>>(token: string): T | null {
  const parts = token.split('.');
  if (parts.length < 2) {
    return null;
  }

  try {
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    const json = decodeURIComponent(
      atob(padded)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    );
    return JSON.parse(json) as T;
  } catch {
    return null;
  }
}

/** Reads a claim value that may appear under its short name or the long ClaimTypes URI. */
export function readClaim(payload: Record<string, unknown>, shortName: string): string | null {
  if (payload[shortName] != null) {
    return String(payload[shortName]);
  }

  const match = Object.keys(payload).find((key) => key.toLowerCase().endsWith(`/${shortName}`));
  return match ? String(payload[match]) : null;
}

export function readClaimArray(payload: Record<string, unknown>, shortName: string): string[] {
  const direct = payload[shortName];
  if (Array.isArray(direct)) {
    return direct.map(String);
  }
  if (typeof direct === 'string') {
    return [direct];
  }

  const match = Object.keys(payload).find((key) => key.toLowerCase().endsWith(`/${shortName}`));
  if (!match) {
    return [];
  }
  const value = payload[match];
  return Array.isArray(value) ? value.map(String) : [String(value)];
}
