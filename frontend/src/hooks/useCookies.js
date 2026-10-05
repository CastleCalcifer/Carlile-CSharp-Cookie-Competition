import { useState, useEffect } from 'react'

export function useCookies() {
    const [cookies, setCookies] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        fetch('http://localhost:8080/api/cookies')
            .then(res => {
                if (!res.ok) throw new Error('Failed to load cookies')
                return res.json()
            })
            .then(data => setCookies(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { cookies, loading, error }
}