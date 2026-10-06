const competitionYear = Number(import.meta.env.VITE_COMPETITION_YEAR || 2025)

async function request(path, options = {}) {
    const response = await fetch(path, {
        ...options,
        credentials: 'include',
        headers: {
            ...(options.body ? { 'Content-Type': 'application/json' } : {}),
            ...options.headers,
        },
    })

    const text = await response.text()
    let data = null
    if (text) {
        try {
            data = JSON.parse(text)
        } catch {
            data = text
        }
    }

    if (!response.ok || data?.success === false) {
        throw new Error(data?.message || (typeof data === 'string' ? data : `Request failed (${response.status})`))
    }

    return data && typeof data === 'object' && Object.prototype.hasOwnProperty.call(data, 'data')
        ? data.data
        : data
}

export function getCookies({ excludeBakerId } = {}) {
    const params = new URLSearchParams({ year: String(competitionYear) })
    if (excludeBakerId != null) params.set('excludeBakerId', String(excludeBakerId))
    return request(`/api/cookies?${params}`)
}

export function getBakers() {
    return request('/api/bakers')
}

export function getCurrentBaker() {
    return request('/api/bakers/current')
}

export function loginBaker(bakerName, pin) {
    return request('/api/bakers/login', {
        method: 'POST',
        body: JSON.stringify({ bakerName, pin }),
    })
}

export function logoutBaker() {
    return request('/api/bakers/logout', { method: 'POST' })
}

export function getResults() {
    return request(`/api/results?year=${competitionYear}`)
}

export function submitVotes(cookieIds) {
    return request('/api/voter/vote', {
        method: 'POST',
        body: JSON.stringify({ year: competitionYear, cookieIds, voterId: null }),
    })
}

export function submitAwards({ mostCreativeId, bestPresentationId }) {
    return request('/api/awards', {
        method: 'POST',
        body: JSON.stringify({
            year: competitionYear,
            mostCreativeId,
            bestPresentationId,
            voterId: null,
        }),
    })
}
